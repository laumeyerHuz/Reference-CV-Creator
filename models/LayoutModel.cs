using J2N.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class LayoutModel {

        public string imagePath { get; set; }

        public string powerpointPath { get; set; }

        public string name { get; set; }

        public bool onePager;

        public int maxElements { get; set; }

        public LayoutModel(string powerpointPath, string imagePath, string name) {
            this.powerpointPath = powerpointPath;
            this.imagePath = imagePath;
            this.name = name;
        }
    }
}
