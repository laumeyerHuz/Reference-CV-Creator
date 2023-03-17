using System;
using System.Collections.Generic;
using ReferenceConfigurator.models;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace ReferenceConfigurator.powerpointSlideCreator {
    public class PowerpointSlideCreator {

        private List<ReferenceModel> _referenceModels;
        private int _layoutIndex;
        private PowerPoint.Slide _currentSlide;

        private static readonly string BASE_PATH = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        private static readonly string SLIDE_TEMPLATE_PATH = Path.Combine(BASE_PATH, "ReferenceConfigurator/powerpointTemplate/powerpointTemplate.pptx");

        public PowerpointSlideCreator() {
            _currentSlide = Utils.GetActiveSlide();
        }
        
        public void addReferences(List<ReferenceModel> referenceModels) {
            this._referenceModels = referenceModels;
        }

        public void addLayoutIndex(int layoutIndex) {
            this._layoutIndex = layoutIndex+1;
        }

        public void createSlide() {
            var presentation = Globals.ThisAddIn.Application.ActivePresentation;
            var selectedSlidesNumbers = this.GetSelectedSlideNumbers(presentation);
            var firstSelectedSlide = selectedSlidesNumbers[0];

            // Insert slide from file.  Note that this step will NOT bring the theme
            // from the slide file, only it's text.
            presentation.Slides.InsertFromFile(SLIDE_TEMPLATE_PATH, firstSelectedSlide, _layoutIndex,_layoutIndex);

            // Apply the theme of the newly inserted slide file.  Note that the new 
            // slide gets inserted after selected slide, so add 1 to slide index
            // to target new slide for theme.
            presentation.Slides[firstSelectedSlide + 1].ApplyTheme(SLIDE_TEMPLATE_PATH);
            updateSlideContent(presentation.Slides[firstSelectedSlide + 1]);
        }

        private int[] GetSelectedSlideNumbers(Presentation presentation) {
            var selectedSlides = new List<int>();

            foreach (Slide slide in presentation.Slides.Application.ActiveWindow.Selection.SlideRange) {
                selectedSlides.Add(slide.SlideNumber);
            }

            selectedSlides = selectedSlides.OrderByDescending(x => x).ToList();

            return selectedSlides.ToArray();
        }

        private void updateSlideContent(Slide slide) {
            System.Diagnostics.Debug.WriteLine(slide.Shapes.Count);
            int count = 0;
            foreach (Shape s in slide.Shapes) {
                System.Diagnostics.Debug.WriteLine($"{s.Name}");
                if (s.Name.Contains("TextBox")) {
                    s.TextFrame.TextRange.Text = _referenceModels[count].ProjectName;
                    count++;
                }
            }
        }
    }
}
