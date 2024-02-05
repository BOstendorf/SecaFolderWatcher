using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace SecaFolderWatcher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
