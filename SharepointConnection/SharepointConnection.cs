using Microsoft.SharePoint.Client;
using ReferenceConfigurator.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;

namespace ReferenceConfigurator {
    class SharepointConnection {

        private static ClientContext createClientContext() {
            ClientContext ctx = Weblogin.GetWebLoginClientContext(Properties.Settings.Default.url, false);
            return ctx;
        }


        public static ClientContext GetClientContext() {
            return SharepointConnection.createClientContext();
        }

        public static ListItemCollection getSharepointList() {
            ClientContext ctx = GetClientContext();
            List list = ctx.Web.Lists.GetByTitle("Project list");
            ListItemCollection itemColl = list.GetItems(CamlQuery.CreateAllItemsQuery());
            ctx.Load(itemColl);
            ctx.ExecuteQuery();
            return itemColl;
        }

        public static void downloadPowerpointTemplates() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/powerpointTemplate");
            if (System.IO.File.Exists(folderPath)) { return; }
            System.IO.Directory.CreateDirectory(folderPath);
            ClientContext ctx = Weblogin.GetWebLoginClientContext(Properties.Settings.Default.template, false);
            List list = ctx.Web.Lists.GetByTitle("Dokumente");

            ctx.Load(list);
            ctx.ExecuteQuery();

            FolderCollection fcol = list.RootFolder.Folders;
            ctx.Load(fcol);
            ctx.ExecuteQuery();

            foreach (Folder f in fcol) {
                if (f.Name == "Templates Reference Configurator") {
                    ctx.Load(f.Files);
                    ctx.ExecuteQuery();
                    FileCollection fileCol = f.Files;

                    foreach (Microsoft.SharePoint.Client.File file in fileCol) {
                        //TODO check for files instead of forcing download
                        var fileName = Path.Combine(basePath, "ReferenceConfigurator/powerpointTemplate", (string)file.Name);
                        var localstream = System.IO.File.Open(fileName, System.IO.FileMode.Create);
                        var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, file.ServerRelativeUrl);
                        var spstream = fileInfo.Stream;
                        spstream.CopyTo(localstream);
                        spstream.Close();
                        localstream.Close();
                    }
                }
            }


        }
    }
}
