namespace SCOScriptCodingHelper.Classes.Widgets
{
    public abstract class WidgetBase
    {

        #region Variables
        public int ID;
        public string Name;
        public bool ReadOnly;
        public WidgetType Type;
        #endregion

        #region Constructor
        public WidgetBase(WidgetType type, string name)
        {
            ID = IVSDKDotNet.Native.Natives.GENERATE_RANDOM_INT();
            Name = name;
            Type = type;
        }
        public WidgetBase(WidgetType type)
        {
            ID = IVSDKDotNet.Native.Natives.GENERATE_RANDOM_INT();
            Type = type;
        }
        #endregion

        public virtual void Cleanup() { }
        public abstract void Draw();

    }
}
