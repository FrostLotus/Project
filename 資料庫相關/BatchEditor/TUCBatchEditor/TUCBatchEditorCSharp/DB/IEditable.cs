﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TUCBatchEditorCSharp.DB
{
    /// <summary>
    /// 控制編輯視窗取得
    /// </summary>
    public interface IEditable
    {
        /// <summary>
        /// 取得鍵
        /// </summary>
        /// <returns></returns>
        string GetKey();
        /// <summary>
        /// 取得參數名稱
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
    /// <summary>
    /// 定義欄位名稱
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldInfoAttribute : Attribute
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否顯示
        /// </summary>
        public bool Show { get; set; }
        /// <summary>
        /// 是否可編輯
        /// </summary>
        public bool Editable { get; set; }
        /// <summary>
        /// 編輯控制項
        /// </summary>
        public Type ControlType { get; set; }
        public FieldInfoAttribute(string name, bool bShow, bool bEditable, Type xControl)
        {
            this.Name = name;
            this.Show = bShow;
            this.Editable = bEditable;
            this.ControlType = xControl;
        }
    }
}
