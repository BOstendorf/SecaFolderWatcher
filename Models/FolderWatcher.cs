
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

  private DirectoryInfo _destfolder {
    get;
    set;
  }

  private DirectoryInfo _safefolder {
    get;
    set;
  }

  public FolderWatcher(DirectoryInfo watchfolder, DirectoryInfo transfolder, bool modeHCHS, string systemID, DirectoryInfo destfolder, DirectoryInfo safefolder) {
    _modeHCHS = modeHCHS;
    _systemID = systemID;
    _transfolder = transfolder;
    _destfolder = destfolder;
    _safefolder = safefolder;
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
      ProcessGDTFile(e.FullPath);
      TranseferFiles();
    }
  }

  private bool FileGeneratedBySeca(string path){
    if (!File.Exists(path)) return false;
    FileInfo fileInfo = new FileInfo(path);
    return (String.Compare(fileInfo.Name, "mddtseca.gdt", true) == 0);
  }

  private bool ProcessGDTFile(string path)
  {
    Logger.LogInformation($"Start processing of file {path}");
    string datetimeString = DateTime.Now.ToString("yyyyMMddhhmmss");
    try
    {
      if (!FileGeneratedBySeca(path)){
        Logger.LogInformation("The current file is not generated by Seca and will therefor be skipped");
        return true;
      }
      Logger.LogInformation("Reading gdt content from file");
      GDT_Content gdtContent = new GDT_Content(path);
      Logger.LogInformation($"GDT content contains the id {gdtContent.gdtField3100_ID} and a reference to the file {gdtContent.gdtField6305_oldFileRefPtr}");

      if ((gdtContent.gdtField3100_ID.Length > 0) && (gdtContent.gdtField6305_oldFileRefPtr.Length > 0)){
      string newGDTFileName = $"{_systemID}_{gdtContent.gdtField3100_ID}_{datetimeString}{Path.GetExtension(path)}";
      gdtContent.gdtField6305_newFileRefPtr = $"{_systemID}_{gdtContent.gdtField3100_ID}_{datetimeString}{Path.GetExtension(gdtContent.gdtField6305_oldFileRefPtr)}";
      
      gdtContent.Copy_gdtField6305_oldFileRefPtr(_transfolder);
      
      string targetPath = Path.Combine(_transfolder.FullName, newGDTFileName);
      Logger.LogInformation($"Writing new GDT Message {targetPath}");
      gdtContent.WriteUpdatedFile(targetPath);

      gdtContent.Delete_gdtField6305_oldFileRefPtr();
    }
      Logger.LogInformation($"Processing done, deleting {path}");
      File.Delete(path);

      //race condition mitigation
      System.Threading.Thread.Sleep(1000);
      ProcessRelatedCSVFiles(path);

      Logger.LogInformation($"Finished processing of .gdt message and related files");
      return true;
    }
    catch (Exception e)
    {
      Logger.LogError($"The .gdt message or one of the related files could not be processed the thrown exceptions message is {e.Message}");
      return false;
    }
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
    System.Threading.Thread.Sleep(1000);
    try
    {
      Logger.LogInformation("Starting transfer of files");
      string[] pdfsToTransfer = Directory.GetFiles(_transfolder.FullName,  "*.pdf");
      string[] toTransfer = Directory.GetFiles(_transfolder.FullName);

      //first transfer .pdf files
      foreach (string pdfPath in pdfsToTransfer){
        if (!TransferSingleFile(pdfPath)) return;
      }

      //transfer rest
      foreach (string path in toTransfer){
        if(String.Compare(Path.GetExtension(path), ".pdf", true) == 0) continue;
        if (!TransferSingleFile(path)) return;
      }
    }
    catch (Exception e)
    {
      Logger.LogError($"An error occured during transfer of files. The thrown Exceptions message is {e.Message}");
    }
  }
}
