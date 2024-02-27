using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SecaFolderWatcher;

public class GDT_Content
{
  public static Dictionary<string, string> gdtTypes = new Dictionary<string, string> {
    {"3000", "Patient-ID"},
    {"3100", "Namenszusatz"},
    {"3101", "Vorname"},
    {"3102", "Name"},
    {"3103", "Geburtsdatum"},
    {"3110", "Geschlecht"},
    {"6305", "Dateireferenz"},
  };

  public static string? getGDTTypeID(string typeDescription) {
    foreach (var pair in gdtTypes)
    {
      if(pair.Value.Equals(typeDescription))
      {
        return pair.Key;
      }
    }
    throw new ArgumentException($"there is no type id for the given type description {typeDescription}");
  }

  public static string? getGDTTypeDescription(string typeID) {
    return gdtTypes.GetValueOrDefault(typeID);
  }
  
  public string gdtField3000_ID {
    get;
    private set;
  } = "";

  public string[] gdtMessage {
    get;
  } = null;

  //old pointer is fullpath
  public string gdtField6305_oldFileRefPtr {
    get;
    private set;
  } = "";

  //new pointer is file name only
  private string gdtField6305_newFileRefPtr {
    get;
    set;
  } = "";

  public List<GDT_MessageLine> gdtMessageLines {
    get;
  } = new List<GDT_MessageLine>();

  public Dictionary<string, string> contentsByType {
    get;
  } = new Dictionary<string, string>();

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
        ProcessGDTTypes(gdtLine);
      }
      DeterminePatientIDPresent();
      Logger.LogInformation($"Found ID is {gdtField3000_ID}");
      Logger.LogInformation($"Found referenced file {gdtField6305_oldFileRefPtr}");
    }
    catch (Exception e)
    {
      Logger.LogError(e.Message);
    }
  }

  private void ProcessGDTTypes(GDT_MessageLine line)
  {
    string? id = getGDTTypeID("Patient-ID");
    if(line.typePart.Equals(id)) {
      if(!DataValidator.CheckDHCC(line.contentPart)) return;
      this.gdtField3000_ID = line.contentPart;
    }
    string? fileRef = getGDTTypeID("Dateireferenz");
    if(line.typePart.Equals(fileRef)) {
      this.gdtField6305_oldFileRefPtr = line.contentPart;
    }
    this.contentsByType.Add(line.typePart, line.contentPart);
  }

  private void DeterminePatientIDPresent() {
    string typeID = getGDTTypeID("Patient-ID");
    if (this.contentsByType.ContainsKey(typeID)) return;
    if (this.contentsByType.ContainsKey(getGDTTypeID("Vorname"))) {
      this.contentsByType.Add(typeID, this.contentsByType["Vorname"]);
      this.gdtField3000_ID = this.contentsByType["Vorname"];
      return;
    }
    if (this.contentsByType.ContainsKey(getGDTTypeID("Name"))) {
      this.contentsByType.Add(typeID, this.contentsByType["Name"]);
      this.gdtField3000_ID = this.contentsByType["Name"];
      return;
    }
    throw new ApplicationException("For the current .gdt message no Patient-ID could be determined");
  }

  public void Delete_gdtField6305_oldFileRefPtr(){
    if (!File.Exists(gdtField6305_oldFileRefPtr)) {
      Logger.LogError($"The referenced file {gdtField6305_oldFileRefPtr} does not exist. Can't delete non existing file");
      return;
    }
    if (gdtField6305_oldFileRefPtr.Equals("")) {
      Logger.LogWarning($"There is no file reference contained in the current gdt message");
      return;
    }
    Logger.LogInformation($"Removing file {gdtField6305_oldFileRefPtr}");
    File.Delete(gdtField6305_oldFileRefPtr);
  }

  public void Copy_gdtField6305_oldFileRefPtr(DirectoryInfo targetDir, string fileName){
    if (!File.Exists(gdtField6305_oldFileRefPtr)) {
      Logger.LogError($"The referenced file {gdtField6305_oldFileRefPtr} does not exist. Can't copy file to other location");
      return;
    }
    if (gdtField6305_oldFileRefPtr.Equals("")) {
      Logger.LogWarning($"There is no file reference contained in the current gdt message");
      return;
    }
      string targetPath = Path.Combine(targetDir.FullName, fileName);
      Logger.LogInformation($"Copying {gdtField6305_oldFileRefPtr} to {targetPath}");
      File.Copy(gdtField6305_oldFileRefPtr, targetPath);
  }

  public static void OnSentecelengthLine_Update(GDT_MessageLine line, int replacedContentLength, int newContentLength){
    if (line.typePart.Equals("8100")){
      int sentenceLength = Convert.ToInt32(line.contentPart) - replacedContentLength + newContentLength;
      line.SetNewContent(sentenceLength.ToString("D5"));
    }
  }

  public void WriteUpdatedFile(string path, string gdtField6305_value) {
    string message = "";
    foreach (GDT_MessageLine line in gdtMessageLines) {
      OnSentecelengthLine_Update(line, gdtField6305_oldFileRefPtr.Length, gdtField6305_value.Length);
      if (line.typePart.Equals("6305")) {
        line.SetNewContent(gdtField6305_value);
      }
      message += line.wholeLine;
    }
    Logger.LogInformation($"writing GDT file to {path} with message being {message}");
    try {
      FileStream fileStream = new FileStream(
          path, FileMode.Create, FileAccess.Write);
      StreamWriter fileWriter = new StreamWriter((Stream)fileStream);
      fileWriter.Write(message);
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
    Logger.LogInformation($"writing GDT file to {targetPath} with message being \n{message}");
    File.WriteAllText(targetPath, message);
    /*StreamWriter outFileWriter = new StreamWriter(targetPath, false);
     *outFileWriter.Write(message);
     *outFileWriter.Close();
    */
    Logger.LogInformation($"finished writing GDT file");
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
     * please note that the spaces are only for readability within this comment.
     * in the actual message the whitespaces have to be omitted!
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
    message += new GDT_MessageLine("3000", dhcc).wholeLine;
    message += new GDT_MessageLine("3101", dhcc).wholeLine;
    message += new GDT_MessageLine("3102", dhcc).wholeLine;
    message += new GDT_MessageLine("3103", dateOfBirth).wholeLine;
    message += new GDT_MessageLine("3110", sex).wholeLine;
    message = message.Replace("LLLLL", message.Length.ToString("D5"));
    return message;
  }
}
