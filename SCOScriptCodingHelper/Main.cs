using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

using IVSDKDotNet;
using IVSDKDotNet.Hooking;
using IVSDKDotNet.Manager;
using IVSDKDotNet.Native;
using static IVSDKDotNet.Native.Natives;

using SCOScriptCodingHelper.Classes;
using SCOScriptCodingHelper.Classes.Widgets;

namespace SCOScriptCodingHelper
{
    public class Main : ManagerPlugin
    {

        #region Variables
        internal static Main Instance;
        private List<NativeCallDelegate> overridenNativeCallDelegates;
        private List<ScriptDebug> scriptsDebug;
        #endregion

        #region Delegates
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeCallDelegate(UIntPtr nativeCallContextPtr);
        #endregion

        #region Natives
        private static int n_DEBUG_ON(UIntPtr nativeCallContextPtr)
        {
            // Create all the debug stuff for the calling script
            uint threadId = GET_ID_OF_THIS_THREAD();
            ScriptDebug script = Instance.GetScriptDebug(threadId);

            if (script == null)
            {
                script = new ScriptDebug(threadId, IVTheScripts.GetNameOfCurrentScript());
                script.IsDebuggingEnabled = true;
                Instance.AddScriptDebug(script);
                Utils.LogToDebugger("Added new script debug for '{0}' ({1})", script.ScriptName, script.ThreadID);
            }
            else
            {
                script.IsDebuggingEnabled = true;
            }

            return 0;
        }
        private static int n_DEBUG_OFF(UIntPtr nativeCallContextPtr)
        {
            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                script.IsDebuggingEnabled = false;
            }
            else
            {
                LogNotCalledDebugOnNative("DEBUG_OFF");
            }

            return 0;
        }

        // Debug file natives
        private static int n_OPEN_DEBUG_FILE(UIntPtr nativeCallContextPtr)
        {
            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                if (!script.HasDebugFile())
                {
                    script.CreateDebugFile();
                }
                else
                {
                    Logging.LogWarning("Cannot open another debug file for script '{0}' as it already has one open.", IVTheScripts.GetNameOfCurrentScript());
                }
            }
            else
            {
                LogNotCalledDebugOnNative("OPEN_DEBUG_FILE");
            }

            return 0;
        }
        private static int n_CLOSE_DEBUG_FILE(UIntPtr nativeCallContextPtr)
        {
            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                if (script.HasDebugFile())
                {
                    script.CloseDebugFile();
                }
                else
                {
                    Logging.LogWarning("There is no open debug file to close for script '{0}'.", IVTheScripts.GetNameOfCurrentScript());
                }
            }
            else
            {
                LogNotCalledDebugOnNative("CLOSE_DEBUG_FILE");
            }

            return 0;
        }
        private static int n_SAVE_INT_TO_DEBUG_FILE(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out int arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                if (script.HasDebugFile())
                {
                    script.DebugFile.Write(arg1);
                }
                else
                {
                    Logging.LogWarning("Script '{0}' does not currently have a debug file open.", IVTheScripts.GetNameOfCurrentScript());
                }
            }
            else
            {
                LogNotCalledDebugOnNative("SAVE_INT_TO_DEBUG_FILE");
            }

            return 0;
        }
        private static int n_SAVE_FLOAT_TO_DEBUG_FILE(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out float arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                if (script.HasDebugFile())
                {
                    script.DebugFile.Write(arg1);
                }
                else
                {
                    Logging.LogWarning("Script '{0}' does not currently have a debug file open.", IVTheScripts.GetNameOfCurrentScript());
                }
            }
            else
            {
                LogNotCalledDebugOnNative("SAVE_FLOAT_TO_DEBUG_FILE");
            }

            return 0;
        }
        private static int n_SAVE_NEWLINE_TO_DEBUG_FILE(UIntPtr nativeCallContextPtr)
        {
            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                if (script.HasDebugFile())
                {
                    script.DebugFile.Write(Environment.NewLine);
                }
                else
                {
                    Logging.LogWarning("Script '{0}' does not currently have a debug file open.", IVTheScripts.GetNameOfCurrentScript());
                }
            }
            else
            {
                LogNotCalledDebugOnNative("SAVE_NEWLINE_TO_DEBUG_FILE");
            }

            return 0;
        }
        private static int n_SAVE_STRING_TO_DEBUG_FILE(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                if (script.HasDebugFile())
                {
                    script.DebugFile.Write(arg1);
                }
                else
                {
                    Logging.LogWarning("Script '{0}' does not currently have a debug file open.", IVTheScripts.GetNameOfCurrentScript());
                }
            }
            else
            {
                LogNotCalledDebugOnNative("SAVE_STRING_TO_DEBUG_FILE");
            }

            return 0;
        }

        // Debug widget natives
        private static int n_CREATE_WIDGET_GROUP(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup widgetGroup = script.CreateWidgetGroup(arg1);
                Utils.LogToDebugger("Added new widget group '{0}' to script '{1}' ({2}). ID: {3}", arg1, script.ScriptName, script.ThreadID, widgetGroup.ID);
                nativeCallContext.SetResult(0, widgetGroup.ID);
            }
            else
            {
                LogNotCalledDebugOnNative("CREATE_WIDGET_GROUP");
            }

            return 0;
        }
        private static int n_END_WIDGET_GROUP(UIntPtr nativeCallContextPtr)
        {
            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                script.EndCurrentWidgetGroup();
            }
            else
            {
                LogNotCalledDebugOnNative("END_WIDGET_GROUP");
            }

            return 0;
        }
        private static int n_ADD_WIDGET_SLIDER(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);
            nativeCallContext.GetArgument(1, out IntPtr arg2);
            nativeCallContext.GetArgument(2, out int arg3);
            nativeCallContext.GetArgument(3, out int arg4);
            nativeCallContext.GetArgument(4, out float arg5);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                int id = 0;

                if (currentWidgetGroup != null)
                {
                    WidgetSlider widget = currentWidgetGroup.CreateWidgetSlider(arg1, arg2, arg3, arg4, arg5);
                    id = widget.ID;
                }
                else
                {
                    LogNoActiveWidgetGroup("ADD_WIDGET_SLIDER");
                }

                nativeCallContext.SetResult(0, id);
            }
            else
            {
                LogNotCalledDebugOnNative("ADD_WIDGET_SLIDER");
            }

            return 0;
        }
        private static int n_ADD_WIDGET_FLOAT_SLIDER(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);
            nativeCallContext.GetArgument(1, out IntPtr arg2);
            nativeCallContext.GetArgument(2, out float arg3);
            nativeCallContext.GetArgument(3, out float arg4);
            nativeCallContext.GetArgument(4, out float arg5);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                int id = 0;

                if (currentWidgetGroup != null)
                {
                    WidgetFloatSlider widget = currentWidgetGroup.CreateWidgetFloatSlider(arg1, arg2, arg3, arg4, arg5);
                    id = widget.ID;
                }
                else
                {
                    LogNoActiveWidgetGroup("ADD_WIDGET_FLOAT_SLIDER");
                }

                nativeCallContext.SetResult(0, id);
            }
            else
            {
                LogNotCalledDebugOnNative("ADD_WIDGET_FLOAT_SLIDER");
            }

            return 0;
        }
        private static int n_ADD_WIDGET_FLOAT_READ_ONLY(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);
            nativeCallContext.GetArgument(1, out IntPtr arg2);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                int id = 0;

                if (currentWidgetGroup != null)
                {
                    WidgetFloatSlider widget = currentWidgetGroup.CreateWidgetFloatSlider(arg1, arg2, float.MinValue, float.MaxValue, 1f);
                    widget.ReadOnly = true;
                    id = widget.ID;
                }
                else
                {
                    LogNoActiveWidgetGroup("ADD_WIDGET_FLOAT_READ_ONLY");
                }

                nativeCallContext.SetResult(0, id);
            }
            else
            {
                LogNotCalledDebugOnNative("ADD_WIDGET_FLOAT_READ_ONLY");
            }

            return 0;
        }
        private static int n_ADD_WIDGET_TOGGLE(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);
            nativeCallContext.GetArgument(1, out IntPtr arg2);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                int id = 0;

                if (currentWidgetGroup != null)
                {
                    WidgetToggle widget = currentWidgetGroup.CreateWidgetToggle(arg1, arg2);
                    id = widget.ID;
                }
                else
                {
                    LogNoActiveWidgetGroup("ADD_WIDGET_TOGGLE");
                }

                nativeCallContext.SetResult(0, id);
            }
            else
            {
                LogNotCalledDebugOnNative("ADD_WIDGET_TOGGLE");
            }

            return 0;
        }
        private static int n_ADD_WIDGET_STRING(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                int id = 0;

                if (currentWidgetGroup != null)
                {
                    WidgetString widget = currentWidgetGroup.CreateWidgetString(arg1);
                    id = widget.ID;
                }
                else
                {
                    LogNoActiveWidgetGroup("ADD_WIDGET_STRING");
                }

                nativeCallContext.SetResult(0, id);
            }
            else
            {
                LogNotCalledDebugOnNative("ADD_WIDGET_STRING");
            }

            return 0;
        }
        private static int n_DELETE_WIDGET_GROUP(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out int arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                nativeCallContext.SetResult(0, script.DeleteWidgetGroup(arg1));
            }
            else
            {
                LogNotCalledDebugOnNative("DELETE_WIDGET_GROUP");
            }

            return 0;
        }
        private static int n_DELETE_WIDGET(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out int arg1);
            nativeCallContext.GetArgument(1, out int arg2);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                // Find the widget group
                WidgetGroup widgetGroup = script.GetWidgetGroup(arg1);

                if (widgetGroup == null)
                    return 0;

                // Try to remove widget from found widget group
                nativeCallContext.SetResult(0, widgetGroup.DeleteWidget(arg2));
            }
            else
            {
                LogNotCalledDebugOnNative("DELETE_WIDGET");
            }

            return 0;
        }
        private static int n_DOES_WIDGET_GROUP_EXIST(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out int arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                nativeCallContext.SetResult(0, script.DoesWidgetGroupExist(arg1));
            }
            else
            {
                LogNotCalledDebugOnNative("DOES_WIDGET_GROUP_EXIST");
            }

            return 0;
        }

        private static int n_START_NEW_WIDGET_COMBO(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                int id = 0;

                if (currentWidgetGroup != null)
                {
                    WidgetCombo widget = currentWidgetGroup.CreateWidgetCombo();
                    id = widget.ID;
                }
                else
                {
                    LogNoActiveWidgetGroup("START_NEW_WIDGET_COMBO");
                }

                nativeCallContext.SetResult(0, id);
            }
            else
            {
                LogNotCalledDebugOnNative("START_NEW_WIDGET_COMBO");
            }

            return 0;
        }
        private static int n_ADD_TO_WIDGET_COMBO(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                
                if (currentWidgetGroup != null)
                {
                    currentWidgetGroup.CurrentCombo.AddItem(arg1);
                }
                else
                {
                    LogNoActiveWidgetGroup("ADD_TO_WIDGET_COMBO");
                }
            }
            else
            {
                LogNotCalledDebugOnNative("ADD_TO_WIDGET_COMBO");
            }

            return 0;
        }
        private static int n_FINISH_WIDGET_COMBO(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);
            nativeCallContext.GetArgument(1, out IntPtr arg2);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();

                if (currentWidgetGroup != null)
                {
                    currentWidgetGroup.CurrentCombo.SetStuff(arg1, arg2);
                    currentWidgetGroup.CurrentCombo = null;
                }
                else
                {
                    LogNoActiveWidgetGroup("FINISH_WIDGET_COMBO");
                }
            }
            else
            {
                LogNotCalledDebugOnNative("FINISH_WIDGET_COMBO");
            }

            return 0;
        }
        private static int n_ADD_TEXT_WIDGET(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out string arg1);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetGroup currentWidgetGroup = script.GetCurrentWidgetGroup();
                int id = 0;

                if (currentWidgetGroup != null)
                {
                    WidgetText widget = currentWidgetGroup.CreateWidgetText(arg1);
                    id = widget.ID;
                }
                else
                {
                    LogNoActiveWidgetGroup("ADD_TEXT_WIDGET");
                }

                nativeCallContext.SetResult(0, id);
            }
            else
            {
                LogNotCalledDebugOnNative("ADD_TEXT_WIDGET");
            }

            return 0;
        }
        private static int n_GET_CONTENTS_OF_TEXT_WIDGET(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out int arg1);
            nativeCallContext.GetArgument(1, out IntPtr arg2);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetBase widget = script.FindWidgetByID(arg1);

                if (widget != null)
                {
                    if (widget.Type == WidgetType.Text)
                    {
                        Utils.WriteStringToAddress(arg2, ((WidgetText)widget).GetContent());
                    }
                    else
                    {
                        Logging.LogWarning("Script '{0}' tried getting the contents of a text widget by its ID ({1}), but this widget is not a text widget.", IVTheScripts.GetNameOfCurrentScript(), arg1);
                    }
                }
                else
                {
                    Logging.LogWarning("Script '{0}' tried getting the contents of a text widget by its ID ({1}), but there is no such widget with this ID.", IVTheScripts.GetNameOfCurrentScript(), arg1);
                }
            }
            else
            {
                LogNotCalledDebugOnNative("GET_CONTENTS_OF_TEXT_WIDGET");
            }

            return 0;
        }
        private static int n_SET_CONTENTS_OF_TEXT_WIDGET(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out int arg1);
            nativeCallContext.GetArgument(1, out string arg2);

            ScriptDebug script = Instance.GetScriptDebug(GET_ID_OF_THIS_THREAD());

            if (script != null)
            {
                WidgetBase widget = script.FindWidgetByID(arg1);

                if (widget != null)
                {
                    if (widget.Type == WidgetType.Text)
                    {
                        ((WidgetText)widget).SetContent(arg2);
                    }
                    else
                    {
                        Logging.LogWarning("Script '{0}' tried setting the contents of a text widget by its ID ({1}), but this widget is not a text widget.", IVTheScripts.GetNameOfCurrentScript(), arg1);
                    }
                }
                else
                {
                    Logging.LogWarning("Script '{0}' tried setting the contents of a text widget by its ID ({1}), but there is no such widget with this ID.", IVTheScripts.GetNameOfCurrentScript(), arg1);
                }
            }
            else
            {
                LogNotCalledDebugOnNative("SET_CONTENTS_OF_TEXT_WIDGET");
            }

            return 0;
        }

        // Console natives
        private static int n_GET_LATEST_CONSOLE_COMMAND(UIntPtr nativeCallContextPtr)
        {
            IVNativeCallContext nativeCallContext = IVNativeCallContext.FromUIntPtr(nativeCallContextPtr);
            nativeCallContext.GetArgument(0, out IntPtr arg1);

            string command = Instance.GetManagerInstance().GetLastConsoleCommand();
            
            if (string.IsNullOrWhiteSpace(command))
                command = "NULL";

            Utils.WriteStringToAddress(arg1, command);

            return 0;
        }
        private static int n_RESET_LATEST_CONSOLE_COMMAND(UIntPtr nativeCallContextPtr)
        {
            Instance.GetManagerInstance().ResetLastConsoleCommand();
            return 0;
        }
        #endregion

        #region Functions
        private ScriptDebug GetScriptDebug(uint threadId)
        {
            return scriptsDebug.Where(x => x.ThreadID == threadId).FirstOrDefault();
        }
        #endregion

        #region Methods
        public void AddScriptDebug(ScriptDebug scriptDebug)
        {
            scriptsDebug.Add(scriptDebug);
        }

        private void SwitchDebuggingStateOfScripts(bool on)
        {
            scriptsDebug.ForEach(x => x.IsDebuggingEnabled = on);
        }
        private void CheckIfScriptsAreStillValid()
        {
            ScriptDebug[] invalidScripts = scriptsDebug.Where(x => !IS_THREAD_ACTIVE(x.ThreadID)).ToArray();

            for (int i = 0; i < invalidScripts.Length; i++)
            {
                ScriptDebug script = invalidScripts[i];
                script.Cleanup();
                scriptsDebug.Remove(script);
            }
        }

        private static void LogNotCalledDebugOnNative(string nativeName)
        {
            Logging.LogWarning("Script '{0}' called the '{1}' native but didn't call the 'DEBUG_ON' native first.", IVTheScripts.GetNameOfCurrentScript(), nativeName);
        }
        private static void LogNoActiveWidgetGroup(string nativeName)
        {
            Logging.LogWarning("Script '{0}' called the '{1}' native but there is no active widget group to add this widget to.", IVTheScripts.GetNameOfCurrentScript(), nativeName);
            Logging.LogWarning("Make sure you add widgets to a widget group between 'CREATE_WIDGET_GROUP' and 'END_WIDGET_GROUP' calls.");
        }
        #endregion

        #region Constructor
        public Main() : base("SCO Script Coding Helper", "ItsClonkAndre")
        {
            Instance = this;

            overridenNativeCallDelegates = new List<NativeCallDelegate>();
            scriptsDebug = new List<ScriptDebug>();

            // IV-SDK .NET stuff
            ForceNoAbort = true;

            OnShutdown += Main_OnShutdown;
            GameHooks.OnRegisterNativeNoChecks += GameHooks_OnRegisterNative;
            GameLoad += Main_GameLoad;
            OnImGuiGlobalRendering += Main_OnImGuiGlobalRendering;
            OnImGuiManagerRendering += Main_OnImGuiManagerRendering;
            Tick += Main_Tick;
        }
        #endregion

        private void Main_OnShutdown(object sender, EventArgs e)
        {
            overridenNativeCallDelegates.Clear();
            scriptsDebug.Clear();
        }

        private HookCallback<bool> GameHooks_OnRegisterNative(ref uint hash, ref IntPtr funcPtr)
        {
            NativeCallDelegate d = null;

            // Override native functions to use our own function
            switch ((eNativeHash)hash)
            {
                case eNativeHash.NATIVE_DEBUG_ON:
                    d = new NativeCallDelegate(n_DEBUG_ON);
                    break;
                case eNativeHash.NATIVE_DEBUG_OFF:
                    d = new NativeCallDelegate(n_DEBUG_OFF);
                    break;

                case eNativeHash.NATIVE_SET_DEBUG_TEXT_VISIBLE:

                    break;

                // Debug file natives
                case eNativeHash.NATIVE_OPEN_DEBUG_FILE:
                    d = new NativeCallDelegate(n_OPEN_DEBUG_FILE);
                    break;
                case eNativeHash.NATIVE_CLOSE_DEBUG_FILE:
                    d = new NativeCallDelegate(n_CLOSE_DEBUG_FILE);
                    break;
                case eNativeHash.NATIVE_SAVE_INT_TO_DEBUG_FILE:
                    d = new NativeCallDelegate(n_SAVE_INT_TO_DEBUG_FILE);
                    break;
                case eNativeHash.NATIVE_SAVE_FLOAT_TO_DEBUG_FILE:
                    d = new NativeCallDelegate(n_SAVE_FLOAT_TO_DEBUG_FILE);
                    break;
                case eNativeHash.NATIVE_SAVE_NEWLINE_TO_DEBUG_FILE:
                    d = new NativeCallDelegate(n_SAVE_NEWLINE_TO_DEBUG_FILE);
                    break;
                case eNativeHash.NATIVE_SAVE_STRING_TO_DEBUG_FILE:
                    d = new NativeCallDelegate(n_SAVE_STRING_TO_DEBUG_FILE);
                    break;

                // Debug widgets ntives
                case eNativeHash.NATIVE_CREATE_WIDGET_GROUP:
                    d = new NativeCallDelegate(n_CREATE_WIDGET_GROUP);
                    break;
                case eNativeHash.NATIVE_END_WIDGET_GROUP:
                    d = new NativeCallDelegate(n_END_WIDGET_GROUP);
                    break;
                case eNativeHash.NATIVE_ADD_WIDGET_SLIDER:
                    d = new NativeCallDelegate(n_ADD_WIDGET_SLIDER);
                    break;
                case eNativeHash.NATIVE_ADD_WIDGET_FLOAT_SLIDER:
                    d = new NativeCallDelegate(n_ADD_WIDGET_FLOAT_SLIDER);
                    break;
                case eNativeHash.NATIVE_ADD_WIDGET_READ_ONLY:

                    break;
                case eNativeHash.NATIVE_ADD_WIDGET_FLOAT_READ_ONLY:
                    d = new NativeCallDelegate(n_ADD_WIDGET_FLOAT_READ_ONLY);
                    break;
                case eNativeHash.NATIVE_ADD_WIDGET_TOGGLE:
                    d = new NativeCallDelegate(n_ADD_WIDGET_TOGGLE);
                    break;
                case eNativeHash.NATIVE_ADD_WIDGET_STRING:
                    d = new NativeCallDelegate(n_ADD_WIDGET_STRING);
                    break;
                case eNativeHash.NATIVE_DELETE_WIDGET_GROUP:
                    d = new NativeCallDelegate(n_DELETE_WIDGET_GROUP);
                    break;
                case eNativeHash.NATIVE_DELETE_WIDGET:
                    d = new NativeCallDelegate(n_DELETE_WIDGET);
                    break;
                case eNativeHash.NATIVE_DOES_WIDGET_GROUP_EXIST:
                    d = new NativeCallDelegate(n_DOES_WIDGET_GROUP_EXIST);
                    break;

                case eNativeHash.NATIVE_START_NEW_WIDGET_COMBO:
                    d = new NativeCallDelegate(n_START_NEW_WIDGET_COMBO);
                    break;
                case eNativeHash.NATIVE_ADD_TO_WIDGET_COMBO:
                    d = new NativeCallDelegate(n_ADD_TO_WIDGET_COMBO);
                    break;
                case eNativeHash.NATIVE_FINISH_WIDGET_COMBO:
                    d = new NativeCallDelegate(n_FINISH_WIDGET_COMBO);
                    break;

                case eNativeHash.NATIVE_ADD_TEXT_WIDGET:
                    d = new NativeCallDelegate(n_ADD_TEXT_WIDGET);
                    break;
                case eNativeHash.NATIVE_GET_CONTENTS_OF_TEXT_WIDGET:
                    d = new NativeCallDelegate(n_GET_CONTENTS_OF_TEXT_WIDGET);
                    break;
                case eNativeHash.NATIVE_SET_CONTENTS_OF_TEXT_WIDGET:
                    d = new NativeCallDelegate(n_SET_CONTENTS_OF_TEXT_WIDGET);
                    break;

                // Console natives
                case eNativeHash.NATIVE_GET_LATEST_CONSOLE_COMMAND:
                    d = new NativeCallDelegate(n_GET_LATEST_CONSOLE_COMMAND);
                    break;
                case eNativeHash.NATIVE_RESET_LATEST_CONSOLE_COMMAND:
                    d = new NativeCallDelegate(n_RESET_LATEST_CONSOLE_COMMAND);
                    break;
            }

            // Override native func and add to list of overriden native call delegates
            if (d != null)
            {
                overridenNativeCallDelegates.Add(d);
                funcPtr = Marshal.GetFunctionPointerForDelegate(d);
            }

            return new HookCallback<bool>(false);
        }

        private void Main_GameLoad(object sender, EventArgs e)
        {
            //IVCDStream.AddImage("IVSDKDotNet/plugins/SCOScriptCodingHelper/Data/additionalScripts.img", 1, -1);
            
            //IVGame.Console.PrintEx("additionalScripts.img was added at index {0}", additionalScriptsIndex);
            //IVGame.Console.PrintEx("FileName: {0}", IVCDStream.GetImageFileName(additionalScriptsIndex));
            //IVGame.Console.PrintEx("FileHandle: {0}", IVCDStream.GetImageFileHandle(additionalScriptsIndex));
        }

        private void Main_OnImGuiGlobalRendering(IntPtr devicePtr, ImGuiIV_DrawingContext ctx)
        {
            for (int i = 0; i < scriptsDebug.Count; i++)
            {
                ScriptDebug scriptDebug = scriptsDebug[i];

                if (!scriptDebug.IsDebuggingEnabled)
                    continue;

                for (int g = 0; g < scriptDebug.WidgetGroups.Count; g++)
                {
                    scriptDebug.WidgetGroups[g].Draw();
                }
            } 
        }
        private void Main_OnImGuiManagerRendering(IntPtr devicePtr)
        {
            ImGuiIV.HelpMarker("Gives you some quick access to debugging features.");
            ImGuiIV.SameLine();
            ImGuiIV.SeparatorText("Debugging");

            ImGuiIV.CheckBox("Enable logging", ref Logging.EnableLogging);
            ImGuiIV.SetItemTooltip("Allows the plugin to log stuff to the console.");

            if (ImGuiIV.Button("Enable debug of all scripts"))
            {
                SwitchDebuggingStateOfScripts(true);
            }
            ImGuiIV.SetItemTooltip("Enables the debugging of all scripts, showing all created debugging windows (widget groups).");
            ImGuiIV.SameLine();
            if (ImGuiIV.Button("Disable debug of all scripts"))
            {
                SwitchDebuggingStateOfScripts(false);
            }
            ImGuiIV.SetItemTooltip("Disables the debugging of all scripts, hiding all created debugging windows (widget groups).");

            ImGuiIV.Spacing(2);
            ImGuiIV.HelpMarker("Displays all the scripts that called the 'DEBUG_ON' native, which prepares them for debugging.");
            ImGuiIV.SameLine();
            ImGuiIV.SeparatorText("Scripts");

            for (int i = 0; i < scriptsDebug.Count; i++)
            {
                ScriptDebug scriptDebug = scriptsDebug[i];

                if (ImGuiIV.TreeNode(string.Format("{0} [ThreadID: {1}]##SCOScriptCodingHelperTreeNode", scriptDebug.ScriptName, scriptDebug.ThreadID)))
                {
                    ImGuiIV.SeparatorText("Details");
                    ImGuiIV.TextUnformatted("Name: {0}", scriptDebug.ScriptName);
                    ImGuiIV.TextUnformatted("ThreadID: {0}", scriptDebug.ThreadID);
                    ImGuiIV.TextUnformatted("WidgetGroups: {0}", scriptDebug.WidgetGroups.Count);
                    ImGuiIV.TextUnformatted("HasDebugFile: {0}", scriptDebug.HasDebugFile());
                    ImGuiIV.CheckBox("IsDebuggingEnabled", ref scriptDebug.IsDebuggingEnabled);

                    ImGuiIV.Spacing(2);
                    ImGuiIV.SeparatorText("Control");
                    if (ImGuiIV.Button("Terminate"))
                    {
                        DESTROY_THREAD((int)scriptDebug.ThreadID);
                    }
                    ImGuiIV.SetItemTooltip("Terminates this script.");

                    ImGuiIV.TreePop();
                }
            }

        }

        private void Main_Tick(object sender, EventArgs e)
        {
            CheckIfScriptsAreStillValid();
        }

    }
}
