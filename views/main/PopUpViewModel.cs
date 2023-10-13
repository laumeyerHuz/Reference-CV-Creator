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
using System.Collections.Generic;
using System.Linq;
using ReferenceConfigurator.views;
using System.Threading.Tasks;

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
        private readonly LuceneInterfaceLogo _luceneInterfaceLogo;

        private readonly StartViewModel Start;
        private readonly SearchReferenceViewModel SearchReference;
        private readonly LayoutReferenceViewModel LayoutReference;
        private readonly SearchProfileViewModel SearchProfile;
        private readonly LayoutProfileViewModel LayoutProfile;
        private readonly SummaryViewModel SummaryReferences;
        private readonly ReferenceConfigurationViewModel ReferenceConfiguration;
        private readonly SummaryViewModel SummaryProfile;
        private readonly ProfileConfigurationViewModel ProfileConfiguration;
        private readonly ProgressBarViewModel ProgressBar;
        private readonly SavedDataViewModel SavedData;
        private readonly SearchLogoViewModel SearchLogo;

        public PopUpViewModel() {
            _luceneInterfaceReference = new LuceneInterfaceReference();
            _luceneInterfaceProfile = new LuceneInterfaceProfile();
            _luceneInterfaceLogo = new LuceneInterfaceLogo();

            //Init Views
            Start = new StartViewModel(this);
            SearchReference = new SearchReferenceViewModel(this, _luceneInterfaceReference);
            SearchProfile = new SearchProfileViewModel(this, _luceneInterfaceProfile);
            
            SummaryReferences = new SummaryReferenceViewModel(this);
            SummaryProfile = new SummaryProfileViewModel(this);
            ReferenceConfiguration = new ReferenceConfigurationViewModel(SearchReference, SummaryReferences);
            ProfileConfiguration = new ProfileConfigurationViewModel(SearchProfile, SummaryProfile);
            SavedData = new SavedDataViewModel(this);
            ProgressBar = new ProgressBarViewModel(this);
            SearchLogo = new SearchLogoViewModel(this,_luceneInterfaceLogo);
            LayoutProfile = new LayoutProfileViewModel(this);
            LayoutReference = new LayoutReferenceViewModel(this);
            ProgressBar.changeStepList("Profile");

            //Starting Views
            _contentViewModel = Start;
            ContentViewModel = Start;
            _stepBar = null;

           

        }



        public void createSlide() {
            if (ContentViewModel == SummaryProfile) {
                PowerpointSlideCreatorProfile powerpointSlideCreator = new PowerpointSlideCreatorProfile();
                powerpointSlideCreator.addReferences(SummaryProfile.getReferenceList().Cast<ProfileModel>().ToList());
                powerpointSlideCreator.addLayoutModel((ProfileLayoutModel)LayoutProfile.Layouts[LayoutProfile._layoutIndex]);
                powerpointSlideCreator.addLanguage(SummaryProfile.getSelectedLanguage());
                powerpointSlideCreator.createSlide();

            } else if (ContentViewModel == SummaryReferences) {
                PowerpointSlideCreatorReference powerpointSlideCreator = new PowerpointSlideCreatorReference();
                powerpointSlideCreator.addReferences(SummaryReferences.getReferenceList().Cast<ReferenceModel>().ToList());
                powerpointSlideCreator.addLayoutModel(LayoutReference.Layouts[LayoutReference._layoutIndex]);
                powerpointSlideCreator.addLanguage(SummaryReferences.getSelectedLanguage());
                powerpointSlideCreator.createSlide();
            } else if(ContentViewModel == SearchLogo) {
                PowerpointSlideCreatorLogo powerpointSlideCreator = new PowerpointSlideCreatorLogo();
                powerpointSlideCreator.addReferences(SearchLogo.getSelected().Cast<LogoModel>().ToList());
                powerpointSlideCreator.addLogo();
                System.Windows.Application.Current.Windows[0].Close();
            }
           
        }

        public void addReference(SearchModel model) {
            if(ContentViewModel == SearchProfile) {
                SummaryProfile.addReference(model);
            } else if(ContentViewModel == SearchReference) {
                SummaryReferences.addReference(model);
            }
        }

        public void removeReference(SearchModel model) {
            if (ContentViewModel == SearchProfile) {
                SummaryProfile.removeReference(model);
            } else if (ContentViewModel == SearchReference) {
                SummaryReferences.removeReference(model);
            }
        }

        public void ChangePath(string path) {
            ContentViewModel = path switch {
                "Profile" => LayoutProfile,
                "Reference" => LayoutReference,
                "Settings" => SavedData,
                "Start" => Start,
                "Logo" => SearchLogo,
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
                "SummaryProfile" => SummaryProfile,
                "SummaryReferences" => SummaryReferences,
                "ReferenceConfiguration" => ReferenceConfiguration,
                "ProfileConfiguration" => ProfileConfiguration,
                "SavedData" => SavedData,
                "SearchLogo" => SearchLogo,
                _ => Start
            };
        }

        public void changeLayout(LayoutModel model) {
            if (ContentViewModel == LayoutProfile) {
                SearchProfile.maxReferences = model.maxElements;
                SummaryProfile.SelectedLayout = model.name;
            } else if (ContentViewModel == LayoutReference) {
                SearchReference.maxReferences = model.maxElements;
                SummaryReferences.SelectedLayout = model.name;
            } 
        }

        public void prev() {
            ProgressBar.prev();
        }

        public void next() {
            ProgressBar.next();
        }

        public Task refreshTemplateReference() {
           return LayoutReference.prepareTemplate();
        }

        public Task refreshTemplateProfile() {
            return LayoutProfile.prepareTemplate();
        }

        public void refreshSearch() {
            _luceneInterfaceProfile.refreshIndex();
            _luceneInterfaceReference.refreshIndex();
            _luceneInterfaceLogo.refreshIndex();
        }
    }
}
