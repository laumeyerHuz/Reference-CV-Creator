#nullable enable

using System;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ReferenceConfigurator.models;
using ReferenceConfigurator.lucene;
using ReferenceConfigurator.powerpointSlideCreator;

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

        private readonly LanguageSelectionViewModel Language;
        private readonly SearchViewModel Search;
        private readonly LayoutViewModel Layout;
        private readonly SelectedReferencesViewModel References;


        public PopUpViewModel() {
            //init Commands
            SwitchItemCommand = new RelayCommand(SwitchItemCmd);
            SelectItemCommand = new RelayCommand<String>(SelectItem);
            _luceneInterface = new LuceneInterface();


            //Init Views
            Language = new LanguageSelectionViewModel();
            Search = new SearchViewModel(this, _luceneInterface);
            Layout = new LayoutViewModel();
            References = new SelectedReferencesViewModel(this);

            //Starting Views
            _mainContentViewModel = Language;
            MainContentViewModel = Language;
            _selectedContentViewModel = References;
            SelectedContentViewModel = References;
        }

        private void SwitchItemCmd() {
        }

        private void SelectItem(string? view) {
            MainContentViewModel = view switch {
                "Language" => Language,
                "Search" => Search,
                "Layout" => Layout,
                _ => Language,
            };
        }

        public void createSlide() {
            PowerpointSlideCreator powerpointSlideCreator = new PowerpointSlideCreator();
            powerpointSlideCreator.addReferences(SelectedContentViewModel.getReferenceList());
            powerpointSlideCreator.addLayoutModel(Layout.Layouts[Layout._layoutIndex]);
            powerpointSlideCreator.createSlide();
        }

        public void addReference(ReferenceModel model) {
            SelectedContentViewModel.addReference(model);
        }
    }
}
