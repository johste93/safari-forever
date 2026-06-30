using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogEntry 
{
    public string message;
    public string stackTrace;
    public LogType type;

    public LogEntry(string message, string stackTrace, LogType type)
    {
        this.message = message;
        this.stackTrace = stackTrace;
        this.type = type;
    }
}
