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
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;

namespace ReferenceConfigurator.lucene {
    public class LuceneInterfaceLogo :LuceneInterface {

        public LuceneInterfaceLogo() {
            indexPath = Path.Combine(basePath, "ReferenceConfigurator/index/Logo");
            dir = FSDirectory.Open(indexPath);
            dateFile = Path.Combine(indexPath, "dateLogo.txt");
            createIndexWriter();
            _reader = DirectoryReader.Open(dir);
            _searcher = new IndexSearcher(_reader);
        }

        protected override void createIndexWriter() {
            DateTime now = DateTime.Now;
            if (!DirectoryReader.IndexExists(dir)) {

                _analyzer = new TokenAnalyzer(AppLuceneVersion);
                IndexWriterConfig indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
                _writer = new IndexWriter(dir, indexConfig);

                FileCollection list = getFileList();
                addFolderToIndex(list);
                System.IO.File.WriteAllText(dateFile, now.ToString());

            } else {
                if (System.IO.File.Exists(dateFile)) {
                    DateTime lastUpdate = DateTime.Parse(System.IO.File.ReadAllText(dateFile));
                    TimeSpan ts = now - lastUpdate;
                    if (ts.TotalDays > 6) {
                        refreshIndex();
                        System.IO.File.WriteAllText(dateFile, now.ToString());
                    }
                } else {
                    System.IO.File.WriteAllText(dateFile, now.ToString());
                }

            }
        }

        protected void addFolderToIndex(FileCollection list) {
            foreach (Microsoft.SharePoint.Client.File file in list) {
                addFileToDoc(file);
            }
            _writer.Flush(true, true);
            _writer.Commit();
            try {
                _writer.Dispose();
                _analyzer.Dispose();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            } finally {
                if (IndexWriter.IsLocked(dir)) {
                    IndexWriter.Unlock(dir);
                }
            }
        }

        protected FileCollection getFileList() {
            return SharepointConnection.getSharepointListLogo();
        }

        protected void addFileToDoc(Microsoft.SharePoint.Client.File file) {
            Document doc = new Document {
                new TextField("CompanyName", Path.GetFileNameWithoutExtension(file.Name), Lucene.Net.Documents.Field.Store.YES)
            };
            _writer.AddDocument(doc);
        }

        private LogoModel getModelFromDoc(Document doc) {
            LogoModel _logoModel = new LogoModel {
                CompanyName = doc.Get("CompanyName")
            };
            return _logoModel;
        }

        public override List<SearchModel> getModelByGeneralSearch(string search) {
            List<SearchModel> _referenceModelList = new List<SearchModel>();
            MultiFieldQueryParser queryParser = new MultiFieldQueryParser(
                AppLuceneVersion,
                new String[] { "CompanyName" },
                new StandardAnalyzer(AppLuceneVersion));
            search = search + "~0.95";
            Query q = queryParser.Parse(search);
            TopDocs _results = _searcher.Search(q, _reader.NumDocs);
            for (int i = 0; i < _results.TotalHits; i++) {
                _referenceModelList.Add(getModelFromDoc(_searcher.Doc(_results.ScoreDocs[i].Doc)));
            }
            return _referenceModelList;
        }

        public override void refreshIndex() {
            if (DirectoryReader.IndexExists(dir)) {
                _analyzer = new TokenAnalyzer(AppLuceneVersion);
                IndexWriterConfig indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
                _writer = new IndexWriter(dir, indexConfig);
                _writer.DeleteAll();
                _writer.Commit();
                FileCollection list = getFileList();
                addFolderToIndex(list);
                _reader = DirectoryReader.Open(dir);
                _searcher = new IndexSearcher(_reader);
            }
        }
    }
}
