using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class ReferenceLayoutModel : LayoutModel {
        public ReferenceLayoutModel(string powerpointPath, string imagePath, string name) : base(powerpointPath, imagePath, name) {
            if (name == "Project One Pager") {
                onePager = true;
            } else {
                onePager = false;
            }
        }
    }
}
