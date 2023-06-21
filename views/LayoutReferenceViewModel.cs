using ReferenceConfigurator.models;
using ReferenceConfigurator.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.views {
    public class LayoutReferenceViewModel : LayoutViewModel {
        public LayoutReferenceViewModel(PopUpViewModel parent) : base(parent) {
            prepareTemplate();
        }

        public override void prepareTemplate() {
            Utils.downloadPowerpointTemplate("Reference");
            Layouts = new ObservableCollection<LayoutModel>(Utils.SlidesToImage("Reference"));
        }
    }
}
