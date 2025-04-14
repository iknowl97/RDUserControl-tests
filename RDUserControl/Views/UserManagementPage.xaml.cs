using System.Windows.Controls;

namespace RDUserControl.Views
{
    public partial class UserManagementPage : UserControl
    {
        public UserManagementPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<ViewModels.UserManagementViewModel>();
        }
    }
}