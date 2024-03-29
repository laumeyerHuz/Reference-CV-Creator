﻿using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using ReferenceConfigurator.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReferenceConfigurator.views {
    public class SavedDataViewModel : MainContentViewModel {

        public IAsyncRelayCommand DeleteSavedLogosCommand { get; }

        public IAsyncRelayCommand DeleteSavedOnePagerCommand { get; }

        public IAsyncRelayCommand RefreshSavedTemplatesCommand { get; }

        public IAsyncRelayCommand RefreshSavedIndexCommand { get; }

        public IAsyncRelayCommand DeleteSavedProfilePicturesCommand { get; }

        public IAsyncRelayCommand RefreshEverythingCommand { get; }

        private PopUpViewModel parent;
        public SavedDataViewModel(PopUpViewModel parent) {
            this.parent = parent;
            DeleteSavedLogosCommand = new AsyncRelayCommand(deleteSavedLogos);
            DeleteSavedOnePagerCommand = new AsyncRelayCommand(deleteSavedOnePager);
            RefreshSavedTemplatesCommand = new AsyncRelayCommand(refreshSavedTemplates);
            RefreshSavedIndexCommand = new AsyncRelayCommand(refreshSavedIndex);
            DeleteSavedProfilePicturesCommand = new AsyncRelayCommand(deleteSavedProfilePictures);
            RefreshEverythingCommand = new AsyncRelayCommand(refreshEverything);
        }

        public async Task deleteSavedLogos() {
            await Task.Run(() => Utils.removeLogos());
        }
        public async Task deleteSavedProfilePictures() {
            await Task.Run(() => Utils.removeProfilePictures());
        }

        public async Task deleteSavedOnePager() {
            await Task.Run(() => Utils.removeOnePager());
        }

        public async Task refreshSavedTemplates() {
            await Task.Run(() => Utils.removeTemplates()).ContinueWith(t => {
                parent.refreshTemplateProfile();
            }, TaskContinuationOptions.OnlyOnRanToCompletion).ContinueWith(t => {
                parent.refreshTemplateReference();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public async Task refreshSavedIndex() {
            await Task.Run(() => parent.refreshSearch());
        }

        public void RefreshConfiguration() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/config");
            if(Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
        }

        public async Task<string> refreshEverything() {
            RefreshConfiguration();
            var TaskLogos = deleteSavedLogos();
            var TaskOnePager = deleteSavedOnePager();
            var TaskSearch = refreshSavedIndex();
            var TaskTemplates = refreshSavedTemplates();
            var TaskProfile = deleteSavedProfilePictures();

            await Task.WhenAll(TaskLogos, TaskOnePager, TaskTemplates, TaskSearch, TaskProfile);
            return "Finished";
        }
    }
}
