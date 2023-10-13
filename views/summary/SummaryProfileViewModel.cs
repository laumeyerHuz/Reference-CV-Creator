using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.views {
    public class SummaryProfileViewModel : SummaryViewModel {

        public override string SelectedLanguage {
            get => _selectedLanguage;
            set {
                SetProperty(ref _selectedLanguage, value);
                changeLanguage(value);
            }
        }

        public SummaryProfileViewModel(PopUpViewModel parent) : base(parent) {
        }

        private void changeLanguage(string language) {
            if (SelectedReferences != null) {
                foreach (ProfileModel model in SelectedReferences) {
                    model.changeLanguage(language);
                }
            }
        }
    }
}
