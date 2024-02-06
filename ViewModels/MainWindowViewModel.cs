using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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

        public ICommand DialogWindowCommand {get;}
        public Interaction<DialogWindowViewModel, DialogResultViewModel?> ShowDialog { get; }


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
      ShowDialog = new Interaction<DialogWindowViewModel,DialogResultViewModel?>();
      DialogWindowCommand = ReactiveCommand.CreateFromTask(async () => {
          var dialog = new DialogWindowViewModel();
          var result = await ShowDialog.Handle(dialog);
          });
    }
}
}
