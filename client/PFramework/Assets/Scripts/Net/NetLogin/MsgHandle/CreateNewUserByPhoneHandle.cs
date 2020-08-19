using PNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MsgRegisterLogin;
using Google.Protobuf;

public class CreateNewUserByPhoneHandle : NetMsgBaseHandle
{
    public override Type SetMessageType()
    {
        return typeof(CreateNewUserByPhoneResponse);
    }

    public override bool PreSendMessage(IMessage message, Action<ErrorInfo> callback)
    {
        var request = CheckAndGetMsg<CreateNewUserByPhoneRequest>(message);
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

    public override void GetMsaage(IMessage message)
    {
        Debug.Log("get message");

        base.GetMsaage(message);
        var response =CheckAndGetMsg<CreateNewUserByPhoneResponse>(message);
        ErrorInfo info = new ErrorInfo();
        info.Result = false;
        switch (response.ReturnCode)
        {
            case 200:
                info.Result = true;
                break;
            case 201:
                info.Info = ("该手机号码已经创建");
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
        getMessgeCallback?.Invoke(info);
    }
}
