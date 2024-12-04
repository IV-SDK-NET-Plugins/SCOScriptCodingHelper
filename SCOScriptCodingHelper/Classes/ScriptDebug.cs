using System;
using System.Collections.Generic;
using System.Linq;

using SCOScriptCodingHelper.Classes.Widgets;

namespace SCOScriptCodingHelper.Classes
{
    public class ScriptDebug
    {

        #region Variables
        public uint ThreadID;
        public string ScriptName;
        public bool IsDebuggingEnabled;

        // Widgets
        public List<WidgetGroup> WidgetGroups;
        public WidgetGroup CurrentWidgetGroup;
        public List<string> ActiveWidgetGroupsList;
        public byte ActiveWidgetGroups;

        // File
        public ScriptDebugFile DebugFile;
        #endregion

        #region Constructor
        public ScriptDebug(uint threadId, string scriptName)
        {
            ThreadID = threadId;
            ScriptName = scriptName;

            WidgetGroups = new List<WidgetGroup>();
            ActiveWidgetGroupsList = new List<string>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Goes through all added widget groups and tries to find the widget with the given <paramref name="id"/>.
        /// </summary>
        public WidgetBase FindWidgetByID(int id)
        {
            return WidgetGroups.Where(x => x.GetWidget(id) != null).Select(x => x.GetWidget(id)).FirstOrDefault();
        }

        public WidgetGroup GetCurrentWidgetGroup()
        {
            if (CurrentWidgetGroup == null)
                return null;

            if (ActiveWidgetGroups > 1)
                return CurrentWidgetGroup.GetChild(ActiveWidgetGroupsList[ActiveWidgetGroupsList.Count - 1]);

            return CurrentWidgetGroup;
        }
        public WidgetGroup GetWidgetGroup(string name)
        {
            return WidgetGroups.Where(x => x.Name == name).FirstOrDefault();
        }
        public WidgetGroup GetWidgetGroup(int id)
        {
            return WidgetGroups.Where(x => x.ID == id).FirstOrDefault();
        }

        public bool DoesWidgetGroupExist(string name)
        {
            return GetWidgetGroup(name) != null;
        }
        public bool DoesWidgetGroupExist(int id)
        {
            return GetWidgetGroup(id) != null;
        }

        public WidgetGroup CreateWidgetGroup(string name)
        {
            if (DoesWidgetGroupExist(name))
                return null;

            // Create new widget group
            WidgetGroup widgetGroup = new WidgetGroup(name, ScriptName, ThreadID);

            // Check if there is a current widget group
            if (CurrentWidgetGroup != null)
            {
                widgetGroup.IsChildren = true;

                // Check where to add the child widget group
                WidgetGroup childWidgetGroup = CurrentWidgetGroup.GetChild(ActiveWidgetGroupsList[ActiveWidgetGroupsList.Count - 1]);

                if (childWidgetGroup != null)
                {
                    // Add to another child widget group
                    childWidgetGroup.Children.Add(widgetGroup);
                }
                else
                {
                    // Add widget group to current widget group
                    CurrentWidgetGroup.Children.Add(widgetGroup);
                }
            }
            else
            {
                // Add widget group to global widget groups for this script
                WidgetGroups.Add(widgetGroup);
            }

            // Store the name of this widget group
            ActiveWidgetGroupsList.Add(name);

            // Set current widget group
            if (ActiveWidgetGroups == 0)
                CurrentWidgetGroup = widgetGroup;

            // Increase global active widget groups counter
            ActiveWidgetGroups++;

            return widgetGroup;
        }
        public bool DeleteWidgetGroup(int id)
        {
            WidgetGroup widgetGroup = GetWidgetGroup(id);

            if (widgetGroup == null)
                return false;

            return WidgetGroups.Remove(widgetGroup);
        }

        public bool HasDebugFile()
        {
            return DebugFile != null;
        }
        public ScriptDebugFile CreateDebugFile()
        {
            if (HasDebugFile())
                return null;

            DebugFile = new ScriptDebugFile(ScriptName);

            if (!DebugFile.Open(string.Format("{0}\\Logs\\{1}_dbg.log", Main.Instance.PluginResourceFolder, ScriptName)))
                Utils.LogToDebugger("Failed to open script debug file for '{0}' ({1})!", ScriptName, ThreadID);

            return DebugFile;
        }
        public void CloseDebugFile()
        {
            if (!HasDebugFile())
                return;

            DebugFile.Close();
            DebugFile = null;
        }
        #endregion

        #region Methods
        public void Cleanup()
        {
            IsDebuggingEnabled = false;

            // Close debug file
            CloseDebugFile();

            // Clear added widget groups
            WidgetGroups.ForEach(x => x.Cleanup());
            WidgetGroups.Clear();
            WidgetGroups = null;
        }

        public void EndCurrentWidgetGroup()
        {
            if (ActiveWidgetGroups == 0)
                return;

            ActiveWidgetGroups--;
            ActiveWidgetGroupsList.RemoveAt(ActiveWidgetGroupsList.Count - 1);

            if (ActiveWidgetGroups == 0)
            {
                CurrentWidgetGroup = null;
                ActiveWidgetGroupsList.Clear();
            }
        }
        #endregion

    }
}
