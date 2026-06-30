using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace MerdoClient;

public class SettingsForm : Form
{
    private readonly SettingsService _settings;
    private readonly Action<int>?    _onVolumeChanged;

    private CustomSlider trkRam        = new();
    private Label        lblRamValue   = new();
    private CustomSlider trkVolume     = new();
    private Label        lblVolumeValue = new();
    private CustomToggle chkClose      = new();
    private CustomToggle chkConsole    = new();
    private Label        lblJavaPath   = new();
    private Button       btnJavaAuto   = new();
    private Button       btnJavaManual = new();
    private Button       btnSave       = new();
    private Button       btnCancel     = new();
    private Label        lblJavaStatus = new();
    
    private System.Windows.Forms.Timer _fadeTimer = new System.Windows.Forms.Timer { Interval = 15 };

    private const int ACCENT     = unchecked((int)0xFFFFCC00); // sarı
    private const int BG         = unchecked((int)0xFF0C0C0F);
    private const int CARD_BG    = unchecked((int)0xFF151518);
    private const int BORDER_CLR = unchecked((int)0xFF252530);
    private const int TEXT_DIM   = unchecked((int)0xFF9595A0);

    public SettingsForm(SettingsService settings, Action<int>? onVolumeChanged = null)
    {
        _settings        = settings;
        _onVolumeChanged = onVolumeChanged;
        BuildUI();
        LoadValues();
        
        // Fade in animation setup
        Opacity = 0;
        _fadeTimer.Tick += (s, e) =>
        {
            if (Opacity >= 1)
            {
                Opacity = 1;
                _fadeTimer.Stop();
            }
            else
            {
                Opacity += 0.08;
            }
        };
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _fadeTimer.Start();
    }

    // ─── Rounded rectangle helper ───────────────────────────────────────
    public static GraphicsPath RoundedRect(Rectangle r, int rad)
    {
        var p = new GraphicsPath();
        if (rad <= 0)
        {
            p.AddRectangle(r);
            return p;
        }
        int d = rad * 2;
        p.AddArc(r.X,              r.Y,              d, d, 180, 90);
        p.AddArc(r.Right - d,      r.Y,              d, d, 270, 90);
        p.AddArc(r.Right - d,      r.Bottom - d,     d, d,   0, 90);
        p.AddArc(r.X,              r.Bottom - d,     d, d,  90, 90);
        p.CloseFigure();
        return p;
    }

    // ─── Card panel helper ───────────────────────────────────────────────
    private Panel Card(int x, int y, int w, int h)
    {
        var pnl = new Panel
        {
            Location = new Point(x, y),
            Size     = new Size(w, h),
            BackColor = Color.Transparent
        };
        pnl.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1);
            using var bg  = new SolidBrush(Color.FromArgb(CARD_BG));
            using var path = RoundedRect(r, 12);
            e.Graphics.FillPath(bg, path);
            using var pen = new Pen(Color.FromArgb(BORDER_CLR), 1);
            e.Graphics.DrawPath(pen, path);
        };
        Controls.Add(pnl);
        return pnl;
    }

    // ─── Section label helper ────────────────────────────────────────────
    private void SectionHeader(Panel card, string icon, string title, int y)
    {
        var lbl = new Label
        {
            Text      = $"{icon}  {title}",
            ForeColor = Color.FromArgb(ACCENT),
            Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Location  = new Point(20, y),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card.Controls.Add(lbl);

        var line = new Panel
        {
            Location  = new Point(20, y + 26),
            Size      = new Size(card.Width - 40, 1),
            BackColor = Color.FromArgb(BORDER_CLR)
        };
        card.Controls.Add(line);
    }

    private void BuildUI()
    {
        Text            = "Merdo Launcher — Ayarlar";
        Size            = new Size(580, 680);
        FormBorderStyle = FormBorderStyle.None;     // borderless
        MaximizeBox     = false;
        MinimizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = Color.FromArgb(BG);
        DoubleBuffered  = true;

        // Custom border + title paint
        Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedRect(r, 16);
            Region = new Region(path);
            using var pen = new Pen(Color.FromArgb(BORDER_CLR), 1.5f);
            e.Graphics.DrawPath(pen, path);
        };

        // Drag to move
        MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); } };

        // ── Başlık ──────────────────────────────────────────────────────
        var lblTitle = new Label
        {
            Text      = "⚙  AYARLAR",
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 15F, FontStyle.Bold),
            Location  = new Point(24, 22),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        Controls.Add(lblTitle);

        // Kapat butonu (X)
        var btnClose = new Label
        {
            Text      = "✕",
            ForeColor = Color.FromArgb(140, 140, 150),
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location  = new Point(Width - 48, 20),
            AutoSize  = true,
            BackColor = Color.Transparent,
            Cursor    = Cursors.Hand
        };
        btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.FromArgb(220, 60, 60);
        btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.FromArgb(140, 140, 150);
        btnClose.Click      += (s, e) => Close();
        Controls.Add(btnClose);

        // Başlık ayırıcı
        Controls.Add(new Panel
        {
            Location  = new Point(0, 65),
            Size      = new Size(Width, 1),
            BackColor = Color.FromArgb(BORDER_CLR)
        });

        int cardX = 24, cardW = Width - 48;

        // ══════════════════════════════════════════════
        // KART 1 — PERFORMANS + MÜZİK
        // ══════════════════════════════════════════════
        var card1 = Card(cardX, 85, cardW, 230);

        SectionHeader(card1, "🎮", "PERFORMANS", 18);

        var lblRamLbl = new Label
        {
            Text      = "Maksimum RAM",
            ForeColor = Color.FromArgb(TEXT_DIM),
            Font      = new Font("Segoe UI", 9.5F),
            Location  = new Point(20, 56),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblRamLbl);

        lblRamValue = new Label
        {
            Text      = "4 GB",
            ForeColor = Color.FromArgb(ACCENT),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location  = new Point(card1.Width - 80, 52),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblRamValue);

        trkRam = new CustomSlider
        {
            Location      = new Point(20, 80),
            Size          = new Size(card1.Width - 40, 24),
            Minimum       = 1024,
            Maximum       = 16384,
            Value         = 4096
        };
        trkRam.ValueChanged += (s, e) => lblRamValue.Text = $"{trkRam.Value / 1024.0:0.#} GB";
        card1.Controls.Add(trkRam);

        // Müzik ayırıcı
        card1.Controls.Add(new Panel
        {
            Location  = new Point(20, 122),
            Size      = new Size(card1.Width - 40, 1),
            BackColor = Color.FromArgb(BORDER_CLR)
        });

        SectionHeader(card1, "🎵", "MÜZİK", 134);

        var lblVolLbl = new Label
        {
            Text      = "Arkaplan Müzik Sesi",
            ForeColor = Color.FromArgb(TEXT_DIM),
            Font      = new Font("Segoe UI", 9.5F),
            Location  = new Point(20, 172),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblVolLbl);

        lblVolumeValue = new Label
        {
            Text      = "25%",
            ForeColor = Color.FromArgb(ACCENT),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location  = new Point(card1.Width - 80, 168),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblVolumeValue);

        trkVolume = new CustomSlider
        {
            Location      = new Point(20, 196),
            Size          = new Size(card1.Width - 40, 24),
            Minimum       = 0,
            Maximum       = 100,
            Value         = 25
        };
        trkVolume.ValueChanged += (s, e) =>
        {
            lblVolumeValue.Text = $"{trkVolume.Value}%";
            _onVolumeChanged?.Invoke(trkVolume.Value);
        };
        card1.Controls.Add(trkVolume);

        // ══════════════════════════════════════════════
        // KART 2 — BAŞLATICI
        // ══════════════════════════════════════════════
        var card2 = Card(cardX, 330, cardW, 110);

        SectionHeader(card2, "🚀", "BAŞLATICI", 18);

        chkClose = new CustomToggle
        {
            Location = new Point(20, 56),
            Text = "Oyun açılınca launcher'ı kapat",
            Checked = true
        };
        card2.Controls.Add(chkClose);

        chkConsole = new CustomToggle
        {
            Location = new Point(20, 84),
            Text = "Konsol penceresini göster (hata ayıklama)",
            Checked = false
        };
        card2.Controls.Add(chkConsole);

        // ══════════════════════════════════════════════
        // KART 3 — JAVA
        // ══════════════════════════════════════════════
        var card3 = Card(cardX, 455, cardW, 155);

        SectionHeader(card3, "☕", "JAVA", 18);

        var lblJavaLbl = new Label
        {
            Text      = "Mevcut Java Yolu",
            ForeColor = Color.FromArgb(TEXT_DIM),
            Font      = new Font("Segoe UI", 9F),
            Location  = new Point(20, 54),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card3.Controls.Add(lblJavaLbl);

        lblJavaPath = new Label
        {
            Text         = "Algılanıyor...",
            ForeColor    = Color.FromArgb(120, 120, 135),
            Font         = new Font("Segoe UI", 8.5F),
            Location     = new Point(20, 72),
            Size         = new Size(card3.Width - 40, 16),
            AutoEllipsis = true,
            BackColor    = Color.Transparent
        };
        card3.Controls.Add(lblJavaPath);

        lblJavaStatus = new Label
        {
            Text      = "",
            ForeColor = Color.FromArgb(40, 200, 90),
            Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
            Location  = new Point(20, 92),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card3.Controls.Add(lblJavaStatus);

        btnJavaAuto = StyledButton("⬇ Java'yı Otomatik Kur", new Point(20, 115), new Size(220, 32),
                                   Color.FromArgb(0, 140, 70), Color.White);
        btnJavaAuto.Click += BtnJavaAuto_Click;
        card3.Controls.Add(btnJavaAuto);

        btnJavaManual = StyledButton("📂 Manuel Seç", new Point(250, 115), new Size(130, 32),
                                     Color.FromArgb(35, 35, 45), Color.White);
        btnJavaManual.FlatAppearance.BorderColor = Color.FromArgb(BORDER_CLR);
        btnJavaManual.FlatAppearance.BorderSize  = 1;
        btnJavaManual.Click += BtnJavaManual_Click;
        card3.Controls.Add(btnJavaManual);

        // ── Alt Kaydet / İptal ──────────────────────────────────────────
        btnSave = StyledButton("✔  Kaydet", new Point(Width - 240, 625), new Size(120, 38),
                               Color.FromArgb(ACCENT), Color.Black);
        btnSave.Click += BtnSave_Click;
        Controls.Add(btnSave);

        btnCancel = StyledButton("İptal", new Point(Width - 110, 625), new Size(86, 38),
                                 Color.FromArgb(32, 32, 42), Color.White);
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(BORDER_CLR);
        btnCancel.FlatAppearance.BorderSize  = 1;
        btnCancel.Click += (s, e) => Close();
        Controls.Add(btnCancel);
    }

    // ─── Helpers ────────────────────────────────────────────────────────
    private static Button StyledButton(string text, Point loc, Size sz, Color bg, Color fg)
    {
        var btn = new Button
        {
            Text      = text,
            Location  = loc,
            Size      = sz,
            BackColor = bg,
            ForeColor = fg,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Cursor    = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        // Rounded region
        btn.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
            using var path = RoundedRect(r, 8);
            using var brush = new SolidBrush(bg);
            e.Graphics.FillPath(brush, path);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var pen = new SolidBrush(fg);
            e.Graphics.DrawString(btn.Text, btn.Font, pen, new RectangleF(0, 0, btn.Width, btn.Height), sf);
        };
        return btn;
    }

    // ─── P/Invoke for dragging ───────────────────────────────────────────
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    // ─── Load / Save ─────────────────────────────────────────────────────
    private void LoadValues()
    {
        var s = _settings.Settings;

        trkRam.Value      = Math.Clamp(s.MaxRamMb, trkRam.Minimum, trkRam.Maximum);
        lblRamValue.Text  = $"{trkRam.Value / 1024.0:0.#} GB";

        trkVolume.Value      = Math.Clamp(s.MusicVolume, 0, 100);
        lblVolumeValue.Text  = $"{trkVolume.Value}%";

        chkClose.Checked   = s.CloseOnLaunch;
        chkConsole.Checked = s.ShowConsole;

        string java = string.IsNullOrEmpty(s.JavaPath)
            ? MinecraftLauncherService.FindSystemJavaPath()
            : s.JavaPath;

        if (!string.IsNullOrEmpty(java))
        {
            lblJavaPath.Text        = java;
            lblJavaStatus.Text      = "✔  Java bulundu";
            lblJavaStatus.ForeColor = Color.FromArgb(40, 200, 90);
        }
        else
        {
            lblJavaPath.Text        = "Java bulunamadı — otomatik kurulum önerilir";
            lblJavaStatus.Text      = "✘  Java yok";
            lblJavaStatus.ForeColor = Color.FromArgb(220, 60, 60);
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var s = _settings.Settings;
        s.MaxRamMb      = trkRam.Value;
        s.MusicVolume   = trkVolume.Value;
        s.CloseOnLaunch = chkClose.Checked;
        s.ShowConsole   = chkConsole.Checked;
        _settings.Save();
        Close();
    }

    private void BtnJavaAuto_Click(object? sender, EventArgs e)
    {
        btnJavaAuto.Enabled = false;
        btnJavaAuto.Text    = "Kuruluyor...";
        lblJavaStatus.Text  = "⏳ winget ile Java indiriliyor...";
        lblJavaStatus.ForeColor = Color.FromArgb(255, 200, 50);

        Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName               = "winget",
                    Arguments              = "install --id EclipseAdoptium.Temurin.21.JRE --accept-source-agreements --accept-package-agreements --silent",
                    UseShellExecute        = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    CreateNoWindow         = true
                };

                using var proc = Process.Start(psi)!;
                proc.WaitForExit();

                Invoke(() =>
                {
                    string java = MinecraftLauncherService.FindSystemJavaPath();
                    if (!string.IsNullOrEmpty(java))
                    {
                        lblJavaPath.Text        = java;
                        lblJavaStatus.Text      = "✔  Java başarıyla kuruldu!";
                        lblJavaStatus.ForeColor = Color.FromArgb(40, 200, 90);
                        _settings.Settings.JavaPath = java;
                        _settings.Save();
                    }
                    else
                    {
                        lblJavaStatus.Text      = "⚠  Kuruldu ama algılanamadı. Sistemi yeniden başlatın.";
                        lblJavaStatus.ForeColor = Color.FromArgb(255, 180, 0);
                    }
                    btnJavaAuto.Text    = "⬇ Java'yı Otomatik Kur";
                    btnJavaAuto.Enabled = true;
                });
            }
            catch (Exception ex)
            {
                Invoke(() =>
                {
                    lblJavaStatus.Text      = "✘  Kurulum başarısız: " + ex.Message;
                    lblJavaStatus.ForeColor = Color.FromArgb(220, 60, 60);
                    btnJavaAuto.Text    = "⬇ Java'yı Otomatik Kur";
                    btnJavaAuto.Enabled = true;
                });
            }
        });
    }

    private void BtnJavaManual_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title    = "javaw.exe dosyasını seçin",
            Filter   = "javaw.exe|javaw.exe|Tüm dosyalar|*.*",
            FileName = "javaw.exe"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _settings.Settings.JavaPath = dlg.FileName;
            lblJavaPath.Text        = dlg.FileName;
            lblJavaStatus.Text      = "✔  Manuel Java seçildi";
            lblJavaStatus.ForeColor = Color.FromArgb(40, 200, 90);
        }
    }
}

// ─── Custom Slider Control ─────────────────────────────────────────────────
public class CustomSlider : Control
{
    public int Minimum { get; set; } = 0;
    public int Maximum { get; set; } = 100;

    private int _value = 50;
    public int Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, Minimum, Maximum);
            Invalidate();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? ValueChanged;

    private bool _isDragging = false;

    public CustomSlider()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        BackColor = Color.Transparent;
        DoubleBuffered = true;
        Cursor = Cursors.Hand;
        Size = new Size(200, 24);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            _isDragging = true;
            UpdateValueFromMouse(e.X);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_isDragging)
        {
            UpdateValueFromMouse(e.X);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _isDragging = false;
    }

    private void UpdateValueFromMouse(int x)
    {
        int thumbW = 16;
        int trackX = thumbW / 2;
        int trackW = Width - thumbW;
        
        float pct = (float)(x - trackX) / trackW;
        pct = Math.Clamp(pct, 0f, 1f);
        
        Value = Minimum + (int)(pct * (Maximum - Minimum));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        int trackH = 6;
        int thumbW = 16;
        int trackY = (Height - trackH) / 2;
        int trackX = thumbW / 2;
        int trackW = Width - thumbW;

        // Draw background track
        var trackRect = new Rectangle(trackX, trackY, trackW, trackH);
        using var trackPath = SettingsForm.RoundedRect(trackRect, trackH / 2);
        using var trackBrush = new SolidBrush(Color.FromArgb(40, 40, 50));
        e.Graphics.FillPath(trackBrush, trackPath);

        // Draw filled track
        float pct = (float)(Value - Minimum) / (Maximum - Minimum);
        int fillW = (int)(trackW * pct);
        if (fillW > 0)
        {
            var fillRect = new Rectangle(trackX, trackY, fillW, trackH);
            using var fillPath = SettingsForm.RoundedRect(fillRect, trackH / 2);
            using var fillBrush = new SolidBrush(Color.FromArgb(unchecked((int)0xFFFFCC00))); // Accent yellow
            e.Graphics.FillPath(fillBrush, fillPath);
        }

        // Draw thumb
        int thumbX = trackX + fillW - (thumbW / 2);
        int thumbY = (Height - thumbW) / 2;
        var thumbRect = new Rectangle(thumbX, thumbY, thumbW, thumbW);
        using var thumbBrush = new SolidBrush(Color.White);
        e.Graphics.FillEllipse(thumbBrush, thumbRect);
    }
}

// ─── Custom Toggle (Checkbox) Control ──────────────────────────────────────
public class CustomToggle : Control
{
    private bool _checked;
    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked != value)
            {
                _checked = value;
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler? CheckedChanged;

    public CustomToggle()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        BackColor = Color.Transparent;
        DoubleBuffered = true;
        Cursor = Cursors.Hand;
        Size = new Size(300, 24);
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Checked = !Checked;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        int toggleW = 40;
        int toggleH = 20;
        int toggleY = (Height - toggleH) / 2;
        
        var rect = new Rectangle(0, toggleY, toggleW, toggleH);
        using var path = SettingsForm.RoundedRect(rect, toggleH / 2);

        if (Checked)
        {
            using var brush = new SolidBrush(Color.FromArgb(unchecked((int)0xFFFFCC00))); // Accent yellow
            e.Graphics.FillPath(brush, path);

            // Thumb
            int tD = toggleH - 4;
            using var thumbBrush = new SolidBrush(Color.Black);
            e.Graphics.FillEllipse(thumbBrush, toggleW - tD - 2, toggleY + 2, tD, tD);
        }
        else
        {
            using var brush = new SolidBrush(Color.FromArgb(40, 40, 50));
            e.Graphics.FillPath(brush, path);

            // Thumb
            int tD = toggleH - 4;
            using var thumbBrush = new SolidBrush(Color.FromArgb(140, 140, 150));
            e.Graphics.FillEllipse(thumbBrush, 2, toggleY + 2, tD, tD);
        }

        // Text
        using var textBrush = new SolidBrush(Color.FromArgb(200, 200, 210));
        using var font = new Font("Segoe UI", 9.5F);
        using var sf = new StringFormat { LineAlignment = StringAlignment.Center };
        var textRect = new Rectangle(toggleW + 10, 0, Width - toggleW - 10, Height);
        e.Graphics.DrawString(Text, font, textBrush, textRect, sf);
    }
}
