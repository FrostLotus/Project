using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketConnect
{
    public class Socket_Client : Socket_Base
    {
        public SocketEvent OnConnect = null;
        public SocketEvent OnDisconnect = null;
        public SocketEvent OnRecive = null;
        public SocketErrorEvent OnError = null;
        public Socket_Client()
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Connect()
        {
            IPAddress m_IPaddress;
            IPEndPoint ipe;

            if (!Connected)
            {
                m_IPaddress = IPAddress.Parse(Host);
                ipe = new IPEndPoint(m_IPaddress, Port);
                try
                {
                    m_socket.Dispose();
                    m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    m_socket.Bind(new IPEndPoint(BitConverter.ToUInt32(IPAddress.Any.GetAddressBytes(), 0), Local_Port));//固定目前IP 固定port號
                    m_socket.BeginConnect(ipe, BeginAccept_Callback, null);
                }
                catch (SocketException e)
                {
                    if (OnError != null) OnError(this, e);
                }
            }
        }
        public void Disconnect()
        {
            try
            {
                m_socket.Disconnect(true);//可重複使用
            }
            catch (SocketException e)
            {
                if (OnError != null) OnError(this, e);
            }
        }
        public bool Check_Connect()
        {
            bool result = false;
            byte[] test_buffer = new byte[1000];

            if (m_socket.Connected)
            {
                if (m_socket.Receive(test_buffer, SocketFlags.Peek) == 0)
                {
                }
            }
            return result;
        }
        private void BeginAccept_Callback(IAsyncResult ar)
        {
            try
            {
                if (m_socket.Connected)
                {
                    if (OnConnect != null) OnConnect(this);
                    BeginReceive(this);
                }
            }
            catch (SocketException e)
            {
            }
        }
        private void Recive_Callback(IAsyncResult ar)
        {
            Socket_Base socket = null;

            socket = (Socket_Base)ar.AsyncState;
            EndReceive(socket, ar);
            BeginReceive(socket);
        }
        private void BeginReceive(Socket_Base socket)
        {
            try
            {
                socket.m_socket.BeginReceive(socket.Buffer, 0, socket.Buffer.Length, SocketFlags.None, Recive_Callback, socket);
            }
            catch (SocketException e)
            {
                //嘗試存取通訊端時發生錯誤。
                if (OnDisconnect != null) OnDisconnect(socket);
            }
            catch (ObjectDisposedException e)
            {
                //這個 Socket 已關閉
                if (OnDisconnect != null) OnDisconnect(socket);
            }
        }
        private void EndReceive(Socket_Base socket, IAsyncResult ar)
        {
            int read_len;

            try
            {
                read_len = socket.m_socket.EndReceive(ar);
                if (read_len <= 0)
                {
                    return;
                }
                else
                {
                    lock (socket.Buffer)
                    {
                        if (read_len + socket.Recive_Length < socket.Buffer.Length)
                        {
                            Array.Copy(socket.Buffer, 0, socket.Buffer, socket.Recive_Length, read_len);
                            socket.Recive_Length = socket.Recive_Length + read_len;
                        }
                    }
                    if (OnRecive != null && socket.Recive_Length > 0) OnRecive(socket);
                }
            }
            catch (SocketException e)
            {
                //嘗試存取通訊端時發生錯誤。
            }
            catch (ObjectDisposedException e)
            {
                //Socket 已關閉。
            }
        }
    }
}
