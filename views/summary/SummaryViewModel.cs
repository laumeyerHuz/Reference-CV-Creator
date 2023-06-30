using ReferenceConfigurator.models;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using HandyControl.Data;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using HandyControl.Controls;
using System.Windows.Data;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ReferenceConfigurator.views {
    public class SummaryViewModel : MainContentViewModel {
        PopUpViewModel parent;

        private ICollectionView _selectedReferences;

        public ICollectionView SelectedReferences {
            get => _selectedReferences;
            set => SetProperty(ref _selectedReferences, value);
        }

        private string _selectedLayout;

        public string SelectedLayout {
            get => _selectedLayout;
            set => SetProperty(ref _selectedLayout, value);
        }

        public ICommand SelectionChangedCommand { get; }

        public ICommand CreateSlideCommand { get; }

        public ICommand NextCommand { get; }

        public ICommand PrevCommand { get; }

        private ObservableCollection<CheckBoxModel> _columnList;

        public ObservableCollection<CheckBoxModel> ColumnList {
            get => _columnList;
            set {
                SetProperty(ref _columnList, value);
            }
        }

        private ObservableCollection<string> _languageList;

        public ObservableCollection<string> LanguageList {
            get => _languageList;
            set {
                SetProperty(ref _languageList, value);
            }
        }

        private string _selectedLanguage;

        public string SelectedLanguage {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        private List<SearchModel> _references;

        public SummaryViewModel(PopUpViewModel parent) {
            this.parent = parent;

            SelectionChangedCommand = new RelayCommand<ReferenceModel>(SelectionChanged);
            CreateSlideCommand = new RelayCommand(createSlide);
            NextCommand = new RelayCommand(next);
            PrevCommand = new RelayCommand(prev);
            _references = new List<SearchModel>();
            _selectedReferences = CollectionViewSource.GetDefaultView(_references);
            SelectedReferences = _selectedReferences;
            _columnList = new ObservableCollection<CheckBoxModel>();
            ColumnList = _columnList;
            _languageList = new ObservableCollection<string>();
            LanguageList = _languageList;
            LanguageList.Add("DE");
            LanguageList.Add("EN");


            SelectedLayout = "No Layout Selected";
            SelectedLanguage = "EN";
        }

        public void addReference(SearchModel reference) {

            _references.Add(reference);
            SelectedReferences = CollectionViewSource.GetDefaultView(_references);
            SelectedReferences.Refresh();
        }

        public void removeReference(SearchModel reference) {
            _references.Remove(reference);
            SelectedReferences = CollectionViewSource.GetDefaultView(_references);
            SelectedReferences.Refresh();
        }

        private void createSlide() {
            if(_references.Count < 0) {
                Growl.Error("No reference slected to be added");
            }
            parent.createSlide();
            //Growl.Success("Slide created scuccessfully");
            System.Windows.Application.Current.Windows[0].Close();
        }

        public List<SearchModel> getReferenceList() {
            return _references;
        }

        private void SelectionChanged(SearchModel selected) {
            if(selected != null) {
                removeReference(selected);
            }         
        }

        public string getSelectedLanguage() {
            return SelectedLanguage;
        }

        public void next() {
            parent.next();
        }

        public void prev() {
            parent.prev();
        }
    }
}
