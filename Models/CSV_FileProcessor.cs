

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
        if (!file.Extension.Equals(".csv"))
        {
            throw new ArgumentException($"The given path is expected to point to a .csv file. The actual path is {file.FullName}");
        }
        if (!File.Exists(file.FullName))
        {
            throw new FileNotFoundException($"The csv file at {file.FullName} could not be found");
        }
    }

    public static string AssignUniqueID()
    {
        string id = $"U_{_uniqueCounter}";
        _uniqueCounter += 1;
        return id;
    }

    // Comment as taken from previous visual basic version
    //  '	The CSV file is made of 2 lines, fields separated by ";"
    //  '	line[0]:  'name1';'id'   ;.....;'nameX';'nameY';'nameZ'
    //  '	line[1]:  'val_1';'idVal';.....;'val_X';'val_Y';'val_Z'
    //  '	CAVE: escaping of ";" within items is not foreseen
    private static string GetID_FromFile(FileInfo csvFile)
    {
        string[] fileLines = File.ReadAllLines(csvFile.FullName);
        if (fileLines.Length < 2)
        {
            throw new FormatException("The given csvFile doesn't match the expected format. The file is expected to contain at least two lines");
        }
        string[] fieldNames = fileLines[0].Split(";");
        string[] fieldValues = fileLines[1].Split(";");
        if (!fieldNames.Contains("id"))
        {
            throw new FormatException("The given .csv file does not contain a field with name id");
        }
        int idIndex = fieldNames.IndexOf("id");
        return fieldValues[idIndex];
    }

    public static bool ProcessCSVFile(FileInfo csvFile, DirectoryInfo transfolder){
      try
      {
        return ProcessCSVFile(csvFile, "", transfolder);
      }
      catch (Exception e)
      {
        Logger.LogErrorVerbose("Tried to process csv file without specification of system id and failed while trying", e.Message);
        return false;
      }
    }

    public static bool ProcessCSVFile(FileInfo csvFile, string system_id, DirectoryInfo transfolder)
    {
        string timestampString = DateTime.Now.ToString("yyyyMMddhhmmss");
        string id;
        try
        {
            id = GetID_FromFile(csvFile);
        }
        catch (Exception e)
        {
          Logger.LogErrorVerbose($"No id could be obtained from the file at {csvFile.FullName}", e.Message);
            Logger.LogWarning("A unique id will be generated as a substitute.");
            id = AssignUniqueID();
        }
        try
        {
          string newFileName = $"{system_id}_{id}_{timestampString}{csvFile.Extension}";
          if (!Directory.Exists(transfolder.FullName)){
            Logger.LogInformation($"The specified Transfolder directory does not exist already and will be created at {transfolder.FullName}");
            Directory.CreateDirectory(transfolder.FullName);
          }
          Logger.LogInformation($"Move {csvFile.FullName} to {transfolder.FullName}{newFileName}");
          csvFile.MoveTo(Path.Combine(transfolder.FullName, newFileName));
        }
        catch (Exception e)
        {
          Logger.LogErrorVerbose("There was some error while trying to process the given .csv file. The process will be aborted", e.Message);
          return false;
        }
        return true;
    }
}
