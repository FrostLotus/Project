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
        Act
    }
    public partial class Form1 : Form
    {
        public string STX = "@";
        public string ETX = "#\x0d";
        public string Recive_Code = "";
        string Connect_IP = "192.168.2.99";
        string Local_IP = "192.168.2.104";

        Client_Socket m_client_UnP_Socket = new Client_Socket();
        int UnP_Connect_Port = 4096;
        int Unp_Local_Port = 62000;//非必要 UnPassive 接受任意外Port連線

        Client_Socket m_client_FuP_Socket = new Client_Socket();
        int FuP_Connect_Port = 8192;
        int Fup_Local_Port = 62000;//必要 FullPassive 接受單一Port連線

        Server_Socket m_Server_Act_Socket = new Server_Socket();
        int Act_Client_Port = 8192;
        int Act_Server_Port = 62000;//必要 Active 以SERVER連線必須有端口



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
                m_client_UnP_Socket.Host = Connect_IP;
                m_client_UnP_Socket.Port = UnP_Connect_Port;
                m_client_UnP_Socket.Local_Port = Unp_Local_Port;
                m_client_UnP_Socket.Connect();

                Timer_Timeout.Enabled = false;
                Timer_Timeout.Interval = 30000;
                Timer_Timeout.Elapsed += On_Timeout;

                Timer_Heart_Beat.Enabled = true;
                Timer_Heart_Beat.Interval = 5000;
                Timer_Heart_Beat.Elapsed += On_Heart_Beat;

                m_client_UnP_Socket.OnRecive += PLC_OnRecive;
                m_client_UnP_Socket.OnConnect += PLC_OnConnect;
                m_client_UnP_Socket.OnDisconnect += PLC_OnDisconnect;

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
                if (m_client_UnP_Socket.Connected)
                {
                    string tmpstr = Txt_UnP_WriteData.Text;
                    m_client_UnP_Socket.Write_Clean_CMD(tmpstr);
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

                m_client_UnP_Socket.OnRecive -= PLC_OnRecive;
                m_client_UnP_Socket.OnConnect -= PLC_OnConnect;
                m_client_UnP_Socket.OnDisconnect -= PLC_OnDisconnect;
                m_client_UnP_Socket.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        //----------
        private void Btn_FuP_Connect_Click(object sender, EventArgs e)
        {
            m_client_FuP_Socket.Host = Connect_IP;
            m_client_FuP_Socket.Port = FuP_Connect_Port;

            m_client_FuP_Socket.Connect();

            Timer_Timeout.Enabled = false;
            Timer_Timeout.Interval = 30000;//30
            Timer_Timeout.Elapsed += On_Timeout;

            Timer_Heart_Beat.Enabled = true;
            Timer_Heart_Beat.Interval = 5000;
            Timer_Heart_Beat.Elapsed += On_Heart_Beat;

            m_client_FuP_Socket.OnRecive += PLC_OnRecive;
            m_client_FuP_Socket.OnConnect += PLC_OnConnect;
            m_client_FuP_Socket.OnDisconnect += PLC_OnDisconnect;

        }
        private void Btn_FuP_Write_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_client_FuP_Socket.Connected)
                {
                    string tmpstr = Txt_FuP_WriteData.Text;
                    m_client_FuP_Socket.Write_Clean_CMD(tmpstr);
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

                m_client_FuP_Socket.OnRecive -= PLC_OnRecive;
                m_client_FuP_Socket.OnConnect -= PLC_OnConnect;
                m_client_FuP_Socket.OnDisconnect -= PLC_OnDisconnect;
                m_client_FuP_Socket.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        //----------
        private void Btn_Act_Connect_Click(object sender, EventArgs e)
        {


            m_Server_Act_Socket.Host = Local_IP;
            m_Server_Act_Socket.Port = Act_Server_Port;
            m_Server_Act_Socket.StartServer();

            Timer_Timeout.Enabled = false;
            Timer_Timeout.Interval = 30000;//30
            Timer_Timeout.Elapsed += On_Timeout;

            Timer_Heart_Beat.Enabled = true;
            Timer_Heart_Beat.Interval = 5000;
            Timer_Heart_Beat.Elapsed += On_Heart_Beat;

            m_Server_Act_Socket.OnClientRecive += PLC_OnRecive;
            m_Server_Act_Socket.OnClientConnect += PLC_OnConnect;
            m_Server_Act_Socket.OnClientDisconnect += PLC_OnDisconnect;

        }
        private void Btn_Act_Write_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_Server_Act_Socket.Client_List[0].Connected)
                {
                    string tmpstr = Txt_Act_WriteData.Text;
                    m_Server_Act_Socket.Write_Clean_CMD(m_Server_Act_Socket.Client_List[0], tmpstr);

                    m_Server_Act_Socket.Client_List[0].Write_Clean_CMD( tmpstr);
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
                m_Server_Act_Socket.StopServer();
                //m_Server_Act_Socket.Disconnect();

                //m_Server_Act_Socket.OnClientRecive -= PLC_OnRecive;
                //m_Server_Act_Socket.OnClientConnect -= PLC_OnConnect;
                //m_Server_Act_Socket.OnClientDisconnect -= PLC_OnDisconnect;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

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
        public void PLC_OnConnect(Base_Socket s_socket)
        {
            string read_str = "";
            read_str = s_socket.Recive_String();
            Recive_Code = read_str;

            Console.WriteLine($"NEW PLCConnect");
        }
        public void PLC_OnDisconnect(Base_Socket s_socket)
        {
            switch (((IPEndPoint)s_socket.m_socket.RemoteEndPoint).Port)
            {
                case 4096:
                    Btn_Choice = Connect_Data_Flag.UnP;

                    m_client_UnP_Socket.Disconnect();
                    if (!m_client_UnP_Socket.Connected)
                    {
                        m_client_UnP_Socket.Connect();//斷線後重新連線
                    }
                    break;
                case 8192:
                    Btn_Choice = Connect_Data_Flag.FuP;

                    m_client_FuP_Socket.Disconnect();
                    if (!m_client_FuP_Socket.Connected)
                    {
                        m_client_FuP_Socket.Connect();//斷線後重新連線
                    }
                    break;
                case 7777:
                    //Btn_Choice = Connect_Data_Flag.Act;

                    //m_Server_Act_Socket.Disconnect();
                    //if (!m_Server_Act_Socket.Connected)
                    //{
                    //    m_Server_Act_Socket.Connect();//斷線後重新連線
                    //}
                    break;
            }
        }
        public void PLC_OnRecive(Base_Socket s_socket)
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
               
                for(int i=0,j=0;i< read_byte.Length; i += 2,j++)
                {
                    ushort tmp_word = (ushort)((read_byte[i+1]<<8) | read_byte[i]);
                    byte_to_string[j] = tmp_word.ToString();
                    Console.WriteLine(tmp_word.ToString());
                }
                UpdateTextBoxInvoke();
            }
            Timer_Recive.Enabled = true;
        }
        //---------
       


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
