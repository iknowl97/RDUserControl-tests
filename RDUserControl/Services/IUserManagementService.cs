using RDUserControl.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RDUserControl.Services
{
    public interface IUserManagementService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> EnableUserAsync(string username);
        Task<bool> DisableUserAsync(string username);
        Task<bool> ResetPasswordAsync(string username, string newPassword);
        Task<List<UserGroup>> GetAllGroupsAsync();
        Task<UserGroup> GetGroupByNameAsync(string groupName);
        Task<bool> AddUserToGroupAsync(string username, string groupName);
        Task<bool> RemoveUserFromGroupAsync(string username, string groupName);
        Task<bool> BatchEnableUsersAsync(List<string> usernames);
        Task<bool> BatchDisableUsersAsync(List<string> usernames);
    }

    public interface IRdpService
    {
        Task<bool> EnableRdpForUserAsync(string username);
        Task<bool> DisableRdpForUserAsync(string username);
        Task<bool> IsRdpEnabledForUserAsync(string username);
        Task<bool> BatchEnableRdpAsync(List<string> usernames);
        Task<bool> BatchDisableRdpAsync(List<string> usernames);
        Task<bool> IsRdpEnabledOnSystemAsync();
        Task<bool> EnableRdpOnSystemAsync();
        Task<bool> DisableRdpOnSystemAsync();
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string recipient, string subject, string body);
        Task SendPasswordResetEmailAsync(string username, string temporaryPassword);
        Task SendBatchNotificationAsync(string operation, List<string> usernames, bool success);
    }
}