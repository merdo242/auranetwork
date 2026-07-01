using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AuraNetwork;

public class SplashForm : Form
{
    private readonly System.Windows.Forms.Timer _progressTimer = new();
    private readonly System.Windows.Forms.Timer _particleTimer = new();
    private int _progress = 0;
    private string _statusText = "Başlatılıyor...";
    private readonly List<Particle> _particles = new();
    private readonly Random _rng = new();
    private bool _done = false;

    private readonly string[] _steps = new[]
    {
        "Dosyalar yükleniyor...",
        "Hesap servisi başlatılıyor...",
        "Sunucu bağlantısı kontrol ediliyor...",
        "Ayarlar uygulanıyor...",
        "Hazırlanıyor..."
    };
    private int _stepIndex = 0;

    private struct Particle
    {
        public float X, Y, SpeedX, SpeedY, Size, Alpha;
        public Color Color;
    }

    public SplashForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition  = FormStartPosition.CenterScreen;
        Size           = new Size(520, 300);
        DoubleBuffered = true;
        BackColor      = Color.FromArgb(8, 8, 10);
        TopMost        = true;

        // Köşeleri yuvarlat
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(0, 0, 20, 20, 180, 90);
        path.AddArc(Width - 20, 0, 20, 20, 270, 90);
        path.AddArc(Width - 20, Height - 20, 20, 20, 0, 90);
        path.AddArc(0, Height - 20, 20, 20, 90, 90);
        path.CloseFigure();
        Region = new Region(path);

        // Parçacık başlangıcı
        for (int i = 0; i < 30; i++) SpawnParticle();

        // İlerleme zamanlayıcısı
        _progressTimer.Interval = 35;
        _progressTimer.Tick += ProgressTimer_Tick;
        _progressTimer.Start();

        // Parçacık zamanlayıcısı
        _particleTimer.Interval = 16;
        _particleTimer.Tick += (s, e) =>
        {
            UpdateParticles();
            Invalidate();
        };
        _particleTimer.Start();
    }

    private void SpawnParticle()
    {
        _particles.Add(new Particle
        {
            X      = _rng.Next(0, Width),
            Y      = _rng.Next(0, Height),
            SpeedX = (float)(_rng.NextDouble() - 0.5) * 0.6f,
            SpeedY = (float)(-_rng.NextDouble()) * 0.8f - 0.2f,
            Size   = (float)(_rng.NextDouble() * 3 + 1),
            Alpha  = (float)(_rng.NextDouble() * 0.6 + 0.1),
            Color  = _rng.Next(2) == 0
                        ? Color.FromArgb(255, 204, 0)   // Altın
                        : Color.FromArgb(80, 140, 255)  // Mavi
        });
    }

    private void UpdateParticles()
    {
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];
            p.X     += p.SpeedX;
            p.Y     += p.SpeedY;
            p.Alpha -= 0.003f;
            _particles[i] = p;

            if (p.Alpha <= 0 || p.Y < -10)
            {
                _particles.RemoveAt(i);
                SpawnParticle();
            }
        }
    }

    private void ProgressTimer_Tick(object? sender, EventArgs e)
    {
        _progress += 1;

        int stepAt = 100 / _steps.Length;
        int newStep = _progress / stepAt;
        if (newStep < _steps.Length && newStep != _stepIndex)
        {
            _stepIndex = newStep;
            _statusText = _steps[_stepIndex];
        }

        if (_progress >= 100)
        {
            _progress = 100;
            _statusText = "Hazır!";
            _progressTimer.Stop();
            _done = true;

            // 400ms bekle sonra kapat
            var closeTimer = new System.Windows.Forms.Timer { Interval = 400 };
            closeTimer.Tick += (s, ev) => { closeTimer.Stop(); DialogResult = DialogResult.OK; Close(); };
            closeTimer.Start();
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;

        var g = e.Graphics;
        g.SmoothingMode      = SmoothingMode.AntiAlias;
        g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        // --- Arka plan degradesi ---
        using (var bg = new LinearGradientBrush(
            ClientRectangle,
            Color.FromArgb(10, 10, 14),
            Color.FromArgb(18, 18, 24),
            LinearGradientMode.Vertical))
        {
            g.FillRectangle(bg, ClientRectangle);
        }

        // --- Parçacıklar ---
        foreach (var p in _particles)
        {
            int a = Math.Clamp((int)(p.Alpha * 255), 0, 255);
            using var brush = new SolidBrush(Color.FromArgb(a, p.Color));
            g.FillEllipse(brush, p.X - p.Size / 2, p.Y - p.Size / 2, p.Size, p.Size);
        }

        // --- Logo + Başlık + Altyazı ---
        DrawLogo(g, 16, 30);

        // --- Divider ---
        using (var divPen = new Pen(Color.FromArgb(30, 30, 35), 1))
            g.DrawLine(divPen, 40, 120, Width - 40, 120);

        // --- Durum metni (barın ÜSTÜNDE) ---
        using (var statusFont = new Font("Segoe UI", 9F, FontStyle.Regular))
        using (var statusBrush = new SolidBrush(Color.FromArgb(130, 130, 145)))
            g.DrawString(_statusText, statusFont, statusBrush, 40, 197);

        // --- Yüzde (barın ÜSTÜNDE, sağ taraf) ---
        using (var pctFont = new Font("Segoe UI", 9F, FontStyle.Bold))
        using (var pctBrush = new SolidBrush(Color.FromArgb(255, 204, 0)))
            g.DrawString($"%{_progress}", pctFont, pctBrush, Width - 70, 197);

        // --- İlerleme çubuğu arka planı ---
        Rectangle barBg = new Rectangle(40, 215, Width - 80, 8);
        using (var bgPath = RoundedRect(barBg, 4))
        using (var bgBrush = new SolidBrush(Color.FromArgb(25, 25, 30)))
            g.FillPath(bgBrush, bgPath);

        // --- İlerleme çubuğu dolgu ---
        if (_progress > 0)
        {
            int fillW = (int)((Width - 80) * (_progress / 100.0));
            if (fillW > 8)
            {
                Rectangle barFill = new Rectangle(40, 215, fillW, 8);
                using var fillPath = RoundedRect(barFill, 4);
                using var grad = new LinearGradientBrush(barFill,
                    Color.FromArgb(255, 180, 0),
                    Color.FromArgb(255, 220, 60),
                    LinearGradientMode.Horizontal);
                g.FillPath(grad, fillPath);

                // Parlama efekti
                Rectangle glow = new Rectangle(40 + fillW - 12, 213, 12, 12);
                using var glowBrush = new System.Drawing.Drawing2D.PathGradientBrush(
                    new GraphicsPath().Tap(p => p.AddEllipse(glow)))
                {
                    CenterColor    = Color.FromArgb(200, 255, 240, 120),
                    SurroundColors = new[] { Color.Transparent }
                };
                g.FillEllipse(new SolidBrush(Color.FromArgb(80, 255, 230, 80)), glow);
            }
        }

        // --- Versiyon / alt ---
        using (var verFont = new Font("Segoe UI", 8F))
        using (var verBrush = new SolidBrush(Color.FromArgb(60, 60, 70)))
        {
            string ver = "© 2026 Merdo Network  •  v2.0";
            var sz = g.MeasureString(ver, verFont);
            g.DrawString(ver, verFont, verBrush, (Width - sz.Width) / 2, Height - 28);
        }

        // --- Özellikler listesi ---
        string[] features = { "✦  Özel Skin Desteği", "✦  Turnuvalar", "✦  Ekonomi Sistemi" };
        int fy = 140;
        using (var fFont = new Font("Segoe UI", 9F))
        using (var fBrush = new SolidBrush(Color.FromArgb(90, 90, 100)))
        {
            foreach (var feat in features)
            {
                g.DrawString(feat, fFont, fBrush, 40, fy);
                fy += 22;
            }
        }
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int r)
    {
        var path = new GraphicsPath();
        int d = r * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static Image? _largeLogo;
    private static void DrawLogo(Graphics g, int x, int y)
    {
        g.SmoothingMode      = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.InterpolationMode  = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.CompositingMode    = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        // --- Small logo (40x40) ---
        if (_largeLogo == null)
            try { _largeLogo = Image.FromFile(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo_large_new.png")); } catch { }

        int iconSize = 40;
        int iconX = x + 8;
        int iconY = y + 14;

        if (_largeLogo != null)
        {
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
        using (var badgePath = RoundedRect(new Rectangle(badgeX, badgeY, badgeW, badgeH), 5))
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

        // --- Altyazı ---
        using (var subFont  = new Font("Segoe UI", 9F, FontStyle.Regular))
        using (var subBrush = new SolidBrush(Color.FromArgb(100, 100, 110)))
            g.DrawString("Minecraft Launcher v2.0", subFont, subBrush, textX, badgeY + 22);
    }
}

// Tap extension — GraphicsPath için inline işlem
internal static class GraphicsPathExtensions
{
    public static GraphicsPath Tap(this GraphicsPath path, Action<GraphicsPath> action)
    {
        action(path);
        return path;
    }
}
