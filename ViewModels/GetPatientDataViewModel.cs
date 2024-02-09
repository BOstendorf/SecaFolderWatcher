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

  private string _dhcc;
  private IBrush _dhccBackgroundColor = Brushes.Beige;
  public IBrush DHCCBackgroundColor
  {
    get => _dhccBackgroundColor;
    set {
      this.RaiseAndSetIfChanged(ref _dhccBackgroundColor, value);
    }
  }

  private IBrush _sexBackgroundColor = Brushes.Beige;
  public IBrush SexBackgroundColor
  {
    get => _sexBackgroundColor;
    set {
      this.RaiseAndSetIfChanged(ref _sexBackgroundColor, value);
    }
  }

  private IBrush _dateOfBirthBackgroundColor = Brushes.Beige;
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
