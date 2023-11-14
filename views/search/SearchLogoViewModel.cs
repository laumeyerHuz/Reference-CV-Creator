using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using ReferenceConfigurator.lucene;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ReferenceConfigurator.views {
    public class SearchLogoViewModel : SearchViewModel {

        List<SearchModel> selectedModels;

        public ICommand AddToSlideCommand { get; set; }
        public SearchLogoViewModel(PopUpViewModel parent, LuceneInterface luceneInterface) : base(parent, luceneInterface) {
            AddToSlideCommand = new RelayCommand(addToSlide);
        }

        public void addToSlide() {
            parent.createSlide();
        }

        protected override void searchChanged(string search) {
            search = search.Replace("/", "_");
            search = search.Replace(":", "_");

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

        public ObservableCollection<SearchModel> getSelected() {
            return _selectedItems;
        }
    }
}
