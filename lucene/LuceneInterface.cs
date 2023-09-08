using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.Codecs;
using Lucene.Net.QueryParsers;
using System.Diagnostics;
using FSDirectory = Lucene.Net.Store.FSDirectory;
using System;
using System.IO;
using Lucene.Net.Analysis;
using Microsoft.SharePoint.Client;
using System.Linq;
using Lucene.Net.QueryParsers.Classic;
using ReferenceConfigurator.models;
using System.Collections.Generic;
using Lucene.Net.Index.Extensions;

namespace ReferenceConfigurator.lucene {
    public class LuceneInterface {
        protected IndexWriter _writer;
        protected IndexReader _reader;
        protected IndexSearcher _searcher;
        protected Analyzer _analyzer;
        protected const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        protected string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        protected string indexPath;
        protected FSDirectory dir;
        protected string dateFile;

        public LuceneInterface() {}

        protected virtual void createIndexWriter() {
            DateTime now = DateTime.Now;
            if (!DirectoryReader.IndexExists(dir)) {

                _analyzer = new StandardAnalyzer(AppLuceneVersion);
                IndexWriterConfig indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
                _writer = new IndexWriter(dir, indexConfig);

                ListItemCollection list = getSharepointList();
                addSharepointToIndex(list);
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

        protected virtual ListItemCollection getSharepointList() {
            throw new NotImplementedException();
        }

        public virtual List<SearchModel> getModelByGeneralSearch(string search) {
            throw new NotImplementedException();
        }

        protected virtual void addSharepointToIndex(ListItemCollection list) {
            foreach (ListItem listItem in list) {
                addListItemToDoc(listItem);
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

        protected virtual void addListItemToDoc(ListItem listItem) { }

        public virtual void refreshIndex() {
            if (DirectoryReader.IndexExists(dir)) {
                _analyzer = new StandardAnalyzer(AppLuceneVersion);
                IndexWriterConfig indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
                _writer = new IndexWriter(dir, indexConfig);
                _writer.DeleteAll();
                _writer.Commit();
                ListItemCollection list = getSharepointList();
                addSharepointToIndex(list);
                _reader = DirectoryReader.Open(dir);
                _searcher = new IndexSearcher(_reader);
            }
        }
    }
}
