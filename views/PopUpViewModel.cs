#nullable enable

using System;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ReferenceConfigurator.models;
using ReferenceConfigurator.lucene;
using ReferenceConfigurator.powerpointSlideCreator;
using ReferenceConfigurator.Properties;
using AngleSharp.Css.Dom;
using HandyControl.Controls;
using Microsoft.Office.SharePoint.Tools;

namespace ReferenceConfigurator.views {
    public class PopUpViewModel : ViewModelBase {

        private MainContentViewModel _contentViewModel;

        public MainContentViewModel ContentViewModel {
            get => _contentViewModel;
            set => SetProperty(ref _contentViewModel, value);
        }

        private ProgressBarViewModel? _stepBar;

        public ProgressBarViewModel? StepBar {
            get => _stepBar;
            set => SetProperty(ref _stepBar, value);
        }


        private readonly LuceneInterfaceReference _luceneInterfaceReference;
        private readonly LuceneInterfaceProfile _luceneInterfaceProfile;

        private readonly StartViewModel Start;
        private readonly SearchReferenceViewModel SearchReference;
        private readonly LayoutReferenceViewModel LayoutReference;
        private readonly SearchProfileViewModel SearchProfile;
        private readonly SearchProfileConfigurationViewModel SearchProfileConfiguration;
        private readonly LayoutProfileViewModel LayoutProfile;
        private readonly SearchReferenceConfigurationViewModel SearchReferenceConfiguration;
        private readonly SelectedReferencesViewModel References;
        private readonly SelectedReferencesConfigurationViewModel SelectedReferencesConfiguration;
        private readonly ProgressBarViewModel ProgressBar;
        private readonly SavedDataViewModel SavedData;

        public PopUpViewModel() {
            _luceneInterfaceReference = new LuceneInterfaceReference();
            _luceneInterfaceProfile = new LuceneInterfaceProfile();

            //Init Views
            Start = new StartViewModel(this);
            SearchReference = new SearchReferenceViewModel(this, _luceneInterfaceReference);
            LayoutReference = new LayoutReferenceViewModel(this);
            SearchProfile = new SearchProfileViewModel(this, _luceneInterfaceProfile);
            LayoutProfile = new LayoutProfileViewModel(this);
            References = new SelectedReferencesViewModel(this);
            SearchReferenceConfiguration = new SearchReferenceConfigurationViewModel(SearchReference);
            SearchProfileConfiguration = new SearchProfileConfigurationViewModel(SearchProfile);
            SelectedReferencesConfiguration = new SelectedReferencesConfigurationViewModel(References);
            SavedData = new SavedDataViewModel(this);
            ProgressBar = new ProgressBarViewModel(this);
            ProgressBar.changeStepList("Profile");

            //Starting Views
            _contentViewModel = Start;
            ContentViewModel = Start;
            _stepBar = null;

            
        }



        public void createSlide() {
            PowerpointSlideCreator powerpointSlideCreator = new PowerpointSlideCreator();
            powerpointSlideCreator.addReferences(References.getReferenceList());
            powerpointSlideCreator.addLayoutModel(LayoutReference.Layouts[LayoutReference._layoutIndex]);
            powerpointSlideCreator.addLanguage(References.getSelectedLanguage());
            powerpointSlideCreator.createSlide();
        }

        public void addReference(ReferenceModel model) {
            if(ContentViewModel == SearchProfile) {
                
            } else if(ContentViewModel == SearchReference) {
                References.addReference(model);
            }
        }

        public void removeReference(ReferenceModel model) {
            if (ContentViewModel == SearchProfile) {

            } else if (ContentViewModel == SearchReference) {
                References.removeReference(model);
            }
        }

        public void ChangePath(string path) {
            ContentViewModel = path switch {
                "Profile" => LayoutProfile,
                "Reference" => LayoutReference,
                "Settings" => SearchReferenceConfiguration,
                "Start" => Start,
                _ => Start
            };
            ProgressBar.changeStepList(path);
            StepBar = path switch {
                "Start" => null,
                _ => ProgressBar
            };
        }

        public void changeStep(string type) {
            ContentViewModel = type switch {
                "LayoutProfile" => LayoutProfile,
                "LayoutReferences"=>LayoutReference,
                "SearchProfile"=> SearchProfile,
                "SearchReferences" => SearchReference,
                "SummaryProfile" => References,
                "SummaryReferences" => References,
                "SearchReferenceConfiguration" => SearchReferenceConfiguration,
                "SearchProfileConfiguration" => SearchProfileConfiguration,
                "SummaryConfiguration" => SelectedReferencesConfiguration,
                "SavedData" => SavedData,
                _ => Start
            };
        }

        public void changeLayout(LayoutModel model) {
            if (ContentViewModel == LayoutProfile) {

            } else if (ContentViewModel == LayoutReference) {
                SearchReference.maxReferences = model.maxElements;
            }
            References.SelectedLayout = model.name;
            
        }

        public void prev() {
            ProgressBar.prev();
        }

        public void next() {
            ProgressBar.next();
        }

        public void refreshTemplate() {
            LayoutReference.prepareTemplate();
            LayoutProfile.prepareTemplate();
        }

        public void refreshSearch() {
            _luceneInterfaceProfile.refreshIndex();
            _luceneInterfaceReference.refreshIndex();
        }
    }
}
