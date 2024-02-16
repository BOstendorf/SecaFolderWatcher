using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SecaFolderWatcher;
public class SettingsReaderInstance
{
  private string _iniFilePath;
  private Dictionary<string, string> _settings = new Dictionary<string, string>();
  private Dictionary<string, Action<string>> _expectedSettingsMapping = new Dictionary<string, Action<string>>(){
  };

  public SettingsReaderInstance(){
    SetProcessSettingDelegates();
    string executablePath = GetExecutableDirPath();
    Logger.LogInformation($"executable path is ${executablePath}");
    _iniFilePath = GetIniFilePath(executablePath);
    ProcessIniFile();
  }

  public SettingsReaderInstance(string iniFilePath){
    SetProcessSettingDelegates();
    _iniFilePath = iniFilePath;
    Logger.LogInformation($"loading settings file {iniFilePath}");
    ProcessIniFile();
  }

  public string GetSettingValue(string settingName)
  {
    if(!_expectedSettingsMapping.ContainsKey(settingName))
    {
      throw new ArgumentException($"The passed setting name {settingName} does not reference any setting");
    }
    return _settings[settingName];
  }

  public DirectoryInfo GetDirPathOf(string folderSettingName){
    if(!SettingsReader.folderSettingNames.Contains(folderSettingName)){
      throw new ArgumentException($"The passed setting name {folderSettingName} does not reference any setting defining a directory or file path.");
    }
    DirectoryInfo dir = new DirectoryInfo(_settings[folderSettingName]);
    return dir;
  }

  public FileInfo GetFilePathOf(string fileSettingName)
  {
    if(!SettingsReader.fileSettingNames.Contains(fileSettingName)){
      throw new ArgumentException($"The passed setting name {fileSettingName} does not reference any setting defining a directory or file path.");
    }
    FileInfo file = new FileInfo(_settings[fileSettingName]);
    return file;
  }

  private string GetExecutableDirPath(){
    SystemException abort = new SystemException("Could not locate this executables path. Exiting application, because without no .ini file can be located to parse the programs settings");
    Assembly? entryAssembly = Assembly.GetEntryAssembly();
    if(entryAssembly == null) throw abort;
    string? executableDir = Path.GetDirectoryName(entryAssembly.Location);
    if(executableDir == null) throw abort;
    return executableDir;
  }

  private string GetIniFilePath(string executablePath)
  {
    DirectoryInfo previousPath = new DirectoryInfo(executablePath);
    DirectoryInfo? searchPath = previousPath.Parent;
    while (searchPath != null && previousPath != null && !previousPath.Equals(searchPath))
    {
      if (searchPath.Name.Contains("SecaFolderWatcher"))
      {
        FileInfo[] directoryFiles = searchPath.GetFiles();
        foreach (FileInfo file in directoryFiles)
        {
          if (file.Name.Equals(SettingsReader.settingsFile_name))
          {
            return file.FullName;
          }
        }
      }
      previousPath = searchPath;
      searchPath = searchPath.Parent;
    }
    throw new FileNotFoundException($"There is no Path with SecaFolderWatcher/{SettingsReader.settingsFile_name} in the executables parent directories. The executable path is {executablePath}");
  }

  private void SetProcessSettingDelegates()
  {
    _expectedSettingsMapping = new Dictionary<string, Action<string>>(){
    {SettingsReader.settingID_logfile, delegate (string iniValue) { ProcessSettingLOGFILE(iniValue); } },
    {SettingsReader.settingID_logLevel, delegate (string iniValue) { ProcessSettingLOGGING_LEVEL(iniValue); } },
    {SettingsReader.settingID_debugLevel, delegate (string iniValue) { ProcessSettingDEBUG_LEVEL(iniValue); } },
    {SettingsReader.settingID_watchfolder, delegate (string iniValue) { ProcessSettingWATCHFOLDER(iniValue); } },
    {SettingsReader.settingID_transfolder, delegate (string iniValue) { ProcessSettingTRANSFOLDER(iniValue); } },
    {SettingsReader.settingID_destfolder, delegate (string iniValue) { ProcessSettingDESTFOLDER(iniValue); } },
    {SettingsReader.settingID_safefolder, delegate (string iniValue) { ProcessSettingSAFEFOLDER(iniValue); } },
    {SettingsReader.settingID_enableNAKO, delegate (string iniValue) { ProcessSettingENABLE_NAKO(iniValue); } },
    {SettingsReader.settingID_disableNAKO, delegate (string iniValue) { ProcessSettingDISABLE_NAKO(iniValue); } },
    {SettingsReader.settingID_systemID, delegate (string iniValue) { ProcessSettingSYSTEM_ID(iniValue); } },
    {SettingsReader.settingID_mirthIP, delegate (string iniValue) { ProcessSettingMIRTH_IP(iniValue); } },
    {SettingsReader.settingID_autosend, delegate (string iniValue) { ProcessSettingAUTOSEND(iniValue); } },
    };
  }

  private void ProcessSettingLOGFILE(string iniValue)
  {
    _settings[SettingsReader.settingID_logfile] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_logfile} is {iniValue}");
  }

  private void ProcessSettingLOGGING_LEVEL(string iniValue)
  {
    _settings[SettingsReader.settingID_logLevel] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_logLevel} is {iniValue}");
  }

  private void ProcessSettingDEBUG_LEVEL(string iniValue)
  {
    _settings[SettingsReader.settingID_debugLevel] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_debugLevel} is {iniValue}");
  }

  private void ProcessSettingWATCHFOLDER(string iniValue)
  {
    SettingsReader.EnsureDirectoryExists(SettingsReader.settingID_watchfolder, iniValue);
    _settings[SettingsReader.settingID_watchfolder] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_watchfolder} is {iniValue}");
  }

  private void ProcessSettingTRANSFOLDER(string iniValue)
  {
    SettingsReader.EnsureDirectoryExists(SettingsReader.settingID_transfolder, iniValue);
    _settings[SettingsReader.settingID_transfolder] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_transfolder} is {iniValue}");
  }

  private void ProcessSettingDESTFOLDER(string iniValue)
  {
    SettingsReader.EnsureDirectoryExists(SettingsReader.settingID_destfolder, iniValue);
    _settings[SettingsReader.settingID_destfolder] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_destfolder}is {iniValue}");
  }

  private void ProcessSettingSAFEFOLDER(string iniValue)
  {
    SettingsReader.EnsureDirectoryExists(SettingsReader.settingID_safefolder, iniValue);
    _settings[SettingsReader.settingID_safefolder] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_safefolder} is {iniValue}");
  }

  private void ProcessSettingENABLE_NAKO(string iniValue)
  {
    _settings[SettingsReader.settingID_enableNAKO] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_enableNAKO} is {iniValue}");
  }

  private void ProcessSettingDISABLE_NAKO(string iniValue)
  {
    _settings[SettingsReader.settingID_disableNAKO] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_disableNAKO} is {iniValue}");
  }

  private void ProcessSettingSYSTEM_ID(string iniValue)
  {
    _settings[SettingsReader.settingID_systemID] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_systemID} is {iniValue}");
  }

  private void ProcessSettingMIRTH_IP(string iniValue)
  {
    _settings[SettingsReader.settingID_mirthIP] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_mirthIP} is {iniValue}");
  }

  private void ProcessSettingAUTOSEND(string iniValue)
  {
    _settings[SettingsReader.settingID_autosend] = iniValue;
    Logger.LogInformation($"Setting {SettingsReader.settingID_autosend} is {iniValue}");
  }

  private void ProcessIniFile(){
    int settingsSet = 0;
    if(!File.Exists(_iniFilePath)) {
      throw new FileNotFoundException($"The .ini file was expected to be located at {_iniFilePath} but could not be found.");
    }
    string[] fileLines = File.ReadAllLines(_iniFilePath);
    foreach (string line in fileLines){
      if (line.Trim() == "") continue;
      if (!line.Contains("=")) {
        throw new FormatException($"The {SettingsReader.settingsFile_name} file is expected to contain lines of the form SETTING_NAME=SETTING_VALUE the current line is neighter empty nor contains an = indicating a setting mapping \n the given line is {line}");
      }
      string[] lineSplit = line.Trim().Split("=");
      if (!_expectedSettingsMapping.ContainsKey(lineSplit[0])) {
        throw new FormatException($"The SETTING_NAME contained in the current line is not expected. The given SETTING_NAME is {lineSplit[0]}");
      }
      if (lineSplit[1].Equals(""))
      {
        throw new FormatException($"For the Setting {lineSplit[0]} is a value expected, but not given");
      }
      _expectedSettingsMapping[lineSplit[0]](lineSplit[1]);
      settingsSet += 1;
    }
    if (settingsSet < _expectedSettingsMapping.Count)
    {
      string expectedNames = "";
      string setNames = "";
      foreach (string key in _expectedSettingsMapping.Keys){
        expectedNames += $"\t\t {key}\n";
      }
      foreach (string key in _settings.Keys){
        setNames += $"\t\t {key}\n";
      }
      throw new ApplicationException($"The {SettingsReader.settingsFile_name} file is expected to contain the following {_expectedSettingsMapping.Count} settings \n {expectedNames} \n only the following {settingsSet} settings have been set {setNames}");
    }
  }


}
