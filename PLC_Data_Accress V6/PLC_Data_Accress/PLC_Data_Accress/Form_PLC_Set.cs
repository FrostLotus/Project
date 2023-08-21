using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PLC_Data_Access;

namespace PLC_Data_Access
{
    //視窗
    public partial class Form_PLC_Set : Form
    {
        public Form_PLC_Set()
        {
            InitializeComponent();

            //將對應PLC_Connect的值填入txtbox中
            txt_ActBaudRate.Text = TParameter.MxConnect.ActBaudRate.ToString();
            txt_ActControl.Text = TParameter.MxConnect.ActControl.ToString();
            txt_ActCpuType.Text = TParameter.MxConnect.ActCpuType.ToString();
            txt_ActDataBits.Text = TParameter.MxConnect.ActDataBits.ToString();
            txt_ActDestinationIONumber.Text = TParameter.MxConnect.ActDestinationIONumber.ToString();
            txt_ActDestinationPortNumber.Text = TParameter.MxConnect.ActDestinationPortNumber.ToString();
            txt_ActDidPropertyBit.Text = TParameter.MxConnect.ActDidPropertyBit.ToString();
            txt_ActDsidPropertyBit.Text = TParameter.MxConnect.ActDsidPropertyBit.ToString();
            txt_ActIntelligentPreferenceBit.Text = TParameter.MxConnect.ActIntelligentPreferenceBit.ToString();
            txt_ActIONumber.Text = TParameter.MxConnect.ActIONumber.ToString();
            txt_ActNetworkNumber.Text = TParameter.MxConnect.ActNetworkNumber.ToString();
            txt_ActMultiDropChannelNumber.Text = TParameter.MxConnect.ActMultiDropChannelNumber.ToString();
            txt_ActSourceNetworkNumber.Text = TParameter.MxConnect.ActSourceNetworkNumber.ToString();
            txt_ActPacketType.Text = TParameter.MxConnect.ActPacketType.ToString();
            txt_ActPassword.Text = TParameter.MxConnect.ActPassword;
            txt_ActPortNumber.Text = TParameter.MxConnect.ActPortNumber.ToString();
            txt_ActProtocolType.Text = TParameter.MxConnect.ActProtocolType.ToString();
            txt_ActSourceStationNumber.Text = TParameter.MxConnect.ActSourceStationNumber.ToString();
            txt_ActStationNumber.Text = TParameter.MxConnect.ActStationNumber.ToString();
            txt_ActStopBits.Text = TParameter.MxConnect.ActStopBits.ToString();
            txt_ActSumCheck.Text = TParameter.MxConnect.ActSumCheck.ToString();
            txt_ActThoughNetworkType.Text = TParameter.MxConnect.ActThroughNetworkType.ToString();
            txt_ActTimeOut.Text = TParameter.MxConnect.ActTimeOut.ToString();
            txt_ActUnitNumber.Text = TParameter.MxConnect.ActUnitNumber.ToString();
            txt_ActUnitType.Text = TParameter.MxConnect.ActUnitType.ToString();
            txt_ActParity.Text = TParameter.MxConnect.ActParity.ToString();
            txt_ActCpuTimeOut.Text = TParameter.MxConnect.ActCpuTimeOut.ToString();

        }
        //--------------------------------------------------------
        private void Btn_Apply_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            TParameter.MxConnect.CpuName = cb_CPUType.Text;
            TParameter.MxConnect.ActHostAddress = txt_Address.Text;
        }
        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        private void Btn_Link_Click(object sender, EventArgs e)
        {
            try
            {
                TParameter.MxConnect.ProgOpen();
                txt_ReturnCode.Text = TParameter.MxConnect.iReturnCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PLC連線", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //若回傳碼==0
            if (TParameter.MxConnect.iReturnCode == 0)
            {
                btn_Link_Test.Text = "連線成功";
                btn_Link_Test.BackColor = Color.Lime;
            }
            //若回傳碼!=0
            if (TParameter.MxConnect.iReturnCode != 0)
            {
                btn_Link_Test.Text = "連線失敗";
                btn_Link_Test.BackColor = Color.Red;
            }
        }
        private void Cb_CPUType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //當選取CPU-Type列表變動
            TParameter.MxConnect.ActHostAddress = txt_Address.Text;
            //設定屬性
            TParameter.SetPLCProperty(cb_CPUType.SelectedItem.ToString());//設定全屬性

            //將對應的值填入txtbox中
            txt_ActBaudRate.Text = TParameter.MxConnect.ActBaudRate.ToString();
            txt_ActControl.Text = TParameter.MxConnect.ActControl.ToString();
            txt_ActCpuType.Text = TParameter.MxConnect.ActCpuType.ToString();
            txt_ActDataBits.Text = TParameter.MxConnect.ActDataBits.ToString();
            txt_ActDestinationIONumber.Text = TParameter.MxConnect.ActDestinationIONumber.ToString();
            txt_ActDestinationPortNumber.Text = TParameter.MxConnect.ActDestinationPortNumber.ToString();
            txt_ActDidPropertyBit.Text = TParameter.MxConnect.ActDidPropertyBit.ToString();
            txt_ActDsidPropertyBit.Text = TParameter.MxConnect.ActDsidPropertyBit.ToString();
            txt_ActIntelligentPreferenceBit.Text = TParameter.MxConnect.ActIntelligentPreferenceBit.ToString();
            txt_ActIONumber.Text = TParameter.MxConnect.ActIONumber.ToString();
            txt_ActNetworkNumber.Text = TParameter.MxConnect.ActNetworkNumber.ToString();
            txt_ActMultiDropChannelNumber.Text = TParameter.MxConnect.ActMultiDropChannelNumber.ToString();
            txt_ActSourceNetworkNumber.Text = TParameter.MxConnect.ActSourceNetworkNumber.ToString();
            txt_ActPacketType.Text = TParameter.MxConnect.ActPacketType.ToString();
            txt_ActPassword.Text = TParameter.MxConnect.ActPassword;
            txt_ActPortNumber.Text = TParameter.MxConnect.ActPortNumber.ToString();
            txt_ActProtocolType.Text = TParameter.MxConnect.ActProtocolType.ToString();
            txt_ActSourceStationNumber.Text = TParameter.MxConnect.ActSourceStationNumber.ToString();
            txt_ActStationNumber.Text = TParameter.MxConnect.ActStationNumber.ToString();
            txt_ActStopBits.Text = TParameter.MxConnect.ActStopBits.ToString();
            txt_ActSumCheck.Text = TParameter.MxConnect.ActSumCheck.ToString();
            txt_ActThoughNetworkType.Text = TParameter.MxConnect.ActThroughNetworkType.ToString();
            txt_ActTimeOut.Text = TParameter.MxConnect.ActTimeOut.ToString();
            txt_ActUnitNumber.Text = TParameter.MxConnect.ActUnitNumber.ToString();
            txt_ActUnitType.Text = TParameter.MxConnect.ActUnitType.ToString();
            txt_ActParity.Text = TParameter.MxConnect.ActParity.ToString();
            txt_ActCpuTimeOut.Text = TParameter.MxConnect.ActCpuTimeOut.ToString();
        }
    }
}
