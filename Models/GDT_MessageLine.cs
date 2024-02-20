using System;
using System.Text.RegularExpressions;

namespace SecaFolderWatcher;

public class GDT_MessageLine
{
  public string lengthPart {
    get;
    private set;
  }

  public string typePart {
    get;
    private set;
  }

  public string contentPart {
    get;
    private set;
  }

  public string wholeLine {
    get;
    private set;
  }

  public int lineLength {
    get;
    private set;
  }

  private static void ValidateLengthPartFormat(string lengthPart){
    if (!Regex.IsMatch(lengthPart, @"\d{3}")){
      throw new FormatException($"The length bytes do not match the expected format. Given length bytes are {lengthPart}");
    }
  }

  private static void ValidateTypePartFormat(string typePart) {
    if (!Regex.IsMatch(typePart, @"\d{4}")) {
      throw new FormatException($"The type part does not match the expected format");
    }
  }

  public GDT_MessageLine(string line) {
    if (!line.EndsWith("\r\n")) line = line + "\r\n";
    if (line.Length < 7) {
      throw new FormatException($"The provided line does not match the expected format. Passed argument is {line}");
    }
    lengthPart = line.Substring(0, 3);
    ValidateLengthPartFormat(lengthPart);
    lineLength = int.Parse(lengthPart);
    typePart = line.Substring(3, 4);
    ValidateTypePartFormat(typePart);
    contentPart = line.Substring(7);
    if (line.Length != lineLength) {
      throw new FormatException($"The length of the given line doesn't match the encoded line lenght. The encoded length is {lineLength} while the actual length is {line.Length}");
    }
    wholeLine = line;
  }


  public GDT_MessageLine(string Part_length, string Part_type, string Part_content) : this(Part_length + Part_type + Part_content) {}

  public GDT_MessageLine(string Part_type, string Part_content) {
    ValidateTypePartFormat(Part_type);  
    typePart = Part_type;
    this.SetNewContent(Part_content);
  }

  /*  line consists of a 3 byte prefix representing the length of the line
   *  a 4 byte segment representing the field type id
   *  and the content of the field followed by a \r\n
   */
  public static bool FormatValid(string line)
  {
    if(line.Length < 7) return false;
    string lengthPart = line.Substring(0, 3);
    string typePart = line.Substring(3, 4);
    int parsed;
    try {
      parsed = int.Parse(lengthPart);
    }
    catch 
    {
      return false;
    }
    if(parsed != line.Length) return false;
    if(!Regex.IsMatch(typePart, @"\d{4}")) return false;
    if(!line.EndsWith("\r\n")) return false;
    return true;
  }

  public void SetNewContent(string content)
  {
    if (!content.EndsWith("\r\n")) content = content + "\r\n";
    contentPart = content;
    lineLength = content.Length + 7;
    lengthPart = lineLength.ToString("D3");
    wholeLine = lengthPart + typePart + contentPart;
  }
}
