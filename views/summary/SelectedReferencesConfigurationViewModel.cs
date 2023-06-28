using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using HandyControl.Controls;
using System.Collections.ObjectModel;



namespace ReferenceConfigurator.views {
    public class SelectedReferencesConfigurationViewModel : MainContentViewModel {
        private ObservableCollection<CheckBoxModel> _columnList;

        public ObservableCollection<CheckBoxModel> ColumnList {
            get => _columnList;
            set {
                SetProperty(ref _columnList, value);
                _selectedViewModel.ColumnList = _columnList;
            }
        }

        public ICommand SaveConfigurationCommand { get; }

        private SelectedReferencesViewModel _selectedViewModel;

        public SelectedReferencesConfigurationViewModel(SelectedReferencesViewModel selected) {
            _selectedViewModel = selected;
            _columnList = new ObservableCollection<CheckBoxModel>();
            ColumnList = _columnList;
            populate();

            SaveConfigurationCommand = new RelayCommand(SaveConfiguration);
        }

        private void populate() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string fileName = Path.Combine(basePath, "ReferenceConfigurator/config", "selectedConfig.json");
            if (File.Exists(fileName)) {
                string configurationString = File.ReadAllText(fileName);
                ColumnList = JsonSerializer.Deserialize<ObservableCollection<CheckBoxModel>>(configurationString);
            } else {
                ColumnList.Clear();
                ColumnList.Add(new CheckBoxModel() { Name = "ProjectId", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "Partner", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "ProjectName", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "Start", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "End", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "Data", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "Branch", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "Team", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "ProjectDescriptionEN", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "Client", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "Topic", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "ProjectDescriptionDE", IsChecked = false });
            }
        }

        private void SaveConfiguration() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/config");
            Directory.CreateDirectory(folderPath);
            string fileName = Path.Combine(folderPath, "selectedConfig.json");
            string configurationString = JsonSerializer.Serialize(_columnList);
            File.WriteAllText(fileName, configurationString);
            Growl.Info("Configuration Saved.");
        }
    }
}
