using gizindir.data;
using gizindir.model;
using gizindir.helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gizindir.view
{
    public class ChatForm : Form
    {
        private string _currentUserEmail;
        private string _matchedUserEmail;
        private string _matchedUserName;
        private Timer _refreshTimer = new Timer();
        private FlowLayoutPanel _messagesPanel;
        private TextBox _messageBox;
        private Button _sendButton;
        private List<gizindir.model.Message> _messages = new List<gizindir.model.Message>();
        private int _lastMessageCount = 0; // Son mesaj sayısını takip et

        // Dışarıdan erişim için
        public string MatchedUserEmail => _matchedUserEmail;

        public ChatForm(string currentUserEmail, string matchedUserEmail, string matchedUserName)
        {
            _currentUserEmail = currentUserEmail;
            _matchedUserEmail = matchedUserEmail;
            _matchedUserName = matchedUserName;

            InitializeComponents();

            // Form boyutu değiştiğinde mesajları yeniden düzenle
            this.Resize += ChatForm_Resize;

            // Form açılır açılmaz mesajları yükle
            this.Shown += async (s, e) => 
            {
                await LoadMessagesAsync();
                // Form açıldıktan sonra ilk mesajı görebilmek için bir kez daha mesajları yükle
                await Task.Delay(200);
                await LoadMessagesAsync();
            };

            // Mesajları periyodik olarak yenile - daha sık kontrol et
            _refreshTimer.Interval = 2000; // 2 saniyede bir
            _refreshTimer.Tick += async (s, e) => await LoadMessagesAsync();
            _refreshTimer.Start();
        }

        private void InitializeComponents()
        {
            // Form ayarları
            this.Text = $"Sohbet - {_matchedUserName}";
            this.Size = new Size(450, 650); // Form boyutunu büyüt
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = true;
            this.MaximizeBox = true;
            this.FormClosing += (s, e) => _refreshTimer.Stop();

            // Ana panel
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(10);
            this.Controls.Add(mainPanel);

            // Üst panel - kullanıcı bilgileri
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.BackColor = Color.FromArgb(255, 99, 132);
            mainPanel.Controls.Add(topPanel);

            // Kullanıcı adı
            Label nameLabel = new Label();
            nameLabel.Text = _matchedUserName;
            nameLabel.ForeColor = Color.White;
            nameLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            nameLabel.Location = new Point(15, 15);
            nameLabel.AutoSize = true;
            topPanel.Controls.Add(nameLabel);

            // Mesajlar paneli
            _messagesPanel = new FlowLayoutPanel();
            _messagesPanel.Dock = DockStyle.Fill;
            _messagesPanel.FlowDirection = FlowDirection.TopDown;
            _messagesPanel.AutoScroll = true;
            _messagesPanel.WrapContents = false;
            _messagesPanel.BackColor = Color.FromArgb(245, 245, 245);
            _messagesPanel.Padding = new Padding(5, 15, 5, 5); // Üst padding'i azalt
            _messagesPanel.AutoScrollMargin = new Size(0, 20); // Scroll margin ekle
            _messagesPanel.AutoScrollMinSize = new Size(0, 20);
            mainPanel.Controls.Add(_messagesPanel);

            // Alt panel - mesaj yazma
            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 60;
            bottomPanel.BackColor = Color.White;
            bottomPanel.Padding = new Padding(10);
            mainPanel.Controls.Add(bottomPanel);

            // Mesaj yazma kutusu
            _messageBox = new TextBox();
            _messageBox.Multiline = false;
            _messageBox.Width = bottomPanel.Width - 100;
            _messageBox.Height = 40;
            _messageBox.Location = new Point(10, 10);
            _messageBox.Font = new Font("Segoe UI", 10);
            _messageBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            _messageBox.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true; // Enter tuşunun varsayılan davranışını engelle
                    SendMessage();
                }
            };
            bottomPanel.Controls.Add(_messageBox);

            // Gönder butonu
            _sendButton = new Button();
            _sendButton.Text = "Gönder";
            _sendButton.Width = 80;
            _sendButton.Height = 40;
            _sendButton.Location = new Point(bottomPanel.Width - 90, 10);
            _sendButton.BackColor = Color.FromArgb(255, 99, 132);
            _sendButton.ForeColor = Color.White;
            _sendButton.FlatStyle = FlatStyle.Flat;
            _sendButton.FlatAppearance.BorderSize = 0;
            _sendButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _sendButton.Click += (s, e) => SendMessage();
            bottomPanel.Controls.Add(_sendButton);
        }

        private async Task LoadMessagesAsync()
        {
            try
            {
                // UI thread'de çalıştır
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(async () => await LoadMessagesAsync()));
                    return;
                }

                var messageRepo = new MessageRepository();
                var messages = messageRepo.GetMessagesBetweenUsers(_currentUserEmail, _matchedUserEmail);

                // Mesaj sayısı değişmediyse ve son mesaj da aynıysa güncelleme yapma
                if (messages.Count == _lastMessageCount &&
                    messages.Count > 0 && _messages.Count > 0 &&
                    messages.LastOrDefault()?.Id == _messages.LastOrDefault()?.Id)
                    return;

                _messages = messages;
                _lastMessageCount = messages.Count;

                // UI'ı güncelle
                await UpdateMessagesUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mesajlar yüklenirken hata: {ex.Message}");
                // Hata durumunda kullanıcıyı bilgilendir (opsiyonel)
                // MessageBox.Show($"Mesajlar yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task UpdateMessagesUI()
        {
            await Task.Run(() =>
            {
                // UI thread'de çalıştır
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        _messagesPanel.SuspendLayout();
                        _messagesPanel.Controls.Clear();

                        var userRepo = new UserRepository();
                        var currentUser = userRepo.GetUserByEmail(_currentUserEmail);

                        if (currentUser == null) return;

                        foreach (var message in _messages)
                        {
                            // Mesaj balonu oluştur
                            Panel messagePanel = CreateMessageBubble(message, currentUser.Id);
                            _messagesPanel.Controls.Add(messagePanel);
                        }

                        _messagesPanel.ResumeLayout();
                        _messagesPanel.PerformLayout();

                        // Her zaman en son mesaja scroll yap
                        ScrollToBottom();
                        
                        // Panel'i yeniden çiz
                        _messagesPanel.Refresh();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"UI güncellenirken hata: {ex.Message}");
                    }
                }));
            });
        }

        private void ScrollToBottom()
        {
            if (_messagesPanel.Controls.Count > 0)
            {
                try
                {
                    // En son mesaja kaydır
                    var lastControl = _messagesPanel.Controls[_messagesPanel.Controls.Count - 1];
                    
                    // Önce varsayılan scroll pozisyonunu en alta ayarla
                    _messagesPanel.ScrollControlIntoView(lastControl);
                    
                    // Ek olarak programatik kaydırma
                    int maxScroll = _messagesPanel.VerticalScroll.Maximum;
                    _messagesPanel.AutoScrollPosition = new Point(0, maxScroll);
                    
                    // Son kontrol için ek delay sonrasında tekrar scroll
                    Task.Delay(50).ContinueWith(_ => 
                    {
                        try
                        {
                            if (this.IsDisposed) return;
                            
                            this.Invoke(new Action(() => 
                            {
                                _messagesPanel.ScrollControlIntoView(lastControl);
                                _messagesPanel.VerticalScroll.Value = _messagesPanel.VerticalScroll.Maximum;
                                _messagesPanel.Refresh();
                            }));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Gecikmeli scroll sırasında hata: {ex.Message}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Scroll sırasında hata: {ex.Message}");
                }
            }
        }

        private Panel CreateMessageBubble(gizindir.model.Message message, int currentUserId)
        {
            bool isMyMessage = message.SenderId == currentUserId;

            Panel bubblePanel = new Panel();
            bubblePanel.AutoSize = false; // Manuel boyut kontrolü
            bubblePanel.Width = _messagesPanel.Width - 60;
            bubblePanel.Padding = new Padding(10);
            bubblePanel.Margin = new Padding(isMyMessage ? 50 : 10, 5, isMyMessage ? 10 : 50, 5); // Margin'i artırarak mesajlar arasında daha fazla boşluk bırak
            bubblePanel.BackColor = isMyMessage ? Color.FromArgb(255, 99, 132) : Color.White;

            // Köşe yuvarlama efekti için (opsiyonel)
            if (isMyMessage)
            {
                bubblePanel.Anchor = AnchorStyles.Right;
            }

            // Mesaj metni
            Label messageLabel = new Label();
            messageLabel.Text = message.Content;
            messageLabel.AutoSize = true;
            messageLabel.MaximumSize = new Size(bubblePanel.Width - 20, 0);
            messageLabel.ForeColor = isMyMessage ? Color.White : Color.Black;
            messageLabel.Font = new Font("Segoe UI", 10);
            messageLabel.Location = new Point(10, 10);
            bubblePanel.Controls.Add(messageLabel);

            // Saat bilgisi
            Label timeLabel = new Label();
            timeLabel.Text = message.SentAt.ToString("HH:mm");
            timeLabel.AutoSize = true;
            timeLabel.ForeColor = isMyMessage ? Color.FromArgb(230, 230, 230) : Color.FromArgb(150, 150, 150);
            timeLabel.Font = new Font("Segoe UI", 8);
            timeLabel.Location = new Point(bubblePanel.Width - 50, messageLabel.Bottom + 5);
            bubblePanel.Controls.Add(timeLabel);

            // Bubble panel'in boyutunu ayarla - biraz daha ekstra alan ekle
            bubblePanel.Height = Math.Max(messageLabel.Height + timeLabel.Height + 30, 50);

            return bubblePanel;
        }

        private async void SendMessage()
        {
            string content = _messageBox.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            // Butonu geçici olarak devre dışı bırak
            _sendButton.Enabled = false;
            _messageBox.Enabled = false;

            try
            {
                var userRepo = new UserRepository();
                var currentUser = userRepo.GetUserByEmail(_currentUserEmail);
                var matchedUser = userRepo.GetUserByEmail(_matchedUserEmail);

                if (currentUser == null || matchedUser == null)
                {
                    MessageBox.Show("Kullanıcı bilgileri alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Mesajı veritabanına kaydet
                var messageRepo = new MessageRepository();
                messageRepo.SendMessage(currentUser.Id, matchedUser.Id, content);

                // Mesaj kutusunu temizle
                _messageBox.Text = "";

                // Mesajları hemen yenile
                await LoadMessagesAsync();

                // Focus'u mesaj kutusuna ver
                _messageBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesaj gönderilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Buton ve text box'ı yeniden etkinleştir
                _sendButton.Enabled = true;
                _messageBox.Enabled = true;
            }
        }

        // Form kapatılırken timer'ı durdur ve mesajları okundu olarak işaretle
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                // Timer'ı durdur
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();

                // Okunmamış mesajları okundu olarak işaretle
                var userRepo = new UserRepository();
                var messageRepo = new MessageRepository();
                
                var currentUser = userRepo.GetUserByEmail(_currentUserEmail);
                var matchedUser = userRepo.GetUserByEmail(_matchedUserEmail);
                
                if (currentUser != null && matchedUser != null)
                {
                    messageRepo.MarkAllAsRead(matchedUser.Id, currentUser.Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form kapatılırken hata: {ex.Message}");
            }
            finally
            {
                base.OnFormClosed(e);
            }
        }

        // Form boyutu değiştiğinde mesajları yeniden düzenle
        private void ChatForm_Resize(object sender, EventArgs e)
        {
            try
            {
                // Mesaj panelini güncelle
                foreach (Control control in _messagesPanel.Controls)
                {
                    if (control is Panel bubblePanel)
                    {
                        bubblePanel.Width = _messagesPanel.Width - 60;
                        
                        // İçerideki mesaj metnini bul ve genişliğini güncelle
                        foreach (Control child in bubblePanel.Controls)
                        {
                            if (child is Label messageLabel && child.Font.Size >= 10)
                            {
                                messageLabel.MaximumSize = new Size(bubblePanel.Width - 20, 0);
                            }
                            
                            // Zaman etiketini bul ve konumunu güncelle
                            if (child is Label timeLabel && child.Font.Size < 10)
                            {
                                timeLabel.Location = new Point(bubblePanel.Width - 50, timeLabel.Location.Y);
                            }
                        }
                    }
                }
                
                // Mesajları yeniden düzenlemek için refresh
                _messagesPanel.PerformLayout();
                _messagesPanel.Refresh();
                
                // Scroll pozisyonunu koru
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form yeniden boyutlandırılırken hata: {ex.Message}");
            }
        }
    }
}