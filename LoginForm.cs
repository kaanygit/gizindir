using gizindir.data;
using gizindir.UIWidgets;
using System;
using System.Windows.Forms;

namespace gizindir
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.KeyDown += LoginForm_KeyDown;
            this.KeyPreview = true;
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                CustomMessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxIcon.Warning);
                return;
            }

            var repo = new UserRepository();
            var user = repo.GetUserByEmail(email);

            if (user == null)
            {
                CustomMessageBox.Show("Bu email ile bir kullanıcı bulunamadı.", "Hata", MessageBoxIcon.Error);
                return;
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (isPasswordCorrect)
            {
                CustomMessageBox.Show("Giriş başarılı.", "Başarılı", MessageBoxIcon.Information);

                // Yeni oturum oluştur ve token'ı sakla
                string token = repo.CreateSession(user.Id);
                System.IO.File.WriteAllText("session_token.txt", token);

                bool isProfileCompleted = repo.IsProfileCompleted(user.Email);
                if (!isProfileCompleted)
                {
                    var profileForm = new ProfileForm(user);
                    profileForm.Show();
                }
                else
                {
                    var mainForm = new Main(user.Email);
                    mainForm.Show();
                }

                this.Hide();
            }
            else
            {
                CustomMessageBox.Show("Şifre yanlış.", "Hata", MessageBoxIcon.Warning);
            }
        }


    }
}
