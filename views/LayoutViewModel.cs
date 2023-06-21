using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReferenceConfigurator.utils;

namespace ReferenceConfigurator.views {
    public class LayoutViewModel : MainContentViewModel {

        protected ObservableCollection<LayoutModel> _layouts;

        public ObservableCollection<LayoutModel> Layouts {
            get => _layouts;
            set => SetProperty(ref _layouts, value);

        }

        public ICommand SelectLayoutCommand { get; }

        public int _layoutIndex =0;

        private PopUpViewModel parent;

        public ICommand NextCommand { get; }

        public ICommand PrevCommand { get; }

        public LayoutViewModel(PopUpViewModel parent) {
            _layouts = new ObservableCollection<LayoutModel>();
            Layouts = _layouts;

            SelectLayoutCommand = new RelayCommand<string>(SelectLayout);
            NextCommand = new RelayCommand(next);
            PrevCommand = new RelayCommand(prev);
            this.parent = parent;
        }

        public virtual void prepareTemplate() { }


        protected void SelectLayout(string layoutPath) {
            for(int i = 0; i < _layouts.Count; i++) {
                LayoutModel layout = _layouts[i];
                if(layout.imagePath== layoutPath) {
                    _layoutIndex= i;
                    parent.changeLayout(layout.name);
                    break;
                }
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
