﻿using HandyControl.Controls;
using ReferenceConfigurator.lucene;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReferenceConfigurator.views {
    public class SearchReferenceViewModel : SearchViewModel {
        public SearchReferenceViewModel(PopUpViewModel parent, LuceneInterfaceReference luceneInterface) : base(parent, luceneInterface) {
        }

        protected override void searchChanged(string search) {
            string pattern = "([-+\"~*?:\\/])";
            string replacement = "\\$1";

            Regex rgx = new Regex(pattern);
            search = rgx.Replace(search, replacement);
            List<SearchModel> _searchResults = _luceneInterface.getModelByGeneralSearch(search);
            if (_searchResults.Count == 0) {
                Growl.Info("No result has been found");
                return;
            }
            IList<SearchModel> _search = _searchResults;
            SearchResult = CollectionViewSource.GetDefaultView(_search);
        }

    }
}
