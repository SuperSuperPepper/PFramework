using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenControl 
{
    public static string AccessToken {
        get { return PlayerPrefs.GetString("accessToken",""); }
        set { PlayerPrefs.SetString("accessToken", value); }
    }

    public static string RefreshToken
    {
        get { return PlayerPrefs.GetString("refreshToken", ""); }
        set { PlayerPrefs.SetString("refreshToken", value); }
    }

    public static void ClearToken()
    {
        AccessToken = "";
        RefreshToken = "";
    }
}
