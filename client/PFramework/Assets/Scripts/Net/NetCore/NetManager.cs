using UnityEngine;
using System.Collections.Generic;
using System;
using Proto;
using System.IO;
using Google.Protobuf;

namespace PNet
{
    public class NetManager : MonoBehaviour
    {
        private static NetManager _instance;
        public static NetManager Instance
        {
            get
            {
                return _instance;
            }
        }   

        private Dictionary<Type, NetMsgBaseHandle> _handlerDic;
        private SocketClient _socketClient;
        SocketClient socketClient
        {
            get
            {
                if (_socketClient == null)
                {
                    _socketClient = new SocketClient();
                }
                return _socketClient;
            }
        }
        float tempTime=0;

        public bool IsConnected
        {
            get
            {
                if (_socketClient==null)
                {
                    return false;
                }
                return _socketClient.IsConnected;
            }
        }

        public bool isAutoReconnect = true;
        public float ReconnectInterval = 5f;


        public event Action OnServerConnected;
        public event Action OnServerCantConnected;

        public event Action OnServerDisConnected;



        private void Awake()
        {
            _instance = this;

            Init();
            SendConnect();

        }
      

        public void Init()
        {
            _handlerDic = new Dictionary<Type, NetMsgBaseHandle>();
            socketClient.OnRegister(this);
        }

        /// <summary>
        /// 发送链接请求
        /// </summary>
        public void SendConnect()
        {
            NetDebug.Log("Start Connect");
            socketClient.SendConnect();
        }

        /// <summary>
        /// 关闭网络
        /// </summary>
        public void OnRemove()
        {
            socketClient.OnRemove();
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        public void SendMessage(ByteBuffer buffer)
        {
            socketClient.SendMessage(buffer);
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        public bool SendMessage(IMessage obj)
        {
            if (!ProtoDic.ContainProtoType(obj.GetType()))
            {
                NetDebug.LogError(string.Format("Cant find {0} proto",obj));
                return false;
            }
            ByteBuffer buff = new ByteBuffer();
            int protoId = ProtoDic.GetProtoIdByProtoType(obj.GetType());

            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                obj.WriteTo(ms);
                result = ms.ToArray();
            }

            UInt16 lengh = (UInt16)(result.Length + 2);
            //NetDebug.Log("[Help]lengh" + lengh + ",protoId" + protoId);
            //bigendian
            buff.WriteShort(NetAppConst.IsBigendian == true ? Converter.GetBigEndian((UInt16)lengh) : (UInt16)lengh);
            buff.WriteShort(NetAppConst.IsBigendian == true ? Converter.GetBigEndian((UInt16)protoId) : (UInt16)protoId);
            buff.WriteBytes(result);
            SendMessage(buff);
            return true;
        }

        /// <summary>
        /// 连接 
        /// </summary>
        public void OnConnect()
        {          
            OnServerConnected?.Invoke();
            HandleBinding.Bind();
        }

        public void OnCantConnect()
        {           
            OnServerCantConnected?.Invoke();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void OnDisConnect()
        {
            NetDebug.Log("Disconnected!");
            OnServerDisConnected?.Invoke();
          
        }

        /// <summary>
        /// 派发协议
        /// </summary>
        /// <param name="protoId"></param>
        /// <param name="buff"></param>
        public void DispatchProto(int protoId, byte[] buff)
        {
            if (!ProtoDic.ContainProtoId(protoId))
            {
                NetDebug.LogError("未知协议号");
                return;
            }
            Type protoType = ProtoDic.GetProtoTypeByProtoId(protoId);
            try
            {
                MessageParser messageParser = ProtoDic.GetMessageParser(protoType.TypeHandle);
                var toc = messageParser.ParseFrom(buff);
                sEvents.Enqueue(new KeyValuePair<Type, IMessage>(protoType, toc));
            }
            catch
            {
                NetDebug.LogError("DispatchProto Error:" + protoType.ToString());
            }

        }

        static Queue<KeyValuePair<Type, IMessage>> sEvents = new Queue<KeyValuePair<Type, IMessage>>();
        /// <summary>
        /// 交给Command，这里不想关心发给谁。
        /// </summary>
        void Update()
        {
            if (sEvents.Count > 0)
            {
                while (sEvents.Count > 0)
                {
                    KeyValuePair<Type, IMessage> _event = sEvents.Dequeue();
                    if (_handlerDic.ContainsKey(_event.Key))
                    {
                        _handlerDic[_event.Key].GetMsaage(_event.Value);
                    }
                }
            }
            tempTime += Time.deltaTime;
            if (tempTime >= ReconnectInterval)
            {
                tempTime = 0;
                if (isAutoReconnect && IsConnected == false)
                {
                    SendConnect();
                }
            }
        }

        public void AddHandler(Type type, NetMsgBaseHandle handler)
        {
            if (_handlerDic.ContainsKey(type))
            {
                _handlerDic[type] = handler;
            }
            else
            {
                _handlerDic.Add(type, handler);
            }
        }
    }

}
