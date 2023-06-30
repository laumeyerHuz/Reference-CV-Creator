using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using HandyControl.Data;
using System.Windows.Controls;
using ReferenceConfigurator.lucene;
using HandyControl.Controls;
using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;

namespace ReferenceConfigurator.views {
    public class SearchViewModel : MainContentViewModel {

        protected ICollectionView _searchResult;

        public ICollectionView SearchResult {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        protected ObservableCollection<SearchModel> _selectedItems = new ObservableCollection<SearchModel>();

        protected ICollectionView _selectedList;

        public ICollectionView SelectedList {
            get => _selectedList;
            set => SetProperty(ref _selectedList, value);
        }

        protected ObservableCollection<CheckBoxModel> _columnList;

        public ObservableCollection<CheckBoxModel> ColumnList {
            get => _columnList;
            set {
                SetProperty(ref _columnList, value);
            }
        }

        public ICommand SearchChangedCommand { get; }

        public ICommand SelectionChangedCommand { get; }

        public ICommand NextCommand { get; }

        public ICommand PrevCommand { get; }

        public ICommand SelectItemCommand { get; }

        public ICommand AddSelectionCommand { get; }

        public ICommand RemoveSelectionCommand { get; }

        public ICommand RemoveItemCommand { get; }

        protected PopUpViewModel parent;

        protected LuceneInterface _luceneInterface;

        public int maxReferences;

        public SearchViewModel(PopUpViewModel parent, LuceneInterface luceneInterface) {
            this.parent = parent;
            _luceneInterface = luceneInterface;

            IList<SearchModel> search = new List<SearchModel>();
            _searchResult = CollectionViewSource.GetDefaultView(search);
            SearchResult = _searchResult;

            _selectedList = CollectionViewSource.GetDefaultView(_selectedItems);
            SelectedList = _selectedList;

            _columnList = new ObservableCollection<CheckBoxModel>();
            ColumnList = _columnList;

            SearchChangedCommand = new RelayCommand<string>(searchChanged);
            SelectionChangedCommand = new RelayCommand<SearchModel>(SelectionChanged);
            NextCommand = new RelayCommand(next);
            PrevCommand = new RelayCommand(prev);
            AddSelectionCommand = new RelayCommand<SearchModel>(AddSelection);
            RemoveSelectionCommand = new RelayCommand<SearchModel>(RemoveSelection);
            maxReferences = -1;

        }

        protected virtual void searchChanged(string search) {
            List<SearchModel> _searchResults = _luceneInterface.getModelByGeneralSearch(search);
            if (_searchResults.Count == 0) {
                Growl.Info("No result has been found");
                return;
            }
            IList<SearchModel> _search = _searchResults;
            SearchResult = CollectionViewSource.GetDefaultView(_search);
           
        }

        protected virtual void SelectionChanged(SearchModel selected) {
            if (selected != null) {
                if (_selectedItems.Contains(selected)){
                    _selectedItems.Remove(selected);
                    parent.removeReference(selected);
                } else {
                    if(maxReferences > 0) {
                        if(_selectedItems.Count < maxReferences) {
                            _selectedItems.Add(selected);
                            parent.addReference(selected);
                        } else {
                            Growl.Error("Too many items selcted remove one or change to a bigger layout");
                        }
                    } else {
                        _selectedItems.Add(selected);
                        parent.addReference(selected);
                    }
                    
                }
                SelectedList = CollectionViewSource.GetDefaultView(_selectedItems);
            }
        }

        protected virtual void AddSelection(SearchModel selected) {
            if (selected != null) {
                if (_selectedItems.Contains(selected)) {
                    if (maxReferences > 0) {
                        if (_selectedItems.Count < maxReferences) {
                            _selectedItems.Add(selected);
                            parent.addReference(selected);
                        } else {
                            Growl.Error("Too many items selected remove one or change to a bigger layout");
                        }
                    } else {
                        _selectedItems.Add(selected);
                        parent.addReference(selected);
                    }
                } else {
                    Growl.Error("Element already selected");
                }
                SelectedList = CollectionViewSource.GetDefaultView(_selectedItems);
            }
        }

        protected virtual void RemoveSelection(SearchModel selected) {
            if (selected != null) {
                _selectedItems.Remove(selected);
                parent.removeReference(selected);
                SelectedList = CollectionViewSource.GetDefaultView(_selectedItems);
            }
        }

        public void prev() {
            parent.prev();
        }

        public void next() {
            parent.next();
        }

    }
}
