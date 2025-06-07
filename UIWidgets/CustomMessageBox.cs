using System;
using System.Drawing;
using System.Windows.Forms;

namespace gizindir.UIWidgets
{
    public static class CustomMessageBox
    {
        public static void Show(string message, string title = "Bilgi", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            Form msgForm = new Form()
            {
                Width = 400,
                Height = 200,
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.White,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lblMessage = new Label()
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Black
            };

            Button btnOk = new Button()
            {
                Text = "Tamam",
                DialogResult = DialogResult.OK,
                Width = 100,
                Height = 40,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Location = new Point((msgForm.Width - btnOk.Width) / 2, 120);

            msgForm.Controls.Add(lblMessage);
            msgForm.Controls.Add(btnOk);
            msgForm.AcceptButton = btnOk;
            msgForm.ShowDialog();
        }
    }
}
