using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;

namespace PNet
{
    public enum DisType
    {
        Exception,
        Disconnect,
    }

    public class SocketClient :IDisposable
    {
        private TcpClient client = null;
        private NetworkStream outStream = null;
        private MemoryStream memStream;
        private BinaryReader reader;

        private const int MAX_READ = 8192;
        private byte[] byteBuffer = new byte[MAX_READ];
        private NetManager manager;

        public bool IsConnected
        {
            get
            {
                if (client==null)
                {
                    return false;
                }
                return client.Connected;
            }
        }

       
        // Use this for initialization
        public SocketClient()
        {
        }

        /// <summary>
        /// 注册代理
        /// </summary>
        public void OnRegister(NetManager manager)
        {
            memStream = new MemoryStream();
            reader = new BinaryReader(memStream);
            this.manager = manager;
        }

        /// <summary>
        /// 移除代理
        /// </summary>
        public void OnRemove()
        {
            this.Close();
            reader.Close();
            memStream.Close();
            outStream.Close();            
    }

        /// <summary>
        /// 连接服务器
        /// </summary>
        void ConnectServer(string host, int port)
        {
            client = null;
            client = new TcpClient();
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            try
            {
                client.BeginConnect(host, port, new AsyncCallback(OnConnect), this);
            }
            catch (Exception e)
            {
                Close();
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// 连接上服务器
        /// </summary>
        void OnConnect(IAsyncResult asr)
        {
            if (client.Connected)
            {
                try
                {
                    outStream = client.GetStream();
                    //dont send message just connect
                    client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
                    NetDebug.Log("Connected!");
                    manager.OnConnect();                
                }
                catch (Exception e)
                {
                    NetDebug.LogError(e);
                }
            }
            else
            {
                NetDebug.Log("Cant Connect!");
                manager.OnCantConnect();
            }
           
         
        }

        /// <summary>
        /// 写数据
        /// </summary>
        void WriteMessage(byte[] message)
        {
            if (client != null && client.Connected)
            {
                outStream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                NetDebug.LogError("client.connected----->>false");
            }
        }

        /// <summary>
        /// 读取消息
        /// </summary>
        void OnRead(IAsyncResult asr)
        {
            int bytesRead = 0;
            try
            {
                lock (client.GetStream())
                {         //读取字节流到缓冲区
                    bytesRead = client.GetStream().EndRead(asr);
                }
                if (bytesRead < 1)
                {                //包尺寸有问题，断线处理
                    OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                    return;
                }
                OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
                lock (client.GetStream())
                {         //分析完，再次监听服务器发过来的新消息
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                    client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
                }
            }
            catch (Exception ex)
            {
                //PrintBytes();
                OnDisconnected(DisType.Exception, ex.Message);
            }
        }

        /// <summary>
        /// 丢失链接
        /// </summary>
        void OnDisconnected(DisType dis, string msg)
        {           
            //TODO!!?????
            Close();   //关掉客户端链接
            NetManager.Instance.OnDisConnect();
        }

        /// <summary>
        /// 打印字节
        /// </summary>
        /// <param name="bytes"></param>
        void PrintBytes()
        {
            string returnStr = string.Empty;
            for (int i = 0; i < byteBuffer.Length; i++)
            {
                returnStr += byteBuffer[i].ToString("X2");
            }
            Debug.LogError(returnStr);
        }

        /// <summary>
        /// 向链接写入数据流
        /// </summary>
        void OnWrite(IAsyncResult r)
        {
            try
            {
                outStream.EndWrite(r);
            }
            catch (Exception ex)
            {
                Debug.LogError("OnWrite--->>>" + ex.Message);
            }
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        void OnReceive(byte[] bytes, int length)
        {
            //标记 放到末尾 开始写入
            memStream.Seek(0, SeekOrigin.End);
            memStream.Write(bytes, 0, length);
            //Reset to beginning
            memStream.Seek(0, SeekOrigin.Begin);
            while (RemainingBytes() > 2)
            {
                ushort messageLen = NetAppConst.IsBigendian ==true? Converter.GetBigEndian(reader.ReadUInt16()):reader.ReadUInt16();
                if (RemainingBytes() >= messageLen)
                {
                    MemoryStream ms = new MemoryStream();
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        writer.Write(reader.ReadBytes(messageLen));
                        ms.Seek(0, SeekOrigin.Begin);
                        OnReceivedMessage(ms);
                    }
                }
                else
                {
                    memStream.Position = memStream.Position - 2;
                    break;
                }
            }
            byte[] leftover = reader.ReadBytes((int)RemainingBytes());
            memStream.SetLength(0);
            memStream.Write(leftover, 0, leftover.Length);
        }

        /// <summary>
        /// 剩余的字节
        /// </summary>
        private long RemainingBytes()
        {
            return memStream.Length - memStream.Position;
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="ms"></param>
        void OnReceivedMessage(MemoryStream ms)
        {
            using (BinaryReader r = new BinaryReader(ms))
            {
                //上面 seek 到 0时，position也到0
                //NetDebug.LogError("[TEST]" + ms.Position);

                byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
                ByteBuffer buffer = new ByteBuffer(message);
                //2 length | 2 id | (length-2) message
                //length已经在上面
                int mainId = NetAppConst.IsBigendian == true ? Converter.GetBigEndian(buffer.ReadShort()) : buffer.ReadShort(); 
                int pbDataLen = message.Length - 2;
                byte[] pbData = buffer.ReadBytes(pbDataLen);
                manager.DispatchProto(mainId, pbData);
                buffer.Dispose();
            }
        }


        /// <summary>
        /// 会话发送
        /// </summary>
        void SessionSend(byte[] bytes)
        {
            WriteMessage(bytes);
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        public void Close()
        {
            if (client != null)
            {
                if (client.Connected) client.Close();
                //client = null;
            }
        }

        /// <summary>
        /// 发送连接请求
        /// </summary>
        public void SendConnect()
        {
            if (client!=null&& client.Connected)
            {
                return;
            }
            ConnectServer(NetAppConst.SocketAddress, NetAppConst.SocketPort);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(ByteBuffer buffer)
        {
            SessionSend(buffer.ToBytes());
            buffer.Close();
        }

        public void Dispose()
        {
            client.Dispose();
            outStream.Dispose();
            memStream.Dispose();
            reader.Dispose();
            manager = null;
            byteBuffer = null;
        }
    }
}