using System.Windows;

using Prism.Regions;
using Prism.Unity;

using Microsoft.Practices.Unity;
using WilmaDesktop.Constants;
using WilmaDesktop.Views;

namespace WilmaDesktop
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
            => Container.Resolve<IntroductionView>();

        protected override void InitializeShell()
        {
            base.InitializeShell();
            
            var regionManager = Container.Resolve<IRegionManager>();

            regionManager?.RegisterViewWithRegion(
                ContentRegion.MainWindowRight, typeof(SelectServerView));

            Container.RegisterTypeForNavigation<LoginView>(NavigationView.LoginView);

            Application.Current.MainWindow.Show();
        }
    }
}
