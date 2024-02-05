using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Log
{
    public class SimpleLog
    {
        public static void Log(string level, params object[] messages)
        {
            StringBuilder str = new StringBuilder(level);
            foreach (var m in messages)
            {
                str.Append(m.ToString());
                str.Append(" ");
            }
            Debug.Log(str);
        }

        public static void Info(params object[] messages)
        {
            Log("Info: ", messages);
        }
    }
}
