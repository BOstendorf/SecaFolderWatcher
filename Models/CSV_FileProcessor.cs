

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicData;

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
  // Comment as taken from previous visual basic version
  //  '	The CSV file is made of 2 lines, fields separated by ";"
  //  '	line[0]:  'name1';'id'   ;.....;'nameX';'nameY';'nameZ'
  //  '	line[1]:  'val_1';'idVal';.....;'val_X';'val_Y';'val_Z'
  //  '	CAVE: escaping of ";" within items is not foreseen
  private static string GetID_FromFile(FileInfo csvFile) {
    string[] fileLines = File.ReadAllLines(csvFile.FullName);
    if (fileLines.Length < 2) 
    {
      throw new FormatException("The given csvFile doesn't match the expected format. The file is expected to contain at least two lines");
    }
    string[] fieldNames = fileLines[0].Split(";");
    string[] fieldValues = fileLines[1].Split(";");
    if(!fieldNames.Contains("id")){
      throw new FormatException("The given .csv file does not contain a field with name id");
    }
    int idIndex = fieldNames.IndexOf("id");
    return fieldValues[idIndex];
  }

  public static bool ProcessCSVFile(FileInfo csvFile) {
    

    return true;
  }
}
