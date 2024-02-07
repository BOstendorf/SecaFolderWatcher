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

  public string gdtField6305_oldFileRefPtr {
    get;
  } = "";

  public string gdtField6305_newFileRefPtr {
    get;
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
        GDT_MessageLine gdtLine = new GDT_MessageLine(line);
        gdtMessageLines.Add(gdtLine);
        if(String.Compare(gdtLine.typePart, "3000", false) == 0) {
          this.gdtField3100_ID = gdtLine.contentPart;
        }
        if(String.Compare(gdtLine.typePart, "6305", false) == 0) {
          this.gdtField6305_oldFileRefPtr = gdtLine.contentPart;
        }
      }
    }
    catch (Exception e)
    {

    }
  }

  //migrated from previous live version
  //seems kinda pointless, but who am I to jugde
  public void WriteUpdatedFile(string path) {
    foreach (GDT_MessageLine line in gdtMessageLines) {
      if (line.typePart.Equals("8100")) {
        int sentenceLength = Convert.ToInt32(line.contentPart.Length) - gdtField6305_oldFileRefPtr.Length + gdtField6305_newFileRefPtr.Length;
        //line.SetNewContent();
      }
    }
  }
}
