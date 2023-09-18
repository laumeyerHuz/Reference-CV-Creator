using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReferenceConfigurator.views {

    public class ProgressBarViewModel : ViewModelBase {

        private ObservableCollection<StepIndexModel> _stepList;

        public ObservableCollection<StepIndexModel> StepList {
            get => _stepList;
            set => SetProperty(ref _stepList, value);
        }

        private int _stepIndex;

        public ICommand BackCommand {  get; set; }

        public int StepIndex {
            get => _stepIndex;
            set {
                SetProperty(ref _stepIndex, value);
                indexChanged(value);
            }
        }

        private PopUpViewModel parent;

        public ProgressBarViewModel(PopUpViewModel popUpViewModel) {
            this.parent = popUpViewModel;
            BackCommand = new RelayCommand(back);
        }

        public void prev() {
            if (StepIndex > 0) {
                StepIndex -= 1;
            }
        }

        public void next() {
            if (StepIndex < StepList.Count - 1) {
                StepIndex += 1;
            }
        }

        public void changeStepList(string type) {
            if (type == "Profile") {
                StepList = new ObservableCollection<StepIndexModel>(getProfileStepList());
                StepIndex = 0;
            } else if (type == "Reference") {
                StepList = new ObservableCollection<StepIndexModel>(getReferenceStepList());
                StepIndex = 0;
            } else if (type == "Settings") {
                StepList = new ObservableCollection<StepIndexModel>(getSettingsStepList());
                StepIndex = 0;
            } else if (type == "Logo") {
                StepList = new ObservableCollection<StepIndexModel>(getLogoStepList());
                StepIndex = 0;
            }
        }

        private List<StepIndexModel> getProfileStepList() {
            List<StepIndexModel> tmp = new List<StepIndexModel> {
                new StepIndexModel("LayoutProfile", "Select Layout"),
                new StepIndexModel("SearchProfile", "Select Profiles"),
                new StepIndexModel("SummaryProfile", "Summary and Create Slide")
            };

            return tmp;
        }

        private List<StepIndexModel> getReferenceStepList() {
            List<StepIndexModel> tmp = new List<StepIndexModel> {
                new StepIndexModel("LayoutReferences", "Select Layout"),
                new StepIndexModel("SearchReferences", "Select References"),
                new StepIndexModel("SummaryReferences", "Summary and Create Slide")
            };

            return tmp;
        }

        private List<StepIndexModel> getSettingsStepList() {
            List<StepIndexModel> tmp = new List<StepIndexModel> {
                new StepIndexModel("SavedData", "Saved Data"),
                new StepIndexModel("ReferenceConfiguration", "Reference Configuration"),
                new StepIndexModel("ProfileConfiguration", "Profile Configuration")
            };

            return tmp;
        }
        private List<StepIndexModel> getLogoStepList() {
            List<StepIndexModel> tmp = new List<StepIndexModel> {
                new StepIndexModel("SearchLogo", "Search Logo"),
                
            };

            return tmp;
        }

        private void indexChanged(int index) {

            parent.changeStep(StepList[index].Header);
        }

        private void back() {
            parent.ChangePath("Start");
        }
    }
}
