using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Util
{
    public static class MD5Helper
    {
        /// <summary>
        /// md5 加密 (转小写)
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        public static string GetMD5String(string stringData)
        {
            // 使用MD5.Create()创建一个MD5加密提供者
            using (MD5 md5 = MD5.Create())
            {
                // 将原始数据转换为字节数组
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(stringData));

                // 将字节数组转换为十六进制字符串
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // 返回十六进制字符串
                return sBuilder.ToString().ToLower();
            }
        }

        /// <summary>
        /// 验证Md5 加密是否一致
        /// </summary>
        /// <param name="md5Str"></param>
        /// <param name="stringData"></param>
        /// <returns></returns>
        public static bool VerfiyMD5(string md5Str, string stringData)
        {
            return md5Str == GetMD5String(stringData);
        }
    }
}
