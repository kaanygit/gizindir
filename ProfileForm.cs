using gizindir.data;
using gizindir.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gizindir
{
    public partial class ProfileForm : Form
    {
        private UserModel _user;
        private string selectedImagePath = "";

        public ProfileForm(UserModel user)
        {
            InitializeComponent();
            _user = user;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnBrowsePhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    pbProfileImage.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        private void btnSaveContinue_Click(object sender, EventArgs e)
        {
            // Verileri user model'e aktar
            _user.FullName = txtFullName.Text;
            _user.Gender = cmbGender.SelectedItem?.ToString() ?? "";
            _user.InterestedIn = cmbInterestedGender.SelectedItem?.ToString() ?? "";
            _user.BirthDate = dtpBirthDate.Value;
            _user.Bio = txtBio.Text;
            _user.CreatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(selectedImagePath))
            {
                string imgDir = Path.Combine(Application.StartupPath, "profile_images");
                Directory.CreateDirectory(imgDir);

                string imgFileName = $"{Guid.NewGuid()}{Path.GetExtension(selectedImagePath)}";
                string destPath = Path.Combine(imgDir, imgFileName);
                File.Copy(selectedImagePath, destPath, true);
                _user.ProfileImageUrl = destPath;
            }

            // Veritabanına kaydet
            UserRepository repo = new UserRepository();
            repo.UpdateUserProfile(_user);

            MessageBox.Show("Profil kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var mainForm = new Main(_user.Email);
            mainForm.Show();
            this.Close(); // sadece Close yeterli


        }
    }
}
