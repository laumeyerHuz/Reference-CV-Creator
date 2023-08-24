#nullable enable

using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Interop;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace ReferenceConfigurator.utils
{
    public static class Utils
    {
        public static PowerPoint.DocumentWindow? GetActiveWindow()
        {
            var application = Globals.ThisAddIn.Application;
            if (application.Windows.Count == 0) { return null; }
            return application.ActiveWindow;
        }

        public static PowerPoint.Slide? GetActiveSlide()
        {
            var window = GetActiveWindow();
            if (window == null) { return null; }
            if (window.Presentation.Slides.Count == 0) { return null; }
            return (PowerPoint.Slide)window.View.Slide;
        }

        private static readonly int MAX_CACHE_SIZE = 1024;
        private static readonly LinkedList<KeyValuePair<string, Bitmap?>> IMAGE_CACHE = new LinkedList<KeyValuePair<string, Bitmap?>>();

        public static string RemoveEmptyLines(string s) {
            return Regex.Replace(s, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline);
        }
        private static Bitmap? DoLoadImageResource(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    try
                    {
                        return new Bitmap(Image.FromStream(asm.GetManifestResourceStream(resourceNames[i])));
                    }
                    catch (ArgumentException ex)
                    {
                        Debug.WriteLine($"Resource {resourceName} is not an image: {ex.Message}");
                    }
                }
            }
            return null;
        }

        public static Bitmap? LoadImageResource(string resourceName)
        {
            var node = IMAGE_CACHE.First;
            while (node != null && (node.Value.Key != resourceName))
            {
                node = node.Next;
            }
            if (node == null)
            {
                node = new LinkedListNode<KeyValuePair<string, Bitmap?>>(new KeyValuePair<string, Bitmap?>(
                    resourceName, DoLoadImageResource(resourceName)
                    ));
                while (IMAGE_CACHE.Count > MAX_CACHE_SIZE - 1)
                {
                    IMAGE_CACHE.RemoveLast();
                }
            }
            else
            {
                IMAGE_CACHE.Remove(node);
            }
            IMAGE_CACHE.AddFirst(node);
            return node.Value.Value;
        }

        public static List<LayoutModel> SlidesToImage(string type) {
            List<LayoutModel> layoutModels = new List<LayoutModel>();
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string baseFolderName = "ReferenceConfigurator/powerpointTemplate/";
            string folderName = "";
            if (type == "Profile") {
                folderName = "profile";
            } else if (type == "Reference") {
                folderName = "reference";
            }
            string filePath = Path.Combine(basePath,baseFolderName, folderName);
            string indexPath = Path.Combine(basePath, "ReferenceConfigurator/slides/",folderName);

            DirectoryInfo d = new DirectoryInfo(filePath);
            System.IO.Directory.CreateDirectory(indexPath);


            foreach (FileInfo file in d.GetFiles("*.pptx")) {
                string imagePath = indexPath +"/" + Path.GetFileNameWithoutExtension(file.Name) + ".png";

                if (File.Exists(imagePath)) {
                    if (type == "Profile") {
                        layoutModels.Add(new ProfileLayoutModel(file.FullName, imagePath, Path.GetFileNameWithoutExtension(file.Name)));
                    } else if (type == "Reference") {
                        layoutModels.Add(new ReferenceLayoutModel(file.FullName, imagePath, Path.GetFileNameWithoutExtension(file.Name)));
                    }
                    
                } else {
                    try {
                        Application pptApplication = new Application();
                        Presentation pptPresentation = pptApplication.Presentations
                        .Open(file.FullName, MsoTriState.msoFalse, MsoTriState.msoFalse
                        , MsoTriState.msoFalse);
                        pptPresentation.Slides[1].Export(imagePath, "png", 1920, 1080);
                        if (type == "Profile") {
                            layoutModels.Add(new ProfileLayoutModel(file.FullName, imagePath, Path.GetFileNameWithoutExtension(file.Name)));
                        } else if (type == "Reference") {
                            layoutModels.Add(new ReferenceLayoutModel(file.FullName, imagePath, Path.GetFileNameWithoutExtension(file.Name)));
                        }
                        pptPresentation.Close();

                    } catch (Exception e) {
                        System.Diagnostics.Debug.WriteLine(e.ToString());
                    }
                }
            }
            return layoutModels;
        }

        public static void downloadPowerpointTemplate(string type) {
            SharepointConnection.downloadPowerpointTemplates(type);
        }

        public static string getOnePager(int id) {
            string fileName = SharepointConnection.downloadOnePager(id);
            return fileName;
        }

        public static void removeOnePager() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/onePager");
            if(Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
        }

        public static void removeLogos() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/CompanyLogo");
            if (Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
        }

        public static void removeProfilePictures() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/ProfilePictures");
            if (Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
        }

        public static void removeTemplates() {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(basePath, "ReferenceConfigurator/powerpointTemplate");
            if (Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
            folderPath = Path.Combine(basePath, "ReferenceConfigurator/slides");
            if (Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }

        }
    }
}
