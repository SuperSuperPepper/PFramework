using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PNet
{

    public class NetDebug
    {
        public static bool isOpenDebug = true;

        public static void Log(object message)
        {
            if (isOpenDebug)
                UnityEngine.Debug.Log("<color=yellow>[PNet]</color> " + message);
        }

        public static void LogWarning(object message)
        {
            if (isOpenDebug)
                UnityEngine.Debug.LogWarning("<color=yellow>[PNet]</color> " + message);
        }

        public static void LogError(object message)
        {
            if (isOpenDebug)
                UnityEngine.Debug.LogError("<color=yellow>[PNet]</color> " + message);
        }

    }

}
