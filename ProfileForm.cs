using gizindir.data;
using gizindir.model;
using gizindir.UIWidgets;
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
                    pbProfileImage.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        private void btnSaveContinue_Click(object sender, EventArgs e)
        {
            // Tüm alanların dolu olup olmadığını kontrol et
            if (string.IsNullOrEmpty(txtFullName.Text))
            {
                CustomMessageBox.Show("Tam adınızı girmelisiniz.", "Uyarı", MessageBoxIcon.Warning);
                return;
            }

            if (cmbGender.SelectedItem == null)
            {
                CustomMessageBox.Show("Cinsiyetinizi seçmelisiniz.", "Uyarı", MessageBoxIcon.Warning);
                return;
            }

            if (cmbInterestedGender.SelectedItem == null)
            {
                CustomMessageBox.Show("İlgilendiğiniz cinsiyeti seçmelisiniz.", "Uyarı", MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtBio.Text))
            {
                CustomMessageBox.Show("Biyografinizi girmelisiniz.", "Uyarı", MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(selectedImagePath))
            {
                CustomMessageBox.Show("Profil resmi seçmelisiniz.", "Uyarı", MessageBoxIcon.Warning);
                return;
            }

            // Verileri user model'e aktar
            _user.FullName = txtFullName.Text;
            _user.Gender = cmbGender.SelectedItem.ToString();
            _user.InterestedIn = cmbInterestedGender.SelectedItem.ToString();
            _user.BirthDate = dtpBirthDate.Value;
            _user.Bio = txtBio.Text;
            _user.CreatedAt = DateTime.Now;

            try
            {
                // Resim işleme
                string imgDir = Path.Combine(Application.StartupPath, "profile_images");
                Directory.CreateDirectory(imgDir);

                string imgFileName = $"{Guid.NewGuid()}{Path.GetExtension(selectedImagePath)}";
                string destPath = Path.Combine(imgDir, imgFileName);
                File.Copy(selectedImagePath, destPath, true);
                
                // Veritabanında saklanacak resim URL'si
                string imageUrl = $"profile_images/{imgFileName}";
                _user.ProfileImageUrl = imageUrl;

                // Veritabanına kaydet
                UserRepository repo = new UserRepository();
                repo.UpdateUserProfile(_user);

                CustomMessageBox.Show("Profil başarıyla kaydedildi!", "Başarılı", MessageBoxIcon.Information);
                var mainForm = new Main(_user.Email);
                mainForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Profil kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxIcon.Error);
            }
        }
    }
} 