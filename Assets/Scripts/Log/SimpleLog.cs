using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Log
{
    public enum SimpleLogLevel
    {
        Info,
        Warn,
        Error,
    }
    public class SimpleLog
    {
        public static void Log(SimpleLogLevel level, string levelStr, params object[] messages)
        {
            StringBuilder str = new StringBuilder(levelStr);
            foreach (var m in messages)
            {
                str.Append(m.ToString());
                str.Append(" ");
            }
            if (level == SimpleLogLevel.Info)
            {
                Debug.Log(str);
            }
            else if (level == SimpleLogLevel.Warn)
            {
                Debug.LogWarning(str);
            }
            else if (level == SimpleLogLevel.Error)
            {
                Debug.LogError(str);
            }
        }

        public static void Info(params object[] messages)
        {
            Log(SimpleLogLevel.Info, "Info: ", messages);
        }

        public static void Warn(params object[] messages)
        {
            Log(SimpleLogLevel.Warn, "Warn: ", messages);
        }

        public static void Error(params object[] messages)
        {
            Log(SimpleLogLevel.Error, "Error: ", messages);
        }
    }
}
