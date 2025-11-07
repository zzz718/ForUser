using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Common
{
    public interface IPagination
    {

        /// <summary>
        /// 当前页码
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 排序列
        /// </summary>
        List<string> SortFields { get; set; }

    }
    /// <summary>
    /// 分页
    /// </summary>
    public class Pagination : IPagination
    {
        /// <summary>
        ///构造函数
        /// </summary>
        public Pagination()
        {
            PageIndex = 1;
            PageSize = 10;
            SortFields = new List<string> { "id desc" };
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 排序列
        /// </summary>
        public List<string> SortFields { get; set; }

    }
}

