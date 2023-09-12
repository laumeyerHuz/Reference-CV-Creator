using ReferenceConfigurator.models;
using ReferenceConfigurator.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.views {
    public class LayoutProfileViewModel : LayoutViewModel {
        public LayoutProfileViewModel(PopUpViewModel parent) : base(parent) {
            prepareTemplate();
        }

        public override Task prepareTemplate() {
            var TaskLayout = Task.Run(() => { Utils.downloadPowerpointTemplate("Profile"); }).ContinueWith(delegate {
                Layouts = new ObservableCollection<LayoutModel>(Utils.SlidesToImage("Profile"));
            }, TaskScheduler.FromCurrentSynchronizationContext());

            return TaskLayout;
        }
    }
}
