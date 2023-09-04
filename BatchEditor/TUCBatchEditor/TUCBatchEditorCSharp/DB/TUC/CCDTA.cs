using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TUCBatchEditorCSharp.DB.TUC
{
    enum STSN_CODE
    {
        /// <summary>
        /// 待執行
        /// </summary>
        UnUsed = 0,
        /// <summary>
        /// 執行中
        /// </summary>
        Using = 1,
        /// <summary>
        /// 已執行
        /// </summary>
        Used = 2
    }
    /// <summary>
    /// 工單接收檔, CCDTA1(待執行:0)/CCDTA2(執行中:1+已完成:2)
    /// </summary>
    class CCDTA : IEditable
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
        /// 檢測廢品數量
        /// </summary>
        [FieldInfoAttribute("檢測廢品數量", true, false, null)]
        public decimal PFNUM { get; set; }
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

        #region 實作interface
        public string GetKey() { return PDID.ToString(); }
        public string GetName() { return LOTSN; }
        #endregion
    }
}
