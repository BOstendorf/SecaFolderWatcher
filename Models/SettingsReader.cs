using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace SecaFolderWatcher;
public static class SettingsReader
{
  private static bool _initialized = false;
  public static void InitSettingsReader() {
    string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    Logger.LogInformation($"executable path is ${directory}");
  }
}
