using System;
using System.Collections.Generic;
using System.IO;
using Xunit.Abstractions;

namespace SecaFolderWatcher;
public static class Logger
{
    private static string _logPath = Path.GetFullPath(Path.Combine(".", "logs", "log.txt"));
    private static List<Action> _listeningOnLog = new List<Action>();
    private static string _sessionLog = "";
    private static string _lastMessage = "";

    private static bool _testMode = false;
    private static ITestOutputHelper _output;

    private static string _logPrefix = "";
    public static string logPrefix
    {
        get { return _logPrefix; }
        set
        {
            if (value.Equals("")) return;
            _logPrefix = $" <<{value}>> ";
        }
    }

    public static void EnableTestMode(ITestOutputHelper output)
    {
      _output = output;
      _testMode = true;
      _logPrefix = "";
    }

    public static string GetLastMessage()
    {
        return _lastMessage;
    }

    public static void SetLogPath(FileInfo logfile)
    {
        LogInformation($"Switch to use logfile specified in .ini file. Further logs are written to {logfile.FullName}");
        _logPath = logfile.FullName;
    }

    public static string GetSessionLog()
    {
        return _sessionLog;
    }

    public static void EmptySessionLog()
    {
        _sessionLog = "";
    }

    public static void RegisterOnLogCallback(Action action)
    {
        _listeningOnLog.Add(action);
    }

    private static void TriggerOnLogCallbacks()
    {
        foreach (Action action in _listeningOnLog)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    public static void FlushLog(string logPath, string message)
    {
        try
        {
            message = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " " + logPrefix + message;
            _sessionLog = $"{_sessionLog} {message} \n";
            if (!File.Exists(logPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                FileStream stream = File.Create(logPath);
                stream.Close();
            }
            FileStream fileStream = new FileStream(
                logPath,
                FileMode.Append,
                FileAccess.Write
                );
            StreamWriter fileWriter = new StreamWriter((Stream)fileStream);
            fileWriter.WriteLine(message);
            fileWriter.Close();
            fileStream.Close();
            TriggerOnLogCallbacks();
            if(_testMode)
            {
              try {
                _output.WriteLine("Using Output " + message);
              }
              catch {
                Console.WriteLine("Using Console " + message);
              }
            }
            else 
            {
              Console.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            string errMessage = "Logger wasn't able to write log. " +
                "Proceed with caution since Exception handling will not create any logs while logger fails! " +
                "Message of thrown exception is " + ex.Message;
            _sessionLog = $"{_sessionLog} \n {errMessage}";
            throw new Exception(errMessage);
        }
    }
    public static void Log(string message)
    {
        FlushLog(_logPath, message);
    }

    public static void LogError(string message)
    {
        message = "Error Log ::> " + message;
        FlushLog(_logPath, message);
    }

    public static void LogErrorVerbose(string message, string compositeMessage)
    {
      message = $"{message} \r\n \tThe available related error message is \r\n \t\"{compositeMessage}\"";
      LogError(message);
    }

    public static void LogErrorVerbose(string message, Exception e){
      LogErrorVerbose(message, e.Message);
    }

    public static void LogTest(string message){
      message = "Test Log ::> " + message;
      FlushLog(_logPath, message);
    }

    public static void LogInformation(string message)
    {
        message = "Information Log ::> " + message;
        FlushLog(_logPath, message);
    }

    public static void LogWarning(string message)
    {
        message = "Warning Log ::> " + message;
        FlushLog(_logPath, message);
    }

    public static void Log(string message, string path)
    {
        FlushLog(path, message);
    }

    public static void LogError(string message, string path)
    {
        message = "Error Log ::> " + message;
        FlushLog(path, message);
    }

    public static void LogErrorVerbose(string message, string compositeMessage, string path)
    {
      message = $"{message} \r\n \tThe available related error message is \r\n \t\"{compositeMessage}\"";
      LogError(message, path);
    }

    public static void LogErrorVerbose(string message, Exception e, string path){
      LogErrorVerbose(message, e.Message, path);
    }

    public static void LogTest(string message, string path)
    {
      message = "Test Log ::> " + message;
      FlushLog(path, message);
    }

    public static void LogInformation(string message, string path)
    {
        message = "Information Log ::> " + message;
        FlushLog(path, message);
    }

    public static void LogWarning(string message, string path)
    {
        message = "Warning Log ::> " + message;
        FlushLog(path, message);
    }
}
