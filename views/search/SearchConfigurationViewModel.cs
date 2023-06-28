using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using HandyControl.Controls;

namespace ReferenceConfigurator.views {
    public class SearchConfigurationViewModel :MainContentViewModel {

        protected ObservableCollection<CheckBoxModel> _columnList;

        public ObservableCollection<CheckBoxModel> ColumnList {
            get => _columnList;
            set {SetProperty(ref _columnList, value);
                _searchViewModel.ColumnList = _columnList;
            }
        }

        public ICommand SaveConfigurationCommand { get; }

        private SearchViewModel _searchViewModel; 

        public SearchConfigurationViewModel(SearchViewModel search) {
            _searchViewModel = search;
            _columnList = new ObservableCollection<CheckBoxModel>();
            ColumnList = _columnList;
            populate();

            SaveConfigurationCommand = new RelayCommand(SaveConfiguration);
        }

        protected virtual void populate() { }

        protected virtual void SaveConfiguration() { }
    }
}
