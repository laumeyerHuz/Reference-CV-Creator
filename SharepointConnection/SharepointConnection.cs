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
using System.Reflection;
using AngleSharp.Io;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using ReferenceConfigurator.utils;

namespace ReferenceConfigurator {
    class SharepointConnection {

        private static ClientContext createClientContext(string url) {
            string test = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\","");
            string path = Path.Combine(test, "ReferenceConfiguratorCert.pfx");
            string jsonFile = Path.Combine(test, "settings.json");
            dynamic json;
            using (StreamReader reader = new StreamReader(jsonFile)) {
                string file = reader.ReadToEnd();
                json = JsonConvert.DeserializeObject(file);
            }
            string code = json.code;
            var authManager = new AuthenticationManager("5dc404e0-76d3-4703-b5ba-d47d8187f6ba", path, code, "766355ba-7eb5-41f6-bd85-dd049c28169c");
            ClientContext ctx = authManager.GetContext(url);
            return ctx;
        }


        public static ClientContext GetClientContext(string url) {
            return SharepointConnection.createClientContext(url);
        }

        public static ListItemCollection getSharepointListReference() {
            ClientContext ctx = GetClientContext(Settings.Default.url);
            List list = ctx.Web.Lists.GetByTitle("Project list");
            ListItemCollection itemColl = list.GetItems(CamlQuery.CreateAllItemsQuery());
            ctx.Load(itemColl);
            ctx.ExecuteQuery();
            return itemColl;
        }

        public static ListItemCollection getSharepointListProfile() {
            ClientContext ctx = GetClientContext(Settings.Default.template);
            List list = ctx.Web.Lists.GetByTitle("Input for CVs");
            ListItemCollection itemColl = list.GetItems(CamlQuery.CreateAllItemsQuery());
            ctx.Load(itemColl);
            ctx.ExecuteQuery();
            return itemColl;
        }
        public static FileCollection getSharepointListLogo() {
            ClientContext ctx = GetClientContext(Settings.Default.template);
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
                            return fileCol;
                        }
                    }
                }
            }
            return null;
        }

        public static void downloadPowerpointTemplates(string type) {
            
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderName = "";
            if (type == "Profile") {
                folderName = "ReferenceConfigurator/powerpointTemplate/profile";
            } else if( type == "Reference"){
                folderName = "ReferenceConfigurator/powerpointTemplate/reference";
            }
            string folderPath = Path.Combine(basePath, folderName);

            if (System.IO.Directory.Exists(folderPath)) { return; }

            System.IO.Directory.CreateDirectory(folderPath);

            ClientContext ctx = GetClientContext(Settings.Default.template);
            List list = ctx.Web.Lists.GetByTitle("Dokumente");
            ctx.Load(list);
            ctx.ExecuteQuery();
            FolderCollection fcol = list.RootFolder.Folders;
            ctx.Load(fcol);
            ctx.ExecuteQuery();

            foreach (Folder f_first in fcol) {
                if (f_first.Name == "Templates Reference Configurator") {
                    ctx.Load(f_first.Folders);
                    ctx.ExecuteQuery();
                    FolderCollection fileCol_first = f_first.Folders;
                    foreach (Folder f_second in fileCol_first) {
                        if(f_second.Name != type) {
                            continue;
                        }
                        ctx.Load(f_second.Files);
                        ctx.ExecuteQuery();
                        FileCollection fileCol = f_second.Files;

                        foreach (Microsoft.SharePoint.Client.File file in fileCol) {
                            //TODO check for files instead of forcing download
                            var fileName = Path.Combine(basePath, folderName, (string)file.Name);
                            downloadFileHttp(Settings.Default.template, fileName, file);
                        }
                    }
                }
            }
        }

        public static string downloadCompanyLogo(string name) {
            string ogName = name;
            name = name.Replace("/", "_");
            name = name.Replace(":", "_");
            name = name.Replace("&", "_");


            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/CompanyLogo");

            System.IO.Directory.CreateDirectory(folderPath);
            string [] images = Directory.GetFiles(folderPath + "/", name + ".*");
            if (images.Length > 0) {
                return images[0];
            }
            

            ClientContext ctx = GetClientContext(Settings.Default.template);
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
                                if (string.Equals(name, Path.GetFileNameWithoutExtension(file.Name), StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(ogName, Path.GetFileNameWithoutExtension(file.Name), StringComparison.OrdinalIgnoreCase)) {
                                    var fileName = Path.Combine(basePath, "ReferenceConfigurator/CompanyLogo", (string)file.Name);

                                    downloadFileHttp(Settings.Default.template, fileName, file);
                                    return fileName;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static void downloadFileHttp(string siteUrl,string filename,Microsoft.SharePoint.Client.File file) {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReferenceConfiguratorCert.pfx");
            AuthenticationManager auth = new AuthenticationManager("5dc404e0-76d3-4703-b5ba-d47d8187f6ba", path, "G6ckxV7V&kX7cZveIpGkv0kViB=RgbNF?Z", "766355ba-7eb5-41f6-bd85-dd049c28169c");
            // Get the Unique ID via REST not needed
            string accessToken = auth.GetAccessToken(siteUrl);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization" , "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

            var uri = new Uri(siteUrl + "/_layouts/15/download.aspx?UniqueId=" + file.UniqueId);
            var taskDownloadFile = Task.Run(() => client.GetByteArrayAsync(uri));
            taskDownloadFile.Wait();
            var response = taskDownloadFile.Result;
            System.IO.File.WriteAllBytes(filename, response);
        }

        public static async Task downloadFileHttpAsync(string siteUrl, string filename, Microsoft.SharePoint.Client.File file) {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReferenceConfiguratorCert.pfx");
            AuthenticationManager auth = new AuthenticationManager("5dc404e0-76d3-4703-b5ba-d47d8187f6ba", path, "G6ckxV7V&kX7cZveIpGkv0kViB=RgbNF?Z", "766355ba-7eb5-41f6-bd85-dd049c28169c");
            string accessToken = auth.GetAccessToken(siteUrl);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");

            

            var uri = new Uri(siteUrl + "/_layouts/15/download.aspx?UniqueId=" + file.UniqueId);
            var taskDownloadFile =await client.GetByteArrayAsync(uri);
            using FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            await fs.WriteAsync(taskDownloadFile, 0, taskDownloadFile.Length);
        }

        public static string downloadOnePager(string id) {
           
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/onePager");

            System.IO.Directory.CreateDirectory(folderPath);

            string[] onePager = Directory.GetFiles(folderPath + "/", id + "_*.*");
            if (onePager.Length > 0) {
                return onePager[0];
            }

            ClientContext ctx = GetClientContext(Settings.Default.onePager);
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
                                    downloadFileHttp(Settings.Default.onePager,fileName, file);
                                    //var localstream = System.IO.File.Open(fileName, System.IO.FileMode.Create);
                                    //var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, file.ServerRelativeUrl);
                                    //var spstream = fileInfo.Stream;
                                    //spstream.CopyTo(localstream);
                                    //spstream.Close();
                                    //localstream.Close();
                                    return fileName;
                                }
                            }
                        }
                    }
                    
                }
            }
            return "";
        } 

        public static string downloadLanguageFlag(string language) {
           
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/Languages");

            System.IO.Directory.CreateDirectory(folderPath);
            string[] images = Directory.GetFiles(folderPath + "/", language + ".*");
            if (images.Length > 0) {
                return images[0];
            }


            ClientContext ctx = GetClientContext(Settings.Default.template);
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
                        if (f2.Name == "Flags") {
                            ctx.Load(f2.Files);
                            ctx.ExecuteQuery();
                            FileCollection fileCol = f2.Files;

                            foreach (Microsoft.SharePoint.Client.File file in fileCol) {
                                if (string.Equals(language, Path.GetFileNameWithoutExtension(file.Name), StringComparison.OrdinalIgnoreCase)) {
                                    var fileName = Path.Combine(basePath, "ReferenceConfigurator/Languages", (string)file.Name);

                                    downloadFileHttp(Settings.Default.template, fileName, file);
                                    return fileName;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static string downloadLanguageProfilePicture(string name) {

            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/ProfilePictures");

            System.IO.Directory.CreateDirectory(folderPath);
            string[] images = Directory.GetFiles(folderPath + "/", name + ".*");
            if (images.Length > 0) {
                return images[0];
            }


            ClientContext ctx = GetClientContext(Settings.Default.template);
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
                        if (f2.Name == "People Pictures") {
                            ctx.Load(f2.Files);
                            ctx.ExecuteQuery();
                            FileCollection fileCol = f2.Files;

                            foreach (Microsoft.SharePoint.Client.File file in fileCol) {
                                if (string.Equals(name, Path.GetFileNameWithoutExtension(file.Name), StringComparison.OrdinalIgnoreCase)) {
                                    var fileName = Path.Combine(basePath, "ReferenceConfigurator/ProfilePictures", (string)file.Name);

                                    downloadFileHttp(Settings.Default.template, fileName, file);
                                    return fileName;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
