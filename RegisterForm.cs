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
            var user = new UserModel
            {
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                CreatedAt = DateTime.Now
            };

            var repo = new UserRepository();

            if (repo.GetUserByEmail(user.Email) != null)
            {
                MessageBox.Show("Bu email zaten kayıtlı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            repo.AddUser(user);
            MessageBox.Show("Kayıt başarılı! Bilgi ekranına yönlendiriliyorsunuz.");

            // Bilgi toplama formuna geç
            //var infoForm = new InfoForm(user.Email);
            //infoForm.Show();
            this.Hide();
        }
    }
}
