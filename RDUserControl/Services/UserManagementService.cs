using RDUserControl.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace RDUserControl.Services
{
    public class UserManagementService : IUserManagementService
    {
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Task.Run(() =>
            {
                var users = new List<User>();
                using (var context = new PrincipalContext(ContextType.Machine))
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        if (result is UserPrincipal user)
                        {
                            users.Add(new User
                            {
                                Username = user.SamAccountName,
                                DisplayName = user.DisplayName,
                                Domain = Environment.MachineName,
                                IsEnabled = user.Enabled ?? false,
                                LastLogon = user.LastLogon ?? DateTime.MinValue,
                                Groups = GetUserGroups(user.SamAccountName)
                            });
                        }
                    }
                }
                return users;
            });
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await Task.Run(() =>
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(context, username);
                    if (userPrincipal != null)
                    {
                        return new User
                        {
                            Username = userPrincipal.SamAccountName,
                            DisplayName = userPrincipal.DisplayName,
                            Domain = Environment.MachineName,
                            IsEnabled = userPrincipal.Enabled ?? false,
                            LastLogon = userPrincipal.LastLogon ?? DateTime.MinValue,
                            Groups = GetUserGroups(username)
                        };
                    }
                }
                return null;
            });
        }

        public async Task<bool> EnableUserAsync(string username)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var context = new PrincipalContext(ContextType.Machine))
                    {
                        var user = UserPrincipal.FindByIdentity(context, username);
                        if (user != null)
                        {
                            user.Enabled = true;
                            user.Save();
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

        public async Task<bool> DisableUserAsync(string username)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var context = new PrincipalContext(ContextType.Machine))
                    {
                        var user = UserPrincipal.FindByIdentity(context, username);
                        if (user != null)
                        {
                            user.Enabled = false;
                            user.Save();
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

        public async Task<bool> ResetPasswordAsync(string username, string newPassword)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var context = new PrincipalContext(ContextType.Machine))
                    {
                        var user = UserPrincipal.FindByIdentity(context, username);
                        if (user != null)
                        {
                            user.SetPassword(newPassword);
                            user.Save();
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

        public async Task<List<UserGroup>> GetAllGroupsAsync()
        {
            return await Task.Run(() =>
            {
                var groups = new List<UserGroup>();
                using (var context = new PrincipalContext(ContextType.Machine))
                using (var searcher = new PrincipalSearcher(new GroupPrincipal(context)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        if (result is GroupPrincipal group)
                        {
                            groups.Add(new UserGroup
                            {
                                Name = group.Name,
                                Description = group.Description,
                                Members = GetGroupMembers(group.Name)
                            });
                        }
                    }
                }
                return groups;
            });
        }

        public async Task<UserGroup> GetGroupByNameAsync(string groupName)
        {
            return await Task.Run(() =>
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    var groupPrincipal = GroupPrincipal.FindByIdentity(context, groupName);
                    if (groupPrincipal != null)
                    {
                        return new UserGroup
                        {
                            Name = groupPrincipal.Name,
                            Description = groupPrincipal.Description,
                            Members = GetGroupMembers(groupName)
                        };
                    }
                }
                return null;
            });
        }

        public async Task<bool> AddUserToGroupAsync(string username, string groupName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var context = new PrincipalContext(ContextType.Machine))
                    {
                        var user = UserPrincipal.FindByIdentity(context, username);
                        var group = GroupPrincipal.FindByIdentity(context, groupName);

                        if (user != null && group != null)
                        {
                            if (!group.Members.Contains(user))
                            {
                                group.Members.Add(user);
                                group.Save();
                            }
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

        public async Task<bool> RemoveUserFromGroupAsync(string username, string groupName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var context = new PrincipalContext(ContextType.Machine))
                    {
                        var user = UserPrincipal.FindByIdentity(context, username);
                        var group = GroupPrincipal.FindByIdentity(context, groupName);

                        if (user != null && group != null)
                        {
                            if (group.Members.Contains(user))
                            {
                                group.Members.Remove(user);
                                group.Save();
                            }
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

        public async Task<bool> BatchEnableUsersAsync(List<string> usernames)
        {
            bool allSucceeded = true;
            foreach (var username in usernames)
            {
                bool success = await EnableUserAsync(username);
                if (!success) allSucceeded = false;
            }
            return allSucceeded;
        }

        public async Task<bool> BatchDisableUsersAsync(List<string> usernames)
        {
            bool allSucceeded = true;
            foreach (var username in usernames)
            {
                bool success = await DisableUserAsync(username);
                if (!success) allSucceeded = false;
            }
            return allSucceeded;
        }

        private List<string> GetUserGroups(string username)
        {
            var groups = new List<string>();
            try
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    var user = UserPrincipal.FindByIdentity(context, username);
                    if (user != null)
                    {
                        var userGroups = user.GetGroups();
                        groups.AddRange(userGroups.Select(g => g.Name));
                    }
                }
            }
            catch (Exception) { }
            return groups;
        }

        private List<string> GetGroupMembers(string groupName)
        {
            var members = new List<string>();
            try
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    if (group != null)
                    {
                        var groupMembers = group.GetMembers();
                        members.AddRange(groupMembers.OfType<UserPrincipal>().Select(u => u.SamAccountName));
                    }
                }
            }
            catch (Exception) { }
            return members;
        }
    }
}