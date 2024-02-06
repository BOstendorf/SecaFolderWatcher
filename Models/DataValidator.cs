using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SecaFolderWatcher;
public static class DataValidator
{
    public static bool CheckDHCC(string id)
    {
      /* 
       * ^DHCC at start of string
       * \d{5} five numbers between 0 and 9
       * ()? optional group of values
       * u[0-9]{1}[1-9]{1} u followed by one number between 0 and 9 followed by one number between 1 and 9
      */
      string pattern = @"^DHCC\d{5}(U[0-9]{1}[1-9]{1})?$";
      string pattern_lower_case = @"^dhcc\d{5}(u[0-9]{1}[1-9]{1})?$";
      return Regex.IsMatch(id, pattern) || Regex.IsMatch(id, pattern_lower_case);
    }

    public static bool CheckSex(string sex)
    {
      return sex.Equals("M") || sex.Equals("F");
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
