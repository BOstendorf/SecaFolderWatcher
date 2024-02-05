using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Shapes;
using ReactiveUI;


namespace SecaFolderWatcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _InfoText = "";
        public string InfoText {
          get { return _InfoText; }
          set { this.RaiseAndSetIfChanged(ref _InfoText, value); }
        }


    public void Click()
    {
      InfoText = InfoText + "test\n";
      Logger.LogInformation("Clicked Button HCHS");
    }
    public GetPatientDataViewModel PatientData { get; } = new GetPatientDataViewModel();

    public MainWindowViewModel()
    {
      Logger.RegisterOnLogCallback(delegate () {
          InfoText = Logger.GetSessionLog();
          });
      SettingsReader.InitSettingsReader();
    }
    

    public void CreateElement()
    {
      new Rectangle();
    }
}
}
