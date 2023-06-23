﻿using HandyControl.Controls;
using ReferenceConfigurator.lucene;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReferenceConfigurator.views {
    public class SearchProfileViewModel : SearchViewModel {
        public SearchProfileViewModel(PopUpViewModel parent, LuceneInterfaceReference luceneInterface) : base(parent, luceneInterface) {
        }

        protected override void searchChanged(string search) {
            List<ReferenceModel> _searchResults = _luceneInterface.getModelByGeneralSearch(search);
            if (_searchResults.Count == 0) {
                Growl.Info("No result has been found");
                return;
            }
            IList<ReferenceModel> _search = _searchResults;
            SearchResult = CollectionViewSource.GetDefaultView(_search);
        }
    }
}
