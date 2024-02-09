
using System;
using System.Diagnostics;
using System.IO;

namespace SecaFolderWatcher;
public static class ProcessRunner
{
  public static int RunExecutableFile(FileInfo executablePath) {
    if (!File.Exists(executablePath.FullName)) {
      throw new ArgumentException($"The specified executable does not seem to exist. The given path is {executablePath.FullName}");
    }
    Process proc = new Process();
    //runs executable directly rather than shell
    proc.StartInfo.UseShellExecute = false;
    proc.StartInfo.FileName = executablePath.FullName;
    proc.StartInfo.CreateNoWindow = true;
    proc.StartInfo.RedirectStandardOutput = false;
    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

    int exitCode = 0;
    try 
    {
      proc.Start();
      proc.WaitForExit();
      exitCode = proc.ExitCode;
      
      if (exitCode != 0) {
        Logger.LogError($"Tried executing file at {executablePath.FullName} but failed. The recieved exit code is {exitCode}");
      }
    }
    catch (Exception e)
    {
      throw new AggregateException($"The executable for the given path {executablePath.FullName} could not be executed.", e);
    }
    finally{
      if (proc != null) {
        proc.Close();
      }
    }

    return exitCode;
  }
}
