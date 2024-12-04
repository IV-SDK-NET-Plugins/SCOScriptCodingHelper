using IVSDKDotNet;

namespace SCOScriptCodingHelper.Classes
{
    internal class Logging
    {

#if DEBUG
        public static bool EnableLogging = true;
#else
        public static bool EnableLogging = false;
#endif


        public static void Log(string str, params object[] args)
        {
            if (!EnableLogging)
                return;

            IVGame.Console.Print(string.Format("[SCOScriptCodingHelper] {0}", string.Format(str, args)));
        }
        public static void LogWarning(string str, params object[] args)
        {
            if (!EnableLogging)
                return;

            IVGame.Console.PrintWarning(string.Format("[SCOScriptCodingHelper] {0}", string.Format(str, args)));
        }
        public static void LogError(string str, params object[] args)
        {
            if (!EnableLogging)
                return;

            IVGame.Console.PrintError(string.Format("[SCOScriptCodingHelper] {0}", string.Format(str, args)));
        }

        public static void LogDebug(string str, params object[] args)
        {
#if DEBUG
            IVGame.Console.Print(string.Format("[SCOScriptCodingHelper] [DEBUG] {0}", string.Format(str, args)));
#endif
        }

    }
}
