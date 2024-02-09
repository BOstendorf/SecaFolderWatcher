using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Media;
using ReactiveUI;
namespace SecaFolderWatcher.ViewModels;

public class GetPatientDataViewModel : ViewModelBase
{
  //public event PropertyChangedEventHandler? PropertyChanged;
  private static IBrush COLOR_VALID = Brushes.Beige;
  private static IBrush COLOR_INVALID = Brushes.PaleVioletRed;

  private string _dhcc = "";
  private IBrush _dhccBackgroundColor = COLOR_VALID;
  public IBrush DHCCBackgroundColor
  {
    get => _dhccBackgroundColor;
    set {
      this.RaiseAndSetIfChanged(ref _dhccBackgroundColor, value);
    }
  }

  private IBrush _sexBackgroundColor = COLOR_VALID;
  public IBrush SexBackgroundColor
  {
    get => _sexBackgroundColor;
    set {
      this.RaiseAndSetIfChanged(ref _sexBackgroundColor, value);
    }
  }

  private IBrush _dateOfBirthBackgroundColor = COLOR_VALID;
 public IBrush DateOfBirthBackgroundColor
 {
   get => _dateOfBirthBackgroundColor;
   set {
     this.RaiseAndSetIfChanged(ref _dateOfBirthBackgroundColor, value);
   }
 }

  public string DHCC
  {
    get => _dhcc;
    set {
      this.RaiseAndSetIfChanged(ref _dhcc, value);
    }
  }

  private DateTimeOffset _dateOfBirth = new DateTimeOffset();
  public DateTimeOffset DateOfBirth 
  {
    get => _dateOfBirth;
    set {
      this.RaiseAndSetIfChanged(ref _dateOfBirth, value);
    }
  }

  private string _sex = "";
  public string Sex
  {
    get => _sex;
    set {
      Logger.Log(value);
      this.RaiseAndSetIfChanged(ref _sex, value);
    }
  }

  public void ClickOK()
  {
    bool inputsValid = true;
    if (!DataValidator.CheckDHCC(_dhcc)) {
      inputsValid = false;
      DHCCBackgroundColor = COLOR_INVALID;
    }
    if (!DataValidator.CheckDateOfBirth(_dateOfBirth.ToString("ddMMyyyy"))){
      inputsValid = false;
      DateOfBirthBackgroundColor = COLOR_INVALID;
    }
    if (!DataValidator.CheckSex(_sex)){
      inputsValid = false;
      SexBackgroundColor = COLOR_INVALID;
    }
    Logger.LogInformation("Clicked Button OK");
    Logger.LogInformation("currently selected sex is " + _sex);
    Logger.LogInformation($"Currently selected date of birth is {_dateOfBirth}");
    Logger.LogInformation($"currently selected dhcc {_dhcc}");
  }

  public void ClickCancel()
  {
    Logger.LogInformation("Clicked Button Cancel");
  }

  public GetPatientDataViewModel()
  {

  }
}
