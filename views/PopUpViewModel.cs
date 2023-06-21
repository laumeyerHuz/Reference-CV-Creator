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


        private readonly LuceneInterface _luceneInterface;

        private readonly StartViewModel Start;
        private readonly SearchReferenceViewModel SearchReference;
        private readonly LayoutReferenceViewModel LayoutReference;
        private readonly SearchProfileViewModel SearchProfile;
        private readonly LayoutProfileViewModel LayoutProfile;
        private readonly SelectedReferencesViewModel References;
        private readonly SearchConfigurationViewModel SearchConfiguration;
        private readonly SelectedReferencesConfigurationViewModel SelectedReferencesConfiguration;
        private readonly ProgressBarViewModel ProgressBar;
        private readonly SavedDataViewModel SavedData;

        public PopUpViewModel() {
            _luceneInterface = new LuceneInterface();


            //Init Views
            Start = new StartViewModel(this);
            SearchReference = new SearchReferenceViewModel(this, _luceneInterface);
            LayoutReference = new LayoutReferenceViewModel(this);
            SearchProfile = new SearchProfileViewModel(this, _luceneInterface);
            LayoutProfile = new LayoutProfileViewModel(this);
            References = new SelectedReferencesViewModel(this);
            SearchConfiguration = new SearchConfigurationViewModel(SearchReference);
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
                "Settings" => SearchConfiguration,
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
                "SearchConfiguration" => SearchConfiguration,
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
            _luceneInterface.refreshIndex();
        }
    }
}
