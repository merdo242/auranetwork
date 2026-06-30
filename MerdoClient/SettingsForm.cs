using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;

namespace MerdoClient;

public class SettingsForm : Form
{
    private readonly SettingsService _settings;
    private readonly Action<int>?    _onVolumeChanged;

    private TrackBar trkRam        = new();
    private Label    lblRamValue   = new();
    private TrackBar trkVolume     = new();
    private Label    lblVolumeValue = new();
    private CheckBox chkClose      = new();
    private CheckBox chkConsole    = new();
    private Label    lblJavaPath   = new();
    private Button   btnJavaAuto   = new();
    private Button   btnJavaManual = new();
    private Button   btnSave       = new();
    private Button   btnCancel     = new();
    private Label    lblJavaStatus = new();

    private const int ACCENT     = unchecked((int)0xFFFFCC00); // sarı
    private const int BG         = unchecked((int)0xFF0C0C0F);
    private const int CARD_BG    = unchecked((int)0xFF151518);
    private const int BORDER_CLR = unchecked((int)0xFF252530);

    public SettingsForm(SettingsService settings, Action<int>? onVolumeChanged = null)
    {
        _settings        = settings;
        _onVolumeChanged = onVolumeChanged;
        BuildUI();
        LoadValues();
    }

    // ─── Rounded rectangle helper ───────────────────────────────────────
    private static GraphicsPath RoundedRect(Rectangle r, int rad)
    {
        var p = new GraphicsPath();
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
            using var path = RoundedRect(r, 10);
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
            Location  = new Point(16, y),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card.Controls.Add(lbl);

        var line = new Panel
        {
            Location  = new Point(16, y + 22),
            Size      = new Size(card.Width - 32, 1),
            BackColor = Color.FromArgb(BORDER_CLR)
        };
        card.Controls.Add(line);
    }

    private void BuildUI()
    {
        Text            = "Merdo Launcher — Ayarlar";
        Size            = new Size(560, 640);
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
            using var path = RoundedRect(r, 12);
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
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location  = new Point(24, 20),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        Controls.Add(lblTitle);

        // Kapat butonu (X)
        var btnClose = new Label
        {
            Text      = "✕",
            ForeColor = Color.FromArgb(140, 140, 150),
            Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
            Location  = new Point(Width - 44, 18),
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
            Location  = new Point(0, 55),
            Size      = new Size(Width, 1),
            BackColor = Color.FromArgb(BORDER_CLR)
        });

        int cardX = 18, cardW = Width - 36;

        // ══════════════════════════════════════════════
        // KART 1 — PERFORMANS + MÜZİK
        // ══════════════════════════════════════════════
        var card1 = Card(cardX, 68, cardW, 220);

        SectionHeader(card1, "🎮", "PERFORMANS", 14);

        var lblRamLbl = new Label
        {
            Text      = "Maksimum RAM",
            ForeColor = Color.FromArgb(160, 160, 175),
            Font      = new Font("Segoe UI", 9F),
            Location  = new Point(16, 48),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblRamLbl);

        trkRam = new TrackBar
        {
            Location      = new Point(12, 67),
            Size          = new Size(card1.Width - 90, 36),
            Minimum       = 1024,
            Maximum       = 16384,
            TickFrequency = 1024,
            LargeChange   = 1024,
            SmallChange   = 512,
            BackColor     = Color.FromArgb(CARD_BG)
        };
        trkRam.ValueChanged += (s, e) => lblRamValue.Text = $"{trkRam.Value / 1024.0:0.#} GB";
        card1.Controls.Add(trkRam);

        lblRamValue = new Label
        {
            Text      = "4 GB",
            ForeColor = Color.FromArgb(ACCENT),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location  = new Point(card1.Width - 70, 72),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblRamValue);

        // Müzik ayırıcı
        card1.Controls.Add(new Panel
        {
            Location  = new Point(16, 112),
            Size      = new Size(card1.Width - 32, 1),
            BackColor = Color.FromArgb(BORDER_CLR)
        });

        SectionHeader(card1, "🎵", "MÜZİK", 120);

        var lblVolLbl = new Label
        {
            Text      = "Arkaplan Müzik Sesi",
            ForeColor = Color.FromArgb(160, 160, 175),
            Font      = new Font("Segoe UI", 9F),
            Location  = new Point(16, 155),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblVolLbl);

        trkVolume = new TrackBar
        {
            Location      = new Point(12, 174),
            Size          = new Size(card1.Width - 90, 36),
            Minimum       = 0,
            Maximum       = 100,
            TickFrequency = 10,
            LargeChange   = 10,
            SmallChange   = 5,
            BackColor     = Color.FromArgb(CARD_BG)
        };
        trkVolume.ValueChanged += (s, e) =>
        {
            lblVolumeValue.Text = $"{trkVolume.Value}%";
            _onVolumeChanged?.Invoke(trkVolume.Value);
        };
        card1.Controls.Add(trkVolume);

        lblVolumeValue = new Label
        {
            Text      = "25%",
            ForeColor = Color.FromArgb(ACCENT),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location  = new Point(card1.Width - 70, 179),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card1.Controls.Add(lblVolumeValue);

        // ══════════════════════════════════════════════
        // KART 2 — BAŞLATICI
        // ══════════════════════════════════════════════
        var card2 = Card(cardX, 300, cardW, 110);

        SectionHeader(card2, "🚀", "BAŞLATICI", 14);

        chkClose = StyledCheckBox("Oyun açılınca launcher'ı kapat", new Point(16, 48));
        card2.Controls.Add(chkClose);

        chkConsole = StyledCheckBox("Konsol penceresini göster (hata ayıklama)", new Point(16, 76));
        card2.Controls.Add(chkConsole);

        // ══════════════════════════════════════════════
        // KART 3 — JAVA
        // ══════════════════════════════════════════════
        var card3 = Card(cardX, 424, cardW, 155);

        SectionHeader(card3, "☕", "JAVA", 14);

        var lblJavaLbl = new Label
        {
            Text      = "Mevcut Java Yolu",
            ForeColor = Color.FromArgb(160, 160, 175),
            Font      = new Font("Segoe UI", 9F),
            Location  = new Point(16, 48),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card3.Controls.Add(lblJavaLbl);

        lblJavaPath = new Label
        {
            Text         = "Algılanıyor...",
            ForeColor    = Color.FromArgb(100, 100, 115),
            Font         = new Font("Segoe UI", 8F),
            Location     = new Point(16, 66),
            Size         = new Size(card3.Width - 32, 16),
            AutoEllipsis = true,
            BackColor    = Color.Transparent
        };
        card3.Controls.Add(lblJavaPath);

        lblJavaStatus = new Label
        {
            Text      = "",
            ForeColor = Color.FromArgb(40, 200, 90),
            Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            Location  = new Point(16, 84),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        card3.Controls.Add(lblJavaStatus);

        btnJavaAuto = StyledButton("⬇  Java'yı Otomatik Kur", new Point(16, 108), new Size(210, 34),
                                   Color.FromArgb(0, 130, 65), Color.White);
        btnJavaAuto.Click += BtnJavaAuto_Click;
        card3.Controls.Add(btnJavaAuto);

        btnJavaManual = StyledButton("📂  Manuel Seç", new Point(236, 108), new Size(130, 34),
                                     Color.FromArgb(30, 30, 42), Color.White);
        btnJavaManual.FlatAppearance.BorderColor = Color.FromArgb(BORDER_CLR);
        btnJavaManual.FlatAppearance.BorderSize  = 1;
        btnJavaManual.Click += BtnJavaManual_Click;
        card3.Controls.Add(btnJavaManual);

        // ── Alt Kaydet / İptal ──────────────────────────────────────────
        btnSave = StyledButton("✔  Kaydet", new Point(Width - 230, 592), new Size(110, 36),
                               Color.FromArgb(ACCENT), Color.Black);
        btnSave.Click += BtnSave_Click;
        Controls.Add(btnSave);

        btnCancel = StyledButton("İptal", new Point(Width - 112, 592), new Size(90, 36),
                                 Color.FromArgb(28, 28, 36), Color.White);
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
            Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor    = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        // Rounded region
        btn.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
            using var path = RoundedRect(r, 7);
            using var brush = new SolidBrush(bg);
            e.Graphics.FillPath(brush, path);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var pen = new SolidBrush(fg);
            e.Graphics.DrawString(btn.Text, btn.Font, pen, new RectangleF(0, 0, btn.Width, btn.Height), sf);
        };
        return btn;
    }

    private static CheckBox StyledCheckBox(string text, Point loc)
    {
        return new CheckBox
        {
            Text      = text,
            ForeColor = Color.FromArgb(185, 185, 200),
            Font      = new Font("Segoe UI", 9.5F),
            Location  = loc,
            AutoSize  = true,
            BackColor = Color.Transparent
        };
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
                    btnJavaAuto.Text    = "⬇  Java'yı Otomatik Kur";
                    btnJavaAuto.Enabled = true;
                });
            }
            catch (Exception ex)
            {
                Invoke(() =>
                {
                    lblJavaStatus.Text      = "✘  Kurulum başarısız: " + ex.Message;
                    lblJavaStatus.ForeColor = Color.FromArgb(220, 60, 60);
                    btnJavaAuto.Text    = "⬇  Java'yı Otomatik Kur";
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
