using System;
using System.IO;

namespace SCOScriptCodingHelper.Classes
{
    public class ScriptDebugFile
    {

        #region Variables
        private string targetScriptName;
        private StreamWriter streamWriter;
        #endregion

        #region Constructor
        public ScriptDebugFile(string scriptName)
        {
            targetScriptName = scriptName;
        }
        #endregion

        public bool Open(string fileName)
        {
            if (streamWriter != null)
                return false;

            try
            {
                streamWriter = File.AppendText(fileName);
                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to open script debug file for '{0}'! Details: {1}", targetScriptName, ex);
            }

            return false;
        }
        public void Close()
        {
            if (streamWriter == null)
                return;

            streamWriter.Dispose();
            streamWriter = null;
        }

        public void Write(int value)
        {
            if (streamWriter == null)
                return;

            streamWriter.Write(value);
        }
        public void Write(float value)
        {
            if (streamWriter == null)
                return;

            streamWriter.Write(value);
        }
        public void Write(string value)
        {
            if (streamWriter == null)
                return;

            streamWriter.Write(value);
        }

    }
}
