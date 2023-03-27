using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReferenceConfigurator.utils;

namespace ReferenceConfigurator.views {
    public class LayoutViewModel : MainContentViewModel {

        private ObservableCollection<LayoutModel> _layouts;

        public ObservableCollection<LayoutModel> Layouts {
            get => _layouts;
            set => SetProperty(ref _layouts, value);

        }

        public ICommand SelectLayoutCommand { get; }

        public int _layoutIndex =0;

        public LayoutViewModel() { 
            _layouts= new ObservableCollection<LayoutModel>();
            Layouts = _layouts;

            SelectLayoutCommand = new RelayCommand<string>(SelectLayout);
            prepareTemplate();
        }

        private void prepareTemplate() {
            Utils.downloadPowerpointTemplate();
            Layouts = new ObservableCollection<LayoutModel>( Utils.SlidesToImage());
        }


        private void SelectLayout(string layoutPath) {
            for(int i = 0; i < _layouts.Count; i++) {
                LayoutModel layout = _layouts[i];
                if(layout.imagePath== layoutPath) {
                    _layoutIndex= i;
                    break;
                }
            }           
        }
    }
}
