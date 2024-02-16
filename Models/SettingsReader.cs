using System;
using System.IO;

namespace SecaFolderWatcher;
public static class SettingsReader
{
  public static string settingsFile_name { get; } = "SecaFolderWatcher.ini";
  public static string settingID_watchfolder { get; } = "WATCHFOLDER";
  public static string settingID_safefolder { get; } = "SAFEFOLDER";
  public static string settingID_destfolder { get; } = "DESTFOLDER";
  public static string settingID_transfolder { get; } = "TRANSFOLDER";
  public static string settingID_systemID { get; } = "SYSTEM_ID";
  public static string settingID_disableNAKO { get; } = "DISABLE_NAKO";
  public static string settingID_enableNAKO { get; } = "ENABLE_NAKO";
  public static string settingID_logfile { get; } = "LOGFILE";
  public static string settingID_debugLevel { get; } = "DEBUG_LEVEL";
  public static string settingID_logLevel { get; } = "LOGGING_LEVEL";
  public static string settingID_autosend { get; } = "AUTOSEND";
  public static string settingID_mirthIP { get; } = "MIRTH_IP";

  private static SettingsReaderInstance? _reader = null;

  public static string[] folderSettingNames {get; private set; } = new string[]{
      settingID_destfolder,
      settingID_watchfolder,
      settingID_transfolder,
      settingID_safefolder
  };

  public static string[] fileSettingNames {get; private set;} = new string[]{
    settingID_logfile,
    settingID_disableNAKO,
    settingID_enableNAKO
  };

  public static void EnsureDirectoryExists(string dirSettingName, string path){
    DirectoryInfo folder = new DirectoryInfo(path);
    if (!folder.Exists){
      throw new DirectoryNotFoundException($"The path for the {dirSettingName} directory does not seem to exist. The given path is {path} and the path is resolved to {folder.FullName}");
    }
  }

  private static void EnsureInitialized(SettingsReaderInstance? reader){
    if(reader == null) throw new InvalidOperationException("The SettingsReader is not fully initialized");
  }


  public static DirectoryInfo GetDirPathOf(string folderSettingName){
    EnsureInitialized(_reader);
    return _reader!.GetDirPathOf(folderSettingName);
  }

  public static FileInfo GetFilePathOf(string fileSettingName){
    EnsureInitialized(_reader);
    return _reader!.GetFilePathOf(fileSettingName);
  }

  public static string GetSettingValue(string settingName){
    EnsureInitialized(_reader);
    return _reader!.GetSettingValue(settingName);
  }

  public static void InitSettingsReader() {
    _reader = new SettingsReaderInstance();
  }

  public static void InitSettingsReader(string iniFilePath) {
    _reader = new SettingsReaderInstance(iniFilePath);
  }
}
