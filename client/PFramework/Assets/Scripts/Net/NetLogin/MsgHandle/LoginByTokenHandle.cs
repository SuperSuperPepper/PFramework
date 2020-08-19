using MsgRegisterLogin;
using PNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
public class LoginByTokenHandle : NetMsgBaseHandle
{
    public override Type SetMessageType()
    {
        return typeof(LoginByTokenResponse);
    }

    public override bool PreSendMessage(IMessage message, Action<ErrorInfo> callback)
    {
        LoginByTokenRequest request = CheckAndGetMsg<LoginByTokenRequest>(message);       
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
        Debug.Log("get login token");
        LoginByTokenResponse response = CheckAndGetMsg<LoginByTokenResponse>(message);
        ErrorInfo info = new ErrorInfo();
        info.Result = false;
        switch (response.ReturnCode)
        {
            case 200:
                info.Result = true;
                break;
            case 201:
                info.Info = ("token expires");
                break;
            default:
                break;
        }
        getMessgeCallback?.Invoke(info);
    }
}
