using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class LayoutModel {

        public string imagePath { get; set; }

        public string powerpointPath { get; set; }

        public string name { get; set; }

        public bool onePager;

        public LayoutModel(string powerpointPath, string imagePath, string name) {
            this.powerpointPath = powerpointPath;
            this.imagePath = imagePath;
            this.name = name;
            if(name == "Project One Pager_Vorlage") {
                onePager = true;
            } else {
                onePager = false;
            }
        }
    }
}
