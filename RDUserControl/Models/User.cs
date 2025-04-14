using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RDUserControl.Models
{
    public class User : ObservableObject
    {
        private string _username;
        private string _displayName;
        private string _domain;
        private bool _isEnabled;
        private bool _isRdpEnabled;
        private DateTime _lastLogon;
        private List<string> _groups;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public string Domain
        {
            get => _domain;
            set => SetProperty(ref _domain, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsRdpEnabled
        {
            get => _isRdpEnabled;
            set => SetProperty(ref _isRdpEnabled, value);
        }

        public DateTime LastLogon
        {
            get => _lastLogon;
            set => SetProperty(ref _lastLogon, value);
        }

        public List<string> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public User()
        {
            _groups = new List<string>();
        }
    }

    public class UserGroup : ObservableObject
    {
        private string _name;
        private string _description;
        private List<string> _members;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public List<string> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public UserGroup()
        {
            _members = new List<string>();
        }
    }
}