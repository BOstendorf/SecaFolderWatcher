

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;
using SecaFolderWatcher.ViewModels;
using System.Linq;

namespace SecaFolderWatcher.Views
{
    public partial class GetPatientDataView : UserControl
    {
        public GetPatientDataView()
        {
           InitializeComponent();
        }


        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }
    }
}
