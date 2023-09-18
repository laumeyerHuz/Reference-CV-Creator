using HandyControl.Controls;
using ReferenceConfigurator.models;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Graph.Models;

namespace ReferenceConfigurator.views {



    public class ConfigurationViewModel : MainContentViewModel {

        protected ObservableCollection<DataGridCheckBoxModel> _columnList;

        public virtual ObservableCollection<DataGridCheckBoxModel> ColumnList {
            get => _columnList;
            set {
                SetProperty(ref _columnList, value);
                _searchViewModel.ColumnList = _columnList;
                _summaryViewModel.ColumnList = _columnList;
            }
        }

   

        public ICommand SaveConfigurationCommand { get; }

        protected SearchViewModel _searchViewModel;

        protected SummaryViewModel _summaryViewModel;

        protected string saveLocation;

        public ConfigurationViewModel(SearchViewModel search, SummaryViewModel selected) {
            _searchViewModel = search;
            _summaryViewModel = selected;
            _columnList = new ObservableCollection<DataGridCheckBoxModel>();
            
            ColumnList = _columnList;
            
            
            SaveConfigurationCommand = new RelayCommand(SaveConfiguration);
        }

        protected virtual void populate() {
            throw new NotImplementedException();
        }

        protected Boolean loadConfiguration() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string fileName = Path.Combine(basePath, "ReferenceConfigurator/config", saveLocation);
            if (File.Exists(fileName)) {
                string configurationString = File.ReadAllText(fileName);
                try {
                    ColumnList = JsonSerializer.Deserialize<ObservableCollection<DataGridCheckBoxModel>>(configurationString);
                } catch {
                    return false;
                }
                return true;
            } else {
                return false;
            }
        }

        protected void SaveConfiguration() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/config");
            Directory.CreateDirectory(folderPath);
            string fileName = Path.Combine(folderPath, saveLocation);
            string configurationString = JsonSerializer.Serialize(ColumnList);
            File.WriteAllText(fileName, configurationString);
            Growl.Info("Configuration Saved.");
        }
    }
}
