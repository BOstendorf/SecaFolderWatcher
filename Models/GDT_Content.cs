using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SecaFolderWatcher;

public class GDT_Content
{
  public string gdtField3100_ID {
    get;
  } = "";

  public string[] gdtMessage {
    get;
  } = null;

  //old pointer is fullpath
  public string gdtField6305_oldFileRefPtr {
    get;
  } = "";

  //new pointer is file name only
  public string gdtField6305_newFileRefPtr {
    get;
    set;
  } = "";

  public List<GDT_MessageLine> gdtMessageLines {
    get;
  } = new List<GDT_MessageLine>();

  public GDT_Content(string gdtFile_path){
    try
    {
      Logger.LogInformation($"reading GDT file from path: {gdtFile_path}");
      if(!File.Exists(gdtFile_path)) throw new FileNotFoundException($"The file {gdtFile_path} does not seem to exist.");
      gdtMessage = System.IO.File.ReadAllLines(gdtFile_path);
      Logger.LogInformation($"GDT file has been read from {gdtFile_path}");
      foreach (string line in gdtMessage) {
        Logger.LogInformation($"reading line {line}");
        GDT_MessageLine gdtLine = new GDT_MessageLine(line);
        gdtMessageLines.Add(gdtLine);
        if(String.Compare(gdtLine.typePart, "3000", false) == 0) {
          this.gdtField3100_ID = gdtLine.contentPart;
        }
        if(String.Compare(gdtLine.typePart, "6305", false) == 0) {
          this.gdtField6305_oldFileRefPtr = gdtLine.contentPart;
        }
      }
      Logger.LogInformation($"Found ID is {gdtField3100_ID}");
      Logger.LogInformation($"Found referenced file {gdtField6305_oldFileRefPtr}");
    }
    catch (Exception e)
    {
      Logger.LogError(e.Message);
    }
  }

  public void Delete_gdtField6305_oldFileRefPtr(){
    Logger.LogInformation($"Removing file {gdtField6305_oldFileRefPtr}");
    File.Delete(gdtField6305_oldFileRefPtr);
  }

  public void Copy_gdtField6305_oldFileRefPtr(DirectoryInfo targetDir){
      string targetPath = Path.Combine(targetDir.FullName, gdtField6305_newFileRefPtr);
      Logger.LogInformation($"Copying {gdtField6305_oldFileRefPtr} to {targetPath}");
      File.Copy(gdtField6305_oldFileRefPtr, targetPath);
  }

  //migrated from previous live version
  //seems kinda pointless, but who am I to jugde
  public void WriteUpdatedFile(string path) {
    string message = "";
    foreach (GDT_MessageLine line in gdtMessageLines) {
      if (line.typePart.Equals("8100")) {
        int sentenceLength = Convert.ToInt32(line.contentPart.Length) - gdtField6305_oldFileRefPtr.Length + gdtField6305_newFileRefPtr.Length;
        line.SetNewContent(sentenceLength.ToString("D5"));
      }
      if (line.typePart.Equals("6305")) {
        line.SetNewContent(gdtField6305_newFileRefPtr);
      }
      message += line.wholeLine;
    }
    Logger.LogInformation("writing GDT file to {path} with message being {message}");
    try {
      FileStream fileStream = new FileStream(
          path, FileMode.Create, FileAccess.Write);
      StreamWriter fileWriter = new StreamWriter((Stream)fileStream);
      fileWriter.WriteLine(message);
      fileWriter.Close();
      fileStream.Close();
    }
    catch (Exception e)
    {
      Logger.LogErrorVerbose("Could not write gdt file", e.Message);
    }
  }

  public static void SendPatientToWatchfolder(string targetDirPath, string dhcc, string dateOfBirth, string sex){
    SendPatientToWatchfolder(targetDirPath, "secamddt.gdt", dhcc, dateOfBirth, sex);
  }

  /*
   * Function to create a GDT message containing the patient id, the date of birth and the gender.
   * The message should be created as a file in the specified watchfolder
   */
  public static void SendPatientToWatchfolder(string targetDirPath, string targetFileName, string dhcc, string dateOfBirth, string sex)
  {
    string message = GenerateSecaGDTMessage(dhcc, dateOfBirth, sex);
    string targetPath = Path.Combine(targetDirPath, targetFileName);
    StreamWriter outFileWriter = new StreamWriter(targetPath, false);
    outFileWriter.WriteLine(message);
    outFileWriter.Close();
    Logger.LogInformation($"writing GDT file to {targetPath} with message being \n{message}");
  }

  /* Generates gdt message as intendet to be send to Seca device
   * ergo containing the date of birth, sex and the id for the patient
   *
   */
  public static string GenerateSecaGDTMessage(string dhcc, string dateOfBirth, string sex)
  {
    if (!DataValidator.CheckSex(sex)) {
      throw new ArgumentException($"{DataValidator.GetSexFormatDescription()} The actual value is {sex}");
    }
    if (sex.Equals("M")) sex = "1";
    if (sex.Equals("F")) sex = "2";
    if (!DataValidator.CheckDHCC(dhcc)) {
      throw new ArgumentException($"{DataValidator.GetDHCCFormatDescription()} The actual value is {dhcc}");
    }
    if (!DataValidator.CheckDateOfBirth(dateOfBirth)) {
      throw new ArgumentException($"{DataValidator.GetDateOfBirthFormatDescription()} The actual value is {dateOfBirth}");
    }
    /* The format of the sent gdt message is
     * 013 8000 6301 # Satztyp = "Stammdaten übermitteln"
     * 014 8100 <ddddd> # Satzlänge
     * 014 9218 val # Version
     * val 3000 <dhcc> # Namenszusatz
     * val 3101 <dhcc> # Vorname
     * val 3102 <dhcc> # Name
     * 017 3103 <date> # Geburtsdatum
     * 010 3110 <sex>  # Geschlecht
     * the date is given by ddmmyyyy
     * the sex is given as 1 for male and 2 for female (as defined by rule 112)
     */
    string message = "";
    // Satztyp Zeile
    message += new GDT_MessageLine("8000", "6301\r\n").wholeLine;
    // Satzlänge Zeile
    message += new GDT_MessageLine("8100", "LLLLL\r\n").wholeLine;
    // Version Zeile
    message += new GDT_MessageLine("9218", "02.10\r\n").wholeLine;
    message += new GDT_MessageLine("3100", dhcc).wholeLine;
    message += new GDT_MessageLine("3101", dhcc).wholeLine;
    message += new GDT_MessageLine("3102", dhcc).wholeLine;
    message += new GDT_MessageLine("3103", dateOfBirth).wholeLine;
    message += new GDT_MessageLine("3110", sex).wholeLine;
    message = message.Replace("LLLLL", message.Length.ToString("D5"));
    return message;
  }
}
