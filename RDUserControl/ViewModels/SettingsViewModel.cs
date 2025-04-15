using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RDUserControl.Services;
using System.Windows.Input;

namespace RDUserControl.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IEmailService _emailService;
        
        [ObservableProperty]
        private string _smtpServer;
        
        [ObservableProperty]
        private string _smtpUsername;
        
        [ObservableProperty]
        private string _smtpPassword;

        public ICommand? SaveSettingsCommand { get; }

        public SettingsViewModel(IEmailService emailService)
        {
            _emailService = emailService;
            
            SaveSettingsCommand = new RelayCommand(async () => await SaveSettingsAsync());
        }

        private async Task SaveSettingsAsync()
        {
            // Implementation for saving settings
        }
    }
}