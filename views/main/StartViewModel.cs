using CommunityToolkit.Mvvm.Input;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReferenceConfigurator.views {

    public class StartViewModel : MainContentViewModel {

        private ObservableCollection<CardModel> _selectionList;

        public ObservableCollection<CardModel> SelectionList {
            get => _selectionList;
            set => SetProperty(ref _selectionList, value);
        }

        public ICommand SelectCardCommand { get; set;}

        private PopUpViewModel parent;

        public StartViewModel(PopUpViewModel popUpViewModel) {
            _selectionList = new ObservableCollection<CardModel>();
            SelectCardCommand = new RelayCommand<string>(selectCard);
            populate();
            parent = popUpViewModel;
        }

        private void selectCard(string path) {
            parent.ChangePath(path);

        }

        private void populate() {
            SelectionList = new ObservableCollection<CardModel>() {
                new CardModel(
                    "Profile",
                    "pack://application:,,,/ReferenceConfigurator;component/icons/CV.png"
                    ),
                new CardModel(
                    "Reference",
                    "pack://application:,,,/ReferenceConfigurator;component/icons/Reference.png"
                    ),
                new CardModel(
                    "Logo",
                    "pack://application:,,,/ReferenceConfigurator;component/icons/Logo.png"
                    ),
                new CardModel(
                    "Settings",
                    "pack://application:,,,/ReferenceConfigurator;component/icons/Settings.png"
                    )
            };
        }
    }
}
