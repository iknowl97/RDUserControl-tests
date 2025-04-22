csharp
using RDUserControl.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace RDUserControl.Views
{
    public partial class RdpManagementPage : Page, IComponentConnector
    {

        public RdpManagementPage(IRdpService rdpService, IUserManagementService userManagementService, IEmailService emailService)
        {
            InitializeComponent();
            DataContext = new RdpManagementViewModel(rdpService, userManagementService, emailService);
        }




        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.13.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            this._contentLoaded = true;
            if (connectionId == 1)
            {
                this.SystemRdpEnabledCheckBox = ((System.Windows.Controls.CheckBox)(target));
            }
            else if (connectionId == 2)
            {
                this.EnableSystemRdpButton = ((System.Windows.Controls.Button)(target));
            }
            else if (connectionId == 3)
            {
                this.DisableSystemRdpButton = ((System.Windows.Controls.Button)(target));
            }
            else if (connectionId == 4)
            {
                this.UsersListBox = ((System.Windows.Controls.ListBox)(target));
            }
            else if (connectionId == 5)
            {
                this.EnableRdpForUserButton = ((System.Windows.Controls.Button)(target));
            }
            else if (connectionId == 6)
            {
                this.DisableRdpForUserButton = ((System.Windows.Controls.Button)(target));
            }
            else if (connectionId == 7)
            {
                this.BatchEnableRdpButton = ((System.Windows.Controls.Button)(target));
            }
            else if (connectionId == 8)
            {
                this.BatchDisableRdpButton = ((System.Windows.Controls.Button)(target));
            }
        }


    }
}