using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PNet
{

    public class HandleBinding
    {
        static Dictionary<Type, NetMsgBaseHandle> dic = new Dictionary<Type, NetMsgBaseHandle>();
        static bool isBinded =false;

        public static void Bind()
        {
            if (isBinded)
            {
                return;
            }
            //LoginHandle handle = new LoginHandle();
            BindContent();
            isBinded = true;
            NetDebug.Log("Bind finish!");

        }

        public static NetMsgBaseHandle GetHandle(Type t)
        {
            if (dic.TryGetValue(t, out NetMsgBaseHandle v))
            {
                return v;
            }
            NetDebug.LogError(string.Format("Cant find {0} bind,please check!", t.ToString()));
            return null;
        }

        public static NetMsgBaseHandle GetHandle<T>() where T : NetMsgBaseHandle
        {
            return GetHandle(typeof(T));
        }


        static void Bind<T>() where T : NetMsgBaseHandle, new()
        {
            var handle = new T();            
            dic.Add(typeof(T), handle);
        }

        static void BindContent()
        {
            Bind<LoginHandle>();
            Bind<CreateNewUserByPhoneHandle>();
            Bind<LoginByTokenHandle>();
            Bind<RefreshTokenHandle>();
            Bind<LogoutHandle>();

        }

     

    }

}
