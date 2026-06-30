using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;

namespace MerdoClient;

public class SettingsForm : Form
{
    private readonly SettingsService _settings;
    private readonly Action<int>?    _onVolumeChanged; // Form1'deki müzik sesini anlık değiştirir

    // Controls
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

    public SettingsForm(SettingsService settings, Action<int>? onVolumeChanged = null)
    {
        _settings         = settings;
        _onVolumeChanged  = onVolumeChanged;
        BuildUI();
        LoadValues();
    }

    private void BuildUI()
    {
        Text            = "Merdo Launcher — Ayarlar";
        Size            = new Size(540, 560);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = Color.FromArgb(12, 12, 15);
        DoubleBuffered  = true;

        // ── Section helper ──────────────────────────────
        Label Section(string text, int y)
        {
            var lbl = new Label
            {
                Text      = text,
                ForeColor = Color.FromArgb(255, 204, 0),
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location  = new Point(30, y),
                AutoSize  = true
            };
            Controls.Add(lbl);

            var line = new Panel
            {
                Location  = new Point(30, y + 22),
                Size      = new Size(480, 1),
                BackColor = Color.FromArgb(35, 35, 40)
            };
            Controls.Add(line);
            return lbl;
        }

        Label FieldLabel(string text, int y)
        {
            var lbl = new Label
            {
                Text      = text,
                ForeColor = Color.FromArgb(160, 160, 170),
                Font      = new Font("Segoe UI", 9F),
                Location  = new Point(30, y),
                AutoSize  = true
            };
            Controls.Add(lbl);
            return lbl;
        }

        // ── BAŞLIK ──────────────────────────────────────
        Controls.Add(new Label
        {
            Text      = "⚙  AYARLAR",
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location  = new Point(30, 20),
            AutoSize  = true
        });

        // ══════════════════════════════════════════════
        // 1) PERFORMANS
        // ══════════════════════════════════════════════
        Section("PERFORMANS", 60);

        FieldLabel("Maksimum RAM", 95);
        trkRam = new TrackBar
        {
            Location      = new Point(30, 115),
            Size          = new Size(390, 40),
            Minimum       = 1024,
            Maximum       = 16384,
            TickFrequency = 1024,
            LargeChange   = 1024,
            SmallChange   = 512,
            BackColor     = Color.FromArgb(12, 12, 15)
        };
        trkRam.ValueChanged += (s, e) =>
            lblRamValue.Text = $"{trkRam.Value / 1024.0:0.#} GB";
        Controls.Add(trkRam);

        lblRamValue = new Label
        {
            Text      = "4 GB",
            ForeColor = Color.FromArgb(255, 204, 0),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location  = new Point(430, 120),
            AutoSize  = true
        };
        Controls.Add(lblRamValue);

        // ══════════════════════════════════════════════
        // 2) MÜZİK
        // ══════════════════════════════════════════════
        Section("MÜZİK", 170);

        FieldLabel("Arkaplan Müzik Sesi", 205);
        trkVolume = new TrackBar
        {
            Location      = new Point(30, 225),
            Size          = new Size(390, 40),
            Minimum       = 0,
            Maximum       = 100,
            TickFrequency = 10,
            LargeChange   = 10,
            SmallChange   = 5,
            BackColor     = Color.FromArgb(12, 12, 15)
        };
        trkVolume.ValueChanged += (s, e) =>
        {
            lblVolumeValue.Text = $"{trkVolume.Value}%";
            _onVolumeChanged?.Invoke(trkVolume.Value); // anlık uygula
        };
        Controls.Add(trkVolume);

        lblVolumeValue = new Label
        {
            Text      = "25%",
            ForeColor = Color.FromArgb(255, 204, 0),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location  = new Point(430, 230),
            AutoSize  = true
        };
        Controls.Add(lblVolumeValue);

        // ══════════════════════════════════════════════
        // 3) BAŞLATICI
        // ══════════════════════════════════════════════
        Section("BAŞLATICI", 280);

        chkClose = new CheckBox
        {
            Text      = "Oyun açılınca launcher'ı kapat",
            ForeColor = Color.FromArgb(180, 180, 190),
            Font      = new Font("Segoe UI", 9.5F),
            Location  = new Point(30, 315),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        Controls.Add(chkClose);

        chkConsole = new CheckBox
        {
            Text      = "Konsol penceresini göster (hata ayıklama)",
            ForeColor = Color.FromArgb(180, 180, 190),
            Font      = new Font("Segoe UI", 9.5F),
            Location  = new Point(30, 342),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        Controls.Add(chkConsole);

        // ══════════════════════════════════════════════
        // 4) JAVA
        // ══════════════════════════════════════════════
        Section("JAVA", 380);

        FieldLabel("Mevcut Java Yolu", 415);
        lblJavaPath = new Label
        {
            Text         = "Algılanıyor...",
            ForeColor    = Color.FromArgb(100, 100, 110),
            Font         = new Font("Segoe UI", 8F),
            Location     = new Point(30, 435),
            Size         = new Size(480, 18),
            AutoEllipsis = true
        };
        Controls.Add(lblJavaPath);

        lblJavaStatus = new Label
        {
            Text      = "",
            ForeColor = Color.FromArgb(40, 200, 90),
            Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            Location  = new Point(30, 455),
            AutoSize  = true
        };
        Controls.Add(lblJavaStatus);

        btnJavaAuto = new Button
        {
            Text      = "⬇  Java'yı Otomatik Kur",
            Location  = new Point(30, 475),
            Size      = new Size(220, 34),
            BackColor = Color.FromArgb(0, 120, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor    = Cursors.Hand
        };
        btnJavaAuto.FlatAppearance.BorderSize = 0;
        btnJavaAuto.Click += BtnJavaAuto_Click;
        Controls.Add(btnJavaAuto);

        btnJavaManual = new Button
        {
            Text      = "📂  Manuel Seç",
            Location  = new Point(260, 475),
            Size      = new Size(140, 34),
            BackColor = Color.FromArgb(30, 30, 40),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9F),
            Cursor    = Cursors.Hand
        };
        btnJavaManual.FlatAppearance.BorderSize = 0;
        btnJavaManual.Click += BtnJavaManual_Click;
        Controls.Add(btnJavaManual);

        // ══════════════════════════════════════════════
        // Alt butonlar
        // ══════════════════════════════════════════════
        btnSave = new Button
        {
            Text      = "Kaydet",
            Location  = new Point(310, 518),
            Size      = new Size(100, 36),
            BackColor = Color.FromArgb(255, 204, 0),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Cursor    = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;
        Controls.Add(btnSave);

        btnCancel = new Button
        {
            Text      = "İptal",
            Location  = new Point(420, 518),
            Size      = new Size(90, 36),
            BackColor = Color.FromArgb(35, 35, 42),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5F),
            Cursor    = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => Close();
        Controls.Add(btnCancel);
    }

    private void LoadValues()
    {
        var s = _settings.Settings;

        // RAM
        trkRam.Value     = Math.Clamp(s.MaxRamMb, trkRam.Minimum, trkRam.Maximum);
        lblRamValue.Text = $"{trkRam.Value / 1024.0:0.#} GB";

        // Müzik sesi
        trkVolume.Value     = Math.Clamp(s.MusicVolume, 0, 100);
        lblVolumeValue.Text = $"{trkVolume.Value}%";

        // Checkboxes
        chkClose.Checked   = s.CloseOnLaunch;
        chkConsole.Checked = s.ShowConsole;

        // Java yolu
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
        s.MaxRamMb     = trkRam.Value;
        s.MusicVolume  = trkVolume.Value;
        s.CloseOnLaunch = chkClose.Checked;
        s.ShowConsole  = chkConsole.Checked;
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
