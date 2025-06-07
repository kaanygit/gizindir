using gizindir.model;
using gizindir.data;
using System;
using System.Windows.Forms;

namespace gizindir
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new UserRepository();

            if (repo.GetUserByEmail(email) != null)
            {
                MessageBox.Show("Bu email zaten kayıtlı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = new UserModel
            {
                Email = email,
                Password = password,
                CreatedAt = DateTime.Now
            };

            repo.AddUser(user);

            // Kayıttan sonra oturum oluştur
            var createdUser = repo.GetUserByEmail(user.Email);
            string token = repo.CreateSession(createdUser.Id);
            System.IO.File.WriteAllText("session_token.txt", token);

            MessageBox.Show("Kayıt başarılı! Bilgi ekranına yönlendiriliyorsunuz.");

            var profileForm = new ProfileForm(createdUser);
            profileForm.Show();
            this.Hide();
        }

    }
}
