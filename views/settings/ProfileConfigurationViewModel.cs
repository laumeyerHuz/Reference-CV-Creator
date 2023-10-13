using HandyControl.Controls;
using ReferenceConfigurator.models;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ReferenceConfigurator.views {
    public class ProfileConfigurationViewModel : ConfigurationViewModel {

        public override ObservableCollection<DataGridCheckBoxModel> ColumnList {
            get => _columnList;
            set {
                SetProperty(ref _columnList, value);
                _searchViewModel.ColumnList = _columnList;
            }
        }

        public ProfileConfigurationViewModel(SearchViewModel search, SummaryViewModel selected) : base(search, selected) {
            saveLocation = "ProfileConfig.json";
            if (loadConfiguration() == false) {
                populate();
            }
        }

        protected override void populate() {
            ColumnList.Clear();
            ColumnList.Add(new DataGridCheckBoxModel("FirstName", true));
            ColumnList.Add(new DataGridCheckBoxModel("LastName",true ));
            ColumnList.Add(new DataGridCheckBoxModel("Initials", true));
            ColumnList.Add(new DataGridCheckBoxModel("RoleEN", true));
            ColumnList.Add(new DataGridCheckBoxModel("RoleDE", false));
            ColumnList.Add(new DataGridCheckBoxModel("Tribe", true));
            ColumnList.Add(new DataGridCheckBoxModel("Squad", true));
            ColumnList.Add(new DataGridCheckBoxModel("ProductTopicOwner", false));
            ColumnList.Add(new DataGridCheckBoxModel("InternalResponsibility", false));
            ColumnList.Add(new DataGridCheckBoxModel("ProfessionalExperienceEN", false));
            ColumnList.Add(new DataGridCheckBoxModel("ProfessionalExperienceDE", false));
            ColumnList.Add(new DataGridCheckBoxModel("EducationAndTrainingEN", false));
            ColumnList.Add(new DataGridCheckBoxModel("EnductionAndTrainingDE", false));
            ColumnList.Add(new DataGridCheckBoxModel("ProjectExperienceEN", false));
            ColumnList.Add(new DataGridCheckBoxModel("ProjectExperienceDE", false));
            ColumnList.Add(new DataGridCheckBoxModel("IndustryExperienceEN", false));
            ColumnList.Add(new DataGridCheckBoxModel("IndustryExperienceDE", false));
            ColumnList.Add(new DataGridCheckBoxModel("FunctionalExperienceEN", false));
            ColumnList.Add(new DataGridCheckBoxModel("FunctionalExperienceDE", false));
            ColumnList.Add(new DataGridCheckBoxModel("MethodExpertise", false));
            ColumnList.Add(new DataGridCheckBoxModel("ToolExpertise", false));
            ColumnList.Add(new DataGridCheckBoxModel("AdditionalQualifications", false));
            ColumnList.Add(new DataGridCheckBoxModel("LanguagesEn", false));
            ColumnList.Add(new DataGridCheckBoxModel("LanguagesDE", false));
            ColumnList.Add(new DataGridCheckBoxModel("YearsWorkExperience", false));

        }
    }
}
