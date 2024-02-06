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
}
