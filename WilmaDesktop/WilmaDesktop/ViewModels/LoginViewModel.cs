using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Mvvm;
using Prism.Regions;
using WilmAPI.Json;

namespace WilmaDesktop.ViewModels
{
    public class LoginViewModel : BindableBase, INavigationAware
    {
        private WilmaServer _server;
        public WilmaServer Server {
            get { return _server; }
            set
            {
                _server = value;

                RaisePropertyChanged(nameof(Server));
            }
        }

        public LoginViewModel()
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Server = navigationContext
                .Parameters["server"] as WilmaServer;
        }

        #region NavigationInterface
        public bool IsNavigationTarget(NavigationContext navigationContext)
        { return true; }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        { }
#endregion
    }
}
