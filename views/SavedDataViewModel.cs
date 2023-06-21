using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using ReferenceConfigurator.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReferenceConfigurator.views {
    public class SavedDataViewModel :MainContentViewModel {

        public ICommand DeletLogosCommand { get; }

        public ICommand DeletOnePagerCommand { get; }

        public ICommand RefreshSavedTemplates { get; }

        public ICommand RefreshSearchCommand { get; }

        private PopUpViewModel parent;
        public SavedDataViewModel(PopUpViewModel parent) {
            this.parent = parent;
            DeletLogosCommand = new RelayCommand(deletLogos);
            DeletOnePagerCommand = new RelayCommand(deletOnePager);
            RefreshSavedTemplates = new RelayCommand(refreshSavedTemplates);
            RefreshSearchCommand = new RelayCommand(refreshSearch);
        }

        public void deletLogos() {
            Utils.removeLogos();
            Growl.Info("Removed Logos successfully");
        }

        public void deletOnePager() {
            Utils.removeOnePager();
            Growl.Info("Removed One Pager successfully");
        }

        public void refreshSavedTemplates() {
            Utils.removeTemplates();
            parent.refreshTemplate();
            Growl.Info("Refreshed Templates successfully");
        }

        public void refreshSearch() {
            parent.refreshSearch();
            Growl.Info("Refreshed Search successfully");
        }
    }
}
