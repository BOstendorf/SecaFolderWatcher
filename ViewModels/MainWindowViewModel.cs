﻿using System;
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
        public string InfoText
        {
            get { return _InfoText; }
            set { this.RaiseAndSetIfChanged(ref _InfoText, value); }
        }

        public ICommand DialogWindowCommand { get; private set; }
        public Interaction<DialogWindowViewModel, DialogResultViewModel?> ShowDialog { get; private set;}


        public void Click()
        {
            InfoText = InfoText + "test\n";
            Logger.LogInformation("Clicked Button HCHS");
        }
        public GetPatientDataViewModel PatientData { get; } = new GetPatientDataViewModel();

        public MainWindowViewModel()
        {
            Logger.RegisterOnLogCallback(delegate ()
            {
                InfoText = Logger.GetSessionLog();
            });
            PrepareSettings();
            PrepareDialogWindow_GetPatientData();
            GDT_Content gdt = new GDT_Content("./testing/mddtseca.gdt");
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
                Logger.LogError($"There has been an error while trying to process the program settings. The provided error message is \n {e.Message}");
            }
        }

        private void PrepareDialogWindow_GetPatientData()
        {
            ShowDialog = new Interaction<DialogWindowViewModel, DialogResultViewModel?>();
            DialogWindowCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var dialog = new DialogWindowViewModel();
                var result = await ShowDialog.Handle(dialog);
            });
        }
    }
}
