using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Log
{
    public class SimpleLog
    {
        public static void Log(params object[] messages)
        {
            string str = string.Empty;
            foreach (var m in messages)
            {
                str += m.ToString() + " ";
            }
            Debug.Log(str);
        }

        public static void Info(params object[] messages)
        {
            Log("Info: ", messages);
        }
    }
}
