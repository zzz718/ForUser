using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Common
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class MessageModel<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Status { get; set; } = 200;
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; } = "";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Response { get; set; }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static MessageModel<T> OK(string msg)
        {
            return Message(true, msg, 200, default);
        }
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static MessageModel<T> OK(string msg, T response)
        {
            return Message(true, msg, 200, response);
        }
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="status">状态码</param>
        /// <returns></returns>
        public static MessageModel<T> Fail(string msg, int status = 500)
        {
            return Message(false, msg, status, default);
        }
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="status">状态码</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static MessageModel<T> Fail(string msg, int status = 500, T response = default)
        {
            return Message(false, msg, status, response);
        }
        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="success">失败/成功</param>
        /// <param name="msg">消息</param>
        /// <param name="status">状态码</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static MessageModel<T> Message(bool success, string msg, int status, T response)
        {
            return new MessageModel<T>() { Msg = msg, Response = response, Status = status, Success = success };
        }
    }

    /// <summary>
    /// 统一返回信息 要返回response请使用泛型 
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Status { get; set; } = 200;
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; } = "";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public object Response { get; } = null;

        public MessageModel()
        {

        }

        /// <summary>
        ///  构造函数 默认是成功
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="status"></param>
        /// <param name="isSuccess"></param>
        public MessageModel(string msg, int status = 200, bool isSuccess = true)
        {
            this.Msg = msg;
            this.Status = status;
            this.Success = isSuccess;
        }


        public static MessageModel OK(string msg)
        {
            return new MessageModel
            {
                Msg = msg,
                Success = true,
            };
        }

        public static MessageModel Fail(string msg, int status)
        {
            return new MessageModel(msg, status, false);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static MessageModel<T> OK<T>(string msg, T response = default)
        {
            return MessageModel<T>.OK(msg, response);
        }

        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="status">状态码</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static MessageModel<T> Fail<T>(string msg, int status = 500, T response = default)
        {
            return MessageModel.Fail(msg, status, response);
        }
    }
}
