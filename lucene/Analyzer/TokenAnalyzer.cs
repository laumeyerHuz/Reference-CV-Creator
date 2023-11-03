using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Synonym;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.lucene {
    public class TokenAnalyzer : Analyzer {

        private LuceneVersion version; 

        public TokenAnalyzer(LuceneVersion version) {
            this.version = version;
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader) {
            Tokenizer source = new WhitespaceTokenizer(version,reader);
            TokenStream filter = new EdgeNGramTokenFilter(version, source,3,15);
            filter = new LowerCaseFilter(version,filter);
            filter = new StopFilter(version,filter, Lucene.Net.Analysis.En.EnglishAnalyzer.DefaultStopSet);
            return new TokenStreamComponents(source, filter);
        }
    }
}
