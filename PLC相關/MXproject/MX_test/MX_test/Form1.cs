using System;
using System.Timers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActProgTypeLib;
using ActUtlTypeLib;
using ActSupportMsgLib;

namespace MX_test
{
    public partial class Form1 : Form
    {
        delegate void UpdateControl(Control Ctrl, string Msg);
        private object _objLock = new object();

        public MX_Component Mx_Connect = new MX_Component();
        private System.Timers.Timer Timer_DeviceGet = new System.Timers.Timer();
        public Form1()
        {
            InitializeComponent();
            LB_State.Text = "";//清除顯示
            interTimeSet();//預先將當下時間寫入
            //委派 將OnDeviceStatus的資料經由外部EventHandler帶回給主程式用
            Mx_Connect.Prog_Connect.OnDeviceStatus += new _IActProgTypeEvents_OnDeviceStatusEventHandler(this.OnDeviceStatus);
            //this.Timer_DeviceGet.SynchronizingObject = this;
            Timer_DeviceGet.Enabled = false;
            Timer_DeviceGet.Interval = 15000;
            Timer_DeviceGet.Elapsed += On_DeviceGet;
        }
        ///=========================================================
        #region Utility
        #region Open/Close
        private void Btn_UtlOpen_Click(object sender, EventArgs e)
        {

            int iReturnCode;  //回傳代碼
            LB_State.Text = "";
            try
            {
                Mx_Connect.Ult_Connect.ActLogicalStationNumber = 1;
                Mx_Connect.Ult_Connect.ActPassword = "";

                //--------------------------------------------
                iReturnCode = Mx_Connect.Ult_Connect.Open();

                if (iReturnCode == 0)
                { LB_State.Text = "Utl連線成功"; }
                else
                { LB_State.Text = "Utl連線失敗"; }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        private void Btn_UtlClose_Click(object sender, EventArgs e)
        {
            int iReturnCode;  //回傳代碼
            try
            {
                iReturnCode = Mx_Connect.Ult_Connect.Close();
                if (iReturnCode == 0)
                { LB_State.Text = "Utl關閉成功"; }
                else
                { LB_State.Text = "Utl關閉失敗"; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        #endregion
        #endregion
        ///=========================================================
        #region Program
        #region Open/Close
        private void btn_ProgOpen_Click(object sender, EventArgs e)
        {
            int iReturnCode;  //回傳代碼
            try
            {
                iReturnCode = Mx_Connect.ProgOpen();
                if (iReturnCode == 0)
                { LB_State.Text = "Prog連線成功"; }
                else
                { LB_State.Text = "Prog連線失敗"; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        private void btn_ProgClose_Click(object sender, EventArgs e)
        {
            int iReturnCode;  //回傳代碼
            try
            {
                iReturnCode = Mx_Connect.ProgClose();

                if (iReturnCode == 0)
                { LB_State.Text = "Prog關閉成功"; }
                else
                { LB_State.Text = "Prog關閉失敗"; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        #endregion
        //-----------
        #region (int) Read/Write Random
        //int可以溢位要小心值控制
        //對應位置讀取/寫入 EX:D0,D5,D8 讀取=> (Device = D0\nD5\nD8, Size = 3, [])
        //                              寫入=> (Device = D0\nD5\nD8, Size = 5, [1\n2\n3])=>D0=1;D5=2;D8=3;?=58422(隨機抓);?=5266548(隨機抓);
        private void btn_ReadDeviceRandom_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            string DeviceList = "";        //軟元件標籤名稱(List)
            int iSize = 0;		          //DataSize資料點數
            int[] arrDeviceData;		  //DeviceData資料矩陣

            string[] arrData;	          //資料(矩陣)

            //取得標籤名稱
            DeviceList = string.Join("\n", txt_DeviceNameRandom.Lines);//將把text中所有標籤串成List
            //確認設備資料位寬有無問題
            if (!Mx_Connect.GetIntValue(txt_SizeRandom, out iSize))
            {
                return;//失敗則break
            }
            //設備參數(以資料大小)
            arrDeviceData = new int[iSize];
            //-------------------------------------------------------
            //ReadDeviceRandom
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadDeviceRandom(DeviceList, iSize, out arrDeviceData[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //-----------------------------------------------------------
            if (iReturnCode == 0)
            {
                //創建資料陣列
                arrData = new string[iSize];
                //對位轉貼
                for (int i = 0; i < iSize; i++)
                {
                    //將LabelData(short)=>Data(string)
                    arrData[i] = arrDeviceData[i].ToString();
                }
                //顯示在Output Textbox
                txt_Data.Lines = arrData;
            }
        }
        private void btn_WriteDeviceRandom_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sDeviceName = "";       //軟元件名稱(List)
            int iSize = 0;		           //資料大小
            int[] arrDeviceData;		   //資料(矩陣)

            //取得標籤名稱
            sDeviceName = string.Join("\n", txt_DeviceNameRandom.Lines);
            //確認資料位寬有無問題
            if (!Mx_Connect.GetIntValue(txt_SizeRandom, out iSize))
            {
                return;//失敗則break
            }
            //設定參數大小(以DataSize為準)
            arrDeviceData = new int[iSize];
            //確認寫入資料有無問題
            if (!Mx_Connect.GetIntArray(txt_LabelDataRandom, out arrDeviceData))
            {
                return;//失敗則break
            }
            //WriteDeviceRandom
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.WriteDeviceRandom(sDeviceName, iSize, ref arrDeviceData[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        #region (int) Read/Write Block
        //int可以溢位要小心值控制
        //批量讀取/寫入 EX:D0,D1,D2 讀取=> (Device = D0(單一軟元件為開頭), Size = 3, [])
        //                          寫入=> (Device = D0(單一軟元件為開頭), Size = 5, [1,2,3])=>D0=1;D1=2;D2=3;D3=4;D4=5;
        private void btn_ReadDeviceBlock_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            int[] arrLabelData;         //資料(矩陣)
            string[] arrData;	          //標籤(矩陣)

            //取得標籤名稱(以分行"\n"作鏈結)
            sLabelName = string.Join("\n", txt_LabelNameBlock.Lines);
            //取得資料大小
            if (!Mx_Connect.GetIntValue(txt_DataSizeBlock, out iDataSize))
            {
                return;//失敗則break
            }
            //設定LabelData矩陣大小
            arrLabelData = new int[iDataSize];
            //ReadDeviceBlock
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadDeviceBlock(sLabelName, iDataSize, out arrLabelData[0]);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                arrData = new string[iDataSize];
                for (int i = 0; i < iDataSize; i++)
                {
                    arrData[i] = arrLabelData[i].ToString();
                }
                txt_Data.Lines = arrData;
            }
        }
        private void btn_WriteDeviceBlock_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            int[] arrLabelData;		  //資料(矩陣)

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNameBlock.Lines);
            //確認資料位寬有無問題
            if (!Mx_Connect.GetIntValue(txt_DataSizeBlock, out iDataSize))
            {
                return;//失敗則break
            }
            //設定參數大小(以DataSize為準)
            arrLabelData = new int[iDataSize];
            //確認寫入資料有無問題
            if (!Mx_Connect.GetIntArray(txt_LabelDataBlock, out arrLabelData))
            {
                return;//失敗則break
            }
            //WriteDeviceBlock
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.WriteDeviceBlock(sLabelName, iDataSize, ref arrLabelData[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        #region (int) Get/Set Device(PointData)
        //最簡易了解，對單一軟元件(標籤)進行值讀取/寫入之方法
        private void btn_GetDevice_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            string sLabelName = "";       //LabelName標籤名稱(List)
            int arrLabelData;		  //LabelData資料矩陣

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNamePoint.Lines);//將把text中所有標籤鏈結起
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.GetDevice(sLabelName, out arrLabelData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //-----------------------------------------------------------
            if (iReturnCode == 0)
            {
                txt_Data.Text = arrLabelData.ToString();
            }
        }
        private void btn_SetDevice_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            //short
            string sLabelName = "";       //LabelName標籤名稱(List)
            int arrLabelData;		  //LabelData資料矩陣

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNamePoint.Lines);//將把text中所有標籤鏈結起
            if (!Mx_Connect.GetIntValue(txt_LabelDataPoint, out arrLabelData))
            {
                return;//失敗則break
            }
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.SetDevice(sLabelName, arrLabelData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        #region (short) Read/Write Random2
        //short不可溢位(會有錯誤訊息)請小心
        //對應位置讀取/寫入 EX:D0,D5,D8 讀取=> (Device = D0\nD5\nD8, Size = 3, [])
        //                              寫入=> (Device = D0\nD5\nD8, Size = 5, [1\n2\n3])=>D0=1;D5=2;D8=3;?=58422(隨機抓);?=5266548(隨機抓);
        private void btn_ReadDeviceRandom2_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            //short
            string sLabelName = "";       //LabelName標籤名稱(List)
            int iDataSize = 0;		      //DataSize資料大小
            short[] arrLabelData;		  //LabelData資料矩陣

            string[] arrData;	          //標籤(矩陣)

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNameRandom2.Lines);//將把text中所有標籤鏈結起
            //取得設備資料位寬
            if (!Mx_Connect.GetIntValue(txt_DataSizeRandom2, out iDataSize))
            {
                return;//失敗則break
            }
            //設備參數(以資料大小)
            arrLabelData = new short[iDataSize];
            //-------------------------------------------------------
            //ReadDeviceRandom2
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadDeviceRandom2(sLabelName, iDataSize, out arrLabelData[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //-----------------------------------------------------------
            if (iReturnCode == 0)
            {
                //創建資料陣列
                arrData = new string[iDataSize];
                //對位轉貼
                for (int i = 0; i < iDataSize; i++)
                {
                    //將LabelData(short)=>Data(string)
                    arrData[i] = arrLabelData[i].ToString();
                }
                //顯示在Output Textbox
                txt_Data.Lines = arrData;
            }
        }
        private void btn_WriteDeviceRandom2_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            short[] arrLabelData;		  //資料(矩陣)

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNameRandom2.Lines);
            //確認資料位寬有無問題
            if (!Mx_Connect.GetIntValue(txt_DataSizeRandom2, out iDataSize))
            {
                return;//失敗則break
            }
            //設定參數大小(以DataSize為準)
            arrLabelData = new short[iDataSize];
            //確認寫入資料有無問題
            if (!Mx_Connect.GetShortArray(txt_LabelDataRandom2, out arrLabelData))
            {
                return;//失敗則break
            }
            //WriteDeviceRandom2
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.WriteDeviceRandom2(sLabelName, iDataSize, ref arrLabelData[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        #region (short) Read/Write Block2
        //short不可溢位(會有錯誤訊息)請小心
        //批量讀取/寫入 EX:D0,D1,D2 讀取=> (Device = D0(單一軟元件為開頭), Size = 3, [])
        //                          寫入=> (Device = D0(單一軟元件為開頭), Size = 5, [1,2,3])=>D0=1;D1=2;D2=3;D3=4;D4=5;
        private void btn_ReadDeviceBlock2_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            short[] arrLabelData;         //資料(矩陣)
            string[] arrData;	          //標籤(矩陣)

            //取得標籤名稱(以分行"\n"作鏈結)
            sLabelName = string.Join("\n", txt_LabelNameBlock2.Lines);
            //取得資料大小
            if (!Mx_Connect.GetIntValue(txt_DataSizeBlock2, out iDataSize))
            {
                return;//失敗則break
            }
            //設定LabelData矩陣大小
            arrLabelData = new short[iDataSize];
            //ReadDeviceBlock2
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadDeviceBlock2(sLabelName, iDataSize, out arrLabelData[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                arrData = new string[iDataSize];
                for (int i = 0; i < iDataSize; i++)
                {
                    arrData[i] = arrLabelData[i].ToString();
                }
                txt_Data.Lines = arrData;
            }
        }
        private void btn_WriteDeviceBlock2_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            short[] arrLabelData;		  //資料(矩陣)

            int iSizeOfIntArray;

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNameBlock2.Lines);
            //確認資料位寬有無問題
            if (!Mx_Connect.GetIntValue(txt_DataSizeBlock2, out iDataSize))
            {
                return;//失敗則break
            }
            //取得總輸出資料大小
            iSizeOfIntArray = txt_LabelDataBlock2.Lines.Length;


            //設定參數大小(以DataSize為準)
            arrLabelData = new short[iDataSize];

            for (int i = 0; i < iSizeOfIntArray; i++)
            {
                try
                {
                    string tmp = txt_LabelDataBlock2.Lines[i];
                    arrLabelData[i] = short.Parse(txt_LabelDataBlock2.Lines[i]);
                }
                //Exception processing
                catch (Exception exExcepion)
                {
                    MessageBox.Show(exExcepion.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            ////確認寫入資料有無問題
            //if (!GetShortArray(txt_LabelDataBlock2, out arrLabelData))
            //{
            //    return;//失敗則break
            //}
            //WriteDeviceBlock2
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.WriteDeviceBlock2(sLabelName, iDataSize, ref arrLabelData[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        #region (short) Get/Set Device2(PointData)
        private void btn_GetDevice2_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            //short
            string sLabelName = "";       //LabelName標籤名稱(List)
            short arrLabelData;		  //LabelData資料矩陣

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNamePoint2.Lines);//將把text中所有標籤鏈結起
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.GetDevice2(sLabelName, out arrLabelData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //-----------------------------------------------------------
            if (iReturnCode == 0)
            {
                txt_Data.Text = arrLabelData.ToString();
            }
        }
        private void btn_SetDevice2_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            //short
            string sLabelName = "";       //LabelName標籤名稱(List)
            short arrLabelData;		  //LabelData資料矩陣

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNamePoint2.Lines);//將把text中所有標籤鏈結起
            if (!Mx_Connect.GetShortValue(txt_LabelDataPoint2, out arrLabelData))
            {
                return;//失敗則break
            }
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.SetDevice2(sLabelName, arrLabelData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);

        }
        #endregion
        #region (String) Read/Write Block2(lot of set)
        private void btn_ReadDeviceString_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            short[] arrLabelData;         //資料(矩陣)
            string[] arrData;	          //標籤(矩陣)
            byte[] arrLabelDataString;

            //取得標籤名稱(以分行"\n"作鏈結)
            sLabelName = string.Join("\n", txt_LabelNameString.Lines);
            //取得資料大小
            if (!Mx_Connect.GetIntValue(txt_DataSizeString, out iDataSize))
            {
                return;//失敗則break
            }
            //設定LabelData矩陣大小
            arrLabelData = new short[iDataSize];
            arrLabelDataString = new byte[iDataSize * 2];
            //ReadDeviceBlock
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadDeviceBlock2(sLabelName, iDataSize, out arrLabelData[0]);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                var total = System.BitConverter.GetBytes(arrLabelData[0]);
                for (int i = 1; i < arrLabelData.Length; i++)
                {
                    var addvalue = System.BitConverter.GetBytes(arrLabelData[i]);
                    var Full = new byte[total.Length + addvalue.Length];
                    Buffer.BlockCopy(total, 0, Full, 0, total.Length);
                    Buffer.BlockCopy(addvalue, 0, Full, total.Length, addvalue.Length);

                    total = Full;//一直累積完全部byte
                               
                }
                arrData = new string[1];

                arrData[0] = Encoding.ASCII.GetString(total);

                txt_Data.Lines = arrData;
            }
        }
        private void btn_WriteDeviceString_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            short[] arrLabelData;		  //資料(矩陣)

            int iSizeOfIntArray;

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNameString.Lines);
            //確認資料位寬有無問題
            if (!Mx_Connect.GetIntValue(txt_DataSizeString, out iDataSize))
            {
                return;//失敗則break
            }
            //取得總輸出資料大小  STRING 1行
            iSizeOfIntArray = 1;


            //設定參數大小(以DataSize為準)
            arrLabelData = new short[iDataSize];
            string sInCombimeValue = txt_LabelDataString.Lines[0];

            while (sInCombimeValue.Length < iDataSize * 2)//若值不為 軟元件數*元件容量則需補值 超過不管
            {
                sInCombimeValue = "0" + sInCombimeValue;//沒有就補0在前面
            }

            for (int i = 0, j = 0; i < iDataSize; i++, j += 2)
            {
                //arrData[i] = Convert.ToInt16(sInCombimeValue.Substring(j, 2));//word每個兩字元0123456789=>[01][23][45][67][89]
                //每兩個一組轉換成ASCII的byte再轉為輸出用的short
                arrLabelData[i] = BitConverter.ToInt16(Encoding.ASCII.GetBytes(sInCombimeValue.Substring(j, 2)), 0);
            }

            //WriteDeviceBlock2
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.WriteDeviceBlock2(sLabelName, iDataSize, ref arrLabelData[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        #region (Float) Read/Write Block2(2 of set)
        private void btn_ReadDeviceFloat_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            short[] arrLabelData;         //資料(矩陣)
            string[] arrData;	          //標籤(矩陣)
            float FF;

            //取得標籤名稱(以分行"\n"作鏈結)
            sLabelName = string.Join("\n", txt_LabelNameFloat.Lines);
            //取得資料大小
            iDataSize = 2;
            //設定LabelData矩陣大小
            arrLabelData = new short[iDataSize];
            //ReadDeviceBlock
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadDeviceBlock2(sLabelName, iDataSize, out arrLabelData[0]);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                byte[] Low = System.BitConverter.GetBytes(arrLabelData[0]);
                byte[] High = System.BitConverter.GetBytes(arrLabelData[1]);
                byte[] Full = new byte[High.Length + Low.Length];
                Buffer.BlockCopy(Low, 0, Full, 0, Low.Length);
                Buffer.BlockCopy(High, 0, Full, Low.Length, High.Length);
                FF = BitConverter.ToSingle(Full, 0);

                //高低合一
                arrData = new string[1];

                arrData[0] = FF.ToString();

                txt_Data.Lines = arrData;
            }
        }
        private void btn_WriteDeviceFloat_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            string sLabelName = "";       //標籤名稱
            int iDataSize = 0;		      //資料大小
            short[] arrLabelData;		  //資料(矩陣)

            int iSizeOfIntArray;

            //取得標籤名稱
            sLabelName = string.Join("\n", txt_LabelNameFloat.Lines);
            //確認資料位寬有無問題
            iDataSize = 2;
            //取得總輸出資料大小 float限制1行
            iSizeOfIntArray = 1;

            //設定參數大小(以DataSize為準)
            arrLabelData = new short[iDataSize];

            byte[] sp = BitConverter.GetBytes(Convert.ToSingle(txt_LabelDataFloat.Lines[0]));//byte共4位

            arrLabelData[0] = BitConverter.ToInt16(sp, 0);
            arrLabelData[1] = BitConverter.ToInt16(sp, 2);
            //WriteDeviceBlock2
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.WriteDeviceBlock2(sLabelName, iDataSize, ref arrLabelData[0]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        //----------------------------------------------------------
        #region read/write Buffer
        private void btn_ReadBuffer_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            int iStartIO;                 //
            int iAddress;
            int iReadSize;
            short[] arrData;
            string[] arrShowData;

            //取得讀取值的模組的 I/O 編號
            if (!Mx_Connect.GetIntValue(txt_StartIOBuffer, out iStartIO))
            {
                return;//失敗則break
            }
            //取得緩衝存儲器的地址
            if (!Mx_Connect.GetIntValue(txt_AddressBuffer, out iAddress))
            {
                return;//失敗則break
            }
            //取得設備資料位寬
            if (!Mx_Connect.GetIntValue(txt_DataSizeBuffer, out iReadSize))
            {
                return;//失敗則break
            }
            arrData = new short[iReadSize];
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadBuffer(iStartIO, iAddress, iReadSize, out arrData[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                //創建資料陣列
                arrShowData = new string[iReadSize];
                //對位轉貼
                for (int i = 0; i < iReadSize; i++)
                {
                    //將LabelData(short)=>Data(string)
                    arrShowData[i] = arrData[i].ToString();
                }
                //顯示在Output Textbox
                txt_Data.Lines = arrShowData;
            }

        }
        private void btn_WriteBuffer_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            int iStartIO;                 //
            int iAddress;
            int iWriteSize;
            short[] arrData;

            //取得寫入值的模組的 I/O 編號
            if (!Mx_Connect.GetIntValue(txt_StartIOBuffer, out iStartIO))
            {
                return;//失敗則break
            }
            //取得緩衝存儲器的地址
            if (!Mx_Connect.GetIntValue(txt_AddressBuffer, out iAddress))
            {
                return;//失敗則break
            }
            //取得設備資料位寬
            if (!Mx_Connect.GetIntValue(txt_DataSizeBuffer, out iWriteSize))
            {
                return;//失敗則break
            }
            arrData = new short[iWriteSize];
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.WriteBuffer(iStartIO, iAddress, iWriteSize, ref arrData[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //後面不顯示write結果
        }
        #endregion
        //----------------------------------------------------------
        #region get/set ClockData
        private void btn_GetClockData_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            short shYear;
            short shMonth;
            short shDay;
            short shWeek;
            short shHour;
            short shMinute;
            short shSecond;
            string[] sShowData;

            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.GetClockData(out shYear, out shMonth, out shDay, out shWeek,
                                                      out shHour, out shMinute, out shSecond);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //-----------------------------------------------------------
            if (iReturnCode == 0)
            {
                sShowData = new string[7];
                //對位轉貼
                sShowData[0] = "年:" + shYear.ToString();
                sShowData[1] = "月:" + shMonth.ToString();
                sShowData[2] = "日:" + shDay.ToString();
                sShowData[3] = "星期:" + shWeek.ToString();
                sShowData[4] = "時:" + shHour.ToString();
                sShowData[5] = "分:" + shMinute.ToString();
                sShowData[6] = "秒:" + shSecond.ToString();

                //顯示在Output Textbox
                txt_Data.Lines = sShowData;
            }
        }
        private void btn_SetClockData_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            short shYear;
            short shMonth;
            short shDay;
            short shWeek;
            short shHour;
            short shMinute;
            short shSecond;
            string sWeek;
            //
            if (!Mx_Connect.GetShortValue(txt_ClockYear, out shYear))
            {
                return;//失敗則break
            }
            if (!Mx_Connect.GetShortValue(cb_ClockMonth, out shMonth))
            {
                return;//失敗則break
            }
            if (!Mx_Connect.GetShortValue(txt_ClockDay, out shDay))
            {
                return;//失敗則break
            }
            if (!Mx_Connect.GetStringValue(txt_ClockWeek, out sWeek))
            {
                return;//失敗則break
            }
            if (!Mx_Connect.GetShortValue(txt_ClockHour, out shHour))
            {
                return;//失敗則break
            }
            if (!Mx_Connect.GetShortValue(txt_ClockMinute, out shMinute))
            {
                return;//失敗則break
            }
            if (!Mx_Connect.GetShortValue(txt_ClockSecond, out shSecond))
            {
                return;//失敗則break
            }
            try
            {
                //dayofweek
                switch (sWeek.ToString())
                {

                    case "Monday": shWeek = (short)1; break;
                    case "Tuesday": shWeek = (short)2; break;
                    case "Wednesday": shWeek = (short)3; break;
                    case "Thursday": shWeek = (short)4; break;
                    case "Friday": shWeek = (short)5; break;
                    case "Saturday": shWeek = (short)6; break;
                    case "Sunday": shWeek = (short)7; break;
                    default: shWeek = (short)0; break;
                }
                if (shWeek == 0)
                {
                    return;
                }
                iReturnCode = Mx_Connect.Prog_Connect.SetClockData(shYear, shMonth, shDay, shWeek, shHour, shMinute, shSecond);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            ////後面不顯示write結果
        }
        private void btn_PC_NowTime_Click(object sender, EventArgs e)
        {
            interTimeSet();
        }
        #endregion
        #region get/set CPUTypeStatus
        private void btn_GetCPUType_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            string sCpuName;
            int iCpuType;
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.GetCpuType(out sCpuName, out iCpuType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                txt_Data.Text = "CpuName: " + sCpuName + "\r\n" + "CpuCode: " + iCpuType;
            }
        }
        private void btn_SetCPUStatus_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            int iOperation;

            switch (cb_CPUStatus.Text)
            {
                case "RUN": iOperation = 0; break;
                case "STOP": iOperation = 1; break;
                case "PAUSE": iOperation = 2; break;
                default: iOperation = 3; break;

            }
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.SetCpuStatus(iOperation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        #endregion
        #region Entry/Free/On DeviceStatus
        private void btn_EntryDeviceStatus_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼

            string sDeviceList;
            int iDataSize;
            int iMonitorCycle;
            int[] arrData;

            sDeviceList = string.Join("\n", txt_DeviceList.Lines);
            //Check the 'DataSize'.
            if (!Mx_Connect.GetIntValue(txt_DataSize, out iDataSize))
            {
                return;//失敗則break
            }
            //Check the 'MonitorCycle'.
            if (!Mx_Connect.GetIntValue(txt_MonitorCycle, out iMonitorCycle))
            {
                return;//失敗則break
            }
            //Check the 'DeviceData'.
            arrData = new int[iDataSize];
            if (!Mx_Connect.GetIntArray(txt_DeviceData, out arrData))
            {
                return;//失敗則break
            }
            //EntryDeviceStatus
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.EntryDeviceStatus(sDeviceList, iDataSize, iMonitorCycle, ref arrData[0]);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        private void btn_FreeDeviceStatus_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.FreeDeviceStatus();
                //mx_connect.OnDeviceStatus -= new _IActProgTypeEvents_OnDeviceStatusEventHandler(this.OnDeviceStatus);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                txt_Data.Text = "Device監控狀態解除";
            }
        }
        private void OnDeviceStatus(string szDevice, int lData, int lReturnCode)
        {
            string[] arrData;
            //Assign the array for editing the data of 'Data'.
            //+1寫時間狀態
            arrData = new string[txt_Data.Lines.Length + Convert.ToInt32(txt_DataSize.Text) + 1];

            //Set the lateset data of 'Data' to arrData.
            for (int i = 0; i < txt_Data.Lines.Length; i++)
            {
                arrData[i] = txt_Data.Lines[i];
            }
            //Add the content of new event to arrData.
            for (int i = 0; i < Convert.ToInt32(txt_DataSize.Text); i++)
            {
                //由前面陣列大小作持續編排(-1=順序)
                arrData[txt_Data.Lines.Length + i] = string.Format("OnDeviceStatus event by ActProgType [{0}={1}]", szDevice, lData);
            }
            arrData[arrData.Length - 1] = DateTime.Now.ToString();
            //arrData[Convert.ToInt32(txt_DataSize.Text)]
            //= string.Format("OnDeviceStatus event by ActProgType [{0}={1}]", szDevice, lData);

            //The new 'Data' is displayed.
            txt_Data.Lines = arrData;

            //The return code of the method is displayed by the hexadecimal.
            txt_ReturnCode.Text = string.Format("0x{0:x8}", lReturnCode);
        }
        #endregion
        #region Connect/Disconnect
        //由於為串列傳輸使用的Callback模式則僅寫出功能
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            int iReturnCode;              //回傳代碼
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
        }
        #endregion

        private void btn_MDeviceGet_Click(object sender, EventArgs e)
        {
            int iTimeCycle;

            if (!Mx_Connect.GetIntValue(txt_TimeCycle, out iTimeCycle))
            {
                if (!(iTimeCycle > 0 && iTimeCycle < 3600000))
                {
                    MessageBox.Show("時間範圍錯誤", "TimeCycle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            Timer_DeviceGet.Interval = iTimeCycle;
            Timer_DeviceGet.Enabled = true;
        }
        private void btn_MDeviceListGetStop_Click(object sender, EventArgs e)
        {
            Timer_DeviceGet.Enabled = false;
        }
        //----------------------------------------------------------
        #endregion
        #region Message
        #region GetErrorMessage
        private void btn_GetErrorMessage_Click(object sender, EventArgs e)
        {
            //目前僅能回傳英文錯誤訊息
            int iReturnCode;              //回傳代碼

            int iErrorCode;
            string sErrorCode;
            string sErrorMessage;
            //確認ErrorCode是否有帶入
            if (!Mx_Connect.GetStringValue(txt_ErrorCode, out sErrorCode))
            {
                return;//失敗則break
            }
            iErrorCode = Convert.ToInt32(sErrorCode, 16);
            try
            {
                iReturnCode = Mx_Connect.SpMsg_Connect.GetErrorMessage(iErrorCode, out sErrorMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            if (iReturnCode == 0)
            {
                txt_ErrorMessage.Text = sErrorMessage;
            }
        }
        #endregion
        #endregion
        private void interTimeSet()
        {
            int iYear;
            int iMonth;
            int iDay;
            DayOfWeek sWeek;
            int iHour;
            int iMinute;
            int iSecond;

            DateTime NowTime = DateTime.Now;

            iYear = NowTime.Year;
            iMonth = NowTime.Month;
            iDay = NowTime.Day;
            sWeek = NowTime.DayOfWeek;
            iHour = NowTime.Hour;
            iMinute = NowTime.Minute;
            iSecond = NowTime.Second;

            txt_ClockYear.Text = iYear.ToString();
            cb_ClockMonth.Text = iMonth.ToString();
            txt_ClockDay.Text = iDay.ToString();
            txt_ClockWeek.Text = sWeek.ToString();
            txt_ClockHour.Text = iHour.ToString();
            txt_ClockMinute.Text = iMinute.ToString();
            txt_ClockSecond.Text = iSecond.ToString();

        }
        private void On_DeviceGet(object sender, EventArgs e)
        {
            Timer_DeviceGet.Enabled = false;
            MDeviceGet();
            Timer_DeviceGet.Enabled = true;
        }

        private void MDeviceGet()
        {
            int iReturnCode;
            string sDeviceList = "";
            int iSize = 0;
            int[] arrDeviceData;		  //DeviceData資料矩陣

            //取得標籤名稱
            //sDeviceList = string.Join("\n", txt_DeviceList.Lines);//將把text中所有標籤串成List
            foreach (string DeviceName in txt_MDeviceList.Lines)
            {
                iSize++;
                sDeviceList = Mx_Connect.CombineString(sDeviceList, DeviceName);
            }
            //設備參數(以資料大小)
            arrDeviceData = new int[iSize];
            try
            {
                iReturnCode = Mx_Connect.Prog_Connect.ReadDeviceRandom(sDeviceList, iSize, out arrDeviceData[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //顯示回傳值
            txt_ReturnCode.Text = String.Format("0x{0:x8} [HEX]", iReturnCode);
            //-----------------------------------------------------------
            if (iReturnCode == 0)
            {
                //創建資料陣列
                string[] arrData = new string[iSize + 1];
                //對位轉貼
                for (int i = 0; i < iSize; i++)
                {
                    //將LabelData(short)=>Data(string)
                    arrData[i] = arrDeviceData[i].ToString();
                }
                arrData[iSize] = DateTime.Now.ToString();
                //顯示在Output Textbox
                //txt_Data.Lines = arrData;
                //委派
                foreach (string str in arrData)
                {
                    this.BeginInvoke(new UpdateControl(_mUpdateControl), new object[] { this.list_Data, str });
                }

                //Console測試顯示
                //foreach (string OutPut in arrData)
                //{
                //    Console.WriteLine(OutPut);
                //}

            }
        }
        void _mUpdateControl(Control Ctrl, string Msg)
        {
            lock (this._objLock)
            {
                if (Ctrl is ListBox)
                    ((ListBox)Ctrl).Items.Add(Msg);
            }
        }

        //==========================================================
    }
    public class MX_Component
    {
        public ActProgTypeClass Prog_Connect = new ActProgTypeClass();//Program
        public ActUtlTypeClass Ult_Connect = new ActUtlTypeClass();//Utility
        public ActSupportMsgClass SpMsg_Connect = new ActSupportMsgClass();//Message
        //private System.Timers.Timer Timer_DeviceStatusGet = new System.Timers.Timer();


        public MX_Component()
        {
            //Timer_DeviceStatusGet.Enabled = false;
            //Timer_DeviceStatusGet.Interval = 15000;
            //Timer_DeviceGet.Elapsed += On_DeviceGet;
        }

        public void ProgPropertySet()
        {
            //屬性列表Property list
            Prog_Connect.ActBaudRate = 0;
            Prog_Connect.ActControl = 0;
            Prog_Connect.ActCpuType = 528;//18944;//528
            Prog_Connect.ActDataBits = 0;
            Prog_Connect.ActDestinationIONumber = 0;
            Prog_Connect.ActDestinationPortNumber = 5562;
            Prog_Connect.ActDidPropertyBit = 1;
            Prog_Connect.ActDsidPropertyBit = 1;
            Prog_Connect.ActHostAddress = "192.168.2.99";
            Prog_Connect.ActIntelligentPreferenceBit = 0;
            Prog_Connect.ActIONumber = 1023;
            Prog_Connect.ActNetworkNumber = 0;
            Prog_Connect.ActPacketType = 1;
            Prog_Connect.ActPassword = "";
            Prog_Connect.ActPortNumber = 0;
            Prog_Connect.ActProtocolType = 5;//PROTOCOL_TCPIP
            Prog_Connect.ActStationNumber = 255;
            Prog_Connect.ActStopBits = 0;
            Prog_Connect.ActSumCheck = 0;
            Prog_Connect.ActThroughNetworkType = 1;
            Prog_Connect.ActTimeOut = 100;
            Prog_Connect.ActUnitNumber = 0;
            Prog_Connect.ActUnitType = 8193;
        }
        public int ProgOpen()
        {
            int iReturnCode;//回傳代碼
            ProgPropertySet();
            iReturnCode = Prog_Connect.Open();
            return iReturnCode;
        }
        public int ProgClose()
        {
            int iReturnCode;//回傳代碼
            iReturnCode = Prog_Connect.Close();
            return iReturnCode;
        }

        public bool GetIntValue(TextBox txt_SourceText, out int iIntValue)
        {
            iIntValue = 0;
            // TextBox => 32bit int
            try
            {
                iIntValue = Convert.ToInt32(txt_SourceText.Text);
            }
            //格式不對，進EX
            catch (Exception exExcepion)
            {
                MessageBox.Show(exExcepion.Message, "GetIntValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool GetStringValue(TextBox txt_SourceText, out string iStrValue)
        {
            iStrValue = "";
            // TextBox => string
            try
            {
                iStrValue = txt_SourceText.Text;
            }
            //格式不對，進EX
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetStringValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool GetShortValue(TextBox txt_SourceText, out short shshortValue)
        {
            shshortValue = 0;
            // TextBox => 16bit short
            try
            {
                shshortValue = Convert.ToInt16(txt_SourceText.Text);
            }
            //格式不對，進EX
            catch (Exception exExcepion)
            {
                MessageBox.Show(exExcepion.Message, "GetShortValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool GetShortValue(ComboBox cb_SourceText, out short shshortValue)
        {
            shshortValue = 0;
            // ComboxBox => 16bit short
            try
            {
                shshortValue = Convert.ToInt16(cb_SourceText.Text);
            }
            //沒東西，進EX
            catch (Exception exExcepion)
            {
                MessageBox.Show(exExcepion.Message, "GetShortValue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool GetIntArray(TextBox txt_SourceText, out int[] iArrIntValue)
        {
            int iIntArrayCount;		//Size of ShortType array

            //Get the size of ShortType array.
            iIntArrayCount = txt_SourceText.Lines.Length;
            iArrIntValue = new int[iIntArrayCount];

            //Get each element of ShortType array.
            for (int i = 0; i < iIntArrayCount; i++)
            {
                try
                {
                    //int32==int
                    iArrIntValue[i] = Convert.ToInt32(txt_SourceText.Lines[i]);
                }
                //nothing or out of the range, the exception is processed.
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "GetIntArray", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            //Normal End
            return true;
        }
        public bool GetShortArray(TextBox txt_SourceText, out short[] shArrShortValue)
        {
            int iShortArrayCount;		//Size of ShortType array

            //Get the size of ShortType array.
            iShortArrayCount = txt_SourceText.Lines.Length;
            shArrShortValue = new short[iShortArrayCount];

            //Get each element of ShortType array.
            for (int i = 0; i < iShortArrayCount; i++)
            {
                try
                {
                    shArrShortValue[i] = Convert.ToInt16(txt_SourceText.Lines[i]);
                }
                //nothing or out of the range, the exception is processed.
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "GetShortArray", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            //Normal End
            return true;
        }
        public string CombineString(string Combine1, string Combine2)
        {
            string sOutput = "";
            //補上\n
            if (Combine1 == "")
            {
                sOutput = Combine2;
            }
            else
            {
                sOutput = Combine1 + "\n" + Combine2;
            }
            return sOutput;
        }

    }
}
