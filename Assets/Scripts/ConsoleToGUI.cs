using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleToGUI : MonoBehaviour
{
    //#if !UNITY_EDITOR

    public Vector2 guiRectSize;

    static string myLog = "";
    private string output;
    private string stack;
    public Text debugText;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
        debugText.text = myLog;
    }


    void OnGUI()
    {
        //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
        {
            //myLog = GUI.TextArea(new Rect(guiRectSize.x, guiRectSize.y, Screen.width - 10, Screen.height -10), myLog);
        }
    }
    //#endif
}
