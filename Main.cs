using gizindir.data;
using gizindir.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gizindir
{
    public partial class Main : Form
    {
        private string _email;
        private List<UserModel> candidateUsers = new List<UserModel>();
        private int currentIndex = 0;
        private Timer timerSlideLeft = new Timer();
        private Timer timerSlideRight = new Timer();


        // Animasyon için orijinal kart konumu
        private Point cardOriginalLocation;
        // Animasyon adım miktarı
        private int animationStep = 20;

        public Main(string email)
        {
            InitializeComponent();
            _email = email;
            cardOriginalLocation = panelProfileCard.Location; // Tasarımda belirlediğin orijinal konum

            // Timer ayarları (Designer üzerinden de ekleyebilirsin)
            timerSlideLeft.Interval = 10;
            timerSlideLeft.Tick += TimerSlideLeft_Tick;
            timerSlideRight.Interval = 10;
            timerSlideRight.Tick += TimerSlideRight_Tick;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Giriş yapan kullanıcı hariç tüm kullanıcıları getir
            var repo = new UserRepository();
            var allUsers = repo.GetAllUsers();
            candidateUsers = allUsers.Where(u => !u.Email.Equals(_email, StringComparison.OrdinalIgnoreCase)).ToList();

            if (candidateUsers.Count > 0)
                LoadCurrentUserCard();
            else
                MessageBox.Show("Gösterilecek başka kullanıcı bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadCurrentUserCard()
        {
            if (currentIndex >= candidateUsers.Count)
            {
                MessageBox.Show("Tüm kullanıcıları incelediniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Seçilen aday kullanıcıyı yükle
            var user = candidateUsers[currentIndex];

            // Örnek: full_name ve varsayılan değerler
            lblNameAge.Text = string.IsNullOrEmpty(user.FullName) ? "İsim, Yaş" : user.FullName + ", " + "33"; // yaş bilgisi ekle
            // Aşağıdaki etiketleri kullanıcı modeline uygun şekilde doldurabilirsin
            lblProfession.Text = "Software Engineer";
            lblUniversity.Text = "University of ...";

            // Profil resmi (ProfileImageUrl varsa, yoksa placeholder)
            if (!string.IsNullOrEmpty(user.ProfileImageUrl) && System.IO.File.Exists(user.ProfileImageUrl))
                pbMainProfile.Image = Image.FromFile(user.ProfileImageUrl);
            else
                pbMainProfile.Image = Properties.Resources.placeholder; // Proje kaynaklarından bir placeholder

            // Kartı orijinal konuma sıfırla
            panelProfileCard.Location = cardOriginalLocation;
        }

        // Buton Tıklamaları
        private void btnLike_Click(object sender, EventArgs e)
        {
            // Sağa kaydırma animasyonu başlat
            timerSlideRight.Start();
        }

        private void btnDislike_Click(object sender, EventArgs e)
        {
            // Sola kaydırma animasyonu başlat
            timerSlideLeft.Start();
        }

        // Animasyon için Timer Tick olayları
        private async void TimerSlideRight_Tick(object sender, EventArgs e)
        {
            panelProfileCard.Left += animationStep;
            if (panelProfileCard.Left > this.Width)
            {
                timerSlideRight.Stop();
                currentIndex++;
                if (currentIndex < candidateUsers.Count)
                {
                    LoadCurrentUserCard();
                    await SlidePanelToOriginal(true);
                }
                else
                {
                    MessageBox.Show("Tüm kullanıcıları incelediniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private async void TimerSlideLeft_Tick(object sender, EventArgs e)
        {
            panelProfileCard.Left -= animationStep;
            if (panelProfileCard.Right < 0)
            {
                timerSlideLeft.Stop();
                currentIndex++;
                if (currentIndex < candidateUsers.Count)
                {
                    LoadCurrentUserCard();
                    await SlidePanelToOriginal(false);
                }
                else
                {
                    MessageBox.Show("Tüm kullanıcıları incelediniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        // Paneli animasyonlu şekilde orijinal konuma kaydırmak için yöntem
        private async Task SlidePanelToOriginal(bool fromLeft)
        {
            int targetX = cardOriginalLocation.X;
            int startX = fromLeft ? -panelProfileCard.Width : this.Width;

            panelProfileCard.Left = startX;

            while ((fromLeft && panelProfileCard.Left < targetX) || (!fromLeft && panelProfileCard.Left > targetX))
            {
                panelProfileCard.Left += fromLeft ? animationStep : -animationStep;
                await Task.Delay(10);
            }

            panelProfileCard.Left = targetX;
        }


    }
}
