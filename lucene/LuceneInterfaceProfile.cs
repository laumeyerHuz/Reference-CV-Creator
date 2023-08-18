using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using FSDirectory = Lucene.Net.Store.FSDirectory;
using System;
using System.IO;
using Microsoft.SharePoint.Client;
using Lucene.Net.QueryParsers.Classic;
using ReferenceConfigurator.models;
using System.Collections.Generic;


namespace ReferenceConfigurator.lucene {
    public class LuceneInterfaceProfile : LuceneInterface {

        public LuceneInterfaceProfile() {
            indexPath = Path.Combine(basePath, "ReferenceConfigurator/index/Profile");
            dir = FSDirectory.Open(indexPath);
            dateFile = Path.Combine(indexPath, "dateProfile.txt");
            createIndexWriter();
            _reader = DirectoryReader.Open(dir);
            _searcher = new IndexSearcher(_reader);
        }

        protected override ListItemCollection getSharepointList() {
            return SharepointConnection.getSharepointListProfile();
        }

        protected override void addListItemToDoc(ListItem listItem) {
            Document doc = new Document {
                new TextField("FirstName", listItem["Title"].ToString(), Lucene.Net.Documents.Field.Store.YES)
            };
            if (!(listItem["LastName"] is null)) {
                doc.Add(new TextField("LastName", listItem["LastName"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("LastName", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Initials"] is null)) {
                doc.Add(new TextField("Initials", listItem["Initials"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Initials", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["RoleEN"] is null)) {
                doc.Add(new TextField("RoleEN", listItem["RoleEN"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("RoleEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["RoleDE"] is null)) {
                doc.Add(new TextField("RoleDE", listItem["RoleDE"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("RoleDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Tribe"] is null)) {
                doc.Add(new TextField("Tribe", listItem["Tribe"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Tribe", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Squad"] is null)) {
                doc.Add(new TextField("Squad", listItem["Squad"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Squad", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["ProductTopicOwner"] is null)) {
                doc.Add(new TextField("ProductTopicOwner", listItem["ProductTopicOwner"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProductTopicOwner", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["InternalResponsibility"] is null)) {
                doc.Add(new TextField("InternalResponsibility", listItem["InternalResponsibility"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("InternalResponsibility", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["ProfessionalExperienceEN"] is null)) {
                doc.Add(new TextField("ProfessionalExperienceEN", listItem["ProfessionalExperienceEN"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProfessionalExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["ProfessionalExperienceDE"] is null)) {
                doc.Add(new TextField("ProfessionalExperienceDE", listItem["ProfessionalExperienceDE"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProfessionalExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["EducationAndTrainingEN"] is null)) {
                doc.Add(new TextField("EducationAndTrainingEN", listItem["EducationAndTrainingEN"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("EducationAndTrainingEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["EducationAndTrainingDE"] is null)) {
                doc.Add(new TextField("EnductionAndTrainingDE", listItem["EducationAndTrainingDE"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("EnductionAndTrainingDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["ProjectExperienceEN"] is null)) {
                doc.Add(new TextField("ProjectExperienceEN", listItem["ProjectExperienceEN"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProjectExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["ProjectExperienceDE"] is null)) {
                doc.Add(new TextField("ProjectExperienceDE", listItem["ProjectExperienceDE"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProjectExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["IndustryExperienceEN"] is null)) {
                doc.Add(new TextField("IndustryExperienceEN", listItem["IndustryExperienceEN"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("IndustryExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["IndustryExperienceDE"] is null)) {
                doc.Add(new TextField("IndustryExperienceDE", listItem["IndustryExperienceDE"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("IndustryExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["FunctionalExperienceEN"] is null)) {
                doc.Add(new TextField("FunctionalExperienceEN", listItem["FunctionalExperienceEN"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("FunctionalExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["FunctionalExperienceDE"] is null)) {
                doc.Add(new TextField("FunctionalExperienceDE", listItem["FunctionalExperienceDE"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("FunctionalExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["MethodExpertise"] is null)) {
                doc.Add(new TextField("MethodExpertise", listItem["MethodExpertise"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("MethodExpertise", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["ToolExpertise"] is null)) {
                doc.Add(new TextField("ToolExpertise", listItem["ToolExpertise"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ToolExpertise", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["AdditionalQualification"] is null)) {
                doc.Add(new TextField("AdditionalQualifications", listItem["AdditionalQualification"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("AdditionalQualifications", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["LanguagesEN"] is null)) {
                doc.Add(new TextField("LanguagesEN", listItem["LanguagesEN"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("LanguagesEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["LanguagesDE"] is null)) {
                doc.Add(new TextField("LanguagesDE", listItem["LanguagesDE"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("LanguagesDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["YearsWorking"] is null)) {
                DateTime now = DateTime.Now;
                int since = listItem["YearsWorking"].ToString().ToInt32();
                int ts = now.Year - since;

                doc.Add(new TextField("YearsWorkExperience", ts.ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("YearsWorkExperience", "0", Lucene.Net.Documents.Field.Store.YES));
            }

            _writer.AddDocument(doc);
        }

        private ProfileModel getModelFromDoc(Document doc) {
            ProfileModel _profileModel = new ProfileModel {
                FirstName = doc.Get("FirstName"),
                LastName = doc.Get("LastName"),
                Initials = doc.Get("Initials"),
                RoleEN = doc.Get("RoleEN"),
                RoleDE = doc.Get("RoleDE"),
                Tribe = doc.Get("Tribe"),
                Squad = doc.Get("Squad"),
                ProductTopicOwner = doc.Get("ProductTopicOwner"),
                InternalResponsibility = doc.Get("InternalResponsibility"),
                ProfessionalExperienceEN = doc.Get("ProfessionalExperienceEN"),
                ProfessionalExperienceDE = doc.Get("ProfessionalExperienceDE"),
                EducationAndTrainingEN = doc.Get("EducationAndTrainingEN"),
                EnductionAndTrainingDE = doc.Get("EnductionAndTrainingDE"),
                ProjectExperienceEN = doc.Get("ProjectExperienceEN"),
                ProjectExperienceDE = doc.Get("ProjectExperienceDE"),
                IndustryExperienceEN = doc.Get("IndustryExperienceEN"),
                IndustryExperienceDE = doc.Get("IndustryExperienceDE"),
                FunctionalExperienceEN = doc.Get("FunctionalExperienceEN"),
                FunctionalExperienceDE = doc.Get("FunctionalExperienceDE"),
                MethodExpertise = doc.Get("MethodExpertise"),
                ToolExpertise = doc.Get("ToolExpertise"),
                AdditionalQualifications = doc.Get("AdditionalQualifications"),
                LanguagesEN = doc.Get("LanguagesEN"),
                LanguagesDE = doc.Get("LanguagesDE"),
                YearsWorkExperience = doc.Get("YearsWorkExperience").ToInt32(),
            };
            return _profileModel;
        }

        public override List<SearchModel> getModelByGeneralSearch(string search) {
            List<SearchModel> _referenceModelList = new List<SearchModel>();
            MultiFieldQueryParser queryParser = new MultiFieldQueryParser(
                AppLuceneVersion,
                new String[] { "FirstName", "LastName", "Initials", "RoleEN", "RoleDE", "Tribe", "Squad", "ProductTopicOwner", "InternalResponsibility", "ProfessionalExperienceEN", "ProfessionalExperienceDE", "EducationAndTrainingEN", "EnductionAndTrainingDE", "ProjectExperienceEN", "ProjectExperienceDE", "IndustryExperienceEN", "IndustryExperienceDE", "FunctionalExperienceEN", "FunctionalExperienceDE", "MethodExpertise", "ToolExpertise", "AdditionalQualifications", "LanguagesEN", "LanguagesDE", "YearsWorkExperience" },
                new StandardAnalyzer(AppLuceneVersion));
            search = search + "~0.95";
            Query q = queryParser.Parse(search);
            TopDocs _results = _searcher.Search(q, _reader.NumDocs);
            for (int i = 0; i < _results.TotalHits; i++) {
                _referenceModelList.Add(getModelFromDoc(_searcher.Doc(_results.ScoreDocs[i].Doc)));
            }
            return _referenceModelList;
        }
    }
}
