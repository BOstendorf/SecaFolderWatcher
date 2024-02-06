using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace SecaFolderWatcher;
public static class SettingsReader
{
  private static bool _initialized = false;
  private static string _executableDir = ""; 
  private static string _iniFilePath;
  public static void InitSettingsReader() {
    _executableDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    Logger.LogInformation($"executable path is ${_executableDir}");
    _iniFilePath = GetIniFilePath();
  }

  private static string GetIniFilePath(){
    DirectoryInfo searchPath = Directory.GetParent(_executableDir);
    DirectoryInfo previousPath = new DirectoryInfo(_executableDir);
    while (searchPath != null && previousPath != null && !previousPath.Equals(searchPath))
    {
      if (searchPath.Name.Contains("SecaFolderWatcher"))
      {
        FileInfo[] directoryFiles = searchPath.GetFiles();
        foreach (FileInfo file in directoryFiles)
        {
          if (file.Name.Equals("SecaFolderWatcher.ini"))
          {
            return file.FullName;
          }
        }
      }
      previousPath = searchPath;
      searchPath = Directory.GetParent(searchPath.FullName);
    }
    throw new FileNotFoundException("There is no Path with SecaFolderWatcher/SecaFolderWatcher.ini in the executables parent directories. The executable directory is " + _executableDir);
  }
}
