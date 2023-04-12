using ReferenceConfigurator.models;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using HandyControl.Data;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using HandyControl.Controls;

namespace ReferenceConfigurator.views {
    public class SelectedReferencesViewModel : SelectedContentViewModel {
        PopUpViewModel parent;

        private ObservableCollection<ReferenceModel> _selectedReferences;

        public ObservableCollection<ReferenceModel> SelectedReferences {
            get => _selectedReferences;
            set => SetProperty(ref _selectedReferences, value);
        }

        public ICommand SelectionChangedCommand { get; }

        public ICommand CreateSlideCommand { get; }

        public SelectedReferencesViewModel(PopUpViewModel parent) {
            this.parent = parent;
            SelectionChangedCommand = new RelayCommand<EventArgs>(SelectionChanged);
            CreateSlideCommand = new RelayCommand(createSlide);
            _selectedReferences = new ObservableCollection<ReferenceModel>();
            SelectedReferences = _selectedReferences;
        }

        public override void addReference(ReferenceModel reference) {
            if (SelectedReferences.Any(p => p.ProjectId == reference.ProjectId)) {
                Growl.Error("Reference already selected");
                return;
            }
                SelectedReferences.Add(reference);
        }

        public override void removeReference(ReferenceModel reference) {
            ObservableCollection<ReferenceModel> SelectedReferencesCopy = new ObservableCollection<ReferenceModel>(SelectedReferences);
            SelectedReferencesCopy.Remove(reference);
            SelectedReferences = SelectedReferencesCopy;
        }

        private void createSlide() {
            if(SelectedReferences.Count < 0) {
                Growl.Error("No reference slected to be added");
            }
            parent.createSlide();
            Growl.Success("Slide created scuccessfully");
        }

        public override List<ReferenceModel> getReferenceList() {
            return SelectedReferences.ToList();
        }

        private void SelectionChanged(EventArgs args) {
            SelectionChangedEventArgs selected = args as SelectionChangedEventArgs;
            foreach (ReferenceModel model in selected.AddedItems) {
                removeReference(model);
            }           
        }

    }
}
