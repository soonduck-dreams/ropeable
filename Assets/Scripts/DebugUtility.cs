using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugUtility : MonoBehaviour
{
    public static void Log(params object[] args)
    {
        StringBuilder str = new StringBuilder();

        for (int i = 0; i < args.Length; i++)
        {
            str.Append(args[i].ToString());
            str.Append(' ');
        }

        Debug.Log(str.ToString());
    }
}
