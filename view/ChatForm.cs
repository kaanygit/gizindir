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
        
        // Dışarıdan erişim için
        public string MatchedUserEmail => _matchedUserEmail;
        
        public ChatForm(string currentUserEmail, string matchedUserEmail, string matchedUserName)
        {
            _currentUserEmail = currentUserEmail;
            _matchedUserEmail = matchedUserEmail;
            _matchedUserName = matchedUserName;
            
            InitializeComponents();
            LoadMessages();
            
            // Mesajları periyodik olarak yenile
            _refreshTimer.Interval = 5000; // 5 saniyede bir
            _refreshTimer.Tick += (s, e) => LoadMessages();
            _refreshTimer.Start();
        }
        
        private void InitializeComponents()
        {
            // Form ayarları
            this.Text = $"Sohbet - {_matchedUserName}";
            this.Size = new Size(400, 600);
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
            _messageBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SendMessage(); };
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
        
        private void LoadMessages()
        {
            try
            {
                var messageRepo = new MessageRepository();
                var messages = messageRepo.GetMessagesBetweenUsers(_currentUserEmail, _matchedUserEmail);
                
                // Mesajlar değişmediyse güncelleme yapma
                if (messages.Count == _messages.Count && messages.LastOrDefault()?.Id == _messages.LastOrDefault()?.Id)
                    return;
                
                _messages = messages;
                
                _messagesPanel.SuspendLayout();
                _messagesPanel.Controls.Clear();
                
                var userRepo = new UserRepository();
                var currentUser = userRepo.GetUserByEmail(_currentUserEmail);
                
                foreach (var message in messages)
                {
                    // Mesaj balonu oluştur
                    Panel messagePanel = CreateMessageBubble(message, currentUser?.Id ?? 0);
                    _messagesPanel.Controls.Add(messagePanel);
                }
                
                _messagesPanel.ResumeLayout();
                
                // Scroll'u en alta getir
                if (_messagesPanel.Controls.Count > 0)
                {
                    _messagesPanel.ScrollControlIntoView(_messagesPanel.Controls[_messagesPanel.Controls.Count - 1]);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mesajlar yüklenirken hata: {ex.Message}");
            }
        }
        
        private Panel CreateMessageBubble(gizindir.model.Message message, int currentUserId)
        {
            bool isMyMessage = message.SenderId == currentUserId;
            
            Panel bubblePanel = new Panel();
            bubblePanel.AutoSize = true;
            bubblePanel.MaximumSize = new Size(_messagesPanel.Width - 50, 0);
            bubblePanel.Padding = new Padding(10);
            bubblePanel.Margin = new Padding(isMyMessage ? 50 : 10, 5, isMyMessage ? 10 : 50, 5);
            bubblePanel.BackColor = isMyMessage ? Color.FromArgb(255, 99, 132) : Color.White;
            
            // Mesaj metni
            Label messageLabel = new Label();
            messageLabel.Text = message.Content;
            messageLabel.AutoSize = true;
            messageLabel.MaximumSize = new Size(bubblePanel.MaximumSize.Width - 20, 0);
            messageLabel.ForeColor = isMyMessage ? Color.White : Color.Black;
            messageLabel.Font = new Font("Segoe UI", 10);
            bubblePanel.Controls.Add(messageLabel);
            
            // Saat bilgisi
            Label timeLabel = new Label();
            timeLabel.Text = message.SentAt.ToString("HH:mm");
            timeLabel.AutoSize = true;
            timeLabel.ForeColor = isMyMessage ? Color.FromArgb(230, 230, 230) : Color.FromArgb(150, 150, 150);
            timeLabel.Font = new Font("Segoe UI", 8);
            timeLabel.Location = new Point(messageLabel.Right - timeLabel.Width, messageLabel.Bottom + 5);
            bubblePanel.Controls.Add(timeLabel);
            
            // Bubble panel'in boyutunu ayarla
            bubblePanel.Height = messageLabel.Height + timeLabel.Height + 25;
            
            // Sağ tarafa hizalama
            if (isMyMessage)
            {
                bubblePanel.Anchor = AnchorStyles.Right;
            }
            
            return bubblePanel;
        }
        
        private void SendMessage()
        {
            string content = _messageBox.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;
            
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
                
                // Mesajları yenile
                LoadMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesaj gönderilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
