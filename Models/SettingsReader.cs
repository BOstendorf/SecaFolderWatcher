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
  private static Dictionary<string, string> _settings = new Dictionary<string, string>();
  private static int _settingsSet = 0;

  private static string[] _folderSettingNames = new string[]{"WATCHFOLDER", "TRANSFOLDER", "DESTFOLDER", "SAFEFOLDER"};

  private static Dictionary<string, Action<string>> _expectedSettingsMapping = new Dictionary<string, Action<string>>(){
    {"LOGFILE", delegate (string iniValue) { ProcessSettingLOGFILE(iniValue); } },
    {"LOGGING_LEVEL", delegate (string iniValue) { ProcessSettingLOGGING_LEVEL(iniValue); } },
    {"DEBUG_LEVEL", delegate (string iniValue) { ProcessSettingDEBUG_LEVEL(iniValue); } },
    {"WATCHFOLDER", delegate (string iniValue) { ProcessSettingWATCHFOLDER(iniValue); } },
    {"TRANSFOLDER", delegate (string iniValue) { ProcessSettingTRANSFOLDER(iniValue); } },
    {"DESTFOLDER", delegate (string iniValue) { ProcessSettingDESTFOLDER(iniValue); } },
    {"SAFEFOLDER", delegate (string iniValue) { ProcessSettingSAFEFOLDER(iniValue); } },
    {"ENABLE_NAKO", delegate (string iniValue) { ProcessSettingENABLE_NAKO(iniValue); } },
    {"DISABLE_NAKO", delegate (string iniValue) { ProcessSettingDISABLE_NAKO(iniValue); } },
    {"SYSTEM_ID", delegate (string iniValue) { ProcessSettingSYSTEM_ID(iniValue); } },
    {"MIRTH_IP", delegate (string iniValue) { ProcessSettingMIRTH_IP(iniValue); } },
    {"AUTOSEND", delegate (string iniValue) { ProcessSettingAUTOSEND(iniValue); } },
  };

  private static void ProcessSettingLOGFILE(string iniValue)
  {
    _settings["LOGFILE"] = iniValue;
    Logger.LogInformation($"Setting LOGFILE is {iniValue}");
  }

  private static void ProcessSettingLOGGING_LEVEL(string iniValue)
  {
    _settings["LOGGING_LEVEL"] = iniValue;
    Logger.LogInformation($"Setting LOGGING_LEVEL is {iniValue}");
  }

  private static void ProcessSettingDEBUG_LEVEL(string iniValue)
  {
    _settings["DEBUG_LEVEL"] = iniValue;
    Logger.LogInformation($"Setting DEBUG_LEVEL is {iniValue}");
  }

  private static void ProcessSettingWATCHFOLDER(string iniValue)
  {
    if (!Directory.Exists(iniValue)){
      throw new DirectoryNotFoundException($"The path for the watchfolder directory does not seem to exist. The given path is {iniValue}");
    }
    _settings["WATCHFOLDER"] = iniValue;
    Logger.LogInformation($"Setting WATCHFOLDER is {iniValue}");
  }

  private static void ProcessSettingTRANSFOLDER(string iniValue)
  {
    if (!Directory.Exists(iniValue)){
      throw new DirectoryNotFoundException($"The path for the transfolder directory does not seem to exist. The given path is {iniValue}");
    }
    _settings["TRANSFOLDER"] = iniValue;
    Logger.LogInformation($"Setting TRANSFOLDER is {iniValue}");
  }

  private static void ProcessSettingDESTFOLDER(string iniValue)
  {
    if (!Directory.Exists(iniValue)){
      throw new DirectoryNotFoundException($"The path for the destfolder directory does not seem to exist. The given path is {iniValue}");
    }
    _settings["DESTFOLDER"] = iniValue;
    Logger.LogInformation($"Setting DESTFOLDER is {iniValue}");
  }

  private static void ProcessSettingSAFEFOLDER(string iniValue)
  {
    if (!Directory.Exists(iniValue)){
      throw new DirectoryNotFoundException($"The path for the safefolder directory does not seem to exist. The given path is {iniValue}");
    }
    _settings["SAFEFOLDER"] = iniValue;
    Logger.LogInformation($"Setting SAFEFOLDER is {iniValue}");
  }

  private static void ProcessSettingENABLE_NAKO(string iniValue)
  {
    _settings["ENABLE_NAKO"] = iniValue;
    Logger.LogInformation($"Setting ENABLE_NAKO is {iniValue}");
  }

  private static void ProcessSettingDISABLE_NAKO(string iniValue)
  {
    _settings["DISABLE_NAKO"] = iniValue;
    Logger.LogInformation($"Setting DISABLE_NAKO is {iniValue}");
  }

  private static void ProcessSettingSYSTEM_ID(string iniValue)
  {
    _settings["SYSTEM_ID"] = iniValue;
    Logger.LogInformation($"Setting SYSTEM_ID is {iniValue}");
  }

  private static void ProcessSettingMIRTH_IP(string iniValue)
  {
    _settings["MIRTH_IP"] = iniValue;
    Logger.LogInformation($"Setting MIRTH_IP is {iniValue}");
  }

  private static void EnsureInitialized(){
    if(!_initialized) throw new InvalidOperationException("The SettingsReader is not fully initialized");
  }

  private static void ProcessSettingAUTOSEND(string iniValue)
  {
    _settings["AUTOSEND"] = iniValue;
    Logger.LogInformation($"Setting AUTOSEND is {iniValue}");
  }

  public static void InitSettingsReader() {
    _executableDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    Logger.LogInformation($"executable path is ${_executableDir}");
    _iniFilePath = GetIniFilePath();
    ProcessIniFile();
  }

  private static void ProcessIniFile(){
    string[] fileLines = File.ReadAllLines(_iniFilePath);
    foreach (string line in fileLines){
      if (line.Trim() == "") continue;
      if (!line.Contains("=")) {
        throw new FormatException($"The SecaFolderWatcher.ini file is expected to contain lines of the form SETTING_NAME=SETTING_VALUE the current line is neighter empty nor contains an = indicating a setting mapping \n the given line is {line}");
      }
      string[] lineSplit = line.Trim().Split("=");
      if (!_expectedSettingsMapping.ContainsKey(lineSplit[0])) {
        throw new FormatException($"The SETTING_NAME contained in the current line is not expected. The given SETTING_NAME is {lineSplit[0]}");
      }
      _expectedSettingsMapping[lineSplit[0]](lineSplit[1]);
      _settingsSet += 1;
    }
    if (_settingsSet < _expectedSettingsMapping.Count)
    {
      string expectedNames = "";
      string setNames = "";
      foreach (string key in _expectedSettingsMapping.Keys){
        expectedNames += $"\t\t {key}\n";
      }
      foreach (string key in _settings.Keys){
        setNames += $"\t\t {key}\n";
      }
      throw new ApplicationException($"The SecaFolderWatcher.ini file is expected to contain the following {_expectedSettingsMapping.Count} settings \n {expectedNames} \n only the following {_settingsSet} settings have been set {setNames}");
    }
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
