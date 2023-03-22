using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class SearchModel {
        [Category("Project")]
        public int ProjectID { get; set; }
        [Category("Project")]
        public string Branch { get; set; }
        [Category("Project")]
        public string Subject { get; set; }
        [Category("Project")]
        public string ProjectName { get; set; }

        [Category("Client")]
        public string Client { get; set; }

        [Category("Team Members")]
        public string ProjectLeader { get; set; }
        [Category("Team Members")]
        public string Partner { get; set; }
        [Category("Team Members")]
        public string Team { get; set; }

        [Category("Date")]
        public string Start { get; set; }
        [Category("Date")]
        public string End { get; set; }

        [Category("Description")]
        public string Tag { get; set; }
    }
}
