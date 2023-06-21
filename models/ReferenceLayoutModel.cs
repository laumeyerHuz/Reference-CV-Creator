using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class ReferenceLayoutModel : LayoutModel {
        public ReferenceLayoutModel(string powerpointPath, string imagePath, string name) : base(powerpointPath, imagePath, name) {
            if (name == "Project One Pager") {
                onePager = true;
                maxElements = -1;
            } else {
                onePager = false;
                List<string> tmp = Regex.Matches(name, @"\d+").Cast<Match>().Select(p => p.Value).ToList();
                maxElements = tmp.Aggregate(1, (a, b) => a * b.ToInt32());
            }
        }
    }
}
