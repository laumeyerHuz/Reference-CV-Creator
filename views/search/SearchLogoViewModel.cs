using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using ReferenceConfigurator.lucene;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        public ObservableCollection<SearchModel> getSelected() {
            return _selectedItems;
        }
    }
}
