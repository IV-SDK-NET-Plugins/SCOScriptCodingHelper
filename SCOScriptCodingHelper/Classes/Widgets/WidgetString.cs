using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes.Widgets
{
    public class WidgetString : WidgetBase
    {

        #region Variables
        private string text;
        #endregion

        #region Constructor
        public WidgetString(string txt) : base(WidgetType.String)
        {
            text = txt;
        }
        #endregion

        public override void Draw()
        {
            ImGuiIV.TextUnformatted(text);
        }

    }
}
