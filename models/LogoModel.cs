using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class LogoModel : SearchModel {
        public string CompanyName { get; set; }
        public string LogoFile { get; set; }
        public LogoModel() { }
    }
}
