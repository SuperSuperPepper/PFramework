using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PNet
{
    public abstract class NetMsgBaseHandle
    {
        Type BindMsgType;

        public abstract Type SetMessageType();

        public static List<NetMsgBaseHandle> hanleList = new List<NetMsgBaseHandle>();

        public Action<ErrorInfo> getMessgeCallback;

        public NetMsgBaseHandle()
        {
            Init();
        }

        public virtual void Init()
        {
            BindMsgType = SetMessageType();
            if (BindMsgType.ToString().EndsWith("Request"))
            {
                // 并不希望强制约定名称，可能会造成隐藏陷阱，但handle绑定 request肯定是手写的错误，这里进行检查
                NetDebug.LogError("Handle bind Requeset,Please check " + BindMsgType);
            }

            NetManager.Instance.AddHandler(BindMsgType, this);
            NetMsgBaseHandle.hanleList.Add(this);
        }


        public virtual void GetMsaage(IMessage message)
        {

        }

        public void RegisterCallBack(Action<ErrorInfo> callback)
        {
            if (callback!=null)
            {
                getMessgeCallback = callback;
            }
        }

        /// <summary>
        /// 预检查
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public virtual bool PreSendMessage(IMessage message, Action<ErrorInfo> callback)
        {
            return true;
        }

        public void SendMessage(IMessage message,Action<ErrorInfo> callback)
        {
            if (PreSendMessage(message, callback))            
                NetManager.Instance.SendMessage(message);                                   
        }

        /// <summary>
        /// 获取Message类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public T CheckAndGetMsg<T>(IMessage message) where T: class,IMessage
        {
            if (message is T data)
            {
                return data;
            }
            else
            {
                NetDebug.LogError(string.Format("Cant Conver {0} to {1}", message, typeof(T)));
                return null;
            }

        }
    }

    public class ErrorInfo
    {
        public bool Result;
        public string Info;
        public IMessage Message;
    }

}