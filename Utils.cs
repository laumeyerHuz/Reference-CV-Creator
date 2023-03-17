﻿#nullable enable


using Azure;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Interop;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace ReferenceConfigurator
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

        public static void SlidesToImage() {
            try {
                Application pptApplication = new Application();
                Presentation pptPresentation = pptApplication.Presentations
                .Open("powerpointTemplate/powerpointTemplate.pptx", MsoTriState.msoFalse, MsoTriState.msoFalse
                , MsoTriState.msoFalse);

                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string indexPath = Path.Combine(basePath, "ReferenceConfigurator/slides/");

                for (int i = 0;i < pptPresentation.Slides.Count; i++) {
                    pptPresentation.Slides[1].Export(indexPath +"slide" + i +".png", "png", 320, 240);
                }
                
            } catch (Exception e) { 
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }
    }
}
