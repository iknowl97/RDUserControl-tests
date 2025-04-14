using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RDUserControl.Models;
using RDUserControl.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RDUserControl.ViewModels
{
    public partial class RdpManagementViewModel : ObservableObject
    {
        private readonly IRdpService _rdpService;
        
        [ObservableProperty]
        private ObservableCollection<User> _users = new();

        [ObservableProperty]
        private bool _systemRdpEnabled;

        public ICommand LoadDataCommand { get; }
        public ICommand EnableRdpForUserCommand { get; }
        public ICommand DisableRdpForUserCommand { get; }
        public ICommand BatchEnableRdpCommand { get; }

        public RdpManagementViewModel(IRdpService rdpService)
        {
            _rdpService = rdpService;
            
            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());
            EnableRdpForUserCommand = new RelayCommand<string>(async (username) => await EnableRdpForUserAsync(username));
            DisableRdpForUserCommand = new RelayCommand<string>(async (username) => await DisableRdpForUserAsync(username));
            BatchEnableRdpCommand = new RelayCommand<List<string>>(async (usernames) => await BatchEnableRdpAsync(usernames));
        }

        private async Task LoadDataAsync()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            Users.Clear();
            foreach (var user in users)
            {
                user.IsRdpEnabled = await _rdpService.IsRdpEnabledForUserAsync(user.Username);
                Users.Add(user);
            }
            SystemRdpEnabled = await _rdpService.IsRdpEnabledOnSystemAsync();
        }

        private async Task EnableRdpForUserAsync(string username)
        {
            if (await _rdpService.EnableRdpForUserAsync(username))
            {
                await LoadDataAsync();
            }
        }

        private async Task DisableRdpForUserAsync(string username)
        {
            if (await _rdpService.DisableRdpForUserAsync(username))
            {
                await LoadDataAsync();
            }
        }

        private async Task BatchEnableRdpAsync(List<string> usernames)
        {
            bool success = await _rdpService.BatchEnableRdpAsync(usernames);
            await _emailService.SendBatchNotificationAsync("RDP Enable", usernames, success);
            await LoadDataAsync();
        }
    }
}