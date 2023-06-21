using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class ReferenceModel {
        public int ProjectId { get; set; }
        public string Partner { get; set; }
        public string ProjectName { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Data { get; set; }
        public string Branch { get; set; }
        public string Team { get; set; }
        public string ProjectDescriptionEN { get; set; }
        public string Client { get; set; }
        public string Topic { get; set; }
        public string ProjectDescriptionDE { get; set; }
        public string Logo { get; set; }
        public bool OnePager { get; set; }
    }
}
