using System;

using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes.Widgets
{
    public class WidgetFloatSlider : WidgetBase
    {

        #region Variables
        private IntPtr variable;
        private float minValue;
        private float maxValue;
        private float speedValue;
        #endregion

        #region Constructor
        public WidgetFloatSlider(string name, IntPtr targetVariable, float min, float max, float speed) : base(WidgetType.FloatSlider, name)
        {
            variable = targetVariable;
            minValue = min;
            maxValue = max;
            speedValue = speed;
        }
        #endregion

        public override void Draw()
        {
            unsafe
            {
                float val = *(float*)(variable.ToInt32());

                if (ImGuiIV.DragFloat(Name, ref val, speedValue, minValue, maxValue))
                {
                    *(float*)(variable.ToInt32()) = val;
                }
            }
        }

    }
}
