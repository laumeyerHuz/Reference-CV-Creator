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

        protected ObservableCollection<ReferenceModel> _selectedItems = new ObservableCollection<ReferenceModel>();

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

        protected LuceneInterfaceReference _luceneInterface;

        public int maxReferences;

        public SearchViewModel(PopUpViewModel parent, LuceneInterfaceReference luceneInterface) {
            this.parent = parent;
            _luceneInterface = luceneInterface;

            IList<ReferenceModel> search = new List<ReferenceModel>();
            _searchResult = CollectionViewSource.GetDefaultView(search);
            SearchResult = _searchResult;
            SearchResult.SortDescriptions.Add(
               new SortDescription("ProjectId", ListSortDirection.Descending));

            _selectedList = CollectionViewSource.GetDefaultView(_selectedItems);
            SelectedList = _selectedList;

            _columnList = new ObservableCollection<CheckBoxModel>();
            ColumnList = _columnList;

            SearchChangedCommand = new RelayCommand<string>(searchChanged);
            SelectionChangedCommand = new RelayCommand<ReferenceModel>(SelectionChanged);
            NextCommand = new RelayCommand(next);
            PrevCommand = new RelayCommand(prev);
            AddSelectionCommand = new RelayCommand<ReferenceModel>(AddSelection);
            RemoveSelectionCommand = new RelayCommand<ReferenceModel>(RemoveSelection);
            maxReferences = -1;

        }

        protected virtual void searchChanged(string search) {
            List<ReferenceModel> _searchResults = _luceneInterface.getModelByGeneralSearch(search);
            if (_searchResults.Count == 0) {
                Growl.Info("No result has been found");
                return;
            }
            IList<ReferenceModel> _search = _searchResults;
            SearchResult = CollectionViewSource.GetDefaultView(_search);
           
        }

        protected virtual void SelectionChanged(ReferenceModel selected) {
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

        protected virtual void AddSelection(ReferenceModel selected) {
            if (selected != null) {
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
                SelectedList = CollectionViewSource.GetDefaultView(_selectedItems);
            }
        }

        protected virtual void RemoveSelection(ReferenceModel selected) {
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
