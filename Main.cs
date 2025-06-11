using gizindir.data;
using gizindir.model;
using gizindir.helpers;
using gizindir.view;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.IO;

namespace gizindir
{
    public partial class Main : Form
    {
        private string _email;
        private List<UserModel> candidateUsers = new List<UserModel>();
        private int currentIndex = 0;
        private Timer animationTimer = new Timer();
        private Timer messageCheckTimer = new Timer(); // Mesaj kontrolü için timer
        private Dictionary<string, DateTime> lastMessageTimes = new Dictionary<string, DateTime>(); // Son mesaj zamanlarını saklamak için

        // Animasyon için değişkenler
        private Point cardOriginalLocation;
        private int animationStep = 15;
        private bool isAnimating = false;
        private string animationDirection = ""; // "left", "right"
        private int animationStartX;
        private int animationTargetX;

        public Main(string email)
        {
            InitializeComponent();
            _email = email;

            // Timer ayarları
            animationTimer.Interval = 16; // ~60 FPS için
            animationTimer.Tick += AnimationTimer_Tick;
            
            // Mesaj kontrol timer'ı ayarları (her 5 saniyede bir kontrol et)
            messageCheckTimer.Interval = 5000;
            messageCheckTimer.Tick += MessageCheckTimer_Tick;
            messageCheckTimer.Start();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Orijinal kart konumunu kaydet
            cardOriginalLocation = panelProfileCard.Location;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları getir
            LoadCandidateUsers();

            if (candidateUsers.Count > 0)
                LoadCurrentUserCard();
            else
                MessageBox.Show("Gösterilecek başka kullanıcı bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Mesajları ve eşleşmeleri yükle
            LoadMatchedChats();
            
            // Son mesaj zamanlarını ilklendir
            InitializeLastMessageTimes();
        }

        private void LoadCandidateUsers()
        {
            try
            {
                var repo = new UserRepository();
                candidateUsers = repo.GetUnseenUsers(_email);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCurrentUserCard()
        {
            if (currentIndex >= candidateUsers.Count)
            {
                ShowEndMessage();
                return;
            }

            try
            {
                var user = candidateUsers[currentIndex];

                // Kullanıcı bilgilerini yükle
                LoadUserInfo(user);
                LoadUserImage(user);

                // Kartı orijinal konuma getir
                panelProfileCard.Location = cardOriginalLocation;
                panelProfileCard.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı bilgileri yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserInfo(UserModel user)
        {
            // İsim ve yaş bilgisi
            string displayName = string.IsNullOrEmpty(user.FullName) ? user.Name : user.FullName;
            if (string.IsNullOrEmpty(displayName)) displayName = "İsimsiz Kullanıcı";

            // Yaş hesaplama (birth_date varsa)
            string ageText = "";
            if (user.BirthDate != default(DateTime))
            {
                int age = DateTime.Now.Year - user.BirthDate.Year;
                if (DateTime.Now.DayOfYear < user.BirthDate.DayOfYear) age--;
                ageText = $", {age}";
            }

            lblNameAge.Text = displayName + ageText;

            // Diğer bilgiler
            lblProfession.Text = !string.IsNullOrEmpty(user.Bio) ? user.Bio : "Henüz bio eklenmemiş";
            lblUniversity.Text = !string.IsNullOrEmpty(user.InterestedIn) ? $"İlgi alanı: {user.InterestedIn}" : "İlgi alanı belirtilmemiş";
        }

        private async void LoadUserImage(UserModel user)
        {
            try
            {
                // Önceki resmi temizle
                if (pbMainProfile.Image != null && pbMainProfile.Image != Properties.Resources.placeholder)
                {
                    // Cache'den gelen resimleri dispose etme, sadece placeholder olmayan yerel resimleri
                    if (!_imageCache.ContainsValue(pbMainProfile.Image))
                    {
                        pbMainProfile.Image.Dispose();
                    }
                }

                // Loading göstergesi (isteğe bağlı)
                pbMainProfile.Image = Properties.Resources.placeholder;

                // URL'den resim yükle
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var loadedImage = await ImageCacheHelper.LoadImageAsync(user.ProfileImageUrl, Properties.Resources.placeholder);

                    // UI thread'de güncelle
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => pbMainProfile.Image = loadedImage));
                    }
                    else
                    {
                        pbMainProfile.Image = loadedImage;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Resim yükleme hatası: {ex.Message}");
                pbMainProfile.Image = Properties.Resources.placeholder;
            }
        }

        // Cache referansı için (dispose kontrolü için)
        private static readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

        private void ShowEndMessage()
        {
            panelProfileCard.Visible = false;
            MessageBox.Show("Tüm kullanıcıları incelediniz! Yeni kullanıcılar için daha sonra tekrar kontrol edin.",
                          "Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Buton Tıklamaları
        private void btnLike_Click(object sender, EventArgs e)
        {
            if (isAnimating || currentIndex >= candidateUsers.Count) return;

            // Beğenme işlemi (isteğe bağlı: veritabanına kaydet)
            ProcessLike(true);

            // Sağa kaydırma animasyonu başlat
            StartSlideAnimation("right");
        }

        private void btnDislike_Click(object sender, EventArgs e)
        {
            if (isAnimating || currentIndex >= candidateUsers.Count) return;

            // Beğenmeme işlemi (isteğe bağlı: veritabanına kaydet)
            ProcessLike(false);

            // Sola kaydırma animasyonu başlat
            StartSlideAnimation("left");
        }

        private void ProcessLike(bool isLike)
        {
            try
            {
                var currentUser = candidateUsers[currentIndex];
                var repo = new UserRepository();

                // Eğer beğeni yapıldıysa match kontrolü yap
                if (isLike)
                {
                    // Önce etkileşimi kaydet
                    repo.RecordUserInteraction(_email, currentUser.Email, true);

                    // Match kontrolü yap
                    using (var conn = DbContext.GetConnection())
                    {
                        conn.Open();
                        var matchCmd = new NpgsqlCommand(@"
                            SELECT EXISTS (
                                SELECT 1 FROM user_interactions
                                WHERE user_email = @shownUserEmail
                                AND shown_user_email = @userEmail
                                AND is_liked = true
                            )", conn);

                        matchCmd.Parameters.AddWithValue("userEmail", _email);
                        matchCmd.Parameters.AddWithValue("shownUserEmail", currentUser.Email);

                        bool isMatch = (bool)matchCmd.ExecuteScalar();
                        if (isMatch)
                        {
                            MessageBox.Show($"Tebrikler! {currentUser.FullName} ile eşleştiniz!", 
                                "Yeni Eşleşme", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                                
                            // Eşleşme olduğunda chat listesine ekle
                            AddNewMatchToChat(currentUser);
                        }
                    }
                }
                else
                {
                    // Beğenmeme durumunda sadece kaydet
                    repo.RecordUserInteraction(_email, currentUser.Email, false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Like işlemi kaydedilirken hata: {ex.Message}");
                MessageBox.Show("İşlem sırasında bir hata oluştu.", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Yeni eşleşmeyi chat listesine ekleyen metod
        private void AddNewMatchToChat(UserModel matchedUser)
        {
            try
            {
                // Dinamik chat nesnesi oluştur
                dynamic newChat = new System.Dynamic.ExpandoObject();
                newChat.Name = string.IsNullOrEmpty(matchedUser.FullName) ? matchedUser.Name : matchedUser.FullName;
                
                // Yaş hesaplama
                int age = 0;
                if (matchedUser.BirthDate != default(DateTime))
                {
                    age = DateTime.Now.Year - matchedUser.BirthDate.Year;
                    if (DateTime.Now.DayOfYear < matchedUser.BirthDate.DayOfYear) age--;
                }
                
                newChat.Age = age;
                newChat.LastMessage = "Yeni eşleşme! Selam ver...";
                newChat.LastMessageTime = DateTime.Now;
                newChat.IsOnline = false;
                newChat.UnreadCount = 0;
                newChat.ProfileImageUrl = matchedUser.ProfileImageUrl;
                newChat.Email = matchedUser.Email;
                
                // Chat paneli oluştur
                Panel chatPanel = CreateChatItem(newChat);
                
                // FlowLayoutPanel'in başına ekle (en yeni eşleşme en üstte görünsün)
                if (flowLayoutPanel1.Controls.Count > 0)
                    flowLayoutPanel1.Controls.Add(chatPanel);
                else
                    flowLayoutPanel1.Controls.Add(chatPanel);
                
                // Paneli en üste taşı
                flowLayoutPanel1.Controls.SetChildIndex(chatPanel, 0);
                
                // Panel boyutunu ayarla
                AdjustFlowLayoutPanelSize();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eşleşme chat'e eklenirken hata: {ex.Message}");
            }
        }

        private void StartSlideAnimation(string direction)
        {
            if (isAnimating) return;

            isAnimating = true;
            animationDirection = direction;
            animationStartX = panelProfileCard.Left;

            if (direction == "right")
                animationTargetX = this.Width + panelProfileCard.Width;
            else
                animationTargetX = -panelProfileCard.Width;

            animationTimer.Start();
        }

        private async void AnimationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Kartı hareket ettir
                if (animationDirection == "right")
                    panelProfileCard.Left += animationStep;
                else
                    panelProfileCard.Left -= animationStep;

                // Hedef konuma ulaştı mı kontrol et
                bool animationComplete = false;
                if (animationDirection == "right" && panelProfileCard.Left >= animationTargetX)
                    animationComplete = true;
                else if (animationDirection == "left" && panelProfileCard.Left <= animationTargetX)
                    animationComplete = true;

                if (animationComplete)
                {
                    animationTimer.Stop();

                    // Sonraki kullanıcıya geç
                    currentIndex++;

                    if (currentIndex < candidateUsers.Count)
                    {
                        // Yeni kartı yükle ve animasyonlu şekilde getir
                        LoadCurrentUserCard();
                        await SlideInNewCard();
                    }
                    else
                    {
                        ShowEndMessage();
                    }

                    isAnimating = false;
                }
            }
            catch (Exception ex)
            {
                animationTimer.Stop();
                isAnimating = false;
                MessageBox.Show($"Animasyon sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SlideInNewCard()
        {
            // Yeni kartı karşı taraftan getir
            int startX = animationDirection == "right" ? -panelProfileCard.Width : this.Width;
            panelProfileCard.Left = startX;

            while (true)
            {
                if (animationDirection == "right")
                {
                    panelProfileCard.Left += animationStep;
                    if (panelProfileCard.Left >= cardOriginalLocation.X)
                        break;
                }
                else
                {
                    panelProfileCard.Left -= animationStep;
                    if (panelProfileCard.Left <= cardOriginalLocation.X)
                        break;
                }

                await Task.Delay(16); // ~60 FPS
            }

            // Tam yerine otur
            panelProfileCard.Left = cardOriginalLocation.X;
        }

        // Form kapanırken kaynakları temizle
        private void CleanupResources()
        {
            try
            {
                animationTimer?.Stop();
                animationTimer?.Dispose();
                
                // Mesaj kontrol timer'ını durdur
                messageCheckTimer?.Stop();
                messageCheckTimer?.Dispose();

                // Profil resmini temizle
                if (pbMainProfile?.Image != null && pbMainProfile.Image != Properties.Resources.placeholder)
                {
                    pbMainProfile.Image.Dispose();
                    pbMainProfile.Image = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        // Form kapandığında çağrılır
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            CleanupResources();

            // Image cache'i temizle
            ImageCacheHelper.Dispose();
        }

        // Klavye kısayolları (isteğe bağlı)
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (isAnimating || currentIndex >= candidateUsers.Count) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.Left:
                    btnDislike_Click(null, null);
                    return true;
                case Keys.Right:
                    btnLike_Click(null, null);
                    return true;
                case Keys.Space:
                    btnLike_Click(null, null);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                // Session token dosyasını sil
                string sessionTokenFile = "session_token.txt";
                if (File.Exists(sessionTokenFile))
                {
                    File.Delete(sessionTokenFile);
                }

                // Kaynakları temizle
                CleanupResources();

                // Form'u kapat
                this.Hide();

                // Onboarding formunu aç
                var onboardingForm = new Onboarding();
                onboardingForm.FormClosed += (s, args) => this.Close();
                onboardingForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Çıkış yapılırken bir hata oluştu: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Eşleşen kullanıcıların chat listesini yükleyen metod
        private void LoadMatchedChats()
        {
            try
            {
                // FlowLayoutPanel ayarları
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
                flowLayoutPanel1.AutoScroll = true;
                flowLayoutPanel1.WrapContents = false;
                flowLayoutPanel1.BackColor = Color.FromArgb(245, 245, 245);

                // Veritabanından eşleşme verilerini getir
                var matchRepo = new MatchRepository();
                var messageRepo = new MessageRepository();
                var userRepo = new UserRepository();

                var matches = matchRepo.GetMatchesByEmail(_email);

                // Eğer hiç eşleşme yoksa
                if (matches.Count == 0)
                {
                    Label noMatchLabel = new Label();
                    noMatchLabel.Text = "Henüz hiç eşleşmeniz yok.\nKeşfet bölümünden kullanıcıları beğenin!";
                    noMatchLabel.AutoSize = false;
                    noMatchLabel.TextAlign = ContentAlignment.MiddleCenter;
                    noMatchLabel.Size = new Size(flowLayoutPanel1.Width - 25, 80);
                    noMatchLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    noMatchLabel.ForeColor = Color.FromArgb(120, 120, 120);
                    flowLayoutPanel1.Controls.Add(noMatchLabel);
                    return;
                }

                // Her eşleşme için
                foreach (var match in matches)
                {
                    // Mevcut kullanıcı bilgilerini getir
                    var currentUser = userRepo.GetUserByEmail(_email);
                    
                    // Son mesaj bilgisini getir
                    var lastMessage = messageRepo.GetLastMessage(_email, match.MatchedUserEmail);
                    
                    // Son mesaj zamanını güncelle
                    if (lastMessage != null)
                    {
                        lastMessageTimes[match.MatchedUserEmail] = lastMessage.SentAt;
                    }
                    
                    // Dinamik chat nesnesi oluştur
                    dynamic chatData = new System.Dynamic.ExpandoObject();
                    
                    // Temel bilgiler
                    chatData.Name = match.MatchedUserName;
                    chatData.Email = match.MatchedUserEmail;
                    
                    // Yaş hesaplama için kullanıcıyı getir
                    var matchedUser = userRepo.GetUserByEmail(match.MatchedUserEmail);
                    
                    int age = 0;
                    if (matchedUser != null && matchedUser.BirthDate != default(DateTime))
                    {
                        age = DateTime.Now.Year - matchedUser.BirthDate.Year;
                        if (DateTime.Now.DayOfYear < matchedUser.BirthDate.DayOfYear) age--;
                        chatData.Age = age;
                    }
                    else
                    {
                        chatData.Age = 0;
                    }
                    
                    // Son mesaj bilgileri
                    if (lastMessage != null)
                    {
                        chatData.LastMessage = lastMessage.Content;
                        chatData.LastMessageTime = lastMessage.SentAt;
                        
                        // Okunmamış mesaj sayısı
                        if (currentUser != null)
                        {
                            chatData.UnreadCount = messageRepo.GetUnreadCount(currentUser.Id, matchedUser != null ? matchedUser.Id : 0);
                        }
                        else
                        {
                            chatData.UnreadCount = 0;
                        }
                    }
                    else
                    {
                        chatData.LastMessage = "Henüz mesaj yok. Selam ver!";
                        chatData.LastMessageTime = match.MatchedAt;
                        chatData.UnreadCount = 0;
                    }
                    
                    // Diğer özellikler
                    chatData.IsOnline = false; // Gerçek uygulama için online durumu kontrol edilebilir
                    chatData.ProfileImageUrl = matchedUser != null ? matchedUser.ProfileImageUrl : "";
                    
                    // Chat paneli oluştur
                    Panel chatPanel = CreateChatItem(chatData);
                    flowLayoutPanel1.Controls.Add(chatPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sohbet listesi yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tek bir chat item paneli oluşturan metod
        private Panel CreateChatItem(dynamic chat)
        {
            // Ana chat paneli
            Panel chatPanel = new Panel();
            chatPanel.Size = new Size(flowLayoutPanel1.Width - 25, 85);
            chatPanel.BackColor = Color.White;
            chatPanel.Margin = new Padding(3, 2, 3, 2);
            chatPanel.Cursor = Cursors.Hand;

            // Modern border efekti
            chatPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, chatPanel.ClientRectangle,
                    Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(230, 230, 230), 1, ButtonBorderStyle.Solid);
            };

            // Profil resmi
            Panel profileImagePanel = new Panel();
            profileImagePanel.Size = new Size(55, 55);
            profileImagePanel.Location = new Point(15, 15);
            profileImagePanel.BackColor = Color.FromArgb(255, 99, 132); // Tinder teması

            // Circular profil resmi efekti
            profileImagePanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 99, 132)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 54, 54);
                }

                // İlk harf
                string initial = chat.Name.ToString().Substring(0, 1).ToUpper();
                using (Font font = new Font("Arial", 18, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    SizeF textSize = e.Graphics.MeasureString(initial, font);
                    float x = (55 - textSize.Width) / 2;
                    float y = (55 - textSize.Height) / 2;
                    e.Graphics.DrawString(initial, font, textBrush, x, y);
                }
            };

            // Online durumu göstergesi
            if (chat.IsOnline)
            {
                Panel onlineIndicator = new Panel();
                onlineIndicator.Size = new Size(16, 16);
                onlineIndicator.Location = new Point(54, 54);
                onlineIndicator.BackColor = Color.FromArgb(67, 198, 172); // Yeşil online
                onlineIndicator.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (Brush brush = new SolidBrush(Color.FromArgb(67, 198, 172)))
                    {
                        e.Graphics.FillEllipse(brush, 2, 2, 12, 12);
                    }
                    using (Pen pen = new Pen(Color.White, 2))
                    {
                        e.Graphics.DrawEllipse(pen, 2, 2, 12, 12);
                    }
                };
                profileImagePanel.Controls.Add(onlineIndicator);
            }

            // İsim ve yaş
            Label nameLabel = new Label();
            nameLabel.Text = $"{chat.Name}, {chat.Age}";
            nameLabel.Location = new Point(85, 15);
            nameLabel.Size = new Size(150, 22);
            nameLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            nameLabel.ForeColor = Color.FromArgb(33, 33, 33);
            nameLabel.BackColor = Color.Transparent;

            // Son mesaj
            Label messageLabel = new Label();
            string lastMessage = chat.LastMessage.ToString();
            if (lastMessage.Length > 35)
                lastMessage = lastMessage.Substring(0, 32) + "...";

            messageLabel.Text = lastMessage;
            messageLabel.Location = new Point(85, 40);
            messageLabel.Size = new Size(180, 18);
            messageLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            messageLabel.ForeColor = chat.UnreadCount > 0 ? Color.FromArgb(33, 33, 33) : Color.FromArgb(120, 120, 120);
            messageLabel.BackColor = Color.Transparent;

            // Zaman bilgisi
            Label timeLabel = new Label();
            DateTime lastTime = chat.LastMessageTime;
            string timeText = "";

            if (lastTime.Date == DateTime.Now.Date)
                timeText = lastTime.ToString("HH:mm");
            else if (lastTime.Date == DateTime.Now.Date.AddDays(-1))
                timeText = "Dün";
            else if ((DateTime.Now - lastTime).Days < 7)
                timeText = lastTime.ToString("dddd");
            else
                timeText = lastTime.ToString("dd.MM");

            timeLabel.Text = timeText;
            timeLabel.Location = new Point(chatPanel.Width - 65, 15);
            timeLabel.Size = new Size(50, 18);
            timeLabel.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            timeLabel.ForeColor = Color.FromArgb(120, 120, 120);
            timeLabel.TextAlign = ContentAlignment.TopRight;
            timeLabel.BackColor = Color.Transparent;

            // Okunmamış mesaj sayısı
            if (chat.UnreadCount > 0)
            {
                Panel unreadBadge = new Panel();
                unreadBadge.Size = new Size(22, 22);
                unreadBadge.Location = new Point(chatPanel.Width - 35, 45);
                unreadBadge.BackColor = Color.FromArgb(255, 99, 132);

                unreadBadge.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (Brush brush = new SolidBrush(Color.FromArgb(255, 99, 132)))
                    {
                        e.Graphics.FillEllipse(brush, 0, 0, 21, 21);
                    }

                    string count = chat.UnreadCount.ToString();
                    using (Font font = new Font("Segoe UI", 9, FontStyle.Bold))
                    using (Brush textBrush = new SolidBrush(Color.White))
                    {
                        SizeF textSize = e.Graphics.MeasureString(count, font);
                        float x = (22 - textSize.Width) / 2;
                        float y = (22 - textSize.Height) / 2;
                        e.Graphics.DrawString(count, font, textBrush, x, y);
                    }
                };

                chatPanel.Controls.Add(unreadBadge);
            }

            // Kontrolleri panele ekle
            chatPanel.Controls.Add(profileImagePanel);
            chatPanel.Controls.Add(nameLabel);
            chatPanel.Controls.Add(messageLabel);
            chatPanel.Controls.Add(timeLabel);

            // Hover efektleri
            chatPanel.MouseEnter += (s, e) =>
            {
                chatPanel.BackColor = Color.FromArgb(248, 248, 248);
            };

            chatPanel.MouseLeave += (s, e) =>
            {
                chatPanel.BackColor = Color.White;
            };

            // Tıklama olayı - Chat formunu aç
            chatPanel.Click += (s, e) => OpenChatForm(chat);

            // Tüm child kontrollerin de tıklama olayını tetiklemesi için
            foreach (Control ctrl in chatPanel.Controls)
            {
                ctrl.Click += (s, e) => OpenChatForm(chat);
                ctrl.MouseEnter += (s, e) => chatPanel.BackColor = Color.FromArgb(248, 248, 248);
                ctrl.MouseLeave += (s, e) => chatPanel.BackColor = Color.White;
            }

            return chatPanel;
        }

        // Chat formunu açan metod
        private void OpenChatForm(dynamic chat)
        {
            try
            {
                string matchedUserEmail = chat.Email;
                string matchedUserName = chat.Name;
                
                // Mevcut kullanıcı ve eşleşen kullanıcı bilgilerini getir
                var userRepo = new UserRepository();
                var currentUser = userRepo.GetUserByEmail(_email);
                var matchedUser = userRepo.GetUserByEmail(matchedUserEmail);
                
                if (currentUser == null || matchedUser == null)
                {
                    MessageBox.Show("Kullanıcı bilgileri alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Mesaj formunu oluşturup aç
                try 
                {
                    // Oluşturulmuş mesaj formunu arayıp, varsa odaklan
                    var existingForm = Application.OpenForms.OfType<view.ChatForm>()
                                .FirstOrDefault(f => f.MatchedUserEmail == matchedUserEmail);
                                
                    if (existingForm != null)
                    {
                        existingForm.Focus();
                        return;
                    }
                    
                    // Yeni mesaj formu oluştur
                    var chatForm = new view.ChatForm(_email, matchedUserEmail, matchedUserName);
                    chatForm.Show();
                    
                    // Okunmamış mesajları okundu olarak işaretle
                    if (currentUser != null && matchedUser != null)
                    {
                        var messageRepo = new MessageRepository();
                        messageRepo.MarkAllAsRead(matchedUser.Id, currentUser.Id);
                        
                        // Chat listesini yenile (okunmamış mesaj sayacını güncellemek için)
                        LoadMatchedChats();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Mesaj formu açılırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sohbet açılırken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // FlowLayoutPanel'in boyutunu ayarlayan metod (Form_Resize olayında çağrılabilir)
        private void AdjustFlowLayoutPanelSize()
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                foreach (Control control in flowLayoutPanel1.Controls)
                {
                    if (control is Panel panel)
                    {
                        panel.Width = flowLayoutPanel1.Width - 25;
                    }
                }
            }
        }

        // Form resize olayında chat panellerini yeniden boyutlandır
        private void Main_Resize(object sender, EventArgs e)
        {
            AdjustFlowLayoutPanelSize();
        }

        // Son mesaj zamanlarını kaydetmek için kullanılan metod
        private void InitializeLastMessageTimes()
        {
            try
            {
                var matchRepo = new MatchRepository();
                var messageRepo = new MessageRepository();
                
                // Tüm eşleşmeleri al
                var matches = matchRepo.GetMatchesByEmail(_email);
                
                // Her eşleşme için son mesaj zamanını sakla
                foreach (var match in matches)
                {
                    var lastMessage = messageRepo.GetLastMessage(_email, match.MatchedUserEmail);
                    if (lastMessage != null)
                    {
                        lastMessageTimes[match.MatchedUserEmail] = lastMessage.SentAt;
                    }
                    else
                    {
                        lastMessageTimes[match.MatchedUserEmail] = DateTime.MinValue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Son mesaj zamanları kaydedilirken hata: {ex.Message}");
            }
        }

        private void MessageCheckTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcı repository'i oluştur
                var userRepo = new UserRepository();
                var currentUser = userRepo.GetUserByEmail(_email);
                if (currentUser == null) return;

                // Eşleşme ve mesaj repolarını oluştur
                var matchRepo = new MatchRepository();
                var messageRepo = new MessageRepository();
                
                // Eşleşmeleri getir
                var matches = matchRepo.GetMatchesByEmail(_email);
                
                // Her eşleşme için yeni mesaj kontrolü yap
                foreach (var match in matches)
                {
                    // Son mesajı al
                    var lastMessage = messageRepo.GetLastMessage(_email, match.MatchedUserEmail);
                    
                    // Eğer son mesaj yoksa devam et
                    if (lastMessage == null) continue;
                    
                    // Son kaydedilen mesaj zamanını al, yoksa DateTime.MinValue
                    DateTime lastRecordedTime = DateTime.MinValue;
                    if (lastMessageTimes.ContainsKey(match.MatchedUserEmail))
                    {
                        lastRecordedTime = lastMessageTimes[match.MatchedUserEmail];
                    }
                    
                    // Eğer yeni mesaj varsa ve bu mesaj bize geldiyse bildirim göster
                    if (lastMessage.SentAt > lastRecordedTime && lastMessage.ReceiverId == currentUser.Id && !lastMessage.IsRead)
                    {
                        // Son mesaj zamanını güncelle
                        lastMessageTimes[match.MatchedUserEmail] = lastMessage.SentAt;
                        
                        // Okunmamış mesajı göster
                        ShowNewMessageNotification(match.MatchedUserName, match.MatchedUserEmail, lastMessage.Content);
                        
                        // Chat listesini güncelle
                        this.Invoke(new Action(() => LoadMatchedChats()));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mesaj kontrolü sırasında hata: {ex.Message}");
            }
        }
        
        private void ShowNewMessageNotification(string senderName, string senderEmail, string messageContent)
        {
            // Kısa mesaj içeriğini hazırla
            string shortMessage = messageContent;
            if (shortMessage.Length > 30)
                shortMessage = shortMessage.Substring(0, 27) + "...";
                
            // Bildirim penceresi göster
            DialogResult result = MessageBox.Show(
                $"{senderName} yeni bir mesaj gönderdi:\n\n{shortMessage}",
                "Yeni Mesaj",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information
            );
            
            // Eğer OK'a basıldıysa, sohbeti aç
            if (result == DialogResult.OK)
            {
                // Dynamic nesne oluştur
                dynamic chatData = new System.Dynamic.ExpandoObject();
                chatData.Email = senderEmail;
                chatData.Name = senderName;
                
                // Sohbeti aç
                this.Invoke(new Action(() => OpenChatForm(chatData)));
            }
        }
    }
}