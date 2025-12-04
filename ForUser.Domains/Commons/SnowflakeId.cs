using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public static class SnowflakeId
    {
        private static readonly object _lock = new object();

        private static readonly long Twepoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .Ticks / TimeSpan.TicksPerMillisecond;

        private const int WorkerIdBits = 5;      // 机器 ID 长度
        private const int DatacenterIdBits = 5;  // 数据中心 ID 长度
        private const int SequenceBits = 12;     // 毫秒内序列号长度

        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        public static long WorkerId { get; private set; }
        public static long DatacenterId { get; private set; }

        private static long _sequence = 0L;
        private static long _lastTimestamp = -1L;

        static SnowflakeId()
        {
            // 默认值：单机部署可设为 0/0
            WorkerId = 1;
            DatacenterId = 1;
        }

        public static void Initialize(long workerId, long datacenterId)
        {
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException($"worker Id can't be > {MaxWorkerId} or < 0");

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
                throw new ArgumentException($"datacenter Id can't be > {MaxDatacenterId} or < 0");

            WorkerId = workerId;
            DatacenterId = datacenterId;
        }

        public static long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                {
                    // 时钟回拨处理
                    timestamp = _lastTimestamp;
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0L;
                }

                _lastTimestamp = timestamp;

                return ((timestamp - Twepoch) << TimestampLeftShift) |
                       (DatacenterId << DatacenterIdShift) |
                       (WorkerId << WorkerIdShift) |
                       _sequence;
            }
        }

        private static long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private static long TimeGen()
        {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
