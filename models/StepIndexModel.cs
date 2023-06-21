using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class StepIndexModel {
        public string Header { get; set; }

        public string Content { get; set; }
        public StepIndexModel(string header, string content) {
            this.Content = content;
            this.Header = header;
        }
    }
}
