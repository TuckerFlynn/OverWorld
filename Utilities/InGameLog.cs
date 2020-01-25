using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InGameLog : MonoBehaviour
{
    public static InGameLog inGameLog;
    public Text log;
    private List<string> logList = new List<string>();

    private void Awake()
    {
        if (inGameLog == null)
        {
            DontDestroyOnLoad(gameObject);
            inGameLog = this;
        }
        else if (inGameLog != this)
        {
            Destroy(this.gameObject);
        }
    }

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[").Append(System.DateTime.Now.ToShortTimeString()).Append("] ");
        builder.Append(logString).AppendLine();
        // Don't log if the same message is repeating
        if (logList.Count > 0)
        {
            if (!builder.ToString().Equals(logList[logList.Count - 1]))
                logList.Add(builder.ToString());
        }
        else
        {
            logList.Add(builder.ToString());
        }

        if (logList.Count > 6)
            logList.RemoveAt(0);
        log.text = "";
        for (int i = 0; i < logList.Count; i++)
        {
            log.text += logList[i];
        }
    }
}
