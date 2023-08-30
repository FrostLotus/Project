using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUCBatchEditorCSharp.DB;
using TUCBatchEditorCSharp.DB.TTA;

namespace TUCBatchEditorCSharp.CustomData
{
    class TTADATA : DataManagerBase2<TUCBatchEditorCSharp.DB.TTA.CCD_TEST>
    {
        //參數基本檔
        List<DB.TTA.CCD_TEST> listInsp = new List<DB.TTA.CCD_TEST>();
        public TTADATA(string strDBCon, IDataCallBack callback) : base(strDBCon, callback)
        {

        }
        protected override string GetQueryCmd(GridDataType eType)
        {
            throw new NotImplementedException();
        }

        protected override CmdObject GetInsertCmd(IEditable xNew)
        {
            throw new NotImplementedException();
        }

        protected override CmdObject GetUpdateCmd(List<CCD_TEST> lsUnUsed, IEditable xOld, IEditable xNew)
        {
            throw new NotImplementedException();
        }

        protected override CmdObject GetDeleteCmd(IEditable xItem, GridDataType eType)
        {
            throw new NotImplementedException();
        }

        public override List<string> GetInspList()
        {
            throw new NotImplementedException();
        }

        









    }
}
