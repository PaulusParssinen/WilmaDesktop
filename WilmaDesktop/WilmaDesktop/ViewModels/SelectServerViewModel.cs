using System;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Generic;

using WilmAPI;
using WilmAPI.Json;

using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;

using WilmaDesktop.Utils;
using WilmaDesktop.Constants;

namespace WilmaDesktop.ViewModels
{
    public class SelectServerViewModel : BindableBase
    {
        private string _filter = string.Empty;
        public string Filter
        {
            get => _filter;
            set
            {
                SetProperty(ref _filter, value);
                _serverCollection.Refresh();
            }
        }

        private ICollectionView _serverCollection;
        private readonly IRegionManager _regionManager;

        private WilmaServer _selectedServer;
        public WilmaServer SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value);
        }

        public NotifyTaskCompletion<List<WilmaServer>> Servers { get; set; }

        public DelegateCommand SelectServerCommand { get; }

        public SelectServerViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            Servers = new NotifyTaskCompletion<
                List<WilmaServer>>(WilmaSession.GetServersAsync());

            Servers.TaskCompleted += OnServersLoaded;

            SelectServerCommand = new DelegateCommand(SelectServer, 
                () => SelectedServer != null)
                .ObservesProperty(() => SelectedServer);
        }

        private void SelectServer()
        {
            var navParams = new NavigationParameters
            {
                {"server", SelectedServer}
            };

            _regionManager.RequestNavigate(
                ContentRegion.MainWindowRight, NavigationView.LoginView, navParams);
        }

        private void OnServersLoaded(object sender, EventArgs e)
        {
            _serverCollection = CollectionViewSource.GetDefaultView(Servers.Result);
            _serverCollection.Filter = ServerFilter;
        }

        private bool ServerFilter(object item)
        {
            var server = item as WilmaServer;

            return server != null && (server.Name.ToLower().Contains(_filter.ToLower()) || 
                                      server.Url.ToLower().Contains(_filter.ToLower())); //This is horrible but whatever..
        }
    }
}
