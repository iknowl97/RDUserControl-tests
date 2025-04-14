using System.Windows.Controls;

namespace RDUserControl.Views
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<ViewModels.DashboardViewModel>();
        }
    }
}