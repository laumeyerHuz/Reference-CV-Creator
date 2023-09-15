using ReferenceConfigurator.models;
using ReferenceConfigurator.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReferenceConfigurator.views {
    public class LayoutReferenceViewModel : LayoutViewModel {
        public LayoutReferenceViewModel(PopUpViewModel parent) : base(parent) {
            prepareTemplate();
        }

        public override Task prepareTemplate() {
            TaskScheduler syncContextScheduler;
            if (SynchronizationContext.Current != null) {
                syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            } else {
                syncContextScheduler = TaskScheduler.Current;
            }

            var TaskLayout = Task.Run(() => { Utils.downloadPowerpointTemplate("Reference"); }).ContinueWith(delegate { 
                Layouts = new ObservableCollection<LayoutModel>(Utils.SlidesToImage("Reference"));
                
            }, syncContextScheduler);  
            return TaskLayout;
        }
    }
}
