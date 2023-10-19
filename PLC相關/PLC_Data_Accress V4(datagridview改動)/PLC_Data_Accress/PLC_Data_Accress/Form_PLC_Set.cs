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
        TParameter m_Param;
        public Form_PLC_Set()
        {
            InitializeComponent();

            //將對應PLC_Connect的值填入txtbox中
            txt_ActBaudRate.Text = m_Param.ActBaudRate.ToString();
            txt_ActControl.Text = m_Param.ActControl.ToString();
            txt_ActCpuType.Text = m_Param.ActCpuType.ToString();
            txt_ActDataBits.Text = m_Param.ActDataBits.ToString();
            txt_ActDestinationIONumber.Text = m_Param.ActDestinationIONumber.ToString();
            txt_ActDestinationPortNumber.Text = m_Param.ActDestinationPortNumber.ToString();
            txt_ActDidPropertyBit.Text = m_Param.ActDidPropertyBit.ToString();
            txt_ActDsidPropertyBit.Text = m_Param.ActDsidPropertyBit.ToString();
            txt_ActIntelligentPreferenceBit.Text = m_Param.ActIntelligentPreferenceBit.ToString();
            txt_ActIONumber.Text = m_Param.ActIONumber.ToString();
            txt_ActNetworkNumber.Text = m_Param.ActNetworkNumber.ToString();
            txt_ActMultiDropChannelNumber.Text = m_Param.ActMultiDropChannelNumber.ToString();
            txt_ActSourceNetworkNumber.Text = m_Param.ActSourceNetworkNumber.ToString();
            txt_ActPacketType.Text = m_Param.ActPacketType.ToString();
            txt_ActPassword.Text = m_Param.ActPassword;
            txt_ActPortNumber.Text = m_Param.ActPortNumber.ToString();
            txt_ActProtocolType.Text = m_Param.ActProtocolType.ToString();
            txt_ActSourceStationNumber.Text = m_Param.ActSourceStationNumber.ToString();
            txt_ActStationNumber.Text = m_Param.ActStationNumber.ToString();
            txt_ActStopBits.Text = m_Param.ActStopBits.ToString();
            txt_ActSumCheck.Text = m_Param.ActSumCheck.ToString();
            txt_ActThroughNetworkType.Text = m_Param.ActThroughNetworkType.ToString();
            txt_ActTimeOut.Text = m_Param.ActTimeOut.ToString();
            txt_ActUnitNumber.Text = m_Param.ActUnitNumber.ToString();
            txt_ActUnitType.Text = m_Param.ActUnitType.ToString();
            txt_ActParity.Text = m_Param.ActParity.ToString();
            txt_ActCpuTimeOut.Text = m_Param.ActCpuTimeOut.ToString();

        }
        //--------------------------------------------------------
        private void Btn_Apply_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            m_Param.CpuName = cb_CPUType.Text;
            m_Param.ActHostAddress = txt_Address.Text;
        }
        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        private void Btn_Link_Click(object sender, EventArgs e)
        {
            try
            {
                m_Param.iReturnCode = m_Param.ProgOpen();
                txt_ReturnCode.Text = m_Param.iReturnCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PLC連線", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //若回傳碼==0
            if (m_Param.iReturnCode == 0)
            {
                btn_Link_Test.Text = "連線成功";
                btn_Link_Test.BackColor = Color.Lime;
            }
            //若回傳碼!=0
            if (m_Param.iReturnCode != 0)
            {
                btn_Link_Test.Text = "連線失敗";
                btn_Link_Test.BackColor = Color.Red;
            }
        }
        private void Cb_CPUType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //當選取CPU-Type列表變動
            m_Param.ActHostAddress = txt_Address.Text;
            //設定屬性
            m_Param.SetPLCProperty(cb_CPUType.SelectedItem.ToString());//設定全屬性

            //將對應的值填入txtbox中
            txt_ActBaudRate.Text = m_Param.ActBaudRate.ToString();
            txt_ActControl.Text = m_Param.ActControl.ToString();
            txt_ActCpuType.Text = m_Param.ActCpuType.ToString();
            txt_ActDataBits.Text = m_Param.ActDataBits.ToString();
            txt_ActDestinationIONumber.Text = m_Param.ActDestinationIONumber.ToString();
            txt_ActDestinationPortNumber.Text = m_Param.ActDestinationPortNumber.ToString();
            txt_ActDidPropertyBit.Text = m_Param.ActDidPropertyBit.ToString();
            txt_ActDsidPropertyBit.Text = m_Param.ActDsidPropertyBit.ToString();
            txt_ActIntelligentPreferenceBit.Text = m_Param.ActIntelligentPreferenceBit.ToString();
            txt_ActIONumber.Text = m_Param.ActIONumber.ToString();
            txt_ActNetworkNumber.Text = m_Param.ActNetworkNumber.ToString();
            txt_ActMultiDropChannelNumber.Text = m_Param.ActMultiDropChannelNumber.ToString();
            txt_ActSourceNetworkNumber.Text = m_Param.ActSourceNetworkNumber.ToString();
            txt_ActPacketType.Text = m_Param.ActPacketType.ToString();
            txt_ActPassword.Text = m_Param.ActPassword;
            txt_ActPortNumber.Text = m_Param.ActPortNumber.ToString();
            txt_ActProtocolType.Text = m_Param.ActProtocolType.ToString();
            txt_ActSourceStationNumber.Text = m_Param.ActSourceStationNumber.ToString();
            txt_ActStationNumber.Text = m_Param.ActStationNumber.ToString();
            txt_ActStopBits.Text = m_Param.ActStopBits.ToString();
            txt_ActSumCheck.Text = m_Param.ActSumCheck.ToString();
            txt_ActThroughNetworkType.Text = m_Param.ActThroughNetworkType.ToString();
            txt_ActTimeOut.Text = m_Param.ActTimeOut.ToString();
            txt_ActUnitNumber.Text = m_Param.ActUnitNumber.ToString();
            txt_ActUnitType.Text = m_Param.ActUnitType.ToString();
            txt_ActParity.Text = m_Param.ActParity.ToString();
            txt_ActCpuTimeOut.Text = m_Param.ActCpuTimeOut.ToString();
        }
    }
}
