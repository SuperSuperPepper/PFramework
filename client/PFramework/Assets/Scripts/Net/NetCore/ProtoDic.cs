
using Google.Protobuf;
using MsgRegisterLogin;
using System;
using System.Collections.Generic;

namespace Proto
{
   public class ProtoDic
   {
       private static List<int> _protoId = new List<int>
       {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
        };

      private static List<Type>_protoType = new List<Type>
      {
            typeof(CreateNewUserByPhoneRequest),
            typeof(CreateNewUserByPhoneResponse),
            typeof(LoginByPhoneRequest),
            typeof(LoginByPhoneResponse),
            typeof(LoginByTokenRequest),
            typeof(LoginByTokenResponse),
            typeof(LogoutRequest),
            typeof(LogoutResponse),
            typeof(RefreshTokenRequest),
            typeof(RefreshTokenResponse),
       };

       private static readonly Dictionary<RuntimeTypeHandle, MessageParser> Parsers = new Dictionary<RuntimeTypeHandle, MessageParser>()
       {
            {typeof(CreateNewUserByPhoneRequest).TypeHandle,CreateNewUserByPhoneRequest.Parser },
            {typeof(CreateNewUserByPhoneResponse).TypeHandle,CreateNewUserByPhoneResponse.Parser },
            {typeof(LoginByPhoneRequest).TypeHandle,LoginByPhoneRequest.Parser },
            {typeof(LoginByPhoneResponse).TypeHandle,LoginByPhoneResponse.Parser },
            {typeof(LoginByTokenRequest).TypeHandle,LoginByTokenRequest.Parser },
            {typeof(LoginByTokenResponse).TypeHandle,LoginByTokenResponse.Parser },
            {typeof(LogoutRequest).TypeHandle,LogoutRequest.Parser },
            {typeof(LogoutResponse).TypeHandle,LogoutResponse.Parser },
            {typeof(RefreshTokenRequest).TypeHandle,RefreshTokenRequest.Parser },
            {typeof(RefreshTokenResponse).TypeHandle,RefreshTokenResponse.Parser },
       };

        public static MessageParser GetMessageParser(RuntimeTypeHandle typeHandle)
        {
            MessageParser messageParser;
            Parsers.TryGetValue(typeHandle, out messageParser);
            return messageParser;
        }

        public static Type GetProtoTypeByProtoId(int protoId)
        {
            int index = _protoId.IndexOf(protoId);
            return _protoType[index];
        }

        public static int GetProtoIdByProtoType(Type type)
        {
            int index = _protoType.IndexOf(type);
            return _protoId[index];
        }

        public static bool ContainProtoId(int protoId)
        {
            if(_protoId.Contains(protoId))
            {
                return true;
            }
            return false;
        }

        public static bool ContainProtoType(Type type)
        {
            if(_protoType.Contains(type))
            {
                return true;
            }
            return false;
        }
    }
}