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
    public delegate void SocketEvent(Socket_Base s_socket);
    public delegate void TCPSocketEvent(Socket s_socket);
    public delegate void TCPSocketDataEvent(byte[] m_byte, int byte_length);
    public delegate void SocketErrorEvent(Socket_Base s_socket, SocketException e);
    public class Socket_Base
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
        /// Start of Text 正文開始
        /// </summary>
        public string STX = "@";
        /// <summary>
        /// End of Text 正文結束
        /// </summary>
        public string ETX = "#\x0d";
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
        public Socket_Base()
        {
        }
        public Socket_Base(Socket socket)
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
                if (max_read_len > Recive_Length) 
                {
                    read_len = Recive_Length;
                } 
                move_len = Buffer.Length - read_len;
                result = new byte[read_len];
                Array.Copy(Buffer, 0, result, 0, read_len);
                Array.Copy(Buffer, read_len, Buffer, 0, move_len);
                Recive_Length = Recive_Length - read_len;
            }
            return result;
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
        public byte[] Recive_Byte()
        {
            return Recive_Byte(Recive_Length);
        }
        //--
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
        public void Send_String(Socket_Base socket,string str)
        {
            byte[] buffer;
            buffer = Encoding.GetEncoding("Big5").GetBytes(str);
            if (socket.m_socket.Connected) socket.m_socket.Send(buffer);
            
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
            //改為Client端是否連接
            if (Connected)
            {
                Send_String(tmp_STX + cmd + tmp_ETX);
                result = true;
            }
            return result;
        }
    }
}
