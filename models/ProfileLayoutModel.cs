using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class ProfileLayoutModel : LayoutModel {

        private int leader  { get; set; } = 0;

        private int teamMates { get; set; } = 0;
        
        private int experts { get; set; } = 0;

        public ProfileLayoutModel(string powerpointPath, string imagePath, string name) : base(powerpointPath, imagePath, name) {
            if (name == "Profile One Pager") {
                maxElements = -1;
                onePager = true;
            } else {
                onePager = false;
                string[] split = name.Split(' ')[2].Split('_');
                this.leader = split[0].ToInt32();
                this.teamMates = split[1].ToInt32();
                this.experts = split[2].ToInt32();
                maxElements = leader+teamMates+experts;
            }
            
        }
    }
}
