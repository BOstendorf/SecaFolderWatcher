using System.ComponentModel;
using System.Runtime.CompilerServices;  
namespace SecaFolderWatcher.ViewModels;

public class GetPatientDataViewModel : ViewModelBase
{
  //public event PropertyChangedEventHandler? PropertyChanged;
  //public ICommand ClickOKCommand { get; }

  public void ClickOK()
  {
    Logger.LogInformation("Clicked Button OK");
  }

  public void ClickCancel()
  {
    Logger.LogInformation("Clicked Button Cancel");
  }

  public GetPatientDataViewModel()
  {

  }
}
