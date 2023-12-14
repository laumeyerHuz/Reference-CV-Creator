using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if(language == "EN") {
                TitleList = new ObservableCollection<string>(_titleListEN);
            } else if (language == "DE") {
                TitleList = new ObservableCollection<string>(_titleListDE);
            }
            SelectedTitle = TitleList[0];
            if (SelectedReferences != null) {
                foreach (ProfileModel model in SelectedReferences) {
                    model.changeLanguage(language);
                }
            }
        }

        protected override void populateTitle() {
            _titleListDE.Add("Wir setzen auf ein bewährtes und erfahrenes Projektteam");

            _titleListDE.Add("Unser erfahrenes Team kombiniert unsere Stärken: THEMA, FUNKTIONS- und BRANCHE-Know-how");

            _titleListDE.Add("Wir bieten FIRMA ein seniores Team mit langjähriger Erfahrung im Bereich BEREICH");
            _titleListDE.Add("Wir bieten ein KUNDE-erfahrenes und in der METHODIK versiertes Team");
            _titleListDE.Add("Wir bieten ein erfahrenes Team mit fundiertem Fachwissen im PRODUKT-Markt und FUNKTION");
            _titleListDE.Add("Wir bieten ein seniores Team mit langjähriger Erfahrung in METHODIK-projekten in INDUSTRIE");

            _titleListEN.Add("We offer a practiced and experienced team with broad and long-lasting expertise in TOPIC");
            _titleListEN.Add("We offer a senior and experienced team with in-depth expertise in TOPIC for SIZE-sized companies");
            _titleListEN.Add("We offer a practiced team with long-lasting expertise in CATEGORY supported by dedicated Category experts");
            _titleListEN.Add("We offer a senior team with many years of experience in the area of TOPIC");
            _titleListEN.Add("We offer a team with long-lasting expertise in INDUSTRY and FUNCTIONAL AREA");

            _titleListEN.Add("We provide a senior and experienced team with in-depth expertise in TOPIC and " +
                "value creationn combined with industry expertise and SPECIFIC knowledge");
            _titleListEN.Add("We provide a senior and experienced team with in-depth expertise in TOPIC and TOPIC");

            _titleListEN.Add("We rely on a proven and experienced project team");

            _titleListEN.Add("We have additional TOPIC, INDUSTRY and FUNCTIONAL AREA experts to draw from on demand");
            _titleListEN.Add("We provide an experienced team of TOPIC and INDUSTRY experts");


        }
    }
}
