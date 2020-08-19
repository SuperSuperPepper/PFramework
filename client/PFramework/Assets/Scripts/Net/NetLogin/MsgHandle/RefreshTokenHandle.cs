using PNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MsgRegisterLogin;
using Google.Protobuf;

public class RefreshTokenHandle : NetMsgBaseHandle
{
    public override Type SetMessageType()
    {
        return typeof(RefreshTokenResponse);
    }

    public override bool PreSendMessage(IMessage message, Action<ErrorInfo> callback)
    {
        RefreshTokenRequest request = CheckAndGetMsg<RefreshTokenRequest>(message);
        if (string.IsNullOrEmpty(request.RefreshToken))
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
        RefreshTokenResponse response = CheckAndGetMsg<RefreshTokenResponse>(message);
        ErrorInfo info = new ErrorInfo();
        info.Result = false;
        if (response.ReturnCode==200)
        {
            info.Result = true;
            TokenControl.AccessToken = response.AccessToken;
            TokenControl.RefreshToken = response.RefreshToken;
        }
        else
        {
            info.Info = "token expires";
        }

        getMessgeCallback?.Invoke(info);
    }
}
