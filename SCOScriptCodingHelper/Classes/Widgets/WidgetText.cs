using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes.Widgets
{
    public class WidgetText : WidgetBase
    {

        #region Variables
        private string content;
        #endregion

        #region Constructor
        public WidgetText(string name) : base(WidgetType.Text, name)
        {
            content = "";
        }
        #endregion

        #region Methods
        public void SetContent(string widgetContent)
        {
            content = widgetContent;
        }
        #endregion

        #region Functions
        public string GetContent()
        {
            return content;
        }
        #endregion

        public override void Draw()
        {
            ImGuiIV.InputText(Name, ref content);
        }

    }
}
