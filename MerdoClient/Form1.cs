using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MerdoClient;

public partial class Form1 : Form
{
    private readonly AccountService _accountService = new();
    private readonly SettingsService _settingsService = new();
    private readonly MinecraftLauncherService _minecraftLauncherService;

    private readonly System.Windows.Forms.Timer _transitionTimer = new();
    private readonly System.Windows.Forms.Timer _launchTimer = new();
    private readonly System.Windows.Forms.Timer _onlineCheckTimer = new();

    private dynamic? _musicPlayer; // Windows Media Player COM
    
    private string _currentUser = string.Empty;
    private string _currentPassword = string.Empty;
    private double _transitionProgress;
    private int _launchProgress;
    private Image? _avatarImage = null;
    private bool _isRegisterMode = false;
    private bool _isFromSavedAccount = false;
    private int _onlinePlayers = 0;
    private int _maxPlayers = 10000;

    private struct NewsSlide
    {
        public string Title;
        public string Subtitle;
        public string Text;
        
        public NewsSlide(string title, string subtitle, string text)
        {
            Title = title;
            Subtitle = subtitle;
            Text = text;
        }
    }

    private readonly NewsSlide[] _newsSlides = new[]
    {
        new NewsSlide("TURNUVALAR", "KAZANMAYA HAZIR OL", "Haftalık turnuvalara katıl, rakiplerinle yarış ve büyük ödüllerin sahibi ol!"),
        new NewsSlide("GÜNCELLEME", "YENİ SÜRÜM YAYINLANDI", "Merdo Launcher v2.0 ile daha yüksek FPS ve optimize edilmiş ram kullanımı sizleri bekliyor."),
        new NewsSlide("ETKİNLİKLER", "BU HAFTA SONU %50 EKSTRA", "Hafta sonu boyunca geçerli tüm etkinliklerde ekstra kredi kazanma şansını kaçırma!")
    };
    
    private int _currentSlideIndex = 0;

    public Form1()
    {
        _minecraftLauncherService = new MinecraftLauncherService(_settingsService);
        InitializeComponent();
        
        // Setup Form properties
        Text = "Merdo Launcher - Giriş Yap";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        DoubleBuffered = true;
        BackColor = Color.FromArgb(8, 8, 10);
        StartPosition = FormStartPosition.CenterScreen;
        try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
        
        // Setup Panels
        pnlHome.Visible = false;
        pnlLogin.Visible = true;
        
        // Paint events for Logo Badge
        pnlLogin.Paint += Panel_Paint;
        pnlHome.Paint += Panel_Paint;

        // Custom Paint for cards and inputs to have anti-aliased rounded corners
        pnlLeftNewsCard.Paint += CardPanel_Paint;
        pnlRightLoginCard.Paint += CardPanel_Paint;
        pnlHomeLeftCard.Paint += CardPanel_Paint;
        pnlHomeRightCard.Paint += CardPanel_Paint;
        
        pnlUserContainer.Paint += InputPanel_Paint;
        pnlPassContainer.Paint += InputPanel_Paint;
        pnlSavedAccounts.Paint += InputPanel_Paint;

        // Custom Paint for Home card inner containers
        pnlOnlinePlayers.Paint += OnlinePlayers_Paint;
        pnlAvatar.Paint += pnlAvatar_Paint;
        pnlRoleBadge.Paint += RoleBadge_Paint;

        // Slide events
        btnNewsPrev.Click += (s, e) =>
        {
            _currentSlideIndex = (_currentSlideIndex - 1 + _newsSlides.Length) % _newsSlides.Length;
            UpdateNewsSlide();
        };
        
        btnNewsNext.Click += (s, e) =>
        {
            _currentSlideIndex = (_currentSlideIndex + 1) % _newsSlides.Length;
            UpdateNewsSlide();
        };

        // Transition Timer
        _transitionTimer.Interval = 16;
        _transitionTimer.Tick += TransitionTimer_Tick;

        // Launch Timer
        _launchTimer.Interval = 16;
        _launchTimer.Tick += LaunchTimer_Tick;

        // Setup Placeholders
        SetupPlaceholder(txtUsername, "Kullanıcı Adı");
        SetupPlaceholder(txtPassword, "Şifre", true);

        // Checkbox Flat Style
        chkRemember.FlatStyle = FlatStyle.Flat;
        chkRemember.FlatAppearance.BorderSize = 1;
        chkRemember.FlatAppearance.CheckedBackColor = Color.FromArgb(0, 120, 215);

        // Apply Hover Effects
        ApplyHoverEffect(btnLogin, Color.FromArgb(255, 220, 50), Color.Black);
        ApplyHoverEffect(btnPlay, Color.FromArgb(255, 220, 50), Color.Black);
        ApplyHoverEffect(btnWebsiteLink, Color.FromArgb(255, 220, 50), Color.Black);

        ApplyHoverEffect(btnShop, Color.FromArgb(35, 35, 40), Color.White);
        ApplyHoverEffect(btnWeb, Color.FromArgb(35, 35, 40), Color.White);
        ApplyHoverEffect(btnDiscord, Color.FromArgb(35, 35, 40), Color.White);
        ApplyHoverEffect(btnSettings, Color.FromArgb(35, 35, 40), Color.White);

        ApplyHoverEffect(btnNewsPrev, Color.FromArgb(255, 204, 0), Color.Black);
        ApplyHoverEffect(btnNewsNext, Color.FromArgb(255, 204, 0), Color.Black);

        ApplyHoverEffect(btnRules, Color.FromArgb(55, 55, 60), Color.White);
        ApplyHoverEffect(btnWebsite, Color.FromArgb(55, 55, 60), Color.White);
        ApplyHoverEffect(btnDiscordLink, Color.FromArgb(55, 55, 60), Color.White);
        ApplyHoverEffect(btnExit, Color.FromArgb(220, 50, 50), Color.White);

        // Settings button click
        btnSettings.Click += (s, e) =>
        {
            using var settingsForm = new SettingsForm(_settingsService, SetMusicVolume);
            settingsForm.ShowDialog(this);
        };

        // Initial Saved Accounts UI Load
        UpdateSavedAccountsUI();

        // Online players check timer
        _onlineCheckTimer.Interval = 20000;
        _onlineCheckTimer.Tick += (s, e) => UpdateOnlinePlayerCount();
        _onlineCheckTimer.Start();
        UpdateOnlinePlayerCount();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Arkaplan müziğini başlat
        StartBackgroundMusic();

        // Check for updates asynchronously
        UpdateCheckerService.CheckForUpdates(this);
        
        // Rounding only buttons and shapes (panels render smoothly through Paint events)
        MakeControlRounded(btnLogin, 8);
        MakeControlRounded(btnPlay, 8);
        MakeControlRounded(btnWebsiteLink, 17); // Pill shape (height is 34)
        
        MakeControlRounded(btnNewsPrev, 6);
        MakeControlRounded(btnNewsNext, 6);
        
        MakeControlRounded(btnShop, 18); // Circle (36x36)
        MakeControlRounded(btnWeb, 18);
        MakeControlRounded(btnDiscord, 18);
        MakeControlRounded(btnSettings, 18);

        MakeControlRounded(btnRules, 6);
        MakeControlRounded(btnWebsite, 6);
        MakeControlRounded(btnDiscordLink, 6);
        MakeControlRounded(btnExit, 6);
    }

    private void StartBackgroundMusic()
    {
        try
        {
            string musicFile = Path.Combine(
                Path.GetDirectoryName(Application.ExecutablePath) ?? AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "bg_music.mp3");

            if (!File.Exists(musicFile)) return;

            var type = Type.GetTypeFromProgID("WMPlayer.OCX");
            if (type == null) return;

            _musicPlayer = Activator.CreateInstance(type);
            _musicPlayer.settings.volume   = _settingsService.Settings.MusicVolume;
            _musicPlayer.settings.setMode("loop", true);
            _musicPlayer.URL = musicFile;
        }
        catch { }
    }

    private void SetMusicVolume(int volume)
    {
        try { if (_musicPlayer != null) _musicPlayer.settings.volume = volume; } catch { }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        try { _musicPlayer?.controls.stop(); } catch { }
        base.OnFormClosed(e);
    }

    private void MakeControlRounded(Control control, int radius)
    {
        var rect = new Rectangle(0, 0, control.Width, control.Height);
        using var path = GetRoundedRectanglePath(rect, radius);
        control.Region = new Region(path);
    }

    public static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int cornerRadius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = cornerRadius * 2;
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void Panel_Paint(object? sender, PaintEventArgs e)
    {
        DrawLogoBadge(e.Graphics, 0, 0);
    }

    private void CardPanel_Paint(object? sender, PaintEventArgs e)
    {
        var panel = (Panel)sender!;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(21, 21, 24)); // Card BG
        using var path = GetRoundedRectanglePath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 12);
        e.Graphics.FillPath(brush, path);
    }

    private void InputPanel_Paint(object? sender, PaintEventArgs e)
    {
        var panel = (Panel)sender!;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(13, 13, 15)); // Input BG
        using var path = GetRoundedRectanglePath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 8);
        e.Graphics.FillPath(brush, path);
    }

    private void OnlinePlayers_Paint(object? sender, PaintEventArgs e)
    {
        var panel = (Panel)sender!;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(13, 13, 15)); // Dark BG
        using var pen = new Pen(Color.FromArgb(30, 80, 140), 1); // Blue border
        using var path = GetRoundedRectanglePath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 17);
        
        e.Graphics.FillPath(brush, path);
        e.Graphics.DrawPath(pen, path);

        // Draw green dot
        using (var greenBrush = new SolidBrush(Color.FromArgb(40, 220, 80)))
        {
            e.Graphics.FillEllipse(greenBrush, 18, 12, 10, 10);
        }

        // Draw text
        using (var font = new Font("Segoe UI", 9.5F, FontStyle.Regular))
        using (var brushText = new SolidBrush(Color.FromArgb(180, 180, 190)))
        {
            e.Graphics.DrawString($"{_onlinePlayers} /{_maxPlayers} Çevrimiçi Oyuncu", font, brushText, 35, 7);
        }
    }

    private void UpdateOnlinePlayerCount()
    {
        System.Threading.Tasks.Task.Run(async () =>
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MerdoLauncher/2.0");

                var response = await client.GetStringAsync("https://api.mcsrvstat.us/2/91.132.49.16");
                using var doc = System.Text.Json.JsonDocument.Parse(response);
                if (doc.RootElement.TryGetProperty("players", out var playersElement))
                {
                    int online = 0;
                    if (playersElement.TryGetProperty("online", out var onlineProp))
                        online = onlineProp.GetInt32();
                    
                    if (playersElement.TryGetProperty("list", out var listProp) && listProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        int listCount = listProp.GetArrayLength();
                        if (listCount > online)
                            online = listCount;
                    }
                    _onlinePlayers = online;

                    int maxVal = 10000;
                    if (playersElement.TryGetProperty("max", out var maxProp))
                    {
                        int m = maxProp.GetInt32();
                        if (m > 0) maxVal = m;
                    }
                    _maxPlayers = maxVal;

                    if (pnlOnlinePlayers != null && !pnlOnlinePlayers.IsDisposed)
                    {
                        this.BeginInvoke(new Action(() => pnlOnlinePlayers.Invalidate()));
                    }
                }
            }
            catch
            {
                // Silent catch
            }
        });
    }


    private void pnlAvatar_Paint(object? sender, PaintEventArgs e)
    {
        var panel = (Panel)sender!;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Draw gold outer border
        using (var goldPen = new Pen(Color.FromArgb(255, 204, 0), 3))
        {
            e.Graphics.DrawEllipse(goldPen, 1, 1, panel.Width - 3, panel.Height - 3);
        }
        
        // Create circle clipping path for face
        using (var path = new GraphicsPath())
        {
            path.AddEllipse(4, 4, panel.Width - 8, panel.Height - 8);
            e.Graphics.SetClip(path);
        }
        
        // Draw avatar image or default fallback
        Rectangle faceRect = new Rectangle(4, 4, panel.Width - 8, panel.Height - 8);
        if (_avatarImage != null)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(_avatarImage, faceRect);
        }
        else
        {
            // Fallback clear if image not loaded
            e.Graphics.Clear(Color.FromArgb(80, 50, 30));
        }
        
        e.Graphics.ResetClip();
    }

    private void RoleBadge_Paint(object? sender, PaintEventArgs e)
    {
        var panel = (Panel)sender!;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var bgColor = panel.Tag is Color c ? c : Color.FromArgb(255, 204, 0); // Use tag color or default yellow
        using var brush = new SolidBrush(bgColor);
        using var path = GetRoundedRectanglePath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 4);
        e.Graphics.FillPath(brush, path);
    }

    private static Image? _largeLogo;
    private void DrawLogoBadge(Graphics g, int x, int y)
    {
        g.SmoothingMode      = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.InterpolationMode  = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.CompositingMode    = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        // --- Small circular logo (40x40) ---
        if (_largeLogo == null)
            try { _largeLogo = Image.FromFile(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo_large_new.png")); } catch { }

        int iconSize = 40;
        int iconX = x + 8;
        int iconY = y + 14;

        if (_largeLogo != null)
        {
            // Draw centered within the icon area – no clip needed (PNG is transparent)
            g.DrawImage(_largeLogo, iconX, iconY, iconSize, iconSize);
        }

        // --- "MERDO" bold text ---
        int textX = iconX + iconSize + 10;
        int textY = y + 18;
        using (var font  = new Font("Segoe UI", 14F, FontStyle.Bold))
        using (var brush = new SolidBrush(Color.White))
            g.DrawString("MERDO", font, brush, textX, textY);

        // --- "LAUNCHER" yellow badge ---
        int badgeX = textX + 82;
        int badgeY = y + 22;
        int badgeW = 78;
        int badgeH = 20;
        using (var badgePath = GetRoundedRectanglePath(new Rectangle(badgeX, badgeY, badgeW, badgeH), 5))
        using (var yellowBrush = new SolidBrush(Color.FromArgb(255, 200, 0)))
            g.FillPath(yellowBrush, badgePath);

        using (var font  = new Font("Segoe UI", 7.5F, FontStyle.Bold))
        using (var brush = new SolidBrush(Color.Black))
        {
            var sz = g.MeasureString("LAUNCHER", font);
            g.DrawString("LAUNCHER", font, brush,
                badgeX + (badgeW - sz.Width) / 2f,
                badgeY + (badgeH - sz.Height) / 2f);
        }
    }

    private void SetupPlaceholder(TextBox textBox, string placeholder, bool isPassword = false)
    {
        textBox.Text = placeholder;
        textBox.ForeColor = Color.FromArgb(85, 85, 94);
        if (isPassword) textBox.UseSystemPasswordChar = false;

        textBox.Enter += (s, e) =>
        {
            if (textBox.Text == placeholder)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.White;
                if (isPassword) textBox.UseSystemPasswordChar = true;
            }
        };

        textBox.Leave += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Color.FromArgb(85, 85, 94);
                if (isPassword) textBox.UseSystemPasswordChar = false;
            }
        };
    }

    private void UpdateNewsSlide()
    {
        var slide = _newsSlides[_currentSlideIndex];
        lblNewsTitle.Text = slide.Title;
        lblNewsSubtitle.Text = slide.Subtitle;
        lblNewsText.Text = slide.Text;
    }

    private void UpdateSavedAccountsUI()
    {
        pnlSavedAccounts.Controls.Clear();
        var saved = _accountService.GetSavedAccounts();
        if (saved.Count == 0)
        {
            pnlSavedAccounts.Controls.Add(lblSavedAccountsPlaceholder);
            lblSavedAccountsPlaceholder.Visible = true;
        }
        else
        {
            lblSavedAccountsPlaceholder.Visible = false;
            
            int xOffset = 10;
            int btnWidth = 115;
            int btnHeight = 28;
            int count = Math.Min(saved.Count, 2);
            
            for (int i = 0; i < count; i++)
            {
                var acc = saved[i];
                var btnAcc = new Button
                {
                    Text = acc.Username,
                    Size = new Size(btnWidth, btnHeight),
                    Location = new Point(xOffset, 8),
                    BackColor = Color.FromArgb(28, 28, 32),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
                };
                btnAcc.FlatAppearance.BorderSize = 0;
                
                // Click handler — kayıtlı hesapla doğrudan giriş (UI alanları atlanır)
                btnAcc.Click += (s, ev) => LoginDirect(acc.Username, acc.Password);
                
                pnlSavedAccounts.Controls.Add(btnAcc);
                MakeControlRounded(btnAcc, 6);
                ApplyHoverEffect(btnAcc, Color.FromArgb(45, 45, 50), Color.FromArgb(255, 204, 0));
                
                xOffset += 125;
            }
        }
    }

    private async System.Threading.Tasks.Task<bool> IsUsernameTakenOnServer(string username)
    {
        try
        {
            using var client = new System.Net.Http.HttpClient();
            client.Timeout = System.TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MerdoLauncher/2.0");

            var response = await client.GetStringAsync($"http://91.132.49.16:8080/check?username={System.Uri.EscapeDataString(username)}");
            using var doc = System.Text.Json.JsonDocument.Parse(response);
            if (doc.RootElement.TryGetProperty("registered", out var registeredProp))
            {
                return registeredProp.GetBoolean();
            }
        }
        catch (System.Exception)
        {
            // Eğer sunucu kapalıysa veya hata alınırsa kayıt işlemine engel olmamak için false dönüyoruz
        }
        return false;
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text == "Kullanıcı Adı" ? "" : txtUsername.Text.Trim();
        string password = txtPassword.Text == "Şifre" ? "" : txtPassword.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowMessage("Kullanıcı adı ve şifre gerekli.", MessageBoxIcon.Warning);
            return;
        }

        if (_isRegisterMode)
        {
            if (_accountService.HasReachedRegisterLimit())
            {
                ShowMessage("Bu cihazda en fazla 3 hesap oluşturulabilir.", MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            lblStatus.Text = "Kullanıcı adı kontrol ediliyor...";

            bool isTaken = await IsUsernameTakenOnServer(username);
            if (isTaken)
            {
                btnLogin.Enabled = true;
                lblStatus.Text = "Kayıt başarısız.";
                ShowMessage("Bu kullanıcı adı zaten sunucuda kayıtlı. Lütfen başka bir kullanıcı adı seçin.", MessageBoxIcon.Error);
                return;
            }

            if (_accountService.Register(username, password))
            {
                _currentUser = username;
              _currentPassword = password;
                lblWelcome.Text = username.ToUpper();
                lblStatus.Text = "Hesap oluşturuldu. Oynamaya hazırsın.";
                
                if (chkRemember.Checked)
                {
                    _accountService.SaveAccountCredential(username, password);
                }
                
                ToggleRegisterMode(false);
                StartTransition();
                ShowMessage("Kayıt başarılı. Ana sayfaya yönlendiriliyorsun.", MessageBoxIcon.Information);
            }
            else
            {
                ShowMessage("Bu kullanıcı adı zaten kayıtlı.", MessageBoxIcon.Error);
            }
            
            btnLogin.Enabled = true;
        }
        else
        {
            if (_accountService.Login(username, password))
            {
                _currentUser = username;
              _currentPassword = password;
                lblWelcome.Text = username.ToUpper();
                lblStatus.Text = "Hazır. Oynamaya başlamak için Oyna butonuna basın.";
                
                if (chkRemember.Checked)
                {
                    _accountService.SaveAccountCredential(username, password);
                }
                else if (!_isFromSavedAccount)
                {
                    // Kayıtlı hesap butonuyla giriş yapılıyorsa hesabı silme
                    _accountService.RemoveSavedAccount(username);
                }
                
                StartTransition();
            }
            else
            {
                ShowMessage("Giriş başarısız. Kayıtlı kullanıcı bilgilerini kontrol edin.", MessageBoxIcon.Error);
            }
        }
        
        UpdateSavedAccountsUI();
    }

    // Kayıtlı hesap butonlarından çağrılır — textbox placeholder mantığını tamamen atlatır
    private void LoginDirect(string username, string password)
    {
        if (_accountService.Login(username, password))
        {
            _currentUser = username;
            _currentPassword = password;
            lblWelcome.Text = username.ToUpper();
            lblStatus.Text = "Hazır. Oynamaya başlamak için Oyna butonuna basın.";

            // Giriş alanlarını da güncelle (görsel tutarlılık)
            txtUsername.Text = username;
            txtUsername.ForeColor = Color.White;
            txtPassword.Text = password;
            txtPassword.ForeColor = Color.White;
            txtPassword.UseSystemPasswordChar = true;

            StartTransition();
        }
        else
        {
            ShowMessage($"Giriş başarısız: '{username}' için kayıtlı bilgiler doğrulanamadı.", MessageBoxIcon.Error);
        }
    }

    private void lblRegister_Click(object sender, EventArgs e)
    {
        ToggleRegisterMode(!_isRegisterMode);
    }

    private void ToggleRegisterMode(bool register)
    {
        _isRegisterMode = register;
        if (_isRegisterMode)
        {
            lblLoginTitle.Text = "KAYIT OL";
            btnLogin.Text = "Kayıt Ol";
            lblRegister.Text = "Zaten hesabın var mı? Giriş yap";
        }
        else
        {
            lblLoginTitle.Text = "GİRİŞ";
            btnLogin.Text = "Giriş Yap";
            lblRegister.Text = "Yeni hesap oluştur";
        }
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
        _currentUser = "";
        txtUsername.Text = "";
        txtPassword.Text = "";
        SetupPlaceholder(txtUsername, "Kullanıcı Adı");
        SetupPlaceholder(txtPassword, "Şifre", true);
        
        Text = "Chicken Launcher - Giriş Yap";
        pnlHome.Visible = false;
        pnlLogin.Visible = true;
        
        UpdateSavedAccountsUI();
    }

    private async void btnPlay_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_currentUser))
        {
            ShowMessage("Önce giriş yapmanız gerekiyor.", MessageBoxIcon.Warning);
            return;
        }

        // UI'ı kilitle, ilerleme çubuğunu başlat
        btnPlay.Enabled = false;
        lblStatus.Text  = "Minecraft hazırlanıyor...";
        pbLaunch.Value  = 4;
        _launchProgress = 4;
        _launchTimer.Start();

        // Minecraft'ı arka planda başlat — UI donmasın
        var result = await System.Threading.Tasks.Task.Run(
            () => _minecraftLauncherService.StartMinecraft(_currentUser, _currentPassword, msg => Invoke(() => lblStatus.Text = msg)));

        _launchTimer.Stop();
        btnPlay.Enabled = true;

        if (result.Success)
        {
            pbLaunch.Value = 100;
            lblStatus.Text = "Minecraft başlatıldı. İyi oyunlar!";

            if (_settingsService.Settings.CloseOnLaunch)
                Application.Exit();
        }
        else
        {
            pbLaunch.Value = 0;
            lblStatus.Text = "Başlatma başarısız.";
            ShowMessage(result.Message, MessageBoxIcon.Error);
        }
    }

    private void StartTransition()
    {
        pnlHome.Visible = true;
        Text = "Merdo Launcher - Oyun Ekranı";
        _transitionProgress = 0;
        _transitionTimer.Start();
        
        // Fetch avatar and role dynamically when entering home
        _ = FetchPlayerAvatarAndRole(_currentUser);
    }

    private async System.Threading.Tasks.Task FetchPlayerAvatarAndRole(string username)
    {
        try
        {
            lblRole.Text = "OYUNCU"; // Default

            using var client = new System.Net.Http.HttpClient();
            client.Timeout = System.TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MerdoLauncher/2.0");

            // 1. Instant override for known staff to avoid API delays
            string? instantRole = null;
            var lowerUser = username.ToLower();
            if (lowerUser == "merdo" || lowerUser == "merdo242")
                instantRole = "KURUCU";

            if (instantRole != null)
            {
                UpdateRoleBadge(instantRole);
            }
            else
            {
                try
                {
                    var response = await client.GetStringAsync($"http://91.132.49.16:24454/check?username={System.Uri.EscapeDataString(username)}");
                    using var doc = System.Text.Json.JsonDocument.Parse(response);
                    if (doc.RootElement.TryGetProperty("role", out var roleProp))
                    {
                        var roleStr = roleProp.GetString();
                        if (!string.IsNullOrEmpty(roleStr))
                        {
                            UpdateRoleBadge(roleStr);
                        }
                    }
                }
                catch { }
            }

            // 2. Fetch Avatar
            try
            {
                var imgBytes = await client.GetByteArrayAsync($"https://minotar.net/helm/{System.Uri.EscapeDataString(username)}/100.png");
                using var ms = new System.IO.MemoryStream(imgBytes);
                var img = Image.FromStream(ms);
                
                Invoke(() => 
                {
                    if (_avatarImage != null) _avatarImage.Dispose();
                    _avatarImage = img;
                    pnlAvatar.Invalidate();
                });
            }
            catch { }
        }
        catch { }
    }

    private void UpdateRoleBadge(string roleStr)
    {
        Invoke(() =>
        {
            lblRole.Text = roleStr.ToUpper();
            // Set badge color based on role
            var roleLower = roleStr.ToLower();
            Color badgeColor;
            if (roleLower.Contains("kurucu") || roleLower.Contains("founder"))
                badgeColor = Color.FromArgb(255, 165, 0); // Orange/Gold
            else if (roleLower.Contains("admin") || roleLower.Contains("owner"))
                badgeColor = Color.FromArgb(220, 50, 50);  // Red
            else if (roleLower.Contains("mod"))
                badgeColor = Color.FromArgb(130, 80, 220); // Purple
            else if (roleLower.Contains("vip"))
                badgeColor = Color.FromArgb(0, 180, 220);  // Blue
            else
                badgeColor = Color.FromArgb(255, 204, 0);  // Yellow (default Oyuncu)

            // Resize badge panel to fit text
            using var g = pnlRoleBadge.CreateGraphics();
            var sz = g.MeasureString(lblRole.Text, lblRole.Font);
            int newW = (int)sz.Width + 20;
            pnlRoleBadge.Width  = newW;
            lblRole.Width       = newW;
            // Re-center badge horizontally
            pnlRoleBadge.Left = (pnlHomeRightCard.Width - newW) / 2;

            // Store color for paint
            pnlRoleBadge.Tag = badgeColor;
            pnlRoleBadge.Invalidate();
        });
    }

    private void TransitionTimer_Tick(object? sender, EventArgs e)
    {
        _transitionProgress += 0.08;
        if (_transitionProgress >= 1)
        {
            _transitionProgress = 1;
            _transitionTimer.Stop();
        }

        int loginAlpha = (int)(255 * (1 - _transitionProgress));
        int homeAlpha = (int)(255 * _transitionProgress);
        pnlLogin.BackColor = Color.FromArgb(loginAlpha, 8, 8, 10);
        pnlHome.BackColor = Color.FromArgb(homeAlpha, 8, 8, 10);
        if (_transitionProgress >= 1)
        {
            pnlLogin.Visible = false;
        }
        else
        {
            pnlLogin.Visible = true;
            pnlHome.Visible = true;
        }
    }

    private void LaunchTimer_Tick(object? sender, EventArgs e)
    {
        if (_launchProgress < 100)
        {
            _launchProgress += 3;
            pbLaunch.Value = Math.Min(_launchProgress, 100);
        }
        else
        {
            _launchTimer.Stop();
        }
    }

    private void ApplyHoverEffect(Button button, Color hoverBg, Color hoverFg)
    {
        Color originalBg = button.BackColor;
        Color originalFg = button.ForeColor;
        button.Cursor = Cursors.Hand;
        
        button.MouseEnter += (_, _) =>
        {
            button.BackColor = hoverBg;
            button.ForeColor = hoverFg;
        };

        button.MouseLeave += (_, _) =>
        {
            button.BackColor = originalBg;
            button.ForeColor = originalFg;
        };
    }

    private void ShowMessage(string message, MessageBoxIcon icon)
    {
        if (icon == MessageBoxIcon.Error)
            MerdoDialog.ShowError(this, message);
        else if (icon == MessageBoxIcon.Warning)
            MerdoDialog.ShowWarning(this, message);
        else
            MerdoDialog.ShowInfo(this, message);
    }
}
