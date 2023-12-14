using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.views {
    public class SummaryReferenceViewModel : SummaryViewModel {

        public override string SelectedLanguage {
            get => _selectedLanguage;
            set {
                SetProperty(ref _selectedLanguage, value);
                changeLanguage(value);
            }
        }

        public SummaryReferenceViewModel(PopUpViewModel parent) : base(parent) {
        }

        private void changeLanguage(string language) {
            if (language == "EN") {
                TitleList = new ObservableCollection<string>(_titleListEN);
            } else if (language == "DE") {
                TitleList = new ObservableCollection<string>(_titleListDE);
            }
            SelectedTitle = TitleList[0];
        }

        protected override void populateTitle() {
            _titleListDE.Add("H&Z hat umfassende Erfahrung mit Optimierungsprogrammen für METHODE- und FUNKTIONSBEREICHE bei INDUSTRIE-Lieferanten");

            _titleListDE.Add("Wir bringen sowohl fundiertes Fachwissen zum THEMA als auch Branchenkenntnisse in der INDUSTRIE mit");
            _titleListDE.Add("Wir haben zahlreiche Wertschöpfungsprojekte in der INDUSTRIE durchgeführt und dabei Benchmarks, Kostenkalkulationen und Quick-Wins realisiert");
            _titleListDE.Add("Wir sind Experten für METHODE und Implementierung für FUNKTIONSBEREICHS-Organisationen");
            _titleListDE.Add("Mit einer Erfolgsbilanz von mehr als 500 Projekten betrachten wir Kostenreduzierung als unser Steckenpferd");

            _titleListEN.Add("H&Z has in depth experience in METHOD and FUNCTIONAL AREA optimization programs at INDUSTRY suppliers");

            _titleListEN.Add("We have a strong footprint in all project-relevant industries");
            _titleListEN.Add("We bring both in-depth expertise on TOPIC as well as INDUSTRY industry knowledge to the table");
            _titleListEN.Add("We conducted numerous value creation projects in INDUSTRY providing benchmarks, should costings and quick-wins realization");
            _titleListEN.Add("We are experts in METHOD and implementation for FUNCTIONAL AREA organizations");

            _titleListEN.Add("With a track record of 500+ projects, we regard cost reduction our home turf");
        }
    }
}
