using System;
using System.Runtime.InteropServices;

using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes.Widgets
{
    public class WidgetToggle : WidgetBase
    {

        #region Variables
        public IntPtr TargetVariable;
        #endregion

        #region Constructor
        public WidgetToggle(string name, IntPtr targetVariable) : base(WidgetType.Toggle, name)
        {
            TargetVariable = targetVariable;
        }
        #endregion

        public override void Draw()
        {
            bool val = Convert.ToBoolean(Marshal.ReadByte(TargetVariable));
  
            if (ImGuiIV.CheckBox(Name, ref val))
                Marshal.WriteByte(TargetVariable, Convert.ToByte(val));
        }

    }
}
