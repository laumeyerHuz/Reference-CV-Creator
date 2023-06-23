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

        public LuceneInterfaceProfile(): base() {
            indexPath = Path.Combine(basePath, "ReferenceConfigurator/index/Profile");
            dir = FSDirectory.Open(indexPath);
            createIndexWriter();
            _reader = DirectoryReader.Open(dir);
            _searcher = new IndexSearcher(_reader);
        }

        protected override void addListItemToDoc(ListItem listItem) {
            //System.Diagnostics.Debug.WriteLine(listItem["Title"]);
            Document doc = new Document {
                new TextField("ProjectID", listItem["Title"].ToString(), Lucene.Net.Documents.Field.Store.YES)
            };
            if (!(listItem["Partner"] is null)) {
                doc.Add(new TextField("Partner", listItem["Partner"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Partner", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Projektname"] is null)) {
                doc.Add(new TextField("ProjectName", listItem["Projektname"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProjectName", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Start"] is null)) {
                doc.Add(new TextField("Start", listItem["Start"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Start", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Ende"] is null)) {
                doc.Add(new TextField("End", listItem["Ende"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("End", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Daten"] is null)) {
                doc.Add(new TextField("Data", ((Microsoft.SharePoint.Client.FieldUrlValue)listItem["Daten"]).Url.ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Data", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Branche"] is null)) {
                doc.Add(new TextField("Branch", listItem["Branche"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Branch", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Team"] is null)) {
                String teamNames = Regex.Replace(listItem["Team"].ToString(), "([a-z])([A-Z])", "$1 $2");
                doc.Add(new TextField("Team", teamNames, Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Team", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Thema"] is null)) {
                doc.Add(new TextField("ProjectDescriptionEN", listItem["Thema"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Subject", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Kunde"] is null)) {
                doc.Add(new TextField("Client", listItem["Kunde"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Client", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Topic_x002f_Product"] is null)) {
                string[] tmp = (string[])listItem["Topic_x002f_Product"];
                string topics = String.Join(", ", tmp);
                doc.Add(new TextField("Topic", topics, Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("Client", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["Projectdescription_x0028_DE_x002"] is null)) {
                doc.Add(new TextField("ProjectDescriptionDE", listItem["Projectdescription_x0028_DE_x002"].ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("ProjectDescriptionDE", "", Lucene.Net.Documents.Field.Store.YES));
            }
            if (!(listItem["OnePager"] is null)) {
                doc.Add(new TextField("OnePager", ((Microsoft.SharePoint.Client.FieldUrlValue)listItem["Daten"]).Url.ToString(), Lucene.Net.Documents.Field.Store.YES));
            } else {
                doc.Add(new TextField("OnePager", "", Lucene.Net.Documents.Field.Store.YES));
            }
            _writer.AddDocument(doc);
        }

        private ReferenceModel getModelFromDoc(Document doc) {
            ReferenceModel _referenceModel = new ReferenceModel {
                ProjectId = doc.Get("ProjectID").ToInt32(),
                Partner = doc.Get("Partner"),
                ProjectName = doc.Get("ProjectName"),
                Start = doc.Get("Start"),
                End = doc.Get("End"),
                Data = doc.Get("Data"),
                Branch = doc.Get("Branch"),
                Team = doc.Get("Team"),
                ProjectDescriptionEN = doc.Get("ProjectDescriptionEN"),
                Client = doc.Get("Client"),
                Topic = doc.Get("Topic"),
                ProjectDescriptionDE = doc.Get("ProjectDescriptionDE"),
                OnePager = doc.Get("OnePager").Length > 1 ? true : false,
            };

            return _referenceModel;
        }

        public List<ReferenceModel> getModelByGeneralSearch(string search) {
            List<ReferenceModel> _referenceModelList = new List<ReferenceModel>();
            MultiFieldQueryParser queryParser = new MultiFieldQueryParser(
                AppLuceneVersion,
                new String[] { "ProjectID", "Partner", "ProjectName", "Branch", "Team", "Subject", "Client", "Topic", "ProjectDescriptionDE", "ProjectDescriptionEN", "OnePager" },
                new StandardAnalyzer(AppLuceneVersion));
            search = search + "~0.8";
            Query q = queryParser.Parse(search);
            TopDocs _results = _searcher.Search(q, _reader.NumDocs);
            for (int i = 0; i < _results.TotalHits; i++) {
                _referenceModelList.Add(getModelFromDoc(_searcher.Doc(_results.ScoreDocs[i].Doc)));
            }
            return _referenceModelList;
        }
    }
}
