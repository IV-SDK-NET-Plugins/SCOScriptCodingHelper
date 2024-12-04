using System;
using System.Runtime.InteropServices;

using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes.Widgets
{
    public class WidgetSlider : WidgetBase
    {

        #region Variables
        private IntPtr variable;
        private int minValue;
        private int maxValue;
        private float speedValue;
        #endregion

        #region Constructor
        public WidgetSlider(string name, IntPtr targetVariable, int min, int max, float speed) : base(WidgetType.IntSlider, name)
        {
            variable = targetVariable;
            minValue = min;
            maxValue = max;
            speedValue = speed;
        }
        #endregion

        public override void Draw()
        {
            int val = Marshal.ReadInt32(variable);

            if (ImGuiIV.DragInt(Name, ref val, speedValue, minValue, maxValue))
                Marshal.WriteInt32(variable, val);
        }

    }
}
