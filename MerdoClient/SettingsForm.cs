using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;

namespace MerdoClient;

public class SettingsForm : Form
{
    private readonly SettingsService _settings;

    // Controls
    private ComboBox cmbVersion     = new();
    private TrackBar trkRam        = new();
    private Label    lblRamValue   = new();
    private CheckBox chkClose      = new();
    private CheckBox chkConsole    = new();
    private Label    lblJavaPath   = new();
    private Button   btnJavaAuto   = new();
    private Button   btnJavaManual = new();
    private Button   btnSave       = new();
    private Button   btnCancel     = new();
    private Label    lblJavaStatus = new();

    public SettingsForm(SettingsService settings)
    {
        _settings = settings;
        BuildUI();
        LoadValues();
    }

    private void BuildUI()
    {
        Text            = "Merdo Launcher — Ayarlar";
        Size            = new Size(540, 580);
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
        var title = new Label
        {
            Text      = "⚙  AYARLAR",
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location  = new Point(30, 20),
            AutoSize  = true
        };
        Controls.Add(title);

        // ══════════════════════════════════════════════
        // 1) OYUN
        // ══════════════════════════════════════════════
        Section("OYUN", 60);

        FieldLabel("Minecraft Sürümü", 95);
        cmbVersion = new ComboBox
        {
            Location      = new Point(30, 115),
            Size          = new Size(480, 28),
            BackColor     = Color.FromArgb(22, 22, 26),
            ForeColor     = Color.White,
            FlatStyle     = FlatStyle.Flat,
            Font          = new Font("Segoe UI", 9.5F),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbVersion.Items.Add("— Otomatik (önerilen) —");
        foreach (var v in _settings.GetAvailableVersions())
            cmbVersion.Items.Add(v);
        Controls.Add(cmbVersion);

        var refreshBtn = new Button
        {
            Text      = "↺",
            Location  = new Point(484, 115),
            Size      = new Size(26, 26),
            BackColor = Color.FromArgb(30, 30, 35),
            ForeColor = Color.FromArgb(255, 204, 0),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 11F),
            Cursor    = Cursors.Hand
        };
        refreshBtn.FlatAppearance.BorderSize = 0;
        refreshBtn.Click += (s, e) =>
        {
            var sel = cmbVersion.SelectedItem?.ToString();
            cmbVersion.Items.Clear();
            cmbVersion.Items.Add("— Otomatik (önerilen) —");
            foreach (var v in _settings.GetAvailableVersions())
                cmbVersion.Items.Add(v);
            cmbVersion.SelectedItem = sel ?? cmbVersion.Items[0];
        };
        Controls.Add(refreshBtn);

        // ══════════════════════════════════════════════
        // 2) PERFORMANS
        // ══════════════════════════════════════════════
        Section("PERFORMANS", 160);

        FieldLabel("Maksimum RAM", 195);
        trkRam = new TrackBar
        {
            Location  = new Point(30, 215),
            Size      = new Size(390, 40),
            Minimum   = 1024,
            Maximum   = 16384,
            TickFrequency = 1024,
            LargeChange   = 1024,
            SmallChange   = 512,
            BackColor = Color.FromArgb(12, 12, 15)
        };
        trkRam.ValueChanged += (s, e) =>
        {
            lblRamValue.Text = $"{trkRam.Value / 1024.0:0.#} GB";
        };
        Controls.Add(trkRam);

        lblRamValue = new Label
        {
            Text      = "4 GB",
            ForeColor = Color.FromArgb(255, 204, 0),
            Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location  = new Point(430, 220),
            AutoSize  = true
        };
        Controls.Add(lblRamValue);

        // ══════════════════════════════════════════════
        // 3) BAŞLATICI
        // ══════════════════════════════════════════════
        Section("BAŞLATICI", 270);

        chkClose = new CheckBox
        {
            Text      = "Oyun açılınca launcher'ı kapat",
            ForeColor = Color.FromArgb(180, 180, 190),
            Font      = new Font("Segoe UI", 9.5F),
            Location  = new Point(30, 305),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        Controls.Add(chkClose);

        chkConsole = new CheckBox
        {
            Text      = "Konsol penceresini göster (hata ayıklama)",
            ForeColor = Color.FromArgb(180, 180, 190),
            Font      = new Font("Segoe UI", 9.5F),
            Location  = new Point(30, 332),
            AutoSize  = true,
            BackColor = Color.Transparent
        };
        Controls.Add(chkConsole);

        // ══════════════════════════════════════════════
        // 4) JAVA
        // ══════════════════════════════════════════════
        Section("JAVA", 370);

        FieldLabel("Mevcut Java Yolu", 405);
        lblJavaPath = new Label
        {
            Text      = "Algılanıyor...",
            ForeColor = Color.FromArgb(100, 100, 110),
            Font      = new Font("Segoe UI", 8F),
            Location  = new Point(30, 425),
            Size      = new Size(480, 18),
            AutoEllipsis = true
        };
        Controls.Add(lblJavaPath);

        lblJavaStatus = new Label
        {
            Text      = "",
            ForeColor = Color.FromArgb(40, 200, 90),
            Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            Location  = new Point(30, 445),
            AutoSize  = true
        };
        Controls.Add(lblJavaStatus);

        btnJavaAuto = new Button
        {
            Text      = "⬇  Java'yı Otomatik Kur",
            Location  = new Point(30, 468),
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
            Location  = new Point(260, 468),
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
            Location  = new Point(310, 510),
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
            Location  = new Point(420, 510),
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

        // Versiyon
        if (string.IsNullOrEmpty(s.SelectedVersion) || !cmbVersion.Items.Contains(s.SelectedVersion))
            cmbVersion.SelectedIndex = 0;
        else
            cmbVersion.SelectedItem = s.SelectedVersion;

        // RAM
        trkRam.Value = Math.Clamp(s.MaxRamMb, trkRam.Minimum, trkRam.Maximum);
        lblRamValue.Text = $"{trkRam.Value / 1024.0:0.#} GB";

        // Checkboxes
        chkClose.Checked   = s.CloseOnLaunch;
        chkConsole.Checked = s.ShowConsole;

        // Java yolu
        string java = string.IsNullOrEmpty(s.JavaPath)
            ? MinecraftLauncherService.FindSystemJavaPath()
            : s.JavaPath;

        if (!string.IsNullOrEmpty(java))
        {
            lblJavaPath.Text   = java;
            lblJavaStatus.Text = "✔  Java bulundu";
            lblJavaStatus.ForeColor = Color.FromArgb(40, 200, 90);
        }
        else
        {
            lblJavaPath.Text   = "Java bulunamadı — otomatik kurulum önerilir";
            lblJavaStatus.Text = "✘  Java yok";
            lblJavaStatus.ForeColor = Color.FromArgb(220, 60, 60);
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var s = _settings.Settings;
        s.SelectedVersion = cmbVersion.SelectedIndex == 0 ? string.Empty : cmbVersion.SelectedItem?.ToString() ?? string.Empty;
        s.MaxRamMb        = trkRam.Value;
        s.CloseOnLaunch   = chkClose.Checked;
        s.ShowConsole     = chkConsole.Checked;
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
            Title       = "javaw.exe dosyasını seçin",
            Filter      = "javaw.exe|javaw.exe|Tüm dosyalar|*.*",
            FileName    = "javaw.exe"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _settings.Settings.JavaPath = dlg.FileName;
            lblJavaPath.Text   = dlg.FileName;
            lblJavaStatus.Text = "✔  Manuel Java seçildi";
            lblJavaStatus.ForeColor = Color.FromArgb(40, 200, 90);
        }
    }
}
