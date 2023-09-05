﻿using System;
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
            List<ProfileModel> partnerList = new List<ProfileModel>();
            List<ProfileModel> coreList = new List<ProfileModel>();
            List<ProfileModel> expertList = new List<ProfileModel>();
            foreach (ProfileModel model in _searchModels) {
                if (model.IsPartner) {
                    partnerList.Add(model);
                } else if (model.IsExpert) {
                    expertList.Add(model);
                } else if (model.IsLeader) {
                    coreList.Insert(0, model);
                } else {
                    coreList.Add(model);
                }
            }
            loadProfilePictures();
            for (int i = slide.Shapes.Count; i >= 1; i--) {
                Shape s = slide.Shapes[i];
                System.Diagnostics.Debug.WriteLine(s.Name);
                if (s.Name.Contains("TextBox") || s.Name.Contains("Textplatzhalter") || s.Name.Contains("Text") || s.Name.Contains("Title") || s.Name.Contains("Titel")) {
                    string placeholder = s.TextFrame.TextRange.Text;
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                    placeholder = rgx.Replace(placeholder, "");
                    string[] split = placeholder.Split(' ');
                    switch (split[0]) {
                        case "logo":
                            ProfileModel logo = null;
                            if (split[1] == "core") {
                                if (split.Count() > 2 && split[2].ToInt32() <= _layout.core && split[2].ToInt32() <= coreList.Count()) {
                                    logo = coreList[split[2].ToInt32() - 1];
                                }
                            } else if (split[1] == "partner") {
                                if (split.Count() > 2 && split[2].ToInt32() <= _layout.partner && split[2].ToInt32() <= partnerList.Count()) {
                                    logo = partnerList[split[2].ToInt32() - 1];
                                }
                            } else if (split[1] == "expert") {
                                if (split.Count() > 2 && split[2].ToInt32() <= _layout.expert && split[2].ToInt32() <= expertList.Count()) {
                                    logo = expertList[split[2].ToInt32() - 1];
                                }
                            }
                            if (logo == null) {
                                break;
                            } else if (logo.ProfilePicture != null) {
                                //System.Drawing.Image img = System.Drawing.Image.FromFile(logo.ProfilePicture);
                                //float[] sizes = resizeImage(s.Width, s.Height, img.Width, img.Height, s.Left, s.Top);
                                //slide.Shapes.AddPicture(logo.ProfilePicture, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
                                slide.Shapes.AddPicture(logo.ProfilePicture, msoFalse, msoTrue, s.Left, s.Top, s.Width, s.Height);

                                s.Delete();
                            }
                            break;
                        case "name":
                            ProfileModel name = null;
                            string additions = "";
                            if (split[1] == "core") {
                                if (split.Count() > 2 && split[2].ToInt32() <= _layout.core && split[2].ToInt32() <= coreList.Count()) {
                                    name = coreList[split[2].ToInt32() - 1];
                                    if (_language == "EN") {
                                        additions = name.RoleEN + "\n" + name.YearsWorkExperience + " years of experience";
                                    } else if (_language == "DE") {
                                        additions = name.RoleDE + "\n" + name.YearsWorkExperience + " Jahre Erfahrung";
                                    }
                                }

                            } else if (split[1] == "partner") {
                                if (split.Count() > 2 && split[2].ToInt32() <= _layout.partner && split[2].ToInt32() <= partnerList.Count()) {
                                    name = partnerList[split[2].ToInt32() - 1];
                                    foreach (CheckBoxModel ep in name.ProjectExperiencesDisplay) {
                                        if (ep.IsChecked) {
                                            additions += ep.Name + ", ";
                                        }
                                    }
                                    additions = additions.Remove(additions.Length - 2);
                                }


                            } else if (split[1] == "expert") {
                                if (split.Count() > 2 && split[2].ToInt32() <= _layout.expert && split[2].ToInt32() <= expertList.Count()) {
                                    name = expertList[split[2].ToInt32() - 1];
                                }
                            }
                            if (name == null) {
                                break;
                            } else if (split[1] == "core") {
                                s.TextFrame.TextRange.Text = name.FirstName + " " + name.LastName + "\n" + additions;
                                int l = name.FirstName.Split(' ').Length + name.LastName.Split(' ').Length;
                                s.TextFrame.TextRange.Words(0, l).Font.Bold = msoTrue;
                            } else if (split[1] == "partner") {
                                s.TextFrame.TextRange.Text = name.FirstName + " " + name.LastName + " – Partner\n" + additions;
                                int l = name.FirstName.Split(' ').Length + name.LastName.Split(' ').Length;
                                s.TextFrame.TextRange.Words(0, l + 2).Font.Bold = msoTrue;
                            } else if (split[1] == "expert") {
                                s.TextFrame.TextRange.Text = name.FirstName + " " + name.LastName;
                                int l = name.FirstName.Split(' ').Length + name.LastName.Split(' ').Length;
                                s.TextFrame.TextRange.Words(0, l).Font.Bold = msoTrue;

                            }
                            break;
                        case "experience":
                            ProfileModel experience = null;
                            if (split.Count() > 1 && split[1].ToInt32() <= _layout.core && split[1].ToInt32() <= coreList.Count()) {
                                experience = coreList[split[1].ToInt32() - 1];
                            }
                            if (experience == null) { break; }
                            string exp = "";
                            foreach (CheckBoxModel ep in experience.ProjectExperiencesDisplay) {
                                if (ep.IsChecked) {
                                    exp += ep.Name + "\n";
                                }
                            }
                            if (exp.Length > 2) {
                                exp = exp.Remove(exp.Length - 1);
                            }
                            s.TextFrame.TextRange.Text = exp;
                            break;
                        default: break;
                    }
                }
            }
        }

        private void updateOnePagerContent(Slide slide, ProfileModel current) {
            loadFlags();
            loadProfilePictures();
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
                                s.TextFrame.TextRange.Text = current.ProfessionalExperienceEN;
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
                        case "Profile":
                            if (current.ProfilePicture != null) {
                                System.Drawing.Image img = System.Drawing.Image.FromFile(current.ProfilePicture);
                                float[] sizes = resizeImage(s.Width, s.Height, img.Width, img.Height, s.Left, s.Top);
                                slide.Shapes.AddPicture(current.ProfilePicture, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
                                s.Delete();
                            }
                            break;
                        case "Lang 1":
                            if (current.flags.Length > 0) {
                                if (current.flags[0] != null) {
                                    System.Drawing.Image imgL = System.Drawing.Image.FromFile(current.flags[0]);
                                    float[] sizesL = resizeImage(s.Width, s.Height, imgL.Width, imgL.Height, s.Left, s.Top);
                                    slide.Shapes.AddPicture(current.flags[0], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    s.Delete();
                                }
                            } else {
                                s.Delete();
                                break;
                            }
                            break;
                        case "Lang 2":
                            if (current.flags.Length > 1) {
                                if (current.flags[1] != null) {
                                    System.Drawing.Image imgL = System.Drawing.Image.FromFile(current.flags[1]);
                                    float[] sizesL = resizeImage(s.Width, s.Height, imgL.Width, imgL.Height, s.Left, s.Top);
                                    slide.Shapes.AddPicture(current.flags[1], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    s.Delete();
                                }
                            } else {
                                s.Delete();
                                break;
                            }
                            break;
                        case "Lang 3":
                            if (current.flags.Length > 2) {
                                if (current.flags[2] != null) {
                                    System.Drawing.Image imgL = System.Drawing.Image.FromFile(current.flags[2]);
                                    float[] sizesL = resizeImage(s.Width, s.Height, imgL.Width, imgL.Height, s.Left, s.Top);
                                    slide.Shapes.AddPicture(current.flags[2], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    s.Delete();
                                }
                            } else {
                                s.Delete();
                                break;
                            }
                            break;
                        case "Lang 4":
                            if (current.flags.Length > 3) {
                                if (current.flags[3] != null) {
                                    System.Drawing.Image imgL = System.Drawing.Image.FromFile(current.flags[3]);
                                    float[] sizesL = resizeImage(s.Width, s.Height, imgL.Width, imgL.Height, s.Left, s.Top);
                                    slide.Shapes.AddPicture(current.flags[3], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    s.Delete();
                                }
                            } else {
                                s.Delete();
                                break;
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

            // first check if we need to scale height
            if (original_height > bound_height) {
                //scale height to fit
                new_height = bound_height;
                //scale width to maintain aspect ratio
                new_width = (new_height * original_width) / original_height;
            }

            //// then check if we need to scale even with the new height
            //if (new_height > bound_height) {
            //    //scale height to fit instead
            //    new_height = bound_height;
            //    //scale width to maintain aspect ratio
            //    new_width = (new_height * original_width) / original_height;
            //}

            newX = middleX - (new_width / 2);
            newY = middleY - (new_height / 2);

            return new float[] { newX, newY, new_width, new_height };
        }

        private void loadFlags() {
            HashSet<string> languages = new HashSet<string>();
            foreach (var model in _searchModels) {
                foreach (string lang in model.LanguagesEN.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    languages.Add(lang);
            }
            Dictionary<string, string> languageFlag = new Dictionary<string, string>();
            foreach (string lang in languages) {
                string file = SharepointConnection.downloadLanguageFlag(lang);
                languageFlag[lang] = file;
            }
            foreach (ProfileModel model in _searchModels) {
                string[] flags = new string[model.LanguagesEN.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Count()];
                for (int i = 0; i < flags.Length; i++) {
                    flags[i] = languageFlag[model.LanguagesEN.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)[i]];
                }
                model.flags = flags;
            }
        }

        private void loadProfilePictures() {
            HashSet<string> pic = new HashSet<string>();
            foreach (var model in _searchModels) {
                pic.Add(model.FirstName + " " + model.LastName);
            }
            Dictionary<string, string> picList = new Dictionary<string, string>();
            foreach (string name in pic) {
                string file = SharepointConnection.downloadLanguageProfilePicture(name);
                picList[name] = file;
            }
            foreach (ProfileModel model in _searchModels) {
                model.ProfilePicture = picList[model.FirstName + " " + model.LastName];
            }
        }

    }
}
