using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;


namespace SpriteLibrary
{
    internal partial class SpriteEntryForm : Form
    {
        SpriteController MyController;
        SpriteController PreviewController;

        ResourceManager myResources = null;
        List<SpriteInfo> SpriteInformation = new List<SpriteInfo>();
        SpriteInfo TempInformation = null;
        Size SnapGridSize = new Size(5,5);
        SpriteDatabase myDatabase = null;
        int CurrentSIIndex = -1;  //The information item we are editing.  -1 means it is a new one.
        int CurrentSIAnimation = -1;

        bool WeAreDragging = false;
        Point DragStart = new Point(-1, -1);
        Rectangle ChosenArea = new Rectangle(1,1,100,100);

        ToolTip myToolTip = new ToolTip();

        Sprite PreviewSprite = null;

        internal SpriteEntryForm(SpriteDatabase theDatabase, List<SpriteInfo> ListToWorkOn, Size GridSize)
        {
            InitializeComponent();
            myDatabase = theDatabase;
            myResources = myDatabase.GetResourceManager();
            SnapGridSize = GridSize;
            LocalSetup();
            SpriteInformation.AddRange(ListToWorkOn);
            if (SpriteInformation.Count > 0)
            {
                SelectNewIndex(0);
            }
        }

        private void LocalSetup()
        {
            //set up the controller for the image-choice window
            pbImageField.BackgroundImageLayout = ImageLayout.Stretch;
            pbImageField.BackgroundImage = new Bitmap(600, 800);
            MyController = new SpriteController(pbImageField);

            //set up the sprite controller for the preview window
            pbPreview.BackgroundImage = new Bitmap(400, 400);
            Graphics.FromImage(pbPreview.BackgroundImage).Clear(Color.Gray);
            pbPreview.BackgroundImageLayout = ImageLayout.Stretch;
            PreviewController = new SpriteController(pbPreview);

            myToolTip.AutoPopDelay = 5000;
            myToolTip.AutomaticDelay = 500;

            rbFromImage.Click += UpdateMenuClick;
            rbMirror.Click += UpdateMenuClick;
            rbRotation.Click += UpdateMenuClick;

            PopulateMenu();
            UpdateMenu();
            SpriteInformationToForm();
            UpdateMenu();

            myToolTip.SetToolTip(btnNewAnimation, "Create another animation for the current sprite.");
            myToolTip.SetToolTip(btnAnimBack, "Move to previous animation within this sprite.");
            myToolTip.SetToolTip(btnAnimFwd, "Move to next animation within this sprite.");
            myToolTip.SetToolTip(btnBack, "Move to previous sprite.");
            myToolTip.SetToolTip(btnFwd, "Move to next sprite.");
            myToolTip.SetToolTip(btnPreviewAnimBack, "Change preview to previous animation.");
            myToolTip.SetToolTip(btnPreviewAnimFwd, "Change preview to next animation.");
            myToolTip.SetToolTip(btnNewSprite, "Create a new sprite.");
            myToolTip.SetToolTip(btnDeleteAnim, "Delete the current animation you are looking at.");
            myToolTip.SetToolTip(btnDelSprite, "Delete the current sprite you are looking at.");

            Icon = Properties.Resources.SLIcon;
        }

        internal List<SpriteInfo> GetUpdatedList()
        {
            return SpriteInformation;
        }

        internal void SetIcon(Icon IconImage)
        {
            Icon = IconImage;
        }

        private void PopulateMenu()
        {
            ResourceManager rm = myResources;
            PopulateMenu(rm);

        }

        private void PopulateMenu(ResourceManager rm)
        {
            if (myResources == null) myResources = rm;
            ResourceSet RS = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            cbStartingImage.Items.Clear();
            foreach (DictionaryEntry entry in RS)
            {
                string resourceKey = entry.Key.ToString();
                object resource = entry.Value;
                if (resource is Image)
                {
                    cbStartingImage.Items.Add(resourceKey);
                }
            }
            cbStartingImage.SelectedIndex = 0;
        }

        private void UpdateMenu()
        {
            SuspendLayout();
            lblCountSprites.Text = CurrentSIIndex+":"+CurrentSIAnimation+" of " +  SpriteInformation.Count.ToString();
            if (TempInformation == null) SetUpEmptyInfo();

            //Put in numbers into the combo-box of which frame to base ourselves off of
            cbAnimation.Items.Clear();
            //We cannot base ourselves off an animation we have not created yet
            for(int i =0; i < CurrentSIAnimation; i++)
            {
                cbAnimation.Items.Add(i.ToString());
            }
            //Update the animation number text
            lblAnimationNumber.Text = CurrentSIAnimation.ToString() + " of " + (TempInformation.Animations.Count() -1);
            if (CurrentSIAnimation == 0)
            {
                rbFromImage.Checked = true;
                panelRadioButtons.Visible = false;
            }
            else
                panelRadioButtons.Visible = true;
            if(TempInformation.Animations.Count >1)
            {
                btnAnimBack.Enabled = true;
                btnAnimFwd.Enabled = true;
                btnPreviewAnimBack.Enabled = true;
                btnPreviewAnimFwd.Enabled = true;
            }
            else
            {
                btnAnimBack.Enabled = false;
                btnAnimFwd.Enabled = false;
                btnPreviewAnimBack.Enabled = false;
                btnPreviewAnimFwd.Enabled = false;
            }
            if (rbFromImage.Checked)
            {
                if(!TCTabPages.TabPages.Contains(tpFromImage))
                    TCTabPages.TabPages.Add(tpFromImage);
                if (TCTabPages.TabPages.Contains(tpMirrorRotate))
                    TCTabPages.TabPages.Remove(tpMirrorRotate);
            }
            else
            {
                if (TCTabPages.TabPages.Contains(tpFromImage))
                    TCTabPages.TabPages.Remove(tpFromImage);
                if (!TCTabPages.TabPages.Contains(tpMirrorRotate))
                    TCTabPages.TabPages.Add(tpMirrorRotate);
            }
            if(rbMirror.Checked)
            {
                cbMirrorH.Visible = true;
                cbMirrorV.Visible = true;
                tbRotation.Visible = false;
            }
            if (rbRotation.Checked)
            {
                cbMirrorH.Visible = false;
                cbMirrorV.Visible = false;
                tbRotation.Visible = true;
            }
            UpdateChosenAreaLabel();
            ResumeLayout();
        }

        private void UpdateMenuClick(object sender, EventArgs e)
        {
            UpdateMenu();
        }

        private void UpdateChosenAreaLabel()
        {
            lblChosenArea.Text = ChosenArea.X + "," + ChosenArea.Y + "," + ChosenArea.Width + "," + ChosenArea.Height;
            UpdateHighlightBox();
        }

        /// <summary>
        /// If multiple frames are selected, retrieve all of their rectangles
        /// </summary>
        /// <returns></returns>
        private List<Rectangle> AnimationFrameAreas()
        {
            List<Rectangle> Frames = new List<Rectangle>();
            Point start = ChosenArea.Location;
            int animations;
            int.TryParse(tbNumFrames.Text, out animations);
            Frames.Add(ChosenArea);
            Image tImage = myDatabase.GetImageFromName(cbStartingImage.SelectedItem.ToString(), true);

            for (int i=1; i< animations; i++)
            {
                start = new Point(start.X + ChosenArea.Width, start.Y);
                if(start.X >= tImage.Width)
                {
                    start.X = 0;
                    start.Y += ChosenArea.Height;
                }
                Rectangle tRec = new Rectangle(start.X, start.Y, ChosenArea.Width, ChosenArea.Height);
                Frames.Add(tRec);
            }
            return Frames;
        }

        private void UpdateHighlightBox()
        {
            int transparency = 50;
            Image NewFrontImage = new Bitmap(pbImageField.BackgroundImage.Width, pbImageField.BackgroundImage.Height);
            Color FillColor = Color.Gray;
            Brush brush = new SolidBrush(Color.FromArgb(transparency, FillColor.R, FillColor.G, FillColor.B));
            Brush nobrush = new SolidBrush(Color.FromArgb(0,0,0,0));
            List<Rectangle> areas = AnimationFrameAreas();
            using (Graphics G = Graphics.FromImage(NewFrontImage))
            {
                G.FillRectangle(brush, 0,0,NewFrontImage.Width,NewFrontImage.Height);
                GraphicsPath path = new GraphicsPath();
                foreach (Rectangle one in areas)
                {
                    path.AddRectangle(one);
                }
                G.SetClip(path);
                G.Clear(Color.Transparent);
                G.ResetClip();
                transparency = 50;
                FillColor = Color.Green;
                int increment = 10;
                if (areas.Count > 7) increment = 5;
                foreach (Rectangle one in areas)
                {
                    transparency += increment;
                    if (transparency > 150) transparency = 160;
                    brush = new SolidBrush(Color.FromArgb(transparency, FillColor.R, FillColor.G, FillColor.B));
                    G.FillRectangle(brush, one);
                }

            }
            pbImageField.Image = NewFrontImage;
            pbImageField.SizeMode = PictureBoxSizeMode.StretchImage;
            pbImageField.Invalidate();
        }

        internal void SetInitialSprite(int StartingSprite)
        {
            CurrentSIIndex = StartingSprite;
            if (CurrentSIIndex >= SpriteInformation.Count)
                CurrentSIIndex = SpriteInformation.Count - 1;
            if (CurrentSIIndex < -1) CurrentSIIndex = -1;
            WeHaveNewItem();
        }

        private void SetUpEmptyInfo()
        {
            string startingimage = cbStartingImage.Text; //grab whatever we were using last
            Console.WriteLine("Setting up an empty info rec.");
            if(startingimage == null || startingimage == "")//If we are not looking at anything yet
            {
                List<string> ImageNames = myDatabase.GetImageNames();
                if (ImageNames.Count > 0)
                    startingimage = ImageNames[0];
                foreach(string name in ImageNames)
                    Console.WriteLine("  Name: " + name);
            }
            Console.WriteLine("NewName=" + startingimage);
            TempInformation = new SpriteInfo();
            TempInformation.SpriteName = "";
            TempInformation.ViewPercent = 100;
            AnimationInfo AI = new AnimationInfo();
            AI.AnimSpeed = 200;
            AI.ImageName = startingimage;
            AI.FieldsToUse = AnimationType.SpriteDefinition;
            AI.Height = 100;
            AI.Width = 100;
            AI.StartPoint = new Point(0, 0);
            TempInformation.Animations.Add(AI);
        }

        private void cbStartingImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            //We have a selected item
            if (cbStartingImage.SelectedIndex >= 0)
            {
                //Load in a new image into our background
                Image NewImage = myDatabase.GetImageFromName(cbStartingImage.SelectedItem.ToString(),true);
                if (NewImage != null)
                {
                    MyController.ReplaceOriginalImage(new Bitmap(NewImage));
                    pbImageField.BackgroundImage = new Bitmap(NewImage);
                    pbImageField.Invalidate();
                }
            }            
        }

        /// <summary>
        /// Take the values stored in TempInformation and push it out to our form
        /// </summary>
        private void SpriteInformationToForm()
        {
            if (TempInformation == null) return;
            //For the main sprite information
            tbSpriteName.Text = TempInformation.SpriteName;
            tbDefaultSize.Text = TempInformation.ViewPercent.ToString();

            //From the current animation
            AnimationInfo AI = null;
            if (CurrentSIAnimation < 0) CurrentSIAnimation = 0;
            if (CurrentSIAnimation >= TempInformation.Animations.Count) CurrentSIAnimation = TempInformation.Animations.Count -1;
            if (CurrentSIAnimation < TempInformation.Animations.Count)
            {
                if (CurrentSIAnimation >= TempInformation.Animations.Count)
                    TempInformation.Animations.Add(new AnimationInfo());
                AI = TempInformation.Animations[CurrentSIAnimation];
                tbAmimationSpeed.Text = AI.AnimSpeed.ToString();
                tbRotation.Text = AI.RotationDegrees.ToString();
                cbStartingImage.Text = AI.ImageName;
                cbMirrorH.Checked = AI.MirrorHorizontally;
                cbMirrorV.Checked = AI.MirrorVertically;
                cbAnimation.Text = AI.AnimationToUse.ToString();
                tbNumFrames.Text = AI.NumFrames.ToString();
                //lblChosenArea.Text = AI.Width + "x" + AI.Height;
                ChosenArea = new Rectangle(AI.StartPoint.X, AI.StartPoint.Y, AI.Width, AI.Height);
                UpdateChosenAreaLabel();
                //Radio buttons
                if (AI.FieldsToUse == AnimationType.SpriteDefinition) rbFromImage.Checked = true;
                if (AI.FieldsToUse == AnimationType.Mirror) rbMirror.Checked = true;
                if (AI.FieldsToUse == AnimationType.Rotation) rbRotation.Checked = true;
            }
        }

        /// <summary>
        /// Take the values stored in TempInformation and push it out to our form
        /// </summary>
        private void FormToSpriteInformation()
        {
            if (TempInformation == null) return;
            //For the main sprite information

            TempInformation.SpriteName = tbSpriteName.Text;
            int.TryParse(tbDefaultSize.Text, out TempInformation.ViewPercent);

            //From the current animation
            AnimationInfo AI = null;
            if (CurrentSIAnimation < 0) CurrentSIAnimation = 0;
            if (CurrentSIAnimation >= TempInformation.Animations.Count) CurrentSIAnimation = TempInformation.Animations.Count - 1;
            if (CurrentSIAnimation < TempInformation.Animations.Count)
            {
                AI = TempInformation.Animations[CurrentSIAnimation];

                int.TryParse(tbAmimationSpeed.Text, out AI.AnimSpeed);
                AI.ImageName = cbStartingImage.Text;

                AI.MirrorHorizontally = cbMirrorH.Checked;
                AI.MirrorVertically = cbMirrorV.Checked;

                int.TryParse(tbRotation.Text, out AI.RotationDegrees);

                int.TryParse(cbAnimation.Text, out AI.AnimationToUse);
                int.TryParse(tbNumFrames.Text, out AI.NumFrames);

                AI.StartPoint = ChosenArea.Location;
                AI.Width = ChosenArea.Width;
                AI.Height = ChosenArea.Height;

                if (rbFromImage.Checked) AI.FieldsToUse = AnimationType.SpriteDefinition;
                if (rbMirror.Checked) AI.FieldsToUse = AnimationType.Mirror;
                if (rbRotation.Checked) AI.FieldsToUse = AnimationType.Rotation;
            }
        }

        /// <summary>
        /// Take the values stored in TempInformation and push it out to our form
        /// </summary>
        private bool ValuesDifferFromData()
        {
            if (TempInformation == null) return true;
            //For the main sprite information
            int tValue;

            if(TempInformation.SpriteName != tbSpriteName.Text) return true;
            int.TryParse(tbDefaultSize.Text, out tValue);
            if(tValue != TempInformation.ViewPercent)return true;

            //From the current animation
            AnimationInfo AI = null;
            if (CurrentSIAnimation < 0) CurrentSIAnimation = 0;
            if (CurrentSIAnimation >= TempInformation.Animations.Count) CurrentSIAnimation = TempInformation.Animations.Count - 1;
            if (CurrentSIAnimation < TempInformation.Animations.Count)
            {
                AI = TempInformation.Animations[CurrentSIAnimation];

                int.TryParse(tbAmimationSpeed.Text, out tValue);
                if (tValue != AI.AnimSpeed) return true;
                if(AI.ImageName != cbStartingImage.Text) return true;

                if(AI.MirrorHorizontally != cbMirrorH.Checked) return true;
                if(AI.MirrorVertically != cbMirrorV.Checked) return true;

                int.TryParse(cbAnimation.Text, out tValue);
                if (tValue != AI.AnimationToUse) return true;

                int.TryParse(tbNumFrames.Text, out tValue);
                if(tValue != AI.NumFrames) return true;

                if(AI.StartPoint != ChosenArea.Location) return true;
                if(AI.Width != ChosenArea.Width) return true;
                if(AI.Height != ChosenArea.Height) return true;

                if (rbFromImage.Checked && AI.FieldsToUse != AnimationType.SpriteDefinition) return true;
                if (rbMirror.Checked && AI.FieldsToUse != AnimationType.Mirror) return true;
                if (rbRotation.Checked && AI.FieldsToUse != AnimationType.Rotation) return true;                
            }
            return false;
        }

        /// <summary>
        /// Given two locations that we have clicked on, find the area we have selected
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        private Rectangle AreaFromGridPoints(Point Start, Point End)
        {
            //Get the points translated from locations on the picturebox
            Point OneImagePoint = MyController.ReturnPointAdjustedForImage(Start);
            Point TwoImagePoint = MyController.ReturnPointAdjustedForImage(End);
            //Now, shrink them to figure out which grid points we have chosen
            Point OneGridPoint = new Point(OneImagePoint.X / SnapGridSize.Width, OneImagePoint.Y / SnapGridSize.Height);
            Point TwoGridPoint = new Point(TwoImagePoint.X / SnapGridSize.Width, TwoImagePoint.Y / SnapGridSize.Height);
            //Find the top-left point and the bottom-right point
            Point StartGridPoint = new Point(Math.Min(OneGridPoint.X, TwoGridPoint.X), Math.Min(OneGridPoint.Y, TwoGridPoint.Y));
            Point EndGridPoint = new Point(Math.Max(OneGridPoint.X, TwoGridPoint.X), Math.Max(OneGridPoint.Y, TwoGridPoint.Y));
            //Translate them back into points on the image
            Point ReturnSPoint = new Point(StartGridPoint.X * SnapGridSize.Width, StartGridPoint.Y * SnapGridSize.Height);
            Point ReturnEPoint = new Point((EndGridPoint.X +1) * SnapGridSize.Width, (EndGridPoint.Y +1) * SnapGridSize.Height);
            //Change it into a rectangle and return it
            Rectangle ReturnRec = new Rectangle(ReturnSPoint.X, ReturnSPoint.Y, ReturnEPoint.X - ReturnSPoint.X, ReturnEPoint.Y - ReturnSPoint.Y);
            return ReturnRec;
        }

        private void SpriteEntryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                myDatabase.Save(); //try saving the file
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void pbImageField_MouseMove(object sender, MouseEventArgs e)
        {
            //If we are dragging, process the dragging
            if (WeAreDragging)
            {
                ChosenArea = AreaFromGridPoints(DragStart, e.Location);
                UpdateChosenAreaLabel();
            }
        }

        private void pbImageField_MouseDown(object sender, MouseEventArgs e)
        {
            //When the mouse goes down, we note that we are trying to drag
            WeAreDragging = true;
            DragStart = e.Location;
        }

        private void pbImageField_MouseUp(object sender, MouseEventArgs e)
        {
            //When the mouse goes up, stop dragging and update
            if(WeAreDragging)
            {
                ChosenArea = AreaFromGridPoints(DragStart, e.Location);
                UpdateChosenAreaLabel();
            }
            WeAreDragging = false;

        }

        int IndexOfName(string spritname)
        {
            for(int i=0; i< SpriteInformation.Count; i++)
            {
                if (SpriteInformation[i].SpriteName == spritname)
                    return i;
            }
            return -1;
        }

        bool VerifySpriteBeforeSaving()
        {
            if (tbSpriteName.Text == "")
            {
                MessageBox.Show("You cannot save a sprite that has no name.");
                return false;
            }
            int index = IndexOfName(tbSpriteName.Text);
            if(index != CurrentSIIndex && index != -1)
            {
                MessageBox.Show("You cannot have two sprites with the same name.");
                return false;
            }
            return true;
        }

        void ApplyChanges()
        {
            if (!VerifySpriteBeforeSaving()) return;
            FormToSpriteInformation();
            //Copy information from the frame we are based off of.  This makes the visible selection
            //equal to the one it is a copy of.
            if(CurrentSIAnimation >=0 && TempInformation.Animations[CurrentSIAnimation].FieldsToUse != AnimationType.SpriteDefinition)
            {
                int which = TempInformation.Animations[CurrentSIAnimation].AnimationToUse;
                if (which >= 0 && which < TempInformation.Animations.Count)
                {
                    AnimationInfo oAI = TempInformation.Animations[TempInformation.Animations[CurrentSIAnimation].AnimationToUse];
                    AnimationInfo nAI = TempInformation.Animations[CurrentSIAnimation];
                    nAI.ImageName = oAI.ImageName;
                    nAI.StartPoint = oAI.StartPoint;
                    nAI.Width = oAI.Width;
                    nAI.Height = oAI.Height;
                    nAI.NumFrames = oAI.NumFrames;
                }
            }
            if (CurrentSIIndex > 0 && CurrentSIIndex < SpriteInformation.Count)
            {
                SpriteInformation[CurrentSIIndex].CopyFrom(TempInformation);
            }
            else
            {
                SpriteInfo tSI = TempInformation.Clone();
                SpriteInformation.Add(tSI);
                CurrentSIIndex = SpriteInformation.IndexOf(tSI);
            }
            UpdateMenu();
        }

        /// <summary>
        /// Prompt to apply changes.  We return true if we continue, or false if we canceled out.
        /// </summary>
        /// <returns></returns>
        bool PromptToApplyChangesAndContinue()
        {
            if (!VerifySpriteBeforeSaving())
            {
                return true; //We could not verify, say we canceled out.
            }
            if (ValuesDifferFromData())
            {
                DialogResult Answer = MessageBox.Show("You have unsaved Changes.  Would you like to save them before proceeding?","Save?",MessageBoxButtons.YesNoCancel);
                if (Answer == DialogResult.Yes) ApplyChanges();
                if (Answer == DialogResult.Cancel) return false;
                if (PreviewSprite != null) PreviewSprite.Destroy();
            }
            return true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }

        private void WeHaveNewItem()
        {
            if (PreviewSprite != null)
            {
                PreviewSprite.Destroy();
                PreviewSprite = null;
            }
            if (CurrentSIIndex >= 0 && CurrentSIIndex < SpriteInformation.Count)
                TempInformation.CopyFrom(SpriteInformation[CurrentSIIndex]);
            else
                SetUpEmptyInfo();
            SpriteInformationToForm();
            UpdateMenu();
        }
        private void SelectNewIndex(int nindex)
        {
            if (nindex < 0) return;
            if (nindex >= SpriteInformation.Count) return;
            CurrentSIIndex = nindex;
            if (PreviewSprite != null) PreviewSprite.Destroy();
            TempInformation = SpriteInformation[nindex].Clone();
            CurrentSIAnimation = 0; //always start at animation 0
            WeHaveNewItem();
            UpdateMenu();
        }

        private void btnFwd_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                if (SpriteInformation.Count == 0) return; //nothing to do 
                CurrentSIIndex++;
                if (CurrentSIIndex >= SpriteInformation.Count) CurrentSIIndex = 0;
                if (TempInformation == null) TempInformation = new SpriteInfo();
                WeHaveNewItem();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                if (SpriteInformation.Count == 0) return; //nothing to do
                CurrentSIIndex--;
                if (CurrentSIIndex < 0) CurrentSIIndex = SpriteInformation.Count - 1;
                if (TempInformation == null) TempInformation = new SpriteInfo();
                WeHaveNewItem();
            }
        }

        private void btnNewSprite_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                TempInformation = null;
                CurrentSIIndex = -1;

                SetUpEmptyInfo();
                SpriteInformationToForm();
                UpdateMenu();
            }
        }

        private void btnNewAnimation_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                AnimationInfo AI = TempInformation.Animations[CurrentSIAnimation].Clone();
                TempInformation.Animations.Add(AI);
                CurrentSIAnimation++;
                SpriteInformationToForm();
                UpdateMenu();
            }
        }

        private void btnAnimBack_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                CurrentSIAnimation--;
                if (CurrentSIAnimation < 0)
                    CurrentSIAnimation = TempInformation.Animations.Count - 1;

                SpriteInformationToForm();
                UpdateMenu();
            }
        }

        private void btnAnimFwd_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                CurrentSIAnimation++;
                if (CurrentSIAnimation >= TempInformation.Animations.Count)
                    CurrentSIAnimation = 0;

                SpriteInformationToForm();
                UpdateMenu();
            }
        }

        private void DoPreview()
        {
            //remove the old one
            if (PreviewSprite != null) PreviewSprite.Destroy();
            //Create a new one
            PreviewSprite = TempInformation.CreateSprite(PreviewController, myDatabase);
            PreviewSprite.PutBaseImageLocation(new Point(1, 1));
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            DoPreview();
        }

        private void btnPreviewAnimBack_Click(object sender, EventArgs e)
        {
            if (PreviewSprite != null && !PreviewSprite.Destroying)
            {
                int Animations = PreviewSprite.AnimationCount;
                int NextAnim = PreviewSprite.AnimationIndex - 1;
                if (NextAnim < 0) NextAnim = Animations - 1;
                PreviewSprite.ChangeAnimation(NextAnim);
            }
            else DoPreview();
        }

        private void btnPreviewAnimFwd_Click(object sender, EventArgs e)
        {
            if (PreviewSprite != null)
            {
                int Animations = PreviewSprite.AnimationCount;
                int NextAnim = PreviewSprite.AnimationIndex + 1;
                if (NextAnim >= Animations) NextAnim = 0;
                PreviewSprite.ChangeAnimation(NextAnim);
            }
            else DoPreview();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SpriteInformationToForm();
        }

        private void btnDelSprite_Click(object sender, EventArgs e)
        {
            if (CurrentSIIndex == -1)
            {
                //we are making a new sprite, but it has not been saved yet.
                if (SpriteInformation.Count > 0)
                {
                    CurrentSIIndex = 0;
                    WeHaveNewItem();
                    return;
                }
                //If we are here, then we have no sprites and we are deleting our temp one. Clear it out
                SetUpEmptyInfo();
                WeHaveNewItem();
                return;
            }
            //delete the current entry.  Deal with zero entries.
            DialogResult Answer = MessageBox.Show("Delete The current Sprite?", "Delete", MessageBoxButtons.YesNo);
            if(Answer == DialogResult.Yes)
            {
                SpriteInformation.RemoveAt(CurrentSIIndex);
                CurrentSIIndex--;
                if (CurrentSIIndex < 0 && SpriteInformation.Count > 0) CurrentSIIndex = 0;
                WeHaveNewItem();
            }
        }

        private void btnDeleteAnim_Click(object sender, EventArgs e)
        {
            //delete the current entry.  Deal with zero entries.
            DialogResult Answer = MessageBox.Show("Delete The current Animation?", "Delete", MessageBoxButtons.YesNo);
            if (Answer == DialogResult.Yes)
            {
                TempInformation.Animations.RemoveAt(CurrentSIAnimation);
                CurrentSIAnimation--;
                if (CurrentSIAnimation < 0 && TempInformation.Animations.Count > 0) CurrentSIAnimation = 0;
                if(TempInformation.Animations.Count == 0)
                {
                    AnimationInfo AI = new AnimationInfo();
                    TempInformation.Animations.Add(AI);
                    CurrentSIAnimation = 0;
                }
                WeHaveNewItem();
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (PromptToApplyChangesAndContinue())
            {
                Close();
            }
        }
    }
}
