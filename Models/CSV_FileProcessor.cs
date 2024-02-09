

using System;
using System.Collections.Generic;
using System.IO;

namespace SecaFolderWatcher;

public class CSV_FileProcessor
{
  private static int _uniqueCounter = 0;

  public static void EnsureCSV(FileInfo file)
  {
    if(!file.Extension.Equals(".csv"))
    {
      throw new ArgumentException($"The given path is expected to point to a .csv file. The actual path is {file.FullName}");
    }
    if(!File.Exists(file.FullName))
    {
      throw new FileNotFoundException($"The csv file at {file.FullName} could not be found");
    }
  }

  private static string AssignUniqueID(){
    string id = $"U_{_uniqueCounter}";
    _uniqueCounter += 1;
    return id;
  }

  public static bool ProcessCSVFile(FileInfo csvFile) {
    

    return true;
  }
}
