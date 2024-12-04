using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes.Widgets
{
    public class WidgetCombo : WidgetBase
    {

        #region Variables
        private IntPtr variable;

        private List<string> items;
        #endregion

        #region Constructor
        public WidgetCombo() : base (WidgetType.Combo)
        {
            items = new List<string>();
        }
        #endregion

        #region Methods
        public void SetStuff(string name, IntPtr targetVariable)
        {
            Name = name;
            variable = targetVariable;
        }
        public void AddItem(string item)
        {
            items.Add(item);
        }
        #endregion

        public override void Cleanup()
        {
            items.Clear();
            items = null;
        }

        public override void Draw()
        {
            if (items == null)
                return;

            // Get value from script variable
            int selectedValue = Marshal.ReadInt32(variable);

            string selectedItem = "NULL";

            if (items.Count != 0)
                selectedItem = items[0];

            if (ImGuiIV.BeginCombo(Name, selectedValue < 0 ? "" : items[selectedValue]))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    string item = items[i];
                    
                    if (ImGuiIV.Selectable(item))
                        Marshal.WriteInt32(variable, i);
                }

                ImGuiIV.EndCombo();
            }
        }

    }
}
