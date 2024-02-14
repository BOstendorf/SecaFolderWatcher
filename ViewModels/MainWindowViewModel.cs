using System;
using System.ComponentModel;
using System.IO;
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
        public string InfoText
        {
            get { return _InfoText; }
            set { this.RaiseAndSetIfChanged(ref _InfoText, value); }
        }
        private IBrush _defaultButtonColor = Brushes.LightGray;

        private IBrush _HCHSButtonColor;
        public IBrush HCHSButtonColor {
          get { return _HCHSButtonColor; }
          set { this.RaiseAndSetIfChanged(ref _HCHSButtonColor, value); }
        }

        private IBrush _NAKOButtonColor;
        public IBrush NAKOButtonColor {
          get { return _NAKOButtonColor; }
          set { this.RaiseAndSetIfChanged(ref _NAKOButtonColor, value); }
        }

        public ICommand DialogWindowCommand { get; private set; }
        public Interaction<DialogWindowViewModel, DialogResultViewModel?> ShowDialog { get; private set; }

        private FolderWatcher _folderWatcher { get; set; }

        private void PrepareFolderWatcher()
        {
            DirectoryInfo watchfolder = SettingsReader.GetDirPathOf(SettingsReader.settingID_watchfolder);
            DirectoryInfo destfolder = SettingsReader.GetDirPathOf(SettingsReader.settingID_destfolder);
            DirectoryInfo transfolder = SettingsReader.GetDirPathOf(SettingsReader.settingID_transfolder);
            DirectoryInfo safefolder = SettingsReader.GetDirPathOf(SettingsReader.settingID_safefolder);
            string systemID = SettingsReader.GetSettingValue(SettingsReader.settingID_systemID);
            _folderWatcher = new FolderWatcher(watchfolder, transfolder, true, systemID, destfolder, safefolder);
            _folderWatcher.TranseferFiles();
            _folderWatcher.EnableWatcherEventLoop();
        }

        public void Click()
        {
            InfoText = InfoText + "test\n";
            Logger.LogInformation("Clicked Button HCHS");
        }

        public void ButtonClearClick()
        {
            Logger.EmptySessionLog();
            InfoText = "";
        }

        public GetPatientDataViewModel PatientData { get; } = new GetPatientDataViewModel();

        public MainWindowViewModel()
        {
            Logger.RegisterOnLogCallback(delegate ()
            {
                InfoText = Logger.GetSessionLog();
            });
            HCHSButtonColor = _defaultButtonColor;
            NAKOButtonColor = _defaultButtonColor;
            PrepareSettings();
            PrepareDialogWindow_GetPatientData();
            PrepareFolderWatcher();
        }

        private void PrepareSettings()
        {
            try
            {
                SettingsReader.InitSettingsReader();
                Logger.SetLogPath(SettingsReader.GetFilePathOf("LOGFILE"));
            }
            catch (Exception e)
            {
              Logger.LogErrorVerbose("There has been an error while trying to process the program settings", e.Message);
            }
        }

        private void PrepareDialogWindow_GetPatientData()
        {
            ShowDialog = new Interaction<DialogWindowViewModel, DialogResultViewModel?>();
            DialogWindowCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    Logger.LogInformation("Disable NAKO Mode");
                    Logger.logPrefix = "HCHS";
                    FileInfo disableNAKO_Executable = SettingsReader.GetFilePathOf(SettingsReader.settingID_disableNAKO);
                    if(ProcessRunner.RunExecutableFile(disableNAKO_Executable) != 0) {
                      throw new SystemException("The disable NAKO script didn't execute correctly. Can't continue... ");
                    }
                    HCHSButtonColor = Brushes.PaleGreen;
                    NAKOButtonColor = Brushes.OrangeRed;
                }
                catch (Exception e)
                {
                  Logger.LogErrorVerbose("Some error occured and the program execution cannot continue", e.Message);

                  HCHSButtonColor = Brushes.OrangeRed;
                  NAKOButtonColor = Brushes.OrangeRed;
                  return;
                }
                var dialog = new DialogWindowViewModel();
                var result = await ShowDialog.Handle(dialog);
            });
        }
    }
}
