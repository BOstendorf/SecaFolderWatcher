

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;
using SecaFolderWatcher.ViewModels;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia;

namespace SecaFolderWatcher.Views
{
    public partial class GetPatientDataView : UserControl
    {
      private DatePicker _datePicker;
        public GetPatientDataView()
        {
           InitializeComponent();
           DataContext = new GetPatientDataViewModel();
           _datePicker = this.FindControl<DatePicker>("datepicker")!;
           Logger.Log(_datePicker.ToString());
        }

        public void OnDateChange(object sender, DatePickerSelectedValueChangedEventArgs e)
        {
          GetPatientDataViewModel viewModel = (GetPatientDataViewModel)DataContext;
          viewModel.HandleChangeDateOfBirth((DateTimeOffset)e.NewDate);
        }

        public void OnDHCCChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
          GetPatientDataViewModel viewModel = (GetPatientDataViewModel)DataContext;
          viewModel.HandleChangeDHCC();
        }

        public void OnSexChange(object sender, SelectionChangedEventArgs e){
          GetPatientDataViewModel viewModel = (GetPatientDataViewModel)DataContext;
          viewModel.HandleChangeSex();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }
    }
}
