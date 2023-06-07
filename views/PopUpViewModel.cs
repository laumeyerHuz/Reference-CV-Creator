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

namespace ReferenceConfigurator.views {
    public class PopUpViewModel : ViewModelBase {

        private MainContentViewModel _mainContentViewModel;

        public MainContentViewModel MainContentViewModel {
            get => _mainContentViewModel;
            set => SetProperty(ref _mainContentViewModel, value);
        }

        private SelectedContentViewModel _selectedContentViewModel;

        public SelectedContentViewModel SelectedContentViewModel {
            get => _selectedContentViewModel;
            set => SetProperty(ref _selectedContentViewModel, value);
        }

        public ICommand SwitchItemCommand { get; }

        public ICommand SelectItemCommand { get; }


        private readonly LuceneInterface _luceneInterface;

        private readonly StartViewModel StartViewModel;
        private readonly SearchReferenceViewModel SearchReference;
        private readonly LayoutReferenceViewModel LayoutReference;
        private readonly SearchProfileViewModel SearchProfile;
        private readonly LayoutProfileViewModel LayoutProfile;
        private readonly SelectedReferencesViewModel References;
        private readonly SearchConfigurationViewModel SearchConfiguration;
        private readonly SelectedReferencesConfigurationViewModel SelectedReferencesConfiguration;


        public PopUpViewModel() {
            //init Commands
            SwitchItemCommand = new RelayCommand(SwitchItemCmd);
            SelectItemCommand = new RelayCommand<string>(SelectItem);
            _luceneInterface = new LuceneInterface();


            //Init Views
            StartViewModel = new StartViewModel();
            SearchReference = new SearchReferenceViewModel(this, _luceneInterface);
            LayoutReference = new LayoutReferenceViewModel(this);
            SearchProfile = new SearchProfileViewModel(this, _luceneInterface);
            LayoutProfile = new LayoutProfileViewModel(this);
            References = new SelectedReferencesViewModel(this);
            SearchConfiguration = new SearchConfigurationViewModel(SearchReference);
            SelectedReferencesConfiguration = new SelectedReferencesConfigurationViewModel(References);

            //Starting Views
            _mainContentViewModel = StartViewModel;
            MainContentViewModel = StartViewModel;
            _selectedContentViewModel = References;
            SelectedContentViewModel = References;

            
        }

        private void SwitchItemCmd() {
            
        }

        private void SelectItem(string? view) {
            MainContentViewModel = view switch {
                "Search" => SearchReference,
                "Layout" => LayoutReference,
                "Search Configuration" => SearchConfiguration,
                "Selected References Configuration" => SelectedReferencesConfiguration,
                "Layout Profile"=> LayoutProfile,
                "Search Profile"=> SearchProfile,
                _ => StartViewModel,
            };
        }

        public void createSlide() {
            PowerpointSlideCreator powerpointSlideCreator = new PowerpointSlideCreator();
            powerpointSlideCreator.addReferences(SelectedContentViewModel.getReferenceList());
            powerpointSlideCreator.addLayoutModel(LayoutReference.Layouts[LayoutReference._layoutIndex]);
            powerpointSlideCreator.addLanguage(References.getSelectedLanguage());
            powerpointSlideCreator.createSlide();
        }

        public void addReference(ReferenceModel model) {
            SelectedContentViewModel.addReference(model);
        }

        public void changeLayout(string name) {
            References.SelectedLayout = name;
        }
    }
}
