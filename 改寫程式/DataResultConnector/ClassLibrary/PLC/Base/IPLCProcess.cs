using ClassLibrary.DataHeader;
using ClassLibrary.SharedComponent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.PLC.Base
{
    //=================================================================
    //Interface
    //=================================================================
    public class IPLCProcess : DataHeadlerBase
    {
        private IPLCProcess m_pIn;// { get; set; }
        private IPLCProcess m_pOut;// { get; set; }
        public IPLCProcess()
        {
            m_pIn = null; m_pOut = null;
        }
        void AttachIn(IPLCProcess pLink)
        {
            m_pIn = pLink;
        }
        void AttachOut(IPLCProcess pLink)
        {
            m_pOut = pLink;
        }
        ///=====================================================================================
        //以下近似callback於視窗部分使用
        public virtual void ON_GPIO_NOTIFY(IntPtr wparam, IntPtr lparam)
        {
            if (m_pIn != null)
                m_pIn.ON_GPIO_NOTIFY(wparam, lparam);
        }
        public virtual int ON_OPEN_PLC(IntPtr lparam)
        {
            if (m_pIn != null)
                return m_pIn.ON_OPEN_PLC(lparam);
            else
                return -1;
        }
        public void ON_PLC_NOTIFY(string strMsg)
        {
            if (m_pOut != null)
            {
                m_pOut.ON_PLC_NOTIFY(strMsg);
            }
        }
        public void ON_SET_PLCPARAM(BATCH_SHARE_SYSTCCL_INITPARAM xParam)
        {
            if (m_pOut != null)
                m_pOut.ON_SET_PLCPARAM(xParam);
        }
        public void ON_PLCDATA_CHANGE(int nFieldId, byte[] pData, int nSizeInByte)
        {
            if (m_pOut != null)
                m_pOut.ON_PLCDATA_CHANGE(nFieldId, pData, nSizeInByte);
        }
        public void ON_BATCH_PLCDATA_CHANGE(int nFieldFirst, int nFieldLast)
        {
            if (m_pOut != null)
                m_pOut.ON_BATCH_PLCDATA_CHANGE(nFieldFirst, nFieldLast);
        }
    };
}
