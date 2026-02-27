using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System;
using System.IO;

public class Debug2 : MonoBehaviour
{
    // Singleton instance
    private static Debug2 instance;
    // List to store individual log entries
    private List<string> logList = new List<string>();
    // StringBuilder to store all logs
    private StringBuilder logBuffer = new StringBuilder();
    // Maximum number of logs to store
    private const int MAX_LOGS = 2048;
    // Counter for pruned logs
    private int prunedLogCount = 0;
#if UNITY_EDITOR
    // Path to the log file in Resources
    private static readonly string logFilePath = Application.dataPath + "/Resources/Debug2AllLog.txt";
#endif

    // Ensure instance exists and persists across scenes
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        instance = FindFirstObjectByType<Debug2>();
        if (instance == null)
        {
            GameObject debugObject = new GameObject("Debug2");
            instance = debugObject.AddComponent<Debug2>();
            DontDestroyOnLoad(debugObject);
        }
    }

    // Awake method to initialize file in Editor
    private void Awake()
    {
#if UNITY_EDITOR
        // Create or clear the log file
        if (!Directory.Exists(Application.dataPath + "/Resources"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources");
        }
        File.WriteAllText(logFilePath, string.Empty); // Clear or create the file
        Debug.Log("Debug2: Log file created/cleared at " + logFilePath);
#endif
    }

    // Public method to access the singleton instance
    public static Debug2 Instance
    {
        get
        {
            if (instance == null)
            {
                Initialize();
            }
            return instance;
        }
    }

    // Log a message (equivalent to Debug.Log)
    public static void Log(string message)
    {
        string formattedMessage = FormatMessage("LOG", message);
        Debug.Log(message);
        Instance.AppendToBuffer(formattedMessage);
    }

    // Log a warning (equivalent to Debug.Warning)
    public static void LogWarning(string message)
    {
        string formattedMessage = FormatMessage("WARNING", message);
        Debug.LogWarning(message);
        Instance.AppendToBuffer(formattedMessage);
    }

    // Log an error (equivalent to Debug.Error)
    public static void LogError(string message)
    {
        string formattedMessage = FormatMessage("ERROR", message);
        Debug.LogError(message);
        Instance.AppendToBuffer(formattedMessage);
    }

    // Log an exception (equivalent to Debug.LogException)
    public static void LogException(Exception exception)
    {
        string formattedMessage = FormatMessage("EXCEPTION", exception.ToString());
        Debug.LogException(exception);
        Instance.AppendToBuffer(formattedMessage);
    }

    // Copy all logs to clipboard with header
    public static void CopyLogsToClipboard()
    {
        string deviceName = SystemInfo.deviceName;
        string header = $"[Log Info] Total Logs: {Instance.logList.Count}, Pruned Logs: {Instance.prunedLogCount}, Device: {deviceName}\n";
        string fullLog = header + Instance.logBuffer.ToString();
        GUIUtility.systemCopyBuffer = fullLog;
        Debug.Log("Debug2: All logs copied to clipboard with header.");
    }

    // Clear the log buffer and list
    public static void ClearLogs()
    {
        Instance.logList.Clear();
        Instance.logBuffer.Clear();
        Instance.prunedLogCount = 0;
        Debug.Log("Debug2: Log buffer cleared.");
#if UNITY_EDITOR
        // Also clear the file
        File.WriteAllText(logFilePath, string.Empty);
#endif
    }

    // Format log message with timestamp and type
    private static string FormatMessage(string type, string message)
    {
        return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {message}\n";
    }

    // Append message to log buffer and list, prune if necessary
    private void AppendToBuffer(string message)
    {
        logList.Add(message);
        logBuffer.Append(message);
        // Prune logs if exceeding MAX_LOGS
        while (logList.Count > MAX_LOGS)
        {
            logList.RemoveAt(0); // Remove oldest log
            prunedLogCount++;
            // Rebuild logBuffer from remaining logs
            logBuffer.Clear();
            foreach (string log in logList)
            {
                logBuffer.Append(log);
            }
        }
#if UNITY_EDITOR
        // Append to file in Editor
        File.AppendAllText(logFilePath, message);
#endif
    }

    // Optional: Get all logs as string
    public static string GetAllLogs()
    {
        return Instance.logBuffer.ToString();
    }

    // Get all logs as a single string with newline separators
    public static string GetStringAllLogs()
    {
        return string.Join("\n", Instance.logList);
    }

    public static int GetLogCount()
    {
        return Instance.logList.Count;
    }
}