using System;
using System.Diagnostics;
using System.Text;

namespace SCOScriptCodingHelper.Classes
{
    internal class Utils
    {

        public static void LogToDebugger(string message, params object[] args)
        {
#if DEBUG
            Debugger.Log(0, "SCOScriptCodingHelper", string.Format(message, args) + "\n");
#endif
        }

        public static unsafe void WriteStringToAddress(IntPtr targetAddress, string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value + "\0"); // Convert to ASCII, add null terminator
            fixed (byte* ptr = bytes)
            {
                Buffer.MemoryCopy(ptr, (void*)targetAddress, bytes.Length, bytes.Length);
            }
        }

    }
}
