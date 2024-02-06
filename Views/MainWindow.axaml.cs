using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using Avalonia.ReactiveUI;
using SecaFolderWatcher.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace SecaFolderWatcher.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(action => action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
    }

    private async Task DoShowDialogAsync(InteractionContext<DialogWindowViewModel, DialogResultViewModel?> interaction)
    {
      var dialog = new DialogWindow();
      dialog.DataContext = interaction.Input;

      var result = await dialog.ShowDialog<DialogResultViewModel?>(this);
      interaction.SetOutput(result);
    }

    public void ButtonClick_HCHS(object source, RoutedEventArgs args)
    {
      Debug.WriteLine("Click");
    }

    public void ButtonClick_NAKO(object source, RoutedEventArgs args)
    {
      Debug.WriteLine("Click");
    }
    public void ButtonClick_Clear(object source, RoutedEventArgs args)
    {
      Debug.WriteLine("Click");
    }
}
