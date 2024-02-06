using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Shapes;
using ReactiveUI;


namespace SecaFolderWatcher.ViewModels
{
    public class DialogWindowViewModel : ViewModelBase
    {
    public GetPatientDataViewModel PatientData { get; } = new GetPatientDataViewModel();
    public DialogWindowViewModel()
    {
    }

}
}
