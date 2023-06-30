using System;
using System.Collections.Generic;
using ReferenceConfigurator.models;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System.Linq;
using ReferenceConfigurator.utils;
using System.Windows.Forms;
using static Microsoft.Office.Core.MsoTriState;
using System.Text.RegularExpressions;
using HandyControl.Controls;

namespace ReferenceConfigurator.powerpointSlideCreator {
    public class PowerpointSlideCreatorProfile {

        private List<ProfileModel> _searchModels;
        private ProfileLayoutModel _layout;
        private string _language;

        public PowerpointSlideCreatorProfile() { }
        public void addReferences(List<ProfileModel> referenceModels) {
            this._searchModels = referenceModels;
        }

        public void addLayoutModel(ProfileLayoutModel layoutModel) {
            this._layout = layoutModel;
        }

        public void addLanguage(string language) {
            this._language = language;
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
            foreach (ProfileModel model in _searchModels) {
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
                updateOnePagerContent(presentation.Slides[firstSelectedSlide + 1], model);
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
            //loadLogos();
            //List<Shape> tmp = new List<Shape>();
            ////foreach(Shape s in slide.Shapes) {
            //for (int i = slide.Shapes.Count; i >= 1; i--) {
            //    Shape s = slide.Shapes[i];
            //    System.Diagnostics.Debug.WriteLine(s.Name);
            //    if (s.Name.Contains("TextBox") || s.Name.Contains("Textplatzhalter")) {
            //        string placeholder = s.TextFrame.TextRange.Text;
            //        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            //        placeholder = rgx.Replace(placeholder, "");
            //        string[] split = placeholder.Split(' ');
            //        if (split[1].ToInt32() > _referenceModels.Count) {
            //            continue;
            //        }
            //        switch (split[0]) {
            //            case "Logo":
            //                if (_referenceModels[split[1].ToInt32() - 1].Logo != null) {
            //                    System.Drawing.Image img = System.Drawing.Image.FromFile(_referenceModels[split[1].ToInt32() - 1].Logo);
            //                    float[] sizes = resizeImage(s.Width, s.Height, img.Width, img.Height, s.Left, s.Top);
            //                    slide.Shapes.AddPicture(_referenceModels[split[1].ToInt32() - 1].Logo, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
            //                    s.Delete();
            //                }
            //                break;
            //            case "Description":
            //                if (_language == "DE") {
            //                    s.TextFrame.TextRange.Text = _referenceModels[split[1].ToInt32() - 1].ProjectDescriptionDE;
            //                } else if (_language == "EN") {
            //                    s.TextFrame.TextRange.Text = _referenceModels[split[1].ToInt32() - 1].ProjectDescriptionEN;
            //                } else {
            //                    Growl.Info("No language selected");
            //                }
            //                break;
            //            default: break;
            //        }
            //    }
            //}
        }

        private void updateOnePagerContent(Slide slide, ProfileModel current) {
            loadFlags();
            for (int i = slide.Shapes.Count; i >= 1; i--) {
                Shape s = slide.Shapes[i];
                if (s.Name.Contains("TextBox") || s.Name.Contains("Textplatzhalter") || s.Name.Contains("Text") || s.Name.Contains("Title") || s.Name.Contains("Titel")) {
                    string placeholder = s.TextFrame.TextRange.Text;
                    switch (placeholder) {
                        case "Role":
                            if (_language == "DE") {
                                s.TextFrame.TextRange.Text = current.RoleDE;
                            } else if (_language == "EN") {
                                s.TextFrame.TextRange.Text = current.RoleEN;
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "Name":
                            s.TextFrame.TextRange.Text = current.FirstName + " " + current.LastName;
                            break;
                        case "years":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = current.YearsWorkExperience + " years experience\n in consulting/\n industry";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = current.YearsWorkExperience + " Jahre Erfahrung\n in der Beratung/\n der Industrie";
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "professional":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text =current.ProfessionalExperienceEN;
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = current.ProfessionalExperienceDE;
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "project":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = current.ProjectExperienceEN;
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = current.ProjectExperienceDE;
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "industry":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = current.IndustryExperienceEN;
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Industrie Erfahrung:\n" + current.IndustryExperienceDE;
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "functional":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = current.FunctionalExperienceEN;
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Funktionale Erfahrung:\n" + current.FunctionalExperienceDE;
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "education":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = current.EducationAndTrainingEN;
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = current.EnductionAndTrainingDE;
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        default: break;
                    }
                }
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

        private void loadFlags() {
            HashSet<string> languages = new HashSet<string>();
            foreach (var model in _searchModels) {
                foreach (string lang in model.LanguagesEN.Split(' '))
                    languages.Add(lang);
            }
            Dictionary<string, string> languageFlag = new Dictionary<string, string>();
            foreach (string lang in languages) {
                string file = SharepointConnection.downloadLanguageFlag(lang);
                languageFlag[lang] = file;
            }
            foreach (ProfileModel model in _searchModels) {
                string[] flags = new string[model.LanguagesEN.Split(' ').Count()];
                for (int i = 0; i < flags.Length; i++) {
                    flags[i] = languageFlag[model.LanguagesEN.Split(' ')[i]];
                }
                model.flags = flags;
            }
        }
    }
}
