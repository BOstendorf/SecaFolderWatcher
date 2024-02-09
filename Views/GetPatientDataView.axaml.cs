

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
          Logger.Log(e.NewDate.ToString());
          viewModel.DateOfBirth = (DateTimeOffset)e.NewDate;
          Logger.Log($"triggered date change event {viewModel.Sex}");
        }

        public void OnDHCCChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
          Logger.Log($"triggered property changed event {e.NewValue}");
        }

        public void OnSexChange(object sender, SelectionChangedEventArgs e){
          Logger.Log($"triggered selecteion change event with selection being {e.ToString()}");
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }
    }
}
