using System;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SecaFolderWatcher;
public static class DataValidator
{
    
    public static string GetDHCCFormatDescription()
    {
      return "A DHCC id is expected to match one of the formats DHCCxxxxx or DHCCxxxxxUxx or dhccxxxxx or dhccxxxxxuxx where x is any number between 0 and 9.";
    }

    public static bool CheckDHCC(string id)
    {
      /* 
       * ^DHCC at start of string
       * \d{5} five numbers between 0 and 9
       * ()? optional group of values
       * u[0-9]{1}[1-9]{1} u followed by one number between 0 and 9 followed by one number between 1 and 9
      */
      string pattern = @"^DHCC\d{5}([U,u][0-9]{1}[1-9]{1})?$";
      string pattern_lower_case = @"^dhcc\d{5}(u[0-9]{1}[1-9]{1})?$";
      return Regex.IsMatch(id, pattern) || Regex.IsMatch(id, pattern_lower_case);
    }

    public static string GetSexFormatDescription()
    {
      return "A person's sex is expected to be given as either M or F";
    }

    public static bool CheckSex(string sex)
    {
      return sex.Equals("M") || sex.Equals("F");
    }

    public static string GetDateOfBirthFormatDescription()
    {
      return "A date of birth is expected to be given as ddmmyyyy";
    }

    public static bool CheckDateOfBirth(string dateString)
    {
      DateTime dateTime;
      return DateTime.TryParseExact(dateString, "ddMMyyyy", new CultureInfo("de-De"), DateTimeStyles.None, out dateTime);
    }

    public static async void getParamsFromServer(string id, string mirth_ip, Delegate callback)
    {
      HttpClient client = new HttpClient();
      string url = "http://" + mirth_ip + "/UKE_ImportApp/AutoComplete.php?esz_id=" + id;
      string response = null;
      try
      {
        response = await client.GetStringAsync(url);
        //todo rest implementation
        Logger.LogInformation(response);
      }
      catch (Exception e)
      {
        Logger.LogError(e.Message);
      }
    }
}
