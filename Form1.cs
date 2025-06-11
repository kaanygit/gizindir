using System;
using System.Configuration;
using System.Windows.Forms;
using Npgsql;

namespace gizindir
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load; // Otomatik bağla
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool connected = false;

            while (!connected)
            {
                string connString = ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString;

                using (var conn = new NpgsqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        connected = true;
                        MessageBox.Show("Veritabanı bağlantısı başarılı.", "Bağlantı Tamam", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Veritabanı bağlantı hatası:\n{ex.Message}\n\nTekrar denemek ister misiniz?",
                            "Bağlantı Hatası",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Error);

                        if (result == DialogResult.Cancel)
                        {
                            Application.Exit(); // Uygulamayı kapat
                            return;
                        }
                    }
                }
            }
        }
    }
}
