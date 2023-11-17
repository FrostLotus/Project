using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SocketConnect
{
    public delegate void SocketEvent(Base_Socket s_socket);
    public delegate void SocketErrorEvent(Base_Socket s_socket, SocketException e);
    public class Base_Socket
    {
        /// <summary>
        /// Socket本體
        /// </summary>
        public Socket m_socket = null;
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
        /// <summary>
        /// 實際暫存占用長度
        /// </summary>
        public int Buffer_Max_Length
        {
            get
            {
                return Buffer.Length;
            }
            set
            {
                Array.Resize(ref Buffer, value);
            }
        }
        /// <summary>
        /// Socket本體連線狀態
        /// </summary>
        public bool Connected
        {
            get
            {
                return m_socket.Connected;
            }
        }
        //---------------------------------------------
        public Base_Socket()
        {
        }
        public Base_Socket(Socket socket)
        {
            m_socket = socket;
        }
        //---------------------------------------------
        public byte[] Recive_Byte(int max_read_len)
        {
            byte[] result;
            int read_len = 0;
            int move_len = 0;

            lock (Buffer)
            {
                read_len = max_read_len;
                if (max_read_len > Recive_Length) read_len = Recive_Length;

                move_len = Buffer.Length - read_len;
                result = new byte[read_len];
                Array.Copy(Buffer, 0, result, 0, read_len);
                Array.Copy(Buffer, read_len, Buffer, 0, move_len);
                Recive_Length = Recive_Length - read_len;
            }

            return result;
        }
        public byte[] Recive_Byte()
        {
            return Recive_Byte(Recive_Length);
        }
        public byte[] Recive_Byte(byte[] end_code)
        {
            byte[] result = null;
            int end_pos = Recive_Length;
            int end_code_pos = -1;

            end_code_pos = End_Code_Pos(end_code);
            if (end_code_pos >= 0)
            {
                result = Recive_Byte(end_code_pos + end_code.Length);
            }
            return result;
        }
        public string Recive_String()
        {
            string result;
            byte[] data = null;


            data = Recive_Byte();
            result = Encoding.GetEncoding("Big5").GetString(data);
            Recive_Length = 0;
            return result;
        }
        public string Recive_String(string end_str)
        {
            string result = "";
            byte[] end_code = null;
            byte[] data = null;

            if (end_str != "")
            {
                end_code = Encoding.GetEncoding("Big5").GetBytes(end_str);
                data = Recive_Byte(end_code);
                result = Encoding.GetEncoding("Big5").GetString(data);
            }
            else
                data = Recive_Byte();

            return result;
        }
        private int End_Code_Pos(byte[] end_code)
        {
            int result = -1;
            bool flag;
            int end_code_len = end_code.Length;

            for (int i = 0; i < Recive_Length - end_code_len + 1; i++)
            {
                flag = true;
                for (int j = 0; j < end_code_len; j++)
                {
                    if (Buffer[i + j] != end_code[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
        //----------
        public void Send_Byte(byte[] data, int size, SocketFlags socketFlags)
        {
            if (Connected) m_socket.Send(data, size, socketFlags);
        }
        public void Send_Byte(byte[] data)
        {
            if (m_socket.Connected) m_socket.Send(data);
        }
        public void Send_String(string str)
        {
            byte[] buffer;
            buffer = Encoding.GetEncoding("Big5").GetBytes(str);
            if (m_socket.Connected) m_socket.Send(buffer);
        }
        public void Send_String(Base_Socket socket,string str)
        {
            byte[] buffer;
            buffer = Encoding.GetEncoding("Big5").GetBytes(str);
            if (socket.m_socket.Connected) socket.m_socket.Send(buffer);
            
        }
        public bool Write_Clean_CMD(string cmd, string tmp_STX = "", string tmp_ETX = "")
        {
            bool result = false;
            //改為Client端是否連接
            if (Connected)
            {
                Send_String(tmp_STX + cmd + tmp_ETX);
                result = true;
            }
            return result;
        }

    }
    
    public class Server_Socket : Base_Socket
    {
        public List<Base_Socket> Client_List = new List<Base_Socket>();
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
        public Server_Socket()
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

                    m_socket.BeginAccept(new AsyncCallback(Accept_Callback),  null);
                    if (OnAccept != null) OnAccept(this);

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
        private void Accept_Callback(IAsyncResult ar)
        {
            //if (in_Connected != false)
            {
                Base_Socket clientsocket = null;
                try
                {
                    //"連線成功!"
                    clientsocket = new Base_Socket();
                   
                    if(m_socket!=null)
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
        private void HandleClient(Base_Socket ClientSocket)
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
            Base_Socket clientSocket = (Base_Socket)ar.AsyncState;
            try
            {
                if (clientSocket.Connected)
                {
                    int read_len = clientSocket.m_socket.EndReceive(ar);
                    if (read_len > 0)
                    {
                        //byte[] receivedData = new byte[bytesRead];
                        //Array.Copy(Buffer, receivedData, bytesRead);

                        //string receivedMessage = Encoding.ASCII.GetString(receivedData);
                        //Console.WriteLine("Received: " + receivedMessage);

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
        private void EndReceive(Base_Socket socket, IAsyncResult ar)
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
            foreach (Base_Socket client in Client_List)
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

        private void BeginDisconnect(Base_Socket socket)
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
            Base_Socket socket = null;

            socket = (Base_Socket)ar.AsyncState;
            socket.m_socket.EndDisconnect(ar);
        }


        //public bool Write_Clean_CMD(Base_Socket socket,string cmd, string tmp_STX = "", string tmp_ETX = "")
        //{
        //    bool result = false;
        //    //改為Client端是否連接
        //    if (socket.Connected)
        //    {
        //        Send_String(socket, tmp_STX + cmd + tmp_ETX);
        //        result = true;
        //    }
        //    return result;
        //}
    }
    public class Client_Socket : Base_Socket
    {
        public string STX = "@";
        public string ETX = "#\x0d";
        public SocketEvent OnConnect = null;
        public SocketEvent OnDisconnect = null;
        public SocketEvent OnRecive = null;
        public SocketErrorEvent OnError = null;

        public Client_Socket()
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
            Base_Socket socket = null;

            socket = (Base_Socket)ar.AsyncState;
            EndReceive(socket, ar);
            BeginReceive(socket);
        }
        private void BeginReceive(Base_Socket socket)
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
        private void EndReceive(Base_Socket socket, IAsyncResult ar)
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

        public bool Write_CMD(string cmd)
        {
            bool result = false;

            if (Connected)
            {
                Send_String(STX + cmd + ETX);
                result = true;
            }
            return result;
        }
        public bool Write_Clean_CMD(string cmd, string tmp_STX = "", string tmp_ETX = "")
        {
            bool result = false;

            if (Connected)
            {
                Send_String(tmp_STX + cmd + tmp_ETX);
                result = true;
            }
            return result;
        }
    }
    ///=========================================================================
   
}
