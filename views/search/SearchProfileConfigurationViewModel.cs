using ReferenceConfigurator.models;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.Json;
using HandyControl.Controls;

namespace ReferenceConfigurator.views {
    public class SearchProfileConfigurationViewModel : SearchConfigurationViewModel {
        public SearchProfileConfigurationViewModel(SearchViewModel search) : base(search) {
        }

        protected override void populate() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string fileName = Path.Combine(basePath, "ReferenceConfigurator/config", "searchProfileConfig.json");
            if (File.Exists(fileName)) {
                string configurationString = File.ReadAllText(fileName);
                ColumnList = JsonSerializer.Deserialize<ObservableCollection<CheckBoxModel>>(configurationString);
            } else {
                ColumnList.Clear();
                ColumnList.Add(new CheckBoxModel() { Name = "FirstName", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "LastName", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "Initials", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "RoleEN", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "RoleDE", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "Tribe", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "Squad", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "ProductTopicOwner", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "InternalResponsibility", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "ProfessionalExperienceEN", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "ProfessionalExperienceDE", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "EducationAndTrainingEN", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "EnductionAndTrainingDE", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "ProjectExperienceEN", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "ProjectExperienceDE", IsChecked = false }); 
                ColumnList.Add(new CheckBoxModel() { Name = "IndustryExperienceEN", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "IndustryExperienceDE", IsChecked = false }); 
                ColumnList.Add(new CheckBoxModel() { Name = "FunctionalExperienceEN", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "FunctionalExperienceDE", IsChecked = false }); 
                ColumnList.Add(new CheckBoxModel() { Name = "MethodExpertise", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "ToolExpertise", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "AdditionalQualifications", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "LanguagesEn", IsChecked = true });
                ColumnList.Add(new CheckBoxModel() { Name = "LanguagesDE", IsChecked = false });
                ColumnList.Add(new CheckBoxModel() { Name = "YearsWorkExperience", IsChecked = true });

            }
        }
        protected override void SaveConfiguration() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/config");
            Directory.CreateDirectory(folderPath);
            string fileName = Path.Combine(folderPath, "searchReferenceProfile.json");
            string configurationString = JsonSerializer.Serialize(_columnList);
            File.WriteAllText(fileName, configurationString);
            Growl.Info("Configuration Saved.");
        }
    }
}
