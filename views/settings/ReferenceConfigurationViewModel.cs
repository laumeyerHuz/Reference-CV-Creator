using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using HandyControl.Controls;
using System.Collections.ObjectModel;



namespace ReferenceConfigurator.views {
    public class ReferenceConfigurationViewModel : ConfigurationViewModel {

        public ReferenceConfigurationViewModel(SearchViewModel search, SummaryViewModel selected) : base(search, selected) {
            saveLocation = "ReferenceConfig.json";
            if (loadConfiguration() == false) {
                populate();
            }
        }

        protected override void populate() {
            ColumnList.Clear();
            ColumnList.Add(new DataGridCheckBoxModel("ProjectId", true));
            ColumnList.Add(new DataGridCheckBoxModel("Partner", false));
            ColumnList.Add(new DataGridCheckBoxModel("ProjectName", true));
            ColumnList.Add(new DataGridCheckBoxModel("Start", false));
            ColumnList.Add(new DataGridCheckBoxModel("End", false));
            ColumnList.Add(new DataGridCheckBoxModel("Data", false));
            ColumnList.Add(new DataGridCheckBoxModel("Industry", false));
            ColumnList.Add(new DataGridCheckBoxModel("Team", false));
            ColumnList.Add(new DataGridCheckBoxModel("ProjectDescriptionEN", true));
            ColumnList.Add(new DataGridCheckBoxModel("Client", true));
            ColumnList.Add(new DataGridCheckBoxModel("Topic", false));
            ColumnList.Add(new DataGridCheckBoxModel("ProjectDescriptionDE", false));
        }
    }
}
