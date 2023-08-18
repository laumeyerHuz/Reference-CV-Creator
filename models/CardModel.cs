using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ReferenceConfigurator.models {
    public class CardModel {

        public string Footer { get; set; }

        public string imagePath { get; set; }
        public string Name { get; set; }

        public CardModel(string name, string footer, string imagePath) {
            this.Name = name;
            this.Footer = footer;
            this.imagePath = imagePath;  
        }

    }
}
