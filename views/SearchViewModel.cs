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

        public ICommand SearchChangedCommand { get; }

        protected ICollectionView _searchResult;

        public ICollectionView SearchResult {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        protected ObservableCollection<CheckBoxModel> _columnList;

        public ObservableCollection<CheckBoxModel> ColumnList {
            get => _columnList;
            set { SetProperty(ref _columnList, value);
            }
        }

        public ICommand SelectionChangedCommand { get; }

        protected PopUpViewModel parent;

        protected LuceneInterface _luceneInterface;

        public SearchViewModel(PopUpViewModel parent, LuceneInterface luceneInterface) {
            this.parent = parent;
            _luceneInterface = luceneInterface;
            SearchChangedCommand = new RelayCommand<string>(searchChanged);
            IList<ReferenceModel> search = new List<ReferenceModel>();
            _searchResult = CollectionViewSource.GetDefaultView(search);
            SearchResult = _searchResult;
            _columnList = new ObservableCollection<CheckBoxModel>();
            ColumnList = _columnList;
            SelectionChangedCommand = new RelayCommand<ReferenceModel>(SelectionChanged);
        }

        protected virtual void searchChanged(string search) { 
            System.Diagnostics.Debug.WriteLine(search);
            List<ReferenceModel> _searchResults = _luceneInterface.getModelByGeneralSearch(search);
            if(_searchResults.Count == 0) {
                Growl.Info("No result has been found");
                return;
            }
            IList<ReferenceModel> _search = _searchResults;
            SearchResult = CollectionViewSource.GetDefaultView(_search);
        }

        protected void SelectionChanged(ReferenceModel selected) {
            if(selected != null) { 
                parent.addReference(selected);
            }
        }

    }
}
