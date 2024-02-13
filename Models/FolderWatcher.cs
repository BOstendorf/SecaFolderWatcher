
using System;
using System.IO;

namespace SecaFolderWatcher;

public class FolderWatcher 
{

  public FileSystemWatcher watcher {
    get;
    private set;
  }

  private bool _modeHCHS {
    get;
    set;
  }

  private string _systemID {
    get;
    set;
  }

  private DirectoryInfo _transfolder {
    get;
    set;
  }

  public FolderWatcher(DirectoryInfo watchfolder, DirectoryInfo transfolder, bool modeHCHS, string systemID) {
    _modeHCHS = modeHCHS;
    _systemID = systemID;
    _transfolder = transfolder;
    watcher = new FileSystemWatcher();
    watcher.Path = watchfolder.FullName;
    watcher.Filter = "*.*";
    watcher.IncludeSubdirectories = false;

    watcher.NotifyFilter = NotifyFilters.Attributes |
      NotifyFilters.LastAccess |
      NotifyFilters.LastWrite |
      NotifyFilters.Security |
      NotifyFilters.Size |
      NotifyFilters.FileName |
      NotifyFilters.DirectoryName;

    watcher.Created += OnCreated;
  }

  private void OnCreated(object sender, FileSystemEventArgs e)
  {
    Logger.LogInformation($"New file: {e.FullPath}");
    if (_modeHCHS){
      ProcessFile(e.FullPath);
      TranseferFiles();
    }
  }

  private bool FileGeneratedBySeca(string path){
    if (!File.Exists(path)) return false;
    FileInfo fileInfo = new FileInfo(path);
    return (String.Compare(fileInfo.Name, "mddtseca.gdt", true) == 0);
  }

  private bool ProcessFile(string path)
  {
    Logger.LogInformation($"Start processing of file {path}");
    string datetimeString = DateTime.Now.ToString("yyyyMMddhhmmss");
    throw new NotImplementedException();
  }

  private void ProcessRelatedCSVFiles(string path){
    string directory = Path.GetDirectoryName(path);
    foreach (string csvPath in Directory.GetFiles(directory, "*.csv")) {
      FileInfo csvFile = new FileInfo(csvPath);
      CSV_FileProcessor.ProcessCSVFile(csvFile, _systemID, _transfolder);
    }
  }

  private void TranseferFiles()
  {
    throw new NotImplementedException();
  }
}
