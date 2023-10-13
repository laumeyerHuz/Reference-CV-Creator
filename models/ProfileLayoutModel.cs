using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class ProfileLayoutModel : LayoutModel {

        public int partner  { get; set; } = 0;

        public int core { get; set; } = 0;
        
        public int expert { get; set; } = 0;

        public ProfileLayoutModel(string powerpointPath, string imagePath, string name) : base(powerpointPath, imagePath, name) {
            if (name == "Profile One Pager" || name == "Profile Two Pager") {
                maxElements = -1;
                onePager = true;
            } else {
                onePager = false;
                string[] split = name.Split(' ')[2].Split('_');
                this.partner = split[0].ToInt32();
                this.core = split[1].ToInt32();
                this.expert = split[2].ToInt32();
                maxElements = partner+core+expert;
            }
            
        }
    }
}
