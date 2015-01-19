using System.ComponentModel.WpfApplication.Shell;
using System.Windows;
using Caliburn.Micro;

namespace System.ComponentModel.WpfApplication
{
    public sealed class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
