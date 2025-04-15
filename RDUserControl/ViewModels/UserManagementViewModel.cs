using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RDUserControl.Models;
using RDUserControl.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RDUserControl.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject, IAsyncInitialization
    {
        private readonly IUserManagementService _userManagementService;

        [ObservableProperty]
        private ObservableCollection<User> users = new();

        public IRelayCommand LoadUsersCommand { get; }
        public IRelayCommand<string>? EnableUserCommand { get; }
        public IRelayCommand<string>? DisableUserCommand { get; }
        public IRelayCommand<string>? ResetPasswordCommand { get; }

        public Task Initialization { get; }

        public UserManagementViewModel(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;

            LoadUsersCommand = new RelayCommand(LoadUsersAsync);
            EnableUserCommand = new RelayCommand<string>(EnableUserAsync);
            DisableUserCommand = new RelayCommand<string>(DisableUserAsync);
            ResetPasswordCommand = new RelayCommand<string>(ResetPasswordAsync);
            Initialization = LoadUsersAsync();
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
            await Task.CompletedTask;
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            Users = new ObservableCollection<User>(users);
        }

    }
}