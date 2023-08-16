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
            txt_ActBaudRate.Text = TParameter.Mx_Connect.ActBaudRate.ToString();
            txt_ActControl.Text = TParameter.Mx_Connect.ActControl.ToString();
            txt_ActCpuType.Text = TParameter.Mx_Connect.ActCpuType.ToString();
            txt_ActDataBits.Text = TParameter.Mx_Connect.ActDataBits.ToString();
            txt_ActDestinationIONumber.Text = TParameter.Mx_Connect.ActDestinationIONumber.ToString();
            txt_ActDestinationPortNumber.Text = TParameter.Mx_Connect.ActDestinationPortNumber.ToString();
            txt_ActDidPropertyBit.Text = TParameter.Mx_Connect.ActDidPropertyBit.ToString();
            txt_ActDsidPropertyBit.Text = TParameter.Mx_Connect.ActDsidPropertyBit.ToString();
            txt_ActIntelligentPreferenceBit.Text = TParameter.Mx_Connect.ActIntelligentPreferenceBit.ToString();
            txt_ActIONumber.Text = TParameter.Mx_Connect.ActIONumber.ToString();
            txt_ActNetworkNumber.Text = TParameter.Mx_Connect.ActNetworkNumber.ToString();
            txt_ActMultiDropChannelNumber.Text = TParameter.Mx_Connect.ActMultiDropChannelNumber.ToString();
            txt_ActSourceNetworkNumber.Text = TParameter.Mx_Connect.ActSourceNetworkNumber.ToString();
            txt_ActPacketType.Text = TParameter.Mx_Connect.ActPacketType.ToString();
            txt_ActPassword.Text = TParameter.Mx_Connect.ActPassword;
            txt_ActPortNumber.Text = TParameter.Mx_Connect.ActPortNumber.ToString();
            txt_ActProtocolType.Text = TParameter.Mx_Connect.ActProtocolType.ToString();
            txt_ActSourceStationNumber.Text = TParameter.Mx_Connect.ActSourceStationNumber.ToString();
            txt_ActStationNumber.Text = TParameter.Mx_Connect.ActStationNumber.ToString();
            txt_ActStopBits.Text = TParameter.Mx_Connect.ActStopBits.ToString();
            txt_ActSumCheck.Text = TParameter.Mx_Connect.ActSumCheck.ToString();
            txt_ActThoughNetworkType.Text = TParameter.Mx_Connect.ActThroughNetworkType.ToString();
            txt_ActTimeOut.Text = TParameter.Mx_Connect.ActTimeOut.ToString();
            txt_ActUnitNumber.Text = TParameter.Mx_Connect.ActUnitNumber.ToString();
            txt_ActUnitType.Text = TParameter.Mx_Connect.ActUnitType.ToString();
            txt_ActParity.Text = TParameter.Mx_Connect.ActParity.ToString();
            txt_ActCpuTimeOut.Text = TParameter.Mx_Connect.ActCpuTimeOut.ToString();

        }
        //--------------------------------------------------------
        private void Btn_Apply_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            TParameter.Mx_Connect.CpuName = cb_CPUType.Text;
            TParameter.Mx_Connect.ActHostAddress = txt_Address.Text;
        }
        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        private void Btn_Link_Click(object sender, EventArgs e)
        {
            try
            {
                TParameter.Mx_Connect.ProgOpen();
                txt_ReturnCode.Text = TParameter.Mx_Connect.iReturnCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PLC連線", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //若回傳碼==0
            if (TParameter.Mx_Connect.iReturnCode == 0)
            {
                btn_Link_Test.Text = "連線成功";
                btn_Link_Test.BackColor = Color.Lime;
            }
            //若回傳碼!=0
            if (TParameter.Mx_Connect.iReturnCode != 0)
            {
                btn_Link_Test.Text = "連線失敗";
                btn_Link_Test.BackColor = Color.Red;
            }
        }
        private void Cb_CPUType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //當選取CPU-Type列表變動
            TParameter.Mx_Connect.ActHostAddress = txt_Address.Text;
            //設定屬性
            TParameter.SetPLCProperty(cb_CPUType.SelectedItem.ToString());//設定全屬性

            //將對應的值填入txtbox中
            txt_ActBaudRate.Text = TParameter.Mx_Connect.ActBaudRate.ToString();
            txt_ActControl.Text = TParameter.Mx_Connect.ActControl.ToString();
            txt_ActCpuType.Text = TParameter.Mx_Connect.ActCpuType.ToString();
            txt_ActDataBits.Text = TParameter.Mx_Connect.ActDataBits.ToString();
            txt_ActDestinationIONumber.Text = TParameter.Mx_Connect.ActDestinationIONumber.ToString();
            txt_ActDestinationPortNumber.Text = TParameter.Mx_Connect.ActDestinationPortNumber.ToString();
            txt_ActDidPropertyBit.Text = TParameter.Mx_Connect.ActDidPropertyBit.ToString();
            txt_ActDsidPropertyBit.Text = TParameter.Mx_Connect.ActDsidPropertyBit.ToString();
            txt_ActIntelligentPreferenceBit.Text = TParameter.Mx_Connect.ActIntelligentPreferenceBit.ToString();
            txt_ActIONumber.Text = TParameter.Mx_Connect.ActIONumber.ToString();
            txt_ActNetworkNumber.Text = TParameter.Mx_Connect.ActNetworkNumber.ToString();
            txt_ActMultiDropChannelNumber.Text = TParameter.Mx_Connect.ActMultiDropChannelNumber.ToString();
            txt_ActSourceNetworkNumber.Text = TParameter.Mx_Connect.ActSourceNetworkNumber.ToString();
            txt_ActPacketType.Text = TParameter.Mx_Connect.ActPacketType.ToString();
            txt_ActPassword.Text = TParameter.Mx_Connect.ActPassword;
            txt_ActPortNumber.Text = TParameter.Mx_Connect.ActPortNumber.ToString();
            txt_ActProtocolType.Text = TParameter.Mx_Connect.ActProtocolType.ToString();
            txt_ActSourceStationNumber.Text = TParameter.Mx_Connect.ActSourceStationNumber.ToString();
            txt_ActStationNumber.Text = TParameter.Mx_Connect.ActStationNumber.ToString();
            txt_ActStopBits.Text = TParameter.Mx_Connect.ActStopBits.ToString();
            txt_ActSumCheck.Text = TParameter.Mx_Connect.ActSumCheck.ToString();
            txt_ActThoughNetworkType.Text = TParameter.Mx_Connect.ActThroughNetworkType.ToString();
            txt_ActTimeOut.Text = TParameter.Mx_Connect.ActTimeOut.ToString();
            txt_ActUnitNumber.Text = TParameter.Mx_Connect.ActUnitNumber.ToString();
            txt_ActUnitType.Text = TParameter.Mx_Connect.ActUnitType.ToString();
            txt_ActParity.Text = TParameter.Mx_Connect.ActParity.ToString();
            txt_ActCpuTimeOut.Text = TParameter.Mx_Connect.ActCpuTimeOut.ToString();
        }
    }
}
