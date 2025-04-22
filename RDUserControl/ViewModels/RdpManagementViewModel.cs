using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RDUserControl.Models;
using RDUserControl.Services;
using RDUserControl.Services;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RDUserControl.ViewModels
{
    public partial class RdpManagementViewModel : ObservableObject, IRdpService
    {
        private readonly IRdpService? _rdpService;
        private readonly IUserManagementService? _userManagementService;
        private readonly IEmailService? _emailService;
        [ObservableProperty]
        private ObservableCollection<User> _users = new();

        [ObservableProperty]
        private bool _systemRdpEnabled;

        public ICommand? LoadDataCommand { get; private set; }
        public ICommand? EnableRdpForUserCommand { get; private set; }
        public ICommand? DisableRdpForUserCommand { get; private set; }
        public ICommand? BatchEnableRdpCommand { get; private set; }
        public ICommand? BatchDisableRdpCommand { get; private set; }
        public ICommand? EnableSystemRdpCommand { get; private set; }
        public ICommand? DisableSystemRdpCommand { get; private set; }
        public ICommand? RefreshCommand { get; private set; }

        public RdpManagementViewModel(IRdpService rdpService, IUserManagementService userManagementService, IEmailService emailService)
        {
            _rdpService = rdpService;
            _userManagementService = userManagementService;
            _emailService = emailService;

            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());
            EnableRdpForUserCommand = new RelayCommand<string>(async (username) => await EnableRdpForUserAsync(username!));
            DisableRdpForUserCommand = new RelayCommand<string>(async (username) => await DisableRdpForUserAsync(username));
            BatchEnableRdpCommand = new RelayCommand<List<string>>(async (usernames) => await BatchEnableRdpAsync(usernames));
            BatchDisableRdpCommand = new RelayCommand<List<string>>(async (usernames) => await BatchDisableRdpAsync(usernames));
            EnableSystemRdpCommand = new RelayCommand(async () => await EnableSystemRdpAsync());
            DisableSystemRdpCommand = new RelayCommand(async () => await DisableSystemRdpAsync());
            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
        }

        public async Task LoadDataAsync()
        {
            if (_userManagementService != null && _rdpService != null)
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
        }

        public async Task<bool> EnableRdpForUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username) || _rdpService == null)
                return false;

            if (await _rdpService.EnableRdpForUserAsync(username))
            {
                await LoadDataAsync();
                return true;
            }
            return false;
        }

        public async Task BatchEnableRdpAsync(List<string> usernames)
        {
            bool success = false;
            if (_rdpService != null && _emailService != null)
            {
                success = await _rdpService.BatchEnableRdpAsync(usernames);
                await _emailService.SendBatchNotificationAsync("RDP Enable", usernames, success);
            }
            await LoadDataAsync();
        }

        public async Task<bool> DisableRdpForUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username) || _rdpService == null)
                return false;

            if (await _rdpService.DisableRdpForUserAsync(username))
            {
                await LoadDataAsync();
                return true;
            }
            return false;
        }

        public Task<bool> IsRdpEnabledForUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username) || _rdpService == null)
                return Task.FromResult(false);
            return _rdpService.IsRdpEnabledForUserAsync(username);
        }

        public async Task EnableSystemRdpAsync()
        {
            if (_rdpService != null)
            {
                await _rdpService.EnableSystemRdpAsync();
                await LoadDataAsync();
            }
        }

        public async Task DisableSystemRdpAsync()
        {
            if (_rdpService != null)
            {
                await _rdpService.DisableSystemRdpAsync();
                await LoadDataAsync();
            }
        }

        public async Task BatchDisableRdpAsync(List<string> usernames)
        {
            bool success = false;
            if (_rdpService != null && _emailService != null)
            {
                success = await _rdpService.BatchDisableRdpAsync(usernames);
                await _emailService.SendBatchNotificationAsync("RDP Disable", usernames, success);
            }
            await LoadDataAsync();
        }
    }
}