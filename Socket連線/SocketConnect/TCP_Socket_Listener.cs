using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketConnect
{
    class TCP_Socket_Listener
    {
        public TcpListener m_tcplistener;
        public List<TcpClient> connectedClients = new List<TcpClient>();

        public TCPSocketEvent OnTCPAccept = null;
        public TCPSocketEvent OnTCPClientConnect = null;
        public TCPSocketEvent OnTCPClientDisconnect = null;
        public TCPSocketDataEvent OnTCPClientRecive = null;
        public TCPSocketEvent OnTCPClientSend = null;

        public IPAddress ipAddress = IPAddress.Parse("127.0.0.1");// 或者使用 IPAddress.Any，表示接受所有網路介面的連線
        /// <summary>
        /// IPAddress位址
        /// </summary>
        public string Host = "127.0.0.1";
        /// <summary>
        /// Port通訊埠
        /// </summary>
        public int Port = 7777;
        /// <summary>
        /// 本地端Port通訊埠
        /// </summary>
        public int Local_Port = 62000;
        /// <summary>
        /// 接收資料長度
        /// </summary>
        public int Recive_Length = 0;
        /// <summary>
        /// 資料暫存
        /// </summary>
        public byte[] Buffer = new byte[10000];

        private bool isServerRunning = false;

        public TCP_Socket_Listener()
        {
            isServerRunning = false;
        }
        public void StartServer()
        {
            try
            {
                if (!isServerRunning)
                {
                    ipAddress = IPAddress.Parse(Host);

                    m_tcplistener = new TcpListener(ipAddress, Port);
                    m_tcplistener.Start();
                    isServerRunning = true;
                    // 監聽連線
                    Thread listenThread = new Thread(ListenForClients);
                    listenThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void StopServer()
        {
            try
            {
                isServerRunning = false;
                // 關閉所有連線
                foreach (TcpClient client in connectedClients)
                {
                    client.Close();
                }
                connectedClients.Clear();
                m_tcplistener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error stopping server: " + ex.Message);
            }
        }
        //---------------------------------------
        private void ListenForClients()
        {
            try
            {
                while (isServerRunning)
                {
                    TcpClient client = m_tcplistener.AcceptTcpClient();
                    //if(client!=null)
                    connectedClients.Add(client);

                    Thread clientThread = new Thread(HandleClientComm);
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error listening for clients: " + ex.Message);
            }
        }

        private void HandleClientComm(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            try
            {
                NetworkStream stream = client.GetStream();
                while (isServerRunning)
                {
                    int bytesRead = stream.Read(Buffer, 0, Buffer.Length);
                    if (bytesRead == 0)
                    {
                        break; //斷開連線
                    }

                    string receivedMessage = Encoding.ASCII.GetString(Buffer, 0, bytesRead);
                    Console.WriteLine($"receivedMessage: {receivedMessage}");

                    if (OnTCPClientRecive != null && bytesRead > 0) OnTCPClientRecive(Buffer, bytesRead);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling client communication: " + ex.Message);
            }
            finally
            {
                client.Close();
                connectedClients.Remove(client);
            }
        }

        public void Send_String(Socket m_socket, string str)
        {
            byte[] buffer;
            buffer = Encoding.GetEncoding("Big5").GetBytes(str);
            if (m_socket.Connected) m_socket.Send(buffer);
        }
        public bool Write_Clean_CMD(Socket socket, string cmd, string tmp_STX = "", string tmp_ETX = "")
        {
            bool result = false;
            //改為Client端是否連接

            Send_String(socket, tmp_STX + cmd + tmp_ETX);
            result = true;

            return result;
        }

    }
}
