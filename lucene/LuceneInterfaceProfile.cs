using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers;
using System.Diagnostics;
using FSDirectory = Lucene.Net.Store.FSDirectory;
using System;
using System.IO;
using Lucene.Net.Analysis;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.QueryParsers.Classic;
using Microsoft.Office.Interop.PowerPoint;
using ReferenceConfigurator.models;
using System.Collections.Generic;
using HandyControl.Controls;

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
            if (!(listItem["field_1"] is null)) {
                doc.Add(new TextField("LastName", listItem["field_1"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("LastName", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_2"] is null)) {
                doc.Add(new TextField("Initials", listItem["field_1"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Initials", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_3"] is null)) {
                doc.Add(new TextField("RoleEN", listItem["field_3"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("RoleEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_4"] is null)) {
                doc.Add(new TextField("RoleDE", listItem["field_4"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("RoleDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_5"] is null)) {
                doc.Add(new TextField("Tribe", listItem["field_5"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Tribe", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_6"] is null)) {
                doc.Add(new TextField("Squad", listItem["field_6"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Squad", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_7"] is null)) {
                doc.Add(new TextField("ProductTopicOwner", listItem["field_7"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProductTopicOwner", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_8"] is null)) {
                doc.Add(new TextField("InternalResponsibility", listItem["field_8"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("InternalResponsibility", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_9"] is null)) {
                doc.Add(new TextField("ProfessionalExperienceEN", listItem["field_9"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProfessionalExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_10"] is null)) {
                doc.Add(new TextField("ProfessionalExperienceDE", listItem["field_10"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProfessionalExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_11"] is null)) {
                doc.Add(new TextField("EducationAndTrainingEN", listItem["field_11"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("EducationAndTrainingEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_12"] is null)) {
                doc.Add(new TextField("EnductionAndTrainingDE", listItem["field_12"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("EnductionAndTrainingDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_13"] is null)) {
                doc.Add(new TextField("ProjectExperienceEN", listItem["field_13"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProjectExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_14"] is null)) {
                doc.Add(new TextField("ProjectExperienceDE", listItem["field_14"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProjectExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_15"] is null)) {
                doc.Add(new TextField("IndustryExperienceEN", listItem["field_15"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("IndustryExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_16"] is null)) {
                doc.Add(new TextField("IndustryExperienceDE", listItem["field_16"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("IndustryExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_17"] is null)) {
                doc.Add(new TextField("FunctionalExperienceEN", listItem["field_17"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("FunctionalExperienceEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_18"] is null)) {
                doc.Add(new TextField("FunctionalExperienceDE", listItem["field_18"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("FunctionalExperienceDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_19"] is null)) {
                doc.Add(new TextField("MethodExpertise", listItem["field_19"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("MethodExpertise", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_20"] is null)) {
                doc.Add(new TextField("ToolExpertise", listItem["field_20"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ToolExpertise", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_21"] is null)) {
                doc.Add(new TextField("AdditionalQualifications", listItem["field_21"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("AdditionalQualifications", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_22"] is null)) {
                doc.Add(new TextField("LanguagesEN", listItem["field_22"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("LanguagesEN", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_23"] is null)) {
                doc.Add(new TextField("LanguagesDE", listItem["field_23"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("LanguagesDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["field_24"] is null)) {
                DateTime now = DateTime.Now;
                int since = listItem["field_24"].ToString().ToInt32();
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
