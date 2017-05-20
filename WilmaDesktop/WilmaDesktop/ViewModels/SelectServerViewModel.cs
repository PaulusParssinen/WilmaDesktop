using System;
using System.Collections.Generic;

using WilmaDesktop.Utils;
using WilmAPI.Json;
using WilmAPI;

using Prism.Mvvm;
using System.ComponentModel;
using System.Windows.Data;

using Prism.Commands;
using Prism.Regions;

using WilmaDesktop.Constants;

namespace WilmaDesktop.ViewModels
{
    public class SelectServerViewModel : BindableBase
    {
        private string _filter = string.Empty;
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (_filter == value) return;

                _filter = value;
                _serverCollection.Refresh();
            }
        }

        private ICollectionView _serverCollection;
        private readonly IRegionManager _regionManager;

        private WilmaServer _selectedServer;
        public WilmaServer SelectedServer
        {
            get { return _selectedServer; }
            set
            {
                _selectedServer = value;

                RaisePropertyChanged(nameof(SelectedServer));
            }
        }

        public NotifyTaskCompletion<List<WilmaServer>> Servers { get; set; }

        public DelegateCommand SelectServerCommand { get; private set; }

        public SelectServerViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            Servers = new NotifyTaskCompletion<
                List<WilmaServer>>(WilmaSession.GetServersAsync());

            Servers.TaskCompleted += OnServersLoaded;

            SelectServerCommand = new DelegateCommand(SelectServer, 
                () => SelectedServer != null) //top tier shit bois
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

            return server.Name.ToLower()
                .Contains(_filter.ToLower()) || 
                   server.Url.ToLower()
                .Contains(_filter.ToLower());
        }
    }
}
