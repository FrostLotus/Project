//#define WriteConsole

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.Net;

namespace SocketConnect
{
    enum Connect_Data_Flag
    {
        None,
        UnP,
        FuP,
        Act,
        Thread_Act
    }
    public partial class Form1 : Form
    {
        public string STX = "@";
        public string ETX = "#\x0d";
        public string Recive_Code = "";
        string Connect_IP = "192.168.2.99";
        string Local_IP = "192.168.2.104";


        Socket_Client m_Socket_client_UnP = new Socket_Client();
        int UnP_Connect_Port = 4096;
        int Unp_Local_Port = 61000;//非必要 UnPassive 接受任意外Port連線

        Socket_Client m_Socket_client_FuP = new Socket_Client();
        int FuP_Connect_Port = 8192;
        int Fup_Local_Port = 62000;//必要 FullPassive 接受單一Port連線

        Socket_Server m_Socket_Server_Act = new Socket_Server();
        TCP_Socket_Listener m_TCP_Socket_Listener_Act = new TCP_Socket_Listener();
        int Act_Client_Port = 7168;
        int Act_Server_Port = 60000;//必要 Active 以SERVER連線必須有端口

        string[] byte_to_string;
        protected System.Timers.Timer Timer_Timeout = new System.Timers.Timer();
        protected System.Timers.Timer Timer_Heart_Beat = new System.Timers.Timer();
        protected System.Timers.Timer Timer_Recive = new System.Timers.Timer();
        public bool Timeout = false;
        public bool RIC_Recive = false;
        Connect_Data_Flag Btn_Choice = Connect_Data_Flag.None;

        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_UnP_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                m_Socket_client_UnP.Host = Connect_IP;
                m_Socket_client_UnP.Port = UnP_Connect_Port;
                m_Socket_client_UnP.Local_Port = Unp_Local_Port;
                m_Socket_client_UnP.Connect();

                Timer_Timeout.Enabled = false;
                Timer_Timeout.Interval = 30000;
                Timer_Timeout.Elapsed += On_Timeout;

                Timer_Heart_Beat.Enabled = true;
                Timer_Heart_Beat.Interval = 5000;
                Timer_Heart_Beat.Elapsed += On_Heart_Beat;

                m_Socket_client_UnP.OnRecive += PLC_OnRecive;
                m_Socket_client_UnP.OnConnect += PLC_OnConnect;
                m_Socket_client_UnP.OnDisconnect += PLC_OnDisconnect;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private void Btn_UnP_Write_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_Socket_client_UnP.Connected)
                {
                    string tmpstr = Txt_UnP_WriteData.Text;
                    m_Socket_client_UnP.Write_Clean_CMD(tmpstr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private void Btn_UnP_Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                Timer_Timeout.Elapsed -= On_Timeout;
                Timer_Heart_Beat.Elapsed -= On_Heart_Beat;
                //Timer_Recive.Elapsed -= On_Recive;

                m_Socket_client_UnP.OnRecive -= PLC_OnRecive;
                m_Socket_client_UnP.OnConnect -= PLC_OnConnect;
                m_Socket_client_UnP.OnDisconnect -= PLC_OnDisconnect;
                m_Socket_client_UnP.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        //----------
        private void Btn_FuP_Connect_Click(object sender, EventArgs e)
        {
            m_Socket_client_FuP.Host = Connect_IP;
            m_Socket_client_FuP.Port = FuP_Connect_Port;
            m_Socket_client_FuP.Local_Port = Fup_Local_Port;
            m_Socket_client_FuP.Connect();

            Timer_Timeout.Enabled = false;
            Timer_Timeout.Interval = 30000;//30
            Timer_Timeout.Elapsed += On_Timeout;

            Timer_Heart_Beat.Enabled = true;
            Timer_Heart_Beat.Interval = 5000;
            Timer_Heart_Beat.Elapsed += On_Heart_Beat;

            m_Socket_client_FuP.OnRecive += PLC_OnRecive;
            m_Socket_client_FuP.OnConnect += PLC_OnConnect;
            m_Socket_client_FuP.OnDisconnect += PLC_OnDisconnect;

        }
        private void Btn_FuP_Write_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_Socket_client_FuP.Connected)
                {
                    string tmpstr = Txt_FuP_WriteData.Text;
                    m_Socket_client_FuP.Write_Clean_CMD(tmpstr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private void Btn_FuP_Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                Timer_Timeout.Elapsed -= On_Timeout;
                Timer_Heart_Beat.Elapsed -= On_Heart_Beat;
                //Timer_Recive.Elapsed -= On_Recive;

                m_Socket_client_FuP.OnRecive -= PLC_OnRecive;
                m_Socket_client_FuP.OnConnect -= PLC_OnConnect;
                m_Socket_client_FuP.OnDisconnect -= PLC_OnDisconnect;
                m_Socket_client_FuP.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        //----------
        private void Btn_Act_Connect_Click(object sender, EventArgs e)
        {
            m_Socket_Server_Act.Host = Local_IP;
            m_Socket_Server_Act.Port = Act_Server_Port;
            m_Socket_Server_Act.StartServer();

            Timer_Timeout.Enabled = false;
            Timer_Timeout.Interval = 30000;//30
            Timer_Timeout.Elapsed += On_Timeout;

            Timer_Heart_Beat.Enabled = true;
            Timer_Heart_Beat.Interval = 5000;
            Timer_Heart_Beat.Elapsed += On_Heart_Beat;

            m_Socket_Server_Act.OnClientRecive += PLC_OnRecive;
            m_Socket_Server_Act.OnClientConnect += PLC_OnConnect;
            m_Socket_Server_Act.OnClientDisconnect += PLC_OnDisconnect;
        }
        private void Btn_Act_Write_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_Socket_Server_Act.Client_List[0].Connected)
                {
                    string tmpstr = Txt_Act_WriteData.Text;
                    m_Socket_Server_Act.Client_List[0].Write_Clean_CMD(tmpstr);
                }
                else
                {
                    Console.WriteLine("Client尚未連接");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private void Btn_Act_Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                Timer_Timeout.Elapsed -= On_Timeout;
                Timer_Heart_Beat.Elapsed -= On_Heart_Beat;
                //Timer_Recive.Elapsed -= On_Recive;
                m_Socket_Server_Act.StopServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        //----------
        private void Btn_Act_Thread_Connect_Click(object sender, EventArgs e)
        {
            m_TCP_Socket_Listener_Act.Host = Local_IP;
            m_TCP_Socket_Listener_Act.Port = Act_Server_Port;
            m_TCP_Socket_Listener_Act.StartServer();

            Timer_Timeout.Enabled = false;
            Timer_Timeout.Interval = 30000;//30
            Timer_Timeout.Elapsed += On_Timeout;

            Timer_Heart_Beat.Enabled = true;
            Timer_Heart_Beat.Interval = 5000;
            Timer_Heart_Beat.Elapsed += On_Heart_Beat;

            m_TCP_Socket_Listener_Act.OnTCPClientRecive += PLC_OnTCPRecive;
            //m_TCP_Socket_Listener_Act.OnTCPClientConnect += PLC_OnTCPConnect;
            //m_TCP_Socket_Listener_Act.OnTCPClientDisconnect += PLC_OnTCPDisconnect;
        }
        private void Btn_Act_Thread_Write_Click(object sender, EventArgs e)
        {

        }
        private void Btn_Act_Thread_Disconnect_Click(object sender, EventArgs e)
        {
            m_TCP_Socket_Listener_Act.StopServer();
        }
        //timer-------------------------------------------------------------------
        private void On_Timeout(object sender, EventArgs e)
        {
            Timer_Timeout.Enabled = false;
            Console.WriteLine("Timeout");
            Timeout = true;
        }
        private void On_Heart_Beat(object sender, EventArgs e)
        {
            Timer_Heart_Beat.Enabled = false;
            //心跳
            //Write_CMD("0001-3");
            Timer_Heart_Beat.Enabled = true;
        }
        private void On_Recive(object sender, EventArgs e)
        {
            Timer_Recive.Enabled = false;

            //Connect();
            Timer_Recive.Enabled = true;
        }
        //OnEvent-----------------------------------------------------------------
        public void PLC_OnConnect(Socket_Base s_socket)
        {
            string read_str = "";
            read_str = s_socket.Recive_String();
            Recive_Code = read_str;
            Console.WriteLine($"NEW PLCConnect");
        }
        public void PLC_OnDisconnect(Socket_Base s_socket)
        {
            switch (((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Port)
            {
                case 4096:
                    Btn_Choice = Connect_Data_Flag.UnP;

                    m_Socket_client_UnP.Disconnect();
                    if (!m_Socket_client_UnP.Connected)
                    {
                        m_Socket_client_UnP.Connect();//斷線後重新連線
                    }
                    break;
                case 8192:
                    Btn_Choice = Connect_Data_Flag.FuP;

                    m_Socket_client_FuP.Disconnect();
                    if (!m_Socket_client_FuP.Connected)
                    {
                        m_Socket_client_FuP.Connect();//斷線後重新連線
                    }
                    break;
                case 7168:
                    //Btn_Choice = Connect_Data_Flag.Act;

                    //m_Server_Act_Socket.Disconnect();
                    //if (!m_Server_Act_Socket.Connected)
                    //{
                    //    m_Server_Act_Socket.Connect();//斷線後重新連線
                    //}
                    break;
            }
        }
        public void PLC_OnRecive(Socket_Base s_socket)
        {
            Timer_Recive.Enabled = false;
            string read_str = "";
            byte[] read_byte;

            read_byte = s_socket.Recive_Byte();
            read_str = s_socket.Recive_String();

            switch (((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Port)
            {
                case 4096:
                    Btn_Choice = Connect_Data_Flag.UnP;
                    break;
                case 8192:
                    Btn_Choice = Connect_Data_Flag.FuP;
                    break;
                case 7168:
                    Btn_Choice = Connect_Data_Flag.Act;
                    break;
            }
            Console.WriteLine($"PLC Address:{((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Address}");
            Console.WriteLine($"PLC Port:{((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Port}");

            byte_to_string = new string[read_byte.Length / 2];
            if (Is_Link_Test(read_str))
            {
                //如果是連結測試
                Console.WriteLine(read_str);
            }
            else
            {
                //其他Code
                //RIC_Recive = true;
                Recive_Code = read_str;

                for (int i = 0, j = 0; i < read_byte.Length; i += 2, j++)
                {
                    ushort tmp_word = (ushort)((read_byte[i + 1] << 8) | read_byte[i]);
                    byte_to_string[j] = tmp_word.ToString();
                    Console.WriteLine(tmp_word.ToString());
                }
                UpdateTextBoxInvoke();
            }
            Timer_Recive.Enabled = true;
        }

        public void PLC_OnTCPRecive(byte[] m_byte,int byte_length)
        {
            Timer_Recive.Enabled = false;
            byte[] read_byte;

            read_byte = m_byte;
            Btn_Choice = Connect_Data_Flag.Thread_Act;
            //switch (((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Port)
            //{
            //    case 4096:
            //        Btn_Choice = Connect_Data_Flag.UnP;
            //        break;
            //    case 8192:
            //        Btn_Choice = Connect_Data_Flag.FuP;
            //        break;
            //    case 7168:
            //        Btn_Choice = Connect_Data_Flag.Act;
            //        break;
            //}
            //Console.WriteLine($"PLC Address:{((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Address}");
            //Console.WriteLine($"PLC Port:{((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Port}");

            byte_to_string = new string[byte_length / 2];

            for (int i = 0, j = 0; i < byte_length; i += 2, j++)
            {
                ushort tmp_word = (ushort)((read_byte[i + 1] << 8) | read_byte[i]);
                byte_to_string[j] = tmp_word.ToString();
                Console.WriteLine(tmp_word.ToString());
            }
            UpdateTextBoxInvoke();

            Timer_Recive.Enabled = true;
        }


        //UpdateInvoke------------------------------------------------------------
        public void UpdateTextBoxInvoke()
        {
            if (this.InvokeRequired)
            {
                Invoke(new Action(() => { UpdateTextBoxInvoke(); }));
            }
            else
            {
                UpdateTextBox(Btn_Choice);
            }
        }
        private void UpdateTextBox(Connect_Data_Flag _Flag)
        {
            switch (_Flag)
            {
                case Connect_Data_Flag.UnP:
                    Add_New_Recive(Txt_UnP_ReadData);
                    break;
                case Connect_Data_Flag.FuP:
                    Add_New_Recive(Txt_FuP_ReadData);
                    break;
                case Connect_Data_Flag.Act:
                    Add_New_Recive(Txt_Act_ReadData);
                    break;
                case Connect_Data_Flag.Thread_Act:
                    Add_New_Recive(Txt_Act_Thread_ReadData);
                    break;

            }
        }
        private void Add_New_Recive(TextBox Txt_ReadData)
        {
            string[] arrData = new string[Txt_ReadData.Lines.Length + byte_to_string.Length + 1];
            //前面的歷史項
            for (int i = 0; i < Txt_ReadData.Lines.Length; i++)
            {
                arrData[i] = Txt_ReadData.Lines[i];
            }
            //追加
            for (int i = 0; i < byte_to_string.Length; i++)
            {
                //由前面陣列大小作持續編排(-1=順序)
                arrData[Txt_ReadData.Lines.Length + i] = byte_to_string[i];
            }
            arrData[arrData.Length - 1] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Txt_ReadData.Lines = arrData;
        }
        public bool Is_Link_Test(string read_str)
        {
            bool result = false;
            //心跳 Herat_Beat Client連結
            if (read_str == "@0002-#" || read_str == "@0004-#" || read_str == "@0008-#")
            {
                result = true;
            }
            return result;
        }


    }
}
