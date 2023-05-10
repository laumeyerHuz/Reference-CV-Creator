using J2N.Collections.Generic;
using Microsoft.SharePoint.Client;
using ReferenceConfigurator.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Threading;
using System.Windows.Navigation;
using PnP.Framework;

namespace ReferenceConfigurator {
    class SharepointConnection {

        private static ClientContext createClientContext(string url) {
            var authManger = new AuthenticationManager();

            ClientContext ctx = authManger.GetContext(url);
            return ctx;
        }


        public static ClientContext GetClientContext(string url) {
            return SharepointConnection.createClientContext(url);
        }

        public static ListItemCollection getSharepointList() {
            ClientContext ctx = GetClientContext(Settings.Default.url);
            List list = ctx.Web.Lists.GetByTitle("Project list");
            ListItemCollection itemColl = list.GetItems(CamlQuery.CreateAllItemsQuery());
            ctx.Load(itemColl);
            ctx.ExecuteQuery();
            return itemColl;
        }

        public static void downloadPowerpointTemplates() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/powerpointTemplate");

            if (System.IO.Directory.Exists(folderPath)) { return; }

            System.IO.Directory.CreateDirectory(folderPath);

            ClientContext ctx = GetClientContext(Settings.Default.template);
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

        public static string downloadCompanyLogo(string name) {

            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/CompanyLogo");

            System.IO.Directory.CreateDirectory(folderPath);

            ClientContext ctx = GetClientContext(Properties.Settings.Default.template);
            List list = ctx.Web.Lists.GetByTitle("Dokumente");
            ctx.Load(list);
            ctx.ExecuteQuery();
            FolderCollection fcol = list.RootFolder.Folders;
            ctx.Load(fcol);
            ctx.ExecuteQuery();

            foreach (Folder f in fcol) {
                if (f.Name == "Templates Reference Configurator") {
                    ctx.Load(f.Folders);
                    ctx.ExecuteQuery();
                    fcol = f.Folders;
                    foreach (Folder f2 in fcol) {
                        if (f2.Name == "Logos") {
                            ctx.Load(f2.Files);
                            ctx.ExecuteQuery();
                            FileCollection fileCol = f2.Files;

                            foreach (Microsoft.SharePoint.Client.File file in fileCol) {
                                name = name.Replace("/", "_");
                                if (name == file.Name.Split('.')[0]) {
                                    var fileName = Path.Combine(basePath, "ReferenceConfigurator/CompanyLogo", (string)file.Name);
                                    var localstream = System.IO.File.Open(fileName, System.IO.FileMode.Create);
                                    var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, file.ServerRelativeUrl);
                                    var spstream = fileInfo.Stream;
                                    spstream.CopyTo(localstream);
                                    spstream.Close();
                                    localstream.Close();
                                    return fileName;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static string downloadOnePager(int id) {

            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/onePager");

            System.IO.Directory.CreateDirectory(folderPath);

            ClientContext ctx = GetClientContext(Properties.Settings.Default.onePager);
            List list = ctx.Web.Lists.GetByTitle("Dokumente");
            ctx.Load(list);
            ctx.ExecuteQuery();
            FolderCollection fcol = list.RootFolder.Folders;
            ctx.Load(fcol);
            ctx.ExecuteQuery();
            foreach(Folder first in fcol) {
                if(first.Name == "General") {
                    fcol = first.Folders;
                    ctx.Load(fcol);
                    ctx.ExecuteQuery();
                    foreach (Folder second in fcol) {
                        if(second.Name == "Project One Pager") {
                            FileCollection fileCol = second.Files;
                            ctx.Load(fileCol);
                            ctx.ExecuteQuery();
                            foreach(Microsoft.SharePoint.Client.File file in fileCol) {
                                if(id.ToString() == file.Name.Split('_')[0]) {
                                    var fileName = Path.Combine(basePath, "ReferenceConfigurator/onePager", (string)file.Name);
                                    var localstream = System.IO.File.Open(fileName, System.IO.FileMode.Create);
                                    var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, file.ServerRelativeUrl);
                                    var spstream = fileInfo.Stream;
                                    spstream.CopyTo(localstream);
                                    spstream.Close();
                                    localstream.Close();
                                    return fileName;
                                }
                            }
                        }
                    }
                    
                }
            }
            return "";
        } 
    }
}
