using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class LayoutModel {

        public string imagePath { get; set; }

        public LayoutModel(string filePath) {
            this.imagePath = filePath;
        }
    }
}
