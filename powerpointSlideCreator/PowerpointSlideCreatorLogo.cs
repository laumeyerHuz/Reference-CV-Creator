using Lucene.Net.Util;
using Microsoft.Office.Interop.PowerPoint;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.Office.Core.MsoTriState;

namespace ReferenceConfigurator.powerpointSlideCreator {
    public class PowerpointSlideCreatorLogo {
        private List<LogoModel> _searchModels;
        public PowerpointSlideCreatorLogo() { }

        public void addReferences(List<LogoModel> logoModel) {
            this._searchModels = logoModel;
        }

        public void addLogo() {
            loadLogos();
            int counter = 10;
            var presentation = Globals.ThisAddIn.Application.ActivePresentation;
            var selectedSlidesNumbers = this.GetSelectedSlideNumbers(presentation);
            var firstSelectedSlide = selectedSlidesNumbers[0];
            foreach (LogoModel model in _searchModels) {
                

                Slide slide = presentation.Slides[firstSelectedSlide];
                System.Drawing.Image img = System.Drawing.Image.FromFile(model.LogoFile);
                float[] sizes = resizeImage(100, 100, img.Width, img.Height, 20, counter);

                slide.Shapes.AddPicture(model.LogoFile, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
                counter += 100;
            }
        }
        private int[] GetSelectedSlideNumbers(Presentation presentation) {
            var selectedSlides = new List<int>();

            foreach (Slide slide in presentation.Slides.Application.ActiveWindow.Selection.SlideRange) {
                selectedSlides.Add(slide.SlideNumber);
            }

            selectedSlides = selectedSlides.OrderByDescending(x => x).ToList();

            return selectedSlides.ToArray();
        }

        private void loadLogos() {
            HashSet<string> clients = new HashSet<string>();
            foreach (var model in _searchModels) {
                clients.Add(model.CompanyName);
            }
            Dictionary<string, string> clientLogo = new Dictionary<string, string>();
            foreach (string client in clients) {
                string file = SharepointConnection.downloadCompanyLogo(client);
                clientLogo[client] = file;
            }
            foreach (LogoModel model in _searchModels) {
                model.LogoFile = clientLogo[model.CompanyName];
            }
        }

        private float[] resizeImage(float widthPowerPoint, float heightPowerPoint, int widthImage, int heightImage, float x, float y) {
            int original_width = widthImage;
            int original_height = heightImage;
            int bound_width = (int)widthPowerPoint;
            int bound_height = (int)heightPowerPoint;
            int new_width = original_width;
            int new_height = original_height;
            int middleX = (int)(x + (widthPowerPoint / 2));
            int middleY = (int)(y + (heightPowerPoint / 2));
            int newX = (int)x;
            int newY = (int)y;

            // first check if we need to scale width
            if (original_width > bound_width) {
                //scale width to fit
                new_width = bound_width;
                //scale height to maintain aspect ratio
                new_height = (new_width * original_height) / original_width;
            }

            // then check if we need to scale even with the new height
            if (new_height > bound_height) {
                //scale height to fit instead
                new_height = bound_height;
                //scale width to maintain aspect ratio
                new_width = (new_height * original_width) / original_height;
            }

            newX = middleX - (new_width / 2);
            newY = middleY - (new_height / 2);

            return new float[] { newX, newY, new_width, new_height };
        }
    }
}
