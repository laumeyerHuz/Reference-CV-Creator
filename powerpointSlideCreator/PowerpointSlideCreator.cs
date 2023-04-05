using System;
using System.Collections.Generic;
using ReferenceConfigurator.models;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System.Linq;
using ReferenceConfigurator.utils;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace ReferenceConfigurator.powerpointSlideCreator {
    public class PowerpointSlideCreator {

        private List<ReferenceModel> _referenceModels;
        private LayoutModel _layout;

        public PowerpointSlideCreator() {
        }
        
        public void addReferences(List<ReferenceModel> referenceModels) {
            this._referenceModels = referenceModels;
        }

        public void addLayoutModel(LayoutModel layoutModel) {
            this._layout = layoutModel;
        }

        public void createSlide() {
            if (_layout.onePager) {
                addOnePagers();
            } else {
                var presentation = Globals.ThisAddIn.Application.ActivePresentation;
                var selectedSlidesNumbers = this.GetSelectedSlideNumbers(presentation);
                var firstSelectedSlide = selectedSlidesNumbers[0];

                // Insert slide from file.  Note that this step will NOT bring the theme
                // from the slide file, only it's text.
                presentation.Slides.InsertFromFile(_layout.powerpointPath, firstSelectedSlide, 1, 1);

                // Apply the theme of the newly inserted slide file.  Note that the new 
                // slide gets inserted after selected slide, so add 1 to slide index
                // to target new slide for theme.
                presentation.Slides[firstSelectedSlide + 1].ApplyTheme(_layout.powerpointPath);
                updateSlideContent(presentation.Slides[firstSelectedSlide + 1]);
            }
        }

        private void addOnePagers() {
            foreach (ReferenceModel model in _referenceModels) {
                string filePath = Utils.getOnePager(model.ProjectId);
                if (filePath == "") {
                    continue;
                }

                int targetSlideRange = Globals.ThisAddIn.Application.ActiveWindow.Presentation.Slides.Count;
                PowerPoint.Presentation target;
                PowerPoint.Presentation source;

                try {
                    target = Globals.ThisAddIn.Application.ActivePresentation;
                    source = Globals.ThisAddIn.Application.Presentations.Open(filePath, Office.MsoTriState.msoFalse, Office.MsoTriState.msoFalse, Office.MsoTriState.msoFalse);

                    int sourceSlideRange = source.Slides.Count + 1;

                    for (int i = 1; i < sourceSlideRange; i++) {
                        source.Slides[i].Copy();
                        target.Slides[targetSlideRange].Select();
                        target.Application.CommandBars.ExecuteMso("PasteSourceFormatting");
                        target.Save();
                    }
                    source.Close();
                } catch (Exception) {
                    MessageBox.Show("Error opening PowerPoint, corruption found inside the powerpoint file. " +
                                    Environment.NewLine + "The corrupted file has been deleted." + Environment.NewLine +
                                    "Please attempt to redownload file.",
                                    "Error Opening PowerPoint",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void updateSlideContent(Slide slide) {
            System.Diagnostics.Debug.WriteLine(slide.Shapes.Count);
            int count = 0;
            foreach (Shape s in slide.Shapes) {
                System.Diagnostics.Debug.WriteLine($"{s.Name}");
                if (s.Name.Contains("TextBox") && count <_referenceModels.Count) {
                    s.TextFrame.TextRange.Text = _referenceModels[count].ProjectName;
                    count++;
                }
            }
        }
    }
}
