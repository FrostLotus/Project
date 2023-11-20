using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketConnect
{
    public class Socket_Server : Socket_Base
    {
        public List<Socket_Base> Client_List = new List<Socket_Base>();
        public SocketEvent OnAccept = null;
        public SocketEvent OnClientConnect = null;
        public SocketEvent OnClientDisconnect = null;
        public SocketEvent OnClientRecive = null;
        public SocketEvent OnClientSend = null;
        public SocketErrorEvent OnError = null;

        private bool in_Connected = false;

        public new bool Connected
        {
            get
            {
                return in_Connected;
            }
        }
        public Socket_Server()
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Buffer_Max_Length = 32000000;
        }
        public void Dispone()
        {
        }
        public void StartServer()
        {
            IPAddress ipa;
            IPEndPoint ipe;

            if (!in_Connected)
            {
                ipa = IPAddress.Parse(Host);
                ipe = new IPEndPoint(ipa, Port);

                try
                {
                    //socket.
                    m_socket.Bind(ipe);
                    m_socket.Listen(10);
                    Console.WriteLine($"Server Start IP: {Host},Port:{Port}");

                    m_socket.BeginAccept(new AsyncCallback(Accept_Callback), null);
                    if (OnAccept != null) OnAccept(this);

                    //設定網路斷線監聽
                    m_socket.IOControl(IOControlCode.KeepAliveValues, GetKeepAliveSetting(1, 5000, 500), null);

                    Console.WriteLine("Server started. Waiting for connections...");

                    in_Connected = true;
                }
                catch (SocketException e)
                {
                    if (OnError != null) OnError(this, e);
                }
            }
        }
        public void StopServer()
        {
            // 主動斷開已建立的客戶端連線
            CloseAllClientConnections();
            Console.WriteLine("Server Clean All Client Connect");
            m_socket.Close();
            Console.WriteLine("Server stopped.");
        }
        //--------------------------------------------------------------
        private byte[] GetKeepAliveSetting(int onOff, int keepAliveTime, int keepAliveInterval)
        {
            byte[] buffer = new byte[12];
            BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
            BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
            BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
            return buffer;
        }
        private void Accept_Callback(IAsyncResult ar)
        {
            //if (in_Connected != false)
            {
                Socket_Base clientsocket = null;
                try
                {
                    //"連線成功!"
                    clientsocket = new Socket_Base();

                    if (m_socket != null)
                        clientsocket.m_socket = this.m_socket.EndAccept(ar);
                    clientsocket.Buffer_Max_Length = Buffer_Max_Length;



                    Client_List.Add(clientsocket);

                    Console.WriteLine($"New Client connected: IP: {((IPEndPoint)clientsocket.m_socket.RemoteEndPoint).Address} , Port: {((IPEndPoint)clientsocket.m_socket.RemoteEndPoint).Port}");

                    if (OnClientConnect != null) OnClientConnect(clientsocket);

                    HandleClient(clientsocket);
                }
                catch (SocketException e)
                {
                }
            }

        }
        private void HandleClient(Socket_Base ClientSocket)
        {
            try
            {
                ClientSocket.m_socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ClientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket_Base clientSocket = (Socket_Base)ar.AsyncState;
            try
            {
                if (clientSocket.Connected)
                {
                    int read_len = clientSocket.m_socket.EndReceive(ar);
                    if (read_len > 0)
                    {
                        // 處理接收到的訊息
                        lock (clientSocket.Buffer)
                        {
                            if (read_len + clientSocket.Recive_Length < clientSocket.Buffer.Length)
                            {
                                Array.Copy(clientSocket.Buffer, 0, clientSocket.Buffer, clientSocket.Recive_Length, read_len);
                                clientSocket.Recive_Length = clientSocket.Recive_Length + read_len;
                            }
                        }
                        if (OnClientRecive != null && clientSocket.Recive_Length > 0) OnClientRecive(clientSocket);

                        // 在此處理完訊息後，重新等待接收新訊息
                        clientSocket.m_socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
                    }
                    else
                    {
                        // 客戶端斷開連線
                        Console.WriteLine("Client disconnected.");
                        clientSocket.m_socket.Close();

                        //// 停止伺服器的功能（停止接受新的連線）
                        //this.Socket.Close();
                        //Console.WriteLine("Server stopped listening for new connections.");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
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
                    if (OnClientRecive != null && socket.Recive_Length > 0) OnClientRecive(socket);
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

        private void CloseAllClientConnections()
        {
            // 逐一斷開已建立的客戶端連線
            // 在此處應該擁有記錄所有客戶端連線的方法，例如使用 List<Socket> 來追蹤連線
            // 這裡僅僅是一個示例，實際應用需要更好的管理連線資訊
            // 將以下的 clientsList 換成你自己實現的客戶端連線列表
            foreach (Socket_Base client in Client_List)
            {
                if (client != null && client.Connected)
                {
                    try
                    {
                        client.m_socket.Shutdown(SocketShutdown.Both);
                        client.m_socket.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error closing client connection: " + ex.Message);
                    }
                }
            }
        }

        private void BeginDisconnect(Socket_Base socket)
        {
            try
            {
                socket.m_socket.BeginDisconnect(true, Disconnect_Callback, socket);
            }
            catch (SocketException e)
            {
            }
        }
        private void Disconnect_Callback(IAsyncResult ar)
        {
            Socket_Base socket = null;

            socket = (Socket_Base)ar.AsyncState;
            socket.m_socket.EndDisconnect(ar);
        }
    }
}
