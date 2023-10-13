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
    public class LayoutProfileViewModel : LayoutViewModel {
        public LayoutProfileViewModel(PopUpViewModel parent) : base(parent) {
            _ = prepareTemplate();
        }

        public override async Task prepareTemplate() {
            //TaskScheduler syncContextScheduler;
            //if (SynchronizationContext.Current != null) {
            //    syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            //} else {
            //    syncContextScheduler = TaskScheduler.Current;
            //}

            //var TaskLayout = Task.Run(() => { Utils.downloadPowerpointTemplate("Profile"); }).ContinueWith(delegate {
            //    Layouts = new ObservableCollection<LayoutModel>(Utils.SlidesToImage("Profile"));
            //}, syncContextScheduler);

            var TaskTemplates = Task.Run(() => { Utils.downloadPowerpointTemplate("Profile"); });
            var TaskImages = await TaskTemplates.ContinueWith(t1 => { return Utils.SlidesToImage("Profile"); }, TaskContinuationOptions.OnlyOnRanToCompletion);
            Layouts = new ObservableCollection<LayoutModel>(TaskImages);
        }
    }
}
