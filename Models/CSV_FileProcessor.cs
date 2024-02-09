

using System.IO;

namespace SecaFolderWatcher;

public class CSV_FileProcessor
{
  private static int _uniqueCounter = 0;

  private static string AssignUniqueID(){
    string id = $"U_{_uniqueCounter}";
    _uniqueCounter += 1;
    return id;
  }

  public static bool ProcessCSVFile(FileInfo csvFile) {
    

    return true;
  }
}
