using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace RDUserControl.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<ViewModels.MainWindowViewModel>();
        }
    }
}