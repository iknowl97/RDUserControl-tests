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
}