using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;

using WilmAPI;
using WilmAPI.Json;

using System.Windows.Controls;
using Microsoft.Practices.Unity;

using WilmaDesktop.Views;

namespace WilmaDesktop.ViewModels
{
    public class LoginViewModel : BindableBase, INavigationAware
    {
        private readonly IUnityContainer _container; 

        private WilmaServer _server;
        public WilmaServer Server {
            get => _server;
            set => SetProperty(ref _server, value);
        }

        private WilmaSession _session;

        private bool _isLoggingin;
        public bool IsLoggingIn
        {
            get => _isLoggingin;
            set => SetProperty(ref _isLoggingin, value);
        }

        public DelegateCommand<object> LoginCommand { get; set; }

        public LoginViewModel(IUnityContainer container)
        {
            _container = container;

            LoginCommand = new DelegateCommand<object>(LoginAsync);
        }

        private async void LoginAsync(object parameter) //I hate async voids too
        {
            var values = (object[])parameter;
            var pwBox = values[1] as PasswordBox;

            if (pwBox == null) return;

            //VALIDATE

            await _session.LoginAsync((string)values[0], pwBox.Password);
            IsLoggingIn = !IsLoggingIn;

            if (_session.IsAuthenticated)
            {
                _container
                    .Resolve<MainWindow>().Show();
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Server = navigationContext
                .Parameters["server"] as WilmaServer;

            _session = new WilmaSession(Server)
            {
                APIKEY = "" //muh secret
            };
        }

        #region NavigationInterface
        public bool IsNavigationTarget(NavigationContext navigationContext)
        { return true; }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        { }
#endregion
    }
}
