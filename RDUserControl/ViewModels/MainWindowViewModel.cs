using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RDUserControl.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;

namespace RDUserControl.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IRdpService _rdpService;
        private readonly IEmailService _emailService;
        
        private object? _currentPage;
        private string _applicationTitle = "RDP User Control";

        public object? CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public string ApplicationTitle
        {
            get => _applicationTitle;
            set => SetProperty(ref _applicationTitle, value);
        }

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToUserManagementCommand { get; }
        public ICommand NavigateToRdpManagementCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }

        public DashboardViewModel DashboardViewModel { get; }
        public UserManagementViewModel UserManagementViewModel { get; }
        public RdpManagementViewModel RdpManagementViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }

        private string _smtpServer = string.Empty;
        private string _smtpUsername = string.Empty;
        private string _smtpPassword = string.Empty;

        public MainWindowViewModel(
            IUserManagementService userManagementService,
            IRdpService rdpService,
            IEmailService emailService,
            DashboardViewModel dashboardViewModel,
            UserManagementViewModel userManagementViewModel,
            RdpManagementViewModel rdpManagementViewModel,
            SettingsViewModel settingsViewModel)
        {
            _userManagementService = userManagementService;
            _rdpService = rdpService;
            _emailService = emailService;

            DashboardViewModel = dashboardViewModel;
            UserManagementViewModel = userManagementViewModel;
            RdpManagementViewModel = rdpManagementViewModel;
            SettingsViewModel = settingsViewModel;

            NavigateToDashboardCommand = new RelayCommand(() => CurrentPage = DashboardViewModel);
            NavigateToUserManagementCommand = new RelayCommand(() => CurrentPage = UserManagementViewModel);
            NavigateToRdpManagementCommand = new RelayCommand(() => CurrentPage = RdpManagementViewModel);
            NavigateToSettingsCommand = new RelayCommand(() => CurrentPage = SettingsViewModel);

            // Set default page
            CurrentPage = DashboardViewModel;
        }
    }

    public class DashboardViewModel : ObservableObject
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IRdpService _rdpService;
        
        private int _totalUsers;
        private int _enabledUsers;
        private int _rdpEnabledUsers;
        private bool _systemRdpEnabled;

        public int TotalUsers
        {
            get => _totalUsers;
            set => SetProperty(ref _totalUsers, value);
        }

        public int EnabledUsers
        {
            get => _enabledUsers;
            set => SetProperty(ref _enabledUsers, value);
        }

        public int RdpEnabledUsers
        {
            get => _rdpEnabledUsers;
            set => SetProperty(ref _rdpEnabledUsers, value);
        }

        public bool SystemRdpEnabled
        {
            get => _systemRdpEnabled;
            set => SetProperty(ref _systemRdpEnabled, value);
        }

        public ICommand RefreshCommand { get; }

        public DashboardViewModel(IUserManagementService userManagementService, IRdpService rdpService)
        {
            _userManagementService = userManagementService;
            _rdpService = rdpService;

            RefreshCommand = new AsyncRelayCommand(LoadDashboardDataAsync);

            // Load data initially
            _ = LoadDashboardDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDashboardDataAsync()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            TotalUsers = users.Count;
            EnabledUsers = users.Count(u => u.IsEnabled);
            RdpEnabledUsers = users.Count(u => u.IsRdpEnabled);
            SystemRdpEnabled = await _rdpService.IsRdpEnabledOnSystemAsync();
        }
    }

    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IEmailService _emailService;
        
        private ObservableCollection<Models.User> _users = new ObservableCollection<Models.User>();
        private Models.User? _selectedUser;

        public ObservableCollection<Models.User> Users => _users;
        public Models.User? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand EnableUserCommand { get; }
        public ICommand DisableUserCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand BatchEnableCommand { get; }
        public ICommand BatchDisableCommand { get; }

        public UserManagementViewModel(IUserManagementService userManagementService, IEmailService emailService)
        {
            _userManagementService = userManagementService;
            _emailService = emailService;

            Users = new ObservableCollection<Models.User>();

            RefreshCommand = new AsyncRelayCommand(LoadUsersAsync);
            EnableUserCommand = new AsyncRelayCommand<string>(EnableUserAsync);
            DisableUserCommand = new AsyncRelayCommand<string>(DisableUserAsync);
            ResetPasswordCommand = new AsyncRelayCommand<string>(ResetPasswordAsync);
            BatchEnableCommand = new AsyncRelayCommand<List<string>>(BatchEnableUsersAsync);
            BatchDisableCommand = new AsyncRelayCommand<List<string>>(BatchDisableUsersAsync);

            // Load data initially
            _ = LoadUsersAsync();
        }

        private async System.Threading.Tasks.Task LoadUsersAsync()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private async System.Threading.Tasks.Task EnableUserAsync(string username)
        {
            if (await _userManagementService.EnableUserAsync(username))
            {
                await LoadUsersAsync();
            }
        }

        private async System.Threading.Tasks.Task DisableUserAsync(string username)
        {
            if (await _userManagementService.DisableUserAsync(username))
            {
                await LoadUsersAsync();
            }
        }

        private async System.Threading.Tasks.Task ResetPasswordAsync(string username)
        {
            // Generate a random password
            string newPassword = Guid.NewGuid().ToString().Substring(0, 8);
            
            if (await _userManagementService.ResetPasswordAsync(username, newPassword))
            {
                await _emailService.SendPasswordResetEmailAsync(username, newPassword);
            }
        }

        private async System.Threading.Tasks.Task BatchEnableUsersAsync(List<string> usernames)
        {
            bool success = await _userManagementService.BatchEnableUsersAsync(usernames);
            await _emailService.SendBatchNotificationAsync("Enable", usernames, success);
            await LoadUsersAsync();
        }

        private async System.Threading.Tasks.Task BatchDisableUsersAsync(List<string> usernames)
        {
            bool success = await _userManagementService.BatchDisableUsersAsync(usernames);
            await _emailService.SendBatchNotificationAsync("Disable", usernames, success);
            await LoadUsersAsync();
        }
    }

    public class RdpManagementViewModel : ObservableObject
    {
        private readonly IRdpService _rdpService;
        private readonly IUserManagementService _userManagementService;
        private readonly IEmailService _emailService;
        
        private ObservableCollection<Models.User> _users = new ObservableCollection<Models.User>();
        private Models.User? _selectedUser;
        private bool _systemRdpEnabled;

        public ObservableCollection<Models.User> Users => _users;
        public Models.User? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public bool SystemRdpEnabled
        {
            get => _systemRdpEnabled;
            set => SetProperty(ref _systemRdpEnabled, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand EnableRdpForUserCommand { get; }
        public ICommand DisableRdpForUserCommand { get; }
        public ICommand BatchEnableRdpCommand { get; }
        public ICommand BatchDisableRdpCommand { get; }
        public ICommand EnableSystemRdpCommand { get; }
        public ICommand DisableSystemRdpCommand { get; }

        public RdpManagementViewModel(
            IRdpService rdpService, 
            IUserManagementService userManagementService,
            IEmailService emailService)
        {
            _rdpService = rdpService;
            _userManagementService = userManagementService;
            _emailService = emailService;

            Users = new ObservableCollection<Models.User>();

            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
            EnableRdpForUserCommand = new AsyncRelayCommand<string>(EnableRdpForUserAsync);
            DisableRdpForUserCommand = new AsyncRelayCommand<string>(DisableRdpForUserAsync);
            BatchEnableRdpCommand = new AsyncRelayCommand<List<string>>(BatchEnableRdpAsync);
            BatchDisableRdpCommand = new AsyncRelayCommand<List<string>>(BatchDisableRdpAsync);
            EnableSystemRdpCommand = new AsyncRelayCommand(EnableSystemRdpAsync);
            DisableSystemRdpCommand = new AsyncRelayCommand(DisableSystemRdpAsync);

            // Load data initially
            _ = LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
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

        private async System.Threading.Tasks.Task EnableRdpForUserAsync(string username)
        {
            if (await _rdpService.EnableRdpForUserAsync(username))
            {
                await LoadDataAsync();
            }
        }

        private async System.Threading.Tasks.Task DisableRdpForUserAsync(string username)
        {
            if (await _rdpService.DisableRdpForUserAsync(username))
            {
                await LoadDataAsync();
            }
        }

        private async System.Threading.Tasks.Task BatchEnableRdpAsync(List<string> usernames)
        {
            bool success = await _rdpService.BatchEnableRdpAsync(usernames);
            await _emailService.SendBatchNotificationAsync("RDP Enable", usernames, success);
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task BatchDisableRdpAsync(List<string> usernames)
        {
            bool success = await _rdpService.BatchDisableRdpAsync(usernames);
            await _emailService.SendBatchNotificationAsync("RDP Disable", usernames, success);
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task EnableSystemRdpAsync()
        {
            if (await _rdpService.EnableRdpOnSystemAsync())
            {
                SystemRdpEnabled = true;
            }
        }

        private async System.Threading.Tasks.Task DisableSystemRdpAsync()
        {
            if (await _rdpService.DisableRdpOnSystemAsync())
            {
                SystemRdpEnabled = false;
            }
        }
    }

    public class SettingsViewModel : ObservableObject
    {
        private string _smtpServer;
        private int _smtpPort = 25;
        private string _smtpUsername;
        private string _smtpPassword;
        private bool _enableEmailNotifications;

        public string SmtpServer
        {
            get => _smtpServer;
            set => SetProperty(ref _smtpServer, value);
        }

        public int SmtpPort
        {
            get => _smtpPort;
            set => SetProperty(ref _smtpPort, value);
        }

        public string SmtpUsername
        {
            get => _smtpUsername;
            set => SetProperty(ref _smtpUsername, value);
        }

        public string SmtpPassword
        {
            get => _smtpPassword;
            set => SetProperty(ref _smtpPassword, value);
        }

        public bool EnableEmailNotifications
        {
            get => _enableEmailNotifications;
            set => SetProperty(ref _enableEmailNotifications, value);
        }

        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            LoadSettings();
        }

        private void LoadSettings()
        {
            // In a real app, load from configuration or registry
            SmtpServer = "smtp.example.com";
            SmtpPort = 25;
            SmtpUsername = "";
            SmtpPassword = "";
            EnableEmailNotifications = false;
        }

        private void SaveSettings()
        {
            // In a real app, save to configuration or registry
        }
    }
}