using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TUCBatchEditorCSharp.DB.TTA
{
    //DB包含:IEditable介面
    /// <summary>
    /// 測試資料庫
    /// </summary>
    class CCDTTA : IEditable
    {
        /// <summary>
        /// 索引值
        /// </summary>
        [FieldInfoAttribute("索引值", false, false, null)]
        public int PDID { get; set; }
        /// <summary>
        /// 批次號
        /// </summary>
        [FieldInfoAttribute("批次號", true, true, typeof(TextBox))]
        public string LOTSN { get; set; }
        /// <summary>
        /// 料號
        /// </summary>
        [FieldInfoAttribute("料號", true, true, typeof(TextBox))]
        public string MPN { get; set; }
        /// <summary>
        /// 參數編碼
        /// </summary>
        [FieldInfoAttribute("參數編碼", true, true, typeof(ComboBox))]
        public string PARAM { get; set; }
        /// <summary>
        /// 狀態碼.0 待執行,1執行中,2完成
        /// </summary>
        [FieldInfoAttribute("狀態碼", false, false, null)]
        public int STSN { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        [FieldInfoAttribute("開始時間", true, false, null)]
        public DateTime STIME { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        [FieldInfoAttribute("結束時間", true, false, null)]
        public DateTime ETIME { get; set; }
        public string GetKey()
        {
            return PDID.ToString();
        }
        public string GetName()
        {
            return LOTSN;
        }
    }
}
