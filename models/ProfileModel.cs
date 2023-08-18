using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class ProfileModel : SearchModel {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initials { get; set; }
        public string RoleEN { get; set; }
        public string RoleDE { get; set; }
        public string Tribe { get; set; }
        public string Squad { get; set; }
        public string ProductTopicOwner { get; set; }
        public string InternalResponsibility { get; set; }
        public string ProfessionalExperienceEN { get; set; }
        public string ProfessionalExperienceDE { get; set; }
        public string EducationAndTrainingEN { get; set; }
        public string EnductionAndTrainingDE { get; set; }

        private string _projectExperienceEN;
        public string ProjectExperienceEN {
            get => _projectExperienceEN;
            set {
                _projectExperienceEN = value;
                ProjectExperiencesEN = split(value);
                changeLanguage("EN");
            }
        }

        private string _projectExperienceDE;
        public string ProjectExperienceDE {
            get => _projectExperienceDE;
            set {
                _projectExperienceDE = value;
                ProjectExperiencesDE = split(value);
            }
        }
        public string IndustryExperienceEN { get; set; }
        public string IndustryExperienceDE { get; set; }
        public string FunctionalExperienceEN { get; set; }
        public string FunctionalExperienceDE { get; set; }
        public string MethodExpertise { get; set; }
        public string ToolExpertise { get; set; }
        public string AdditionalQualifications { get; set; }
        public string LanguagesEN { get; set; }
        public string LanguagesDE { get; set; }
        public int YearsWorkExperience { get; set; }
        public string ParterDescriptionEN { get; set; }
        public string ParterDescriptionDE { get; set; }

        public string[] flags { get; set; }

        public string ProfilePicture { get; set; }

        public List<CheckBoxModel> ProjectExperiencesEN { get; set; }

        public List<CheckBoxModel> ProjectExperiencesDE { get; set; }

        private ObservableCollection<CheckBoxModel> _projectExperiencesDisplay;

        public ObservableCollection<CheckBoxModel> ProjectExperiencesDisplay {
            get => _projectExperiencesDisplay;
            set {
                SetProperty(ref _projectExperiencesDisplay, value);
            }
        }

        private bool partner;
        public bool IsPartner {
            get => partner;
            set {
                SetProperty(ref partner, value);
                changeLanguage(currentLanguage);
            }
        }

        private bool expert;
        public bool IsExpert {
            get => expert;
            set {
                SetProperty(ref expert, value);
                changeLanguage(currentLanguage);

            }
        }



        public ProfileModel() { }

        private List<CheckBoxModel> split(string input) {
            List<CheckBoxModel> tmp = new List<CheckBoxModel>();
            foreach (string line in input.Split('\n')) {
                tmp.Add(new CheckBoxModel() {
                    Name = line,
                    IsChecked = true,
                });
            }
            return tmp;
        }

        public void changeLanguage(string language) {
            if (partner) {
                switch (language) {
                    case "EN":
                        ProjectExperiencesDisplay = new ObservableCollection<CheckBoxModel>(split(ParterDescriptionEN));
                        break;
                    case "DE":
                        ProjectExperiencesDisplay = new ObservableCollection<CheckBoxModel>(split(ParterDescriptionDE));
                        break;
                }
            } else if (expert) {
                switch (language) {
                    case "EN":
                        ProjectExperiencesDisplay = null;
                        break;
                    case "DE":
                        ProjectExperiencesDisplay = null;
                        break;
                }
            } else {
                switch (language) {
                    case "EN":
                        ProjectExperiencesDisplay = new ObservableCollection<CheckBoxModel>(ProjectExperiencesEN);
                        currentLanguage = "EN";
                        break;
                    case "DE":
                        ProjectExperiencesDisplay = new ObservableCollection<CheckBoxModel>(ProjectExperiencesEN);
                        currentLanguage = "DE";
                        break;
                }
            } 
        }

        private string currentLanguage = "EN";
    }
}
