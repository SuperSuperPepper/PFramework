using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;
using MsgRegisterLogin;
using PNet;

public class LoginHandle : NetMsgBaseHandle
{
    public override Type SetMessageType()
    {
        return typeof(LoginByPhoneResponse);
    }

    public override void GetMsaage(IMessage message)
    {
        base.GetMsaage(message);
        var response = CheckAndGetMsg<LoginByPhoneResponse>(message); 
        ErrorInfo info = new ErrorInfo();
        info.Result = false;
        
        if (response!=null)
        {
            switch (response.ReturnCode)
            {
                case 200:
                    info.Result = true;
                    Debug.Log("login success");
                    Debug.Log(response.AccessToken);
                    TokenControl.AccessToken = response.AccessToken;
                    TokenControl.RefreshToken = response.RefreshToken;
                    Debug.Log(response.RefreshToken);
                    //todo: add to token control
                    break;
                case 201:
                    info.Info = ("手机号码错误");
                    break;
                case 202:
                    info.Info = ("密码错误");
                    break;
                case 203:
                    info.Info = ("服务器数据库错误");
                    break;
                default:
                    break;
            }                    
        }
        getMessgeCallback?.Invoke(info);        
    }

    public override bool PreSendMessage(IMessage message, Action<ErrorInfo> callback)
    {
        var request = CheckAndGetMsg<LoginByPhoneRequest>(message);
        if (!NetUtility.IsTelephone(request.Phone))
        {
            callback(new ErrorInfo()
            {
                Result = false,
                Info = "输入的手机号码错误"
            } //需要换成多国语言版本
            );
            return false;
        }
        if (!NetUtility.IsPassword(request.Password))
        {
            callback(new ErrorInfo()
            {
                Result = false,
                Info = "密码需要包含数字英文，且在长度在6-20"
            } //需要换成多国语言版本
            );
            return false;
        }

        return true;
    }
   

  
}
