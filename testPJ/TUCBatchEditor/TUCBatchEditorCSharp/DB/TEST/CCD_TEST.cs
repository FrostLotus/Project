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
    class CCD_TEST : IEditable
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
        /// 參數編碼
        /// </summary>
        [FieldInfoAttribute("參數編碼", true, true, typeof(ComboBox))]
        public string PARAM { get; set; }
        /// <summary>
        /// TEST
        /// </summary>
        [FieldInfoAttribute("TEST", true, true, null)]
        public int TTA { get; set; }

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
