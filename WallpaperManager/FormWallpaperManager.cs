using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using WallpaperManager.Settings;
using System.Text.RegularExpressions;
using System.IO;

namespace WallpaperManager
{
    public partial class FormWallpaperManager : Form
    {

        private XmlSettings Settings;

        public FormWallpaperManager()
        {
            InitializeComponent();

            Settings = XmlSettings.Load();

            LoadProfiles();
            LogDisplay();

            SystemEvents.DisplaySettingsChanged += new EventHandler(DisplaySettingsChanged);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SystemEvents.DisplaySettingsChanged -= new EventHandler(DisplaySettingsChanged);

            base.OnClosing(e);
        }

        private XmlProfile CurrentProfile
        {
            get { return (XmlProfile)comboBoxProfiles.SelectedItem; }
        }

        private void DisplaySettingsChanged(object sender, EventArgs e)
        {
            LogDisplay();
        }

        private void LoadProfiles()
        {
            comboBoxProfiles.InvokeIfRequired(c =>
            {
                var current = this.CurrentProfile;
                comboBoxProfiles.Items.Clear();
                foreach (XmlProfile profile in Settings.Profiles)
                {
                    comboBoxProfiles.Items.Add(profile);
                    if (current == profile)
                    {
                        comboBoxProfiles.SelectedItem = profile;
                    }
                }

                if (comboBoxProfiles.Items.Count > 0 && comboBoxProfiles.SelectedIndex < 0)
                {
                    comboBoxProfiles.SelectedIndex = 0;
                }
            });
        }

        private void LogDisplay()
        {
            textBoxLog.InvokeIfRequired(c =>
            {
                textBoxLog.AppendText("Screen count:\t" + Screen.AllScreens.Length + "\n");
                foreach (Screen screen in Screen.AllScreens)
                {
                    textBoxLog.AppendText("+ Screen:\t" + screen.DeviceName + "\n");
                    textBoxLog.AppendText("  - Bounds:\t" + screen.Bounds + "\n");
                    textBoxLog.AppendText("  - Primary:\t" + screen.Primary + "\n");
                }
                textBoxLog.AppendText("\n");
            });
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            string name = "";
            if (InputBox("New profile", "Name", ref name) == DialogResult.OK)
            {
                var profile = new XmlProfile(Screen.AllScreens);
                if (!Settings.Profiles.Contains(profile))
                {
                    profile.Name = name;
                    Settings.Profiles.Add(profile);
                    Settings.Save();
                    LoadProfiles();
                }
                else
                {
                    MessageBox.Show("There is already a profile for this screen configuration.", "Profile already exists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void comboBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelScreens.Controls.Clear();
            if (this.CurrentProfile != null)
            {
                DisplayScreens(this.CurrentProfile.Screens);
                buttonDelete.Enabled = true;
            }
            else
            {
                buttonDelete.Enabled = false;
            }
        }

        private void DisplayScreens(List<XmlScreen> screens)
        {
            panelScreens.Controls.Clear();
            foreach (var screen in screens)
            {
                DisplayScreen(screen);
            }

            //Adjust positions
            int minX = 0;
            int minY = 0;
            foreach (Control control in panelScreens.Controls)
            {
                if (control.Left < minX) minX = control.Left;
                if (control.Top < minY) minY = control.Top;
            }

            foreach (Control control in panelScreens.Controls)
            {
                control.Left += minX * -1;
                control.Left += 3 * (control.Left / 192) + 3;
                control.Top += minY * -1;
                control.Top += 3 * (control.Top / 108) + 3;
            }
        }

        private void DisplayScreen(XmlScreen screen)
        {
            var picture = new PictureBox();
            picture.Parent = panelScreens;

            picture.Bounds = screen.Bounds.Scale(0.1).Expand(1);
            picture.BorderStyle = BorderStyle.FixedSingle;
            picture.Tag = screen;
            picture.AllowDrop = true;
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.DragEnter += new DragEventHandler(this.PictureDragEnter);
            picture.DragDrop += new DragEventHandler(this.PictureDragDrop);

            if (!String.IsNullOrEmpty(screen.Wallpaper))
            {
                try
                {
                    Image image = LoadImage(screen.Wallpaper);
                    picture.Image = image;
                }
                catch (ImageLoadException)
                {
                }
            }

            //var toolTip = new ToolTip();
            //toolTip.SetToolTip(picture, screen.Name);
        }

        private void PictureDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PictureDragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s.Length == 1)
            {
                if (((PictureBox)sender).Tag != null) {
                    SaveWallpaper((PictureBox)sender, s[0]);
                }
            }
        }

        private void SaveWallpaper(PictureBox picture, string path)
        {
            try
            {
                XmlScreen screen = (XmlScreen)picture.Tag;
                Image image = LoadImage(path);
                picture.Image = image;
                screen.SetWallpaper(path);
            }
            catch (ImageLoadException)
            {
                MessageBox.Show("Failed to load image.", "Failed to load image.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Image LoadImage(string path) 
        {
            if (File.Exists(path))
            {
                try
                {
                    using (Image image = Image.FromFile(path))
                    {
                        return new Bitmap(image);                        
                    }
                }
                catch (OutOfMemoryException e)
                {
                    throw new ImageLoadException("Invalid image", e);
                }
            }
            else
            {
                throw new ImageLoadException("Image not found");
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Settings.Save();
            Manager.ChangeWallpaper();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            XmlProfile profile = CurrentProfile;
            profile.Delete();
            Settings.Profiles.Remove(profile);
            Settings.Save();
            LoadProfiles();
        }
    }
}
