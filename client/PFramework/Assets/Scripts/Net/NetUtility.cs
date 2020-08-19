using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NetUtility
{
    public static bool IsTelephone(string str_telephone)
    {
        return Regex.IsMatch(str_telephone, @"^(0|86|17951)?(13[0-9]|15[012356789]|17[013678]|18[0-9]|14[57])[0-9]{8}$");
    }

    public static bool IsPassword(string password)
    {
        //密码至少包含 数字和英文，长度6-20
        return Regex.IsMatch(password, @"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$");
    }
}
