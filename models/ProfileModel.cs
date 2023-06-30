using System;
using System.Collections.Generic;
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
        public string ProjectExperienceEN { get; set; }
        public string ProjectExperienceDE { get; set; }
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

        public string [] flags { get; set; }

        public ProfileModel() { }
    }
}
