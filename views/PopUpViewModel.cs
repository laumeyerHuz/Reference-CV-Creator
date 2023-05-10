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
        private readonly SearchViewModel Search;
        private readonly LayoutViewModel Layout;
        private readonly SelectedReferencesViewModel References;
        private readonly SearchConfigurationViewModel SearchConfiguration;
        private readonly SelectedReferencesConfigurationViewModel SelectedReferencesConfiguration;


        public PopUpViewModel() {
            //init Commands
            SwitchItemCommand = new RelayCommand<string>(SwitchItemCmd);
            SelectItemCommand = new RelayCommand<string>(SelectItem);
            _luceneInterface = new LuceneInterface();


            //Init Views
            StartViewModel = new StartViewModel();
            Search = new SearchViewModel(this, _luceneInterface);
            Layout = new LayoutViewModel(this);
            References = new SelectedReferencesViewModel(this);
            SearchConfiguration = new SearchConfigurationViewModel(Search);
            SelectedReferencesConfiguration = new SelectedReferencesConfigurationViewModel(References);

            //Starting Views
            _mainContentViewModel = StartViewModel;
            MainContentViewModel = StartViewModel;
            _selectedContentViewModel = References;
            SelectedContentViewModel = References;

            
        }

        private void SwitchItemCmd(string? view) {
            switch (view) {
                case "Reference":
                    MainContentViewModel = Search;
                    SelectedContentViewModel = References;
                    break;
                //case "Profile":
                //    MainContentViewModel = SearchProfile;
                //    SelectedContentViewModel = Profiles;
                //    break;
                default:
                    break;
            }
        }

        private void SelectItem(string? view) {
            MainContentViewModel = view switch {
                "Search" => Search,
                "Layout" => Layout,
                "Search Configuration" => SearchConfiguration,
                "Selected References Configuration" => SelectedReferencesConfiguration,
                "Layout Profile"=> Layout,
                _ => StartViewModel,
            };
        }

        public void createSlide() {
            PowerpointSlideCreator powerpointSlideCreator = new PowerpointSlideCreator();
            powerpointSlideCreator.addReferences(SelectedContentViewModel.getReferenceList());
            powerpointSlideCreator.addLayoutModel(Layout.Layouts[Layout._layoutIndex]);
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
