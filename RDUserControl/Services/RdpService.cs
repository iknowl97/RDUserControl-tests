using RDUserControl.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management;
using System.Threading.Tasks;

namespace RDUserControl.Services
{
    public class RdpService : IRdpService
    {
        private const string RDP_GROUP_NAME = "Remote Desktop Users";
        private readonly IUserManagementService _userManagementService;

        public RdpService(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<bool> EnableRdpForUserAsync(string username)
        {
            return await _userManagementService.AddUserToGroupAsync(username, RDP_GROUP_NAME);
        }

        public async Task<bool> DisableRdpForUserAsync(string username)
        {
            return await _userManagementService.RemoveUserFromGroupAsync(username, RDP_GROUP_NAME);
        }

        public async Task<bool> IsRdpEnabledForUserAsync(string username)
        {
            var user = await _userManagementService.GetUserByUsernameAsync(username);
            return user != null && user.Groups.Contains(RDP_GROUP_NAME);
        }

        public async Task<bool> BatchEnableRdpAsync(List<string> usernames)
        {
            bool allSucceeded = true;
            foreach (var username in usernames)
            {
                bool success = await EnableRdpForUserAsync(username);
                if (!success) allSucceeded = false;
            }
            return allSucceeded;
        }

        public async Task<bool> BatchDisableRdpAsync(List<string> usernames)
        {
            bool allSucceeded = true;
            foreach (var username in usernames)
            {
                bool success = await DisableRdpForUserAsync(username);
                if (!success) allSucceeded = false;
            }
            return allSucceeded;
        }

        public async Task<bool> IsRdpEnabledOnSystemAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher(
                        "root\\CIMV2", 
                        "SELECT * FROM Win32_TerminalServiceSetting"))
                    {
                        foreach (var queryObj in searcher.Get())
                        {
                            return (bool)(queryObj["AllowTSConnections"].ToString() == "1");
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public async Task<bool> EnableRdpOnSystemAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher(
                        "root\\CIMV2", 
                        "SELECT * FROM Win32_TerminalServiceSetting"))
                    {
                        foreach (var queryObj in searcher.Get())
                        {
                            var classInstance = queryObj as ManagementObject;
                            var inParams = classInstance.GetMethodParameters("SetAllowTSConnections");
                            inParams["AllowTSConnections"] = true;
                            inParams["ModifyFirewallException"] = true;
                            classInstance.InvokeMethod("SetAllowTSConnections", inParams, null);
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public async Task<bool> DisableRdpOnSystemAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher(
                        "root\\CIMV2", 
                        "SELECT * FROM Win32_TerminalServiceSetting"))
                    {
                        foreach (var queryObj in searcher.Get())
                        {
                            var classInstance = queryObj as ManagementObject;
                            var inParams = classInstance.GetMethodParameters("SetAllowTSConnections");
                            inParams["AllowTSConnections"] = false;
                            inParams["ModifyFirewallException"] = true;
                            classInstance.InvokeMethod("SetAllowTSConnections", inParams, null);
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }

    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            // In a real implementation, this would use SMTP or another email service
            await Task.CompletedTask;
        }

        public async Task SendPasswordResetEmailAsync(string username, string temporaryPassword)
        {
            string subject = "Password Reset Notification";
            string body = $"Your password has been reset. Your temporary password is: {temporaryPassword}";
            await SendEmailAsync(username, subject, body);
        }

        public async Task SendBatchNotificationAsync(string operation, List<string> usernames, bool success)
        {
            string subject = $"Batch {operation} Operation {(success ? "Successful" : "Failed")}";
            string body = $"The following users were affected:\n{string.Join("\n", usernames)}";
            
            // In a real implementation, this would send to an administrator
            await SendEmailAsync("admin@example.com", subject, body);
        }
    }
}