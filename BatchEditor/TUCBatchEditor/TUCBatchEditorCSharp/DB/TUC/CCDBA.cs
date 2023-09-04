using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TUCBatchEditorCSharp.DB.TUC
{
    /// <summary>
    /// 參數基本檔
    /// </summary>
    public class CCDBA
    {
        /// <summary>
        /// 參數編碼
        /// </summary>
        public string PARAM { get; set; }
        /// <summary>
        /// 參數內容(參數檔指定路徑)
        /// </summary>
        public string PARNAME { get; set; }
        /// <summary>
        /// 最後變更時間
        /// </summary>
        public DateTime USTIME { get; set; }
    }
}
