using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RDUserControl.Models;
using RDUserControl.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RDUserControl.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly IUserManagementService _userManagementService;
        
        [ObservableProperty]
        private ObservableCollection<User> _users = new();

        public ICommand LoadUsersCommand { get; }
        public ICommand EnableUserCommand { get; }
        public ICommand DisableUserCommand { get; }
        public ICommand ResetPasswordCommand { get; }

        public UserManagementViewModel(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
            
            LoadUsersCommand = new RelayCommand(async () => await LoadUsersAsync());
            EnableUserCommand = new RelayCommand<string>(async (username) => await EnableUserAsync(username));
            DisableUserCommand = new RelayCommand<string>(async (username) => await DisableUserAsync(username));
            ResetPasswordCommand = new RelayCommand<string>(async (username) => await ResetPasswordAsync(username));
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            Users = new ObservableCollection<User>(users);
        }

        private async Task EnableUserAsync(string username)
        {
            await _userManagementService.EnableUserAsync(username);
            await LoadUsersAsync();
        }

        private async Task DisableUserAsync(string username)
        {
            await _userManagementService.DisableUserAsync(username);
            await LoadUsersAsync();
        }

        private async Task ResetPasswordAsync(string username)
        {
            // Implementation for password reset
        }
    }
}