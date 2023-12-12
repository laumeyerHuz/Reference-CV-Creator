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
using PnP.Framework.Diagnostics;

namespace ReferenceConfigurator.powerpointSlideCreator {
    public class PowerpointSlideCreatorProfile {

        private List<ProfileModel> _searchModels;
        private ProfileLayoutModel _layout;
        private string _language;
        private string _title;

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

        public void addTitle(string  title) { 
            this._title = title; 
        }

        public void createSlide() {
            if (_layout.onePager) {
                if (_layout.name == "Profile One Pager") {
                    addOnePagers();
                } else {
                    addTwoPagers();
                }

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
                if (_layout.name == "Profile Template 0_5_0") {
                    updateSlideContentSpecial(presentation.Slides[firstSelectedSlide + 1]);
                } else {
                    updateSlideContent(presentation.Slides[firstSelectedSlide + 1]);

                }
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

        private void addTwoPagers() {
            for (int i = 0; i < _searchModels.Count; i = i + 2) {
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
                ProfileModel one = _searchModels[i];
                ProfileModel two = null;
                if (i < _searchModels.Count) {
                    two = _searchModels[i + 1];
                }
                updateTwoPagerContent(presentation.Slides[firstSelectedSlide + 1], one, two);
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

        private void updateSlideContentSpecial(Slide slide) {
            loadProfilePictures();
            for (int i = slide.Shapes.Count; i >= 1; i--) {
                Shape s = slide.Shapes[i];
                if (Utils.istObjectOfInterest(s.Name)) {
                    string placeholder = s.TextFrame.TextRange.Text;
                    string[] split = placeholder.Split(' ');
                    switch (split[0]) {
                        case "role":
                            int p = split[1].ToInt32() - 1;
                            if (p < _searchModels.Count()) {
                                if (_language == "DE") {
                                    s.TextFrame.TextRange.Text = _searchModels[p].RoleDE;
                                } else if (_language == "EN") {
                                    s.TextFrame.TextRange.Text = _searchModels[p].RoleEN;
                                } else {
                                    Growl.Info("No language selected");
                                }
                            }
                            break;
                        case "Name":
                            p = split[1].ToInt32() - 1;
                            if (p < _searchModels.Count()) {
                                s.TextFrame.TextRange.Text = _searchModels[p].FirstName + "\n" + _searchModels[p].LastName;
                            }
                            break;
                        case "Profile":
                            p = split[1].ToInt32() - 1;
                            if (p<_searchModels.Count() && _searchModels[p].ProfilePicture != null) {
                                System.Drawing.Image img = System.Drawing.Image.FromFile(_searchModels[p].ProfilePicture);
                                float[] sizes = resizeImage(s.Width, s.Height, img.Width, img.Height, s.Left, s.Top);
                                var pic = slide.Shapes.AddPicture(_searchModels[p].ProfilePicture, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
                                pic.Width = sizes[2]; pic.Height = sizes[3];
                                pic.Left = s.Left; pic.Top = s.Top;
                                s.Delete();
                            } else { 
                                s.Delete(); 
                            }
                            break;
                        case "Your":
                            s.TextFrame.TextRange.Text = _title; 
                            break;
                    }
                }
            }
        }

        private void updateSlideContent(Slide slide) {
            List<ProfileModel> partnerList = new List<ProfileModel>();
            List<ProfileModel> coreList = new List<ProfileModel>();
            List<ProfileModel> expertList = new List<ProfileModel>();
            ProfileModel projectManger = null;
            foreach (ProfileModel model in _searchModels) {
                if (model.IsPartner) {
                    partnerList.Add(model);
                } else if (model.IsExpert) {
                    expertList.Add(model);
                } else if (model.IsLeader) {
                    projectManger = model;
                } else {
                    coreList.Add(model);
                }
            }
            partnerList = Utils.sortProfile(partnerList);
            expertList = Utils.sortProfile(expertList);
            coreList = Utils.sortProfile(coreList);
            if (projectManger != null) {
                coreList.Insert(0, projectManger);

            }
            loadProfilePictures();
            for (int i = slide.Shapes.Count; i >= 1; i--) {
                Shape s = slide.Shapes[i];
                System.Diagnostics.Debug.WriteLine(s.Name);
                if (Utils.istObjectOfInterest(s.Name)) {
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
                                var pic = slide.Shapes.AddPicture(logo.ProfilePicture, msoFalse, msoTrue, s.Left, s.Top);
                                pic.Width = s.Width; pic.Height = s.Height;
                                pic.Left = s.Left; pic.Top = s.Top;
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
                                        additions = name.RoleEN + "\n" + name.YearsWorkExperience + " yrs. experience";
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
                                    if (additions.Length >= 2) {
                                        additions = additions.Remove(additions.Length - 2);
                                    }
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
                                s.TextFrame.TextRange.Words(0, l).Font.Bold = msoTrue;
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
                        case "We":
                            s.TextFrame.TextRange.Text = _title;
                            break;
                        case "HZ":
                            if(_language == "EN") {
                                s.TextFrame.TextRange.Text = "H&Z team – selection of possible profiles";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "H&Z Team - Auswahl der möglichen Profile";
                            }
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
                if (Utils.istObjectOfInterest(s.Name)) {
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
                                s.TextFrame.TextRange.Text = current.YearsWorkExperience + " yrs. experience\nin consulting/\nindustry";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = current.YearsWorkExperience + " Jahre Erfahrung\nin der Beratung/\nder Industrie";
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "professional":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.ProfessionalExperienceEN);
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.ProfessionalExperienceDE);
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "project":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.ProjectExperienceEN);
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.ProjectExperienceDE);
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "industry":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.IndustryExperienceEN);
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Industrie Erfahrung:\n" + Utils.RemoveEmptyLines(current.IndustryExperienceDE);
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "functional":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.FunctionalExperienceEN);
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Funktionale Erfahrung:\n" + Utils.RemoveEmptyLines(current.FunctionalExperienceDE);
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "education":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.EducationAndTrainingEN);
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current.EnductionAndTrainingDE);
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "Profile":
                            if (current.ProfilePicture != null) {
                                System.Drawing.Image img = System.Drawing.Image.FromFile(current.ProfilePicture);
                                float[] sizes = resizeImage(s.Width, s.Height, img.Width, img.Height, s.Left, s.Top);
                                var pic = slide.Shapes.AddPicture(current.ProfilePicture, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
                                pic.Width = sizes[2]; pic.Height = sizes[3];
                                pic.Left = s.Left; pic.Top = s.Top;
                                s.Delete();
                            }
                            break;
                        case "Lang 1":
                            if (current.flags.Length > 0) {
                                if (current.flags[0] != null) {
                                    System.Drawing.Image imgL = System.Drawing.Image.FromFile(current.flags[0]);
                                    float[] sizesL = resizeImage(s.Width, s.Height, imgL.Width, imgL.Height, s.Left, s.Top);
                                    var pic = slide.Shapes.AddPicture(current.flags[0], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    pic.Width = sizesL[2]; pic.Height = sizesL[3];
                                    pic.Left = s.Left; pic.Top = s.Top;
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
                                    var pic = slide.Shapes.AddPicture(current.flags[1], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    pic.Width = sizesL[2]; pic.Height = sizesL[3];
                                    pic.Left = s.Left; pic.Top = s.Top;
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
                                    var pic = slide.Shapes.AddPicture(current.flags[2], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    pic.Width = sizesL[2]; pic.Height = sizesL[3];
                                    pic.Left = s.Left; pic.Top = s.Top;
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
                                    var pic = slide.Shapes.AddPicture(current.flags[3], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                    pic.Width = sizesL[2]; pic.Height = sizesL[3];
                                    pic.Left = s.Left; pic.Top = s.Top;
                                    s.Delete();
                                }
                            } else {
                                s.Delete();
                                break;
                            }
                            break;
                        case "Project experience:":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Project experience:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Projekt Erfahrung:";
                            }
                            break;
                        case "Industry experience:":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Industry experience:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Industrie Erfahrung:";
                            }
                            break;
                        case "Functional experience:":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Functional experience:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Funktionale Erfahrung:";
                            }
                            break;
                        case "Education and training:":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Education and training:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Bildung und Fortbildung:";
                            }
                            break;
                        case "Professional experience:":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Professional experience:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Berufserfahrung:";
                            }
                            break;
                        default: break;
                    }
                }
            }
        }

        private void updateTwoPagerContent(Slide slide, ProfileModel current1, ProfileModel current2) {
            loadFlags();
            loadProfilePictures();
            for (int i = slide.Shapes.Count; i >= 1; i--) {
                Shape s = slide.Shapes[i];
                if (s.Name.Contains("TextBox") || s.Name.Contains("Textplatzhalter") || s.Name.Contains("Text") || s.Name.Contains("Title") || s.Name.Contains("Titel")) {
                    string placeholder = s.TextFrame.TextRange.Text;
                    string[] split = placeholder.Split(' ');
                    switch (split[0]) {
                        case "role":
                            if (_language == "DE") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = current1.RoleDE;
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = current2.RoleDE;
                                }
                            } else if (_language == "EN") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = current1.RoleEN;
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = current2.RoleEN;
                                }
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "Name":
                            if (split[1] == "1") {
                                s.TextFrame.TextRange.Text = current1.FirstName + " " + current1.LastName;
                            } else if (split[1] == "2") {
                                s.TextFrame.TextRange.Text = current2.FirstName + " " + current2.LastName;
                            }
                            break;
                        case "years":
                            if (_language == "EN") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = current1.YearsWorkExperience + " yrs. experience";
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = current2.YearsWorkExperience + " yrs. experience";
                                }
                            } else if (_language == "DE") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = current1.YearsWorkExperience + " Jahre Erfahrung";
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = current2.YearsWorkExperience + " Jahre Erfahrung";
                                }
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "project":
                            if (_language == "EN") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current1.ProjectExperienceEN);
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current2.ProjectExperienceEN);
                                }
                            } else if (_language == "DE") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current1.ProjectExperienceDE);
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current2.ProjectExperienceDE);
                                }
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "industry":
                            if (_language == "EN") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current1.IndustryExperienceEN);
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current2.IndustryExperienceEN);
                                }
                            } else if (_language == "DE") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current1.IndustryExperienceDE);
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current2.IndustryExperienceDE);
                                }
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "functional":
                            if (_language == "EN") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current1.FunctionalExperienceEN);
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current2.FunctionalExperienceEN);
                                }
                            } else if (_language == "DE") {
                                if (split[1] == "1") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current1.FunctionalExperienceDE);
                                } else if (split[1] == "2") {
                                    s.TextFrame.TextRange.Text = Utils.RemoveEmptyLines(current2.FunctionalExperienceDE);
                                }
                            } else {
                                Growl.Info("No language selected");
                            }
                            break;
                        case "Profile":
                            if (split[1] == "1") {
                                if (current1.ProfilePicture != null) {
                                    System.Drawing.Image img = System.Drawing.Image.FromFile(current1.ProfilePicture);
                                    float[] sizes = resizeImage(s.Width, s.Height, img.Width, img.Height, s.Left, s.Top);
                                    var pic = slide.Shapes.AddPicture(current1.ProfilePicture, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
                                    pic.Width = sizes[2]; pic.Height = sizes[3];
                                    pic.Left = s.Left; pic.Top = s.Top;
                                    s.Delete();
                                }
                            } else if (split[1] == "2") {
                                if (current2.ProfilePicture != null) {
                                    System.Drawing.Image img = System.Drawing.Image.FromFile(current2.ProfilePicture);
                                    float[] sizes = resizeImage(s.Width, s.Height, img.Width, img.Height, s.Left, s.Top);
                                    var pic = slide.Shapes.AddPicture(current2.ProfilePicture, msoFalse, msoTrue, sizes[0], sizes[1], sizes[2], sizes[3]);
                                    pic.Width = sizes[2]; pic.Height = sizes[3];
                                    pic.Left = s.Left; pic.Top = s.Top;
                                    s.Delete();
                                }
                            }
                            break;
                        case "Lang":
                            string p = split[1].Split('_')[0];
                            int n = split[1].Split('_')[1].ToInt32();
                            if (p == "1") {
                                if (current1.flags.Length > 0) {
                                    if (n <= current1.flags.Length && current1.flags[n - 1] != null) {
                                        System.Drawing.Image imgL = System.Drawing.Image.FromFile(current1.flags[n - 1]);
                                        float[] sizesL = resizeImage(s.Width, s.Height, imgL.Width, imgL.Height, s.Left, s.Top);
                                        var pic = slide.Shapes.AddPicture(current1.flags[n - 1], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                        pic.Width = sizesL[2]; pic.Height = sizesL[3];
                                        pic.Left = s.Left; pic.Top = s.Top;
                                        s.Delete();
                                    } else {
                                        s.Delete();
                                    }
                                } else {
                                    s.Delete();
                                }
                            } else if (p == "2") {
                                if (current2.flags.Length > 0) {
                                    if (n <= current2.flags.Length && current2.flags[n - 1] != null) {
                                        System.Drawing.Image imgL = System.Drawing.Image.FromFile(current2.flags[n - 1]);
                                        float[] sizesL = resizeImage(s.Width, s.Height, imgL.Width, imgL.Height, s.Left, s.Top);
                                        var pic = slide.Shapes.AddPicture(current2.flags[n - 1], msoFalse, msoTrue, sizesL[0], sizesL[1], sizesL[2], sizesL[3]);
                                        pic.Width = sizesL[2]; pic.Height = sizesL[3];
                                        pic.Left = s.Left; pic.Top = s.Top;
                                        s.Delete();
                                    } else {
                                        s.Delete();
                                    }
                                } else {
                                    s.Delete();
                                }
                            }
                            break;
                        case "Project":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Project experience:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Projekt Erfahrung:";
                            }
                            break;
                        case "Industry":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Industry experience:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Industrie Erfahrung:";
                            }
                            break;
                        case "Functional":
                            if (_language == "EN") {
                                s.TextFrame.TextRange.Text = "Functional experience:";
                            } else if (_language == "DE") {
                                s.TextFrame.TextRange.Text = "Funktionale Erfahrung:";
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
