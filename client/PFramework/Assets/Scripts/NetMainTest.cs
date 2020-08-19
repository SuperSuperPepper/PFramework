using UnityEngine;
using System.Collections;
using PNet;

public class NetMainTest : MonoBehaviour 
{
	void Start () 
    {
        InitNet();
        InitManager();
//        ChatView.OpenView("Prefabs/ChatView/ChatView");
	}

    private void InitNet()
    {
        gameObject.AddComponent<NetManager>();
        //NetManager.Instance.SendConnect();
    }

    private void InitManager()
    {
     
    }

    void LoginByPhoneTest()
    {
        Debug.Log("phone login");
        var handle = HandleBinding.GetHandle(typeof(LoginHandle));
        handle.RegisterCallBack((value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }
            else
            {
                Debug.Log("login success");
            }
        });
        handle.SendMessage(new MsgRegisterLogin.LoginByPhoneRequest() { Phone = "15516350119", Password = "cuberoom1" }, (value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }
        });
    }

    void CreateNewUserTest()
    {
        Debug.Log("create new user");
        var createHandle = HandleBinding.GetHandle<CreateNewUserByPhoneHandle>();
        createHandle.RegisterCallBack((info) =>
        {
            if (info.Result)
            {
                Debug.Log("Create success");
            }
            else
            {
                Debug.Log(info.Info);
            }
        });
        createHandle.SendMessage(new MsgRegisterLogin.CreateNewUserByPhoneRequest() { Phone = "15516350119", Password = "cuberoom1" }, (info) =>
        {
            if (!info.Result)
            {
                Debug.LogError(info.Info);
            }
        });
    }

    void LoginByToken()
    {
        Debug.Log("login by token");
        var handle = HandleBinding.GetHandle(typeof(LoginByTokenHandle));
        handle.RegisterCallBack((value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }
            else
            {
                Debug.Log("login success");
            }
        });
        handle.SendMessage(new MsgRegisterLogin.LoginByTokenRequest() { AccessToken =TokenControl.AccessToken}, (value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }           
        });
    }

    void RefreshToken()
    {
        Debug.Log("refresh by token");

        var handle = HandleBinding.GetHandle(typeof(RefreshTokenHandle));
        handle.RegisterCallBack((value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }
            else
            {
                Debug.Log("login success");
            }
        });
        handle.SendMessage(new MsgRegisterLogin.RefreshTokenRequest() { RefreshToken = TokenControl.RefreshToken }, (value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }
          
        });
    }

    void Logout()
    {
        Debug.Log("logout");
        TokenControl.ClearToken();
        var handle = HandleBinding.GetHandle(typeof(LogoutHandle));
        handle.RegisterCallBack((value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }
            else
            {
                Debug.Log("logout");
            }
        });
        handle.SendMessage(new MsgRegisterLogin.LogoutRequest() { AccessToken = TokenControl.AccessToken }, (value) =>
        {
            if (!value.Result)
            {
                Debug.LogError(value.Info);
            }
          
        });
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            //LoginByPhoneTest();
            LoginByToken();
            //Logout();
            //RefreshToken();

        }
    }
}
