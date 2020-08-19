using PNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MsgRegisterLogin;
using Google.Protobuf;

public class LogoutHandle : NetMsgBaseHandle
{
    public override Type SetMessageType()
    {
        return typeof(LogoutResponse);
    }
    public override bool PreSendMessage(IMessage message, Action<ErrorInfo> callback)
    {
        LogoutRequest request = CheckAndGetMsg<LogoutRequest>(message);
        if (string.IsNullOrEmpty(request.AccessToken))
        {
            ErrorInfo info = new ErrorInfo();
            info.Result = false;
            info.Info = "token cant null";
            callback.Invoke(info);
            return false;
        }

        return true;
    }

    public override void GetMsaage(IMessage message)
    {
        LogoutResponse response = CheckAndGetMsg<LogoutResponse>(message);
        ErrorInfo info = new ErrorInfo();
        info.Result = false;
        switch (response.ReturnCode)
        {
            case 200:
                info.Result = true;
                break;
            case 201:
                info.Info = "token err";
                break;
            case 202:
                info.Info = "server err";
                break;

            default:
                break;
        }
        getMessgeCallback?.Invoke(info);
    }
}
