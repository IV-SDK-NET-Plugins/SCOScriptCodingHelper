using System;
using System.Collections.Generic;
using System.Linq;

using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes.Widgets
{
    public class WidgetGroup : WidgetBase
    {

        #region Variables
        public string OwnerScriptName;
        public uint OwnerScriptThreadId;
        public List<WidgetBase> Widgets;
        public List<WidgetGroup> Children;
        public WidgetCombo CurrentCombo;

        public bool IsChildren;
        #endregion

        #region Constructor
        public WidgetGroup(string name, string ownerScriptName, uint ownerScriptThreadId) : base(WidgetType.Window, name)
        {
            OwnerScriptName = ownerScriptName;
            OwnerScriptThreadId = ownerScriptThreadId;
            Widgets = new List<WidgetBase>();
            Children = new List<WidgetGroup>();
        }
        #endregion

        #region Functions
        public WidgetGroup GetChild(string name)
        {
            // Try to find a child widget group with this name within this widget group
            WidgetGroup group = Children.Where(x => x.Name == name).FirstOrDefault();

            // If nothing was found, try to find a child widget group within this widget group's children
            if (group == null)
                return Children.SelectMany(x => x.Children).Where(x => x.Name == name).FirstOrDefault();
            
            return group;
        }

        public WidgetBase GetWidget(int id)
        {
            if (Widgets == null)
                return null;

            return Widgets.Where(x => x.ID == id).FirstOrDefault();
        }
        public bool DeleteWidget(int id)
        {
            WidgetBase widget = GetWidget(id);

            if (widget == null)
                return false;

            widget.Cleanup();
            return Widgets.Remove(widget);
        }

        public WidgetString CreateWidgetString(string text)
        {
            if (Widgets == null)
                return null;

            WidgetString widgetString = new WidgetString(text);

            Widgets.Add(widgetString);

            return widgetString;
        }
        public WidgetText CreateWidgetText(string name)
        {
            if (Widgets == null)
                return null;

            WidgetText widgetText = new WidgetText(name);

            Widgets.Add(widgetText);

            return widgetText;
        }
        public WidgetCombo CreateWidgetCombo()
        {
            if (Widgets == null)
                return null;

            WidgetCombo widgetCombo = new WidgetCombo();

            Widgets.Add(widgetCombo);

            CurrentCombo = widgetCombo;

            return widgetCombo;
        }
        public WidgetToggle CreateWidgetToggle(string name, IntPtr targetVariable)
        {
            if (Widgets == null)
                return null;

            WidgetToggle widgetToggle = new WidgetToggle(name, targetVariable);

            Widgets.Add(widgetToggle);

            return widgetToggle;
        }
        public WidgetFloatSlider CreateWidgetFloatSlider(string name, IntPtr targetVariable, float min, float max, float speed)
        {
            if (Widgets == null)
                return null;

            WidgetFloatSlider widgetFloatSlider = new WidgetFloatSlider(name, targetVariable, min, max, speed);

            Widgets.Add(widgetFloatSlider);

            return widgetFloatSlider;
        }
        public WidgetSlider CreateWidgetSlider(string name, IntPtr targetVariable, int min, int max, float speed)
        {
            if (Widgets == null)
                return null;

            WidgetSlider widgetSlider = new WidgetSlider(name, targetVariable, min, max, speed);

            Widgets.Add(widgetSlider);

            return widgetSlider;
        }
        #endregion

        #region Methods
        private void DrawWidgetContents()
        {
            if (Children.Count != 0)
            {
                if (ImGuiIV.BeginTabBar(string.Format("##{0}_ChildTabBar", Name)))
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        WidgetGroup childWidgetGroup = Children[i];

                        if (ImGuiIV.BeginTabItem(childWidgetGroup.Name))
                        {
                            DrawEachWidget(childWidgetGroup);

                            ImGuiIV.EndTabItem();
                        }
                    }

                    ImGuiIV.EndTabBar();
                }
            }
            else
            {
                DrawEachWidget();
            }
        }
        private void DrawEachWidget(WidgetGroup targetWidgetGroup = null)
        {
            //List<WidgetBase> widgets = Widgets;

            //if (targetWidgetGroup != null)
            //    widgets = targetWidgetGroup.Widgets;

            if (targetWidgetGroup != null)
            {
                targetWidgetGroup.Draw();
            }
            else
            {
                for (int i = 0; i < Widgets.Count; i++)
                {
                    WidgetBase widget = Widgets[i];

                    if (widget.ReadOnly)
                        ImGuiIV.BeginDisabled();

                    widget.Draw();

                    if (widget.ReadOnly)
                        ImGuiIV.EndDisabled();
                }
            }
        }
        #endregion

        public override void Cleanup()
        {
            Widgets.ForEach(x => x.Cleanup());
            Widgets.Clear();
            Widgets = null;
        }

        public override void Draw()
        {
            if (Widgets == null)
                return;

            if (!IsChildren)
            {
                if (ImGuiIV.Begin(string.Format("{0} [{1}, ThreadID: {2}]", Name, OwnerScriptName, OwnerScriptThreadId)))
                {
                    DrawWidgetContents();
                }
                ImGuiIV.End();
            }
            else
            {
                DrawWidgetContents();
            }
        }

    }
}
