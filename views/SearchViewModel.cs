﻿using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using HandyControl.Data;
using System.Windows.Controls;
using ReferenceConfigurator.lucene;

namespace ReferenceConfigurator.views {
    public class SearchViewModel : MainContentViewModel {

        //private SearchModel _searchModel;

        //public SearchModel SearchModel {
        //    get => _searchModel;
        //    set => SetProperty(ref _searchModel, value);
        //}

        //public ICommand SearchChangedCommand { get; }

        //public SearchViewModel() {
        //    //_searchModel = new SearchModel();
        //    //SearchModel = _searchModel;
        //    //SearchChangedCommand = new RelayCommand<FunctionEventArgs<object>>(SearchChanged);
        //}

        //private void SearchChanged(FunctionEventArgs<object> info) {
        //    System.Diagnostics.Debug.WriteLine("Teszt");
        //}


        public ICommand SearchChangedCommand { get; }

        private ObservableCollection<ReferenceModel> _searchResult;

        public ObservableCollection<ReferenceModel> SearchResult {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        private ObservableCollection<CheckBoxModel> _columnList;

        public ObservableCollection<CheckBoxModel> ColumnList {
            get => _columnList;
            set { SetProperty(ref _columnList, value);
                System.Diagnostics.Debug.WriteLine(value.ToString());
                VisibleTest = !VisibleTest;
            }
        }

        private bool _visibleTest = true;

        public bool VisibleTest {
            get => _visibleTest;
            set => SetProperty(ref _visibleTest, value);
        }

        public ICommand SelectionChangedCommand { get; }

        private PopUpViewModel parent;

        private LuceneInterface _luceneInterface;

        public SearchViewModel(PopUpViewModel parent, LuceneInterface luceneInterface) {
            this.parent = parent;
            _luceneInterface = luceneInterface;
            SearchChangedCommand = new RelayCommand<string>(searchChanged);
            _searchResult = new ObservableCollection<ReferenceModel>();
            SearchResult = _searchResult;
            _columnList = new ObservableCollection<CheckBoxModel>();
            ColumnList = _columnList;
            populate();
            SelectionChangedCommand = new RelayCommand<EventArgs>(SelectionChanged);
            VisibleTest = _visibleTest;
        }

        private void populate() {
            ColumnList.Clear();
            ColumnList.Add(new CheckBoxModel() { Name = "ProjectId", IsChecked = true });
            ColumnList.Add(new CheckBoxModel() { Name = "ProjectName", IsChecked = true });
            ColumnList.Add(new CheckBoxModel() { Name="Team", IsChecked=false });
        }

        private void searchChanged(string search) { 
            SearchResult.Clear();
            System.Diagnostics.Debug.WriteLine(search);
            foreach (ReferenceModel model in _luceneInterface.getModelByGeneralSearch(search)) {
                SearchResult.Add(model);
            }
           
        }

        private void SelectionChanged(EventArgs args) {
            SelectionChangedEventArgs selected = args as SelectionChangedEventArgs;
            foreach (ReferenceModel model in selected.AddedItems) {
                parent.addReference(model);
            }
        }

    }
}
