using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AuraNetwork;

public class MerdoDialog : Form
{
    public enum DialogType { Warning, Info, Error, Update }

    private readonly string _message;
    private readonly DialogType _type;
    private bool _hasCancelBtn;

    private MerdoDialog(string message, DialogType type, bool hasCancel = false)
    {
        _message = message;
        _type = type;
        _hasCancelBtn = hasCancel;
        InitUI();
    }

    // ── Public static helpers ───────────────────────────────────────────────
    public static void ShowWarning(IWin32Window? owner, string message)
        => new MerdoDialog(message, DialogType.Warning).ShowDialog(owner);

    public static void ShowInfo(IWin32Window? owner, string message)
        => new MerdoDialog(message, DialogType.Info).ShowDialog(owner);

    public static void ShowError(IWin32Window? owner, string message)
        => new MerdoDialog(message, DialogType.Error).ShowDialog(owner);

    public static DialogResult ShowYesNo(IWin32Window? owner, string message)
        => new MerdoDialog(message, DialogType.Update, hasCancel: true).ShowDialog(owner);

    // ── UI Construction ─────────────────────────────────────────────────────
    private void InitUI()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition   = FormStartPosition.CenterParent;
        Size            = new Size(440, 180);
        BackColor       = Color.FromArgb(18, 18, 22);
        DoubleBuffered  = true;

        // Rounded corners
        var path = new GraphicsPath();
        int r = 10;
        path.AddArc(0, 0, r*2, r*2, 180, 90);
        path.AddArc(Width-r*2, 0, r*2, r*2, 270, 90);
        path.AddArc(Width-r*2, Height-r*2, r*2, r*2, 0, 90);
        path.AddArc(0, Height-r*2, r*2, r*2, 90, 90);
        path.CloseFigure();
        Region = new Region(path);

        // ── Title bar ──────────────────────────────────────────────────────
        var pnlTitle = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 36,
            BackColor = Color.FromArgb(26, 26, 32),
        };
        Controls.Add(pnlTitle);

        var lblTitle = new Label
        {
            Text      = "Uyarı",
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Location  = new Point(12, 10),
            AutoSize  = true,
        };
        pnlTitle.Controls.Add(lblTitle);

        // Close (X) button
        var btnX = new Label
        {
            Text      = "✕",
            ForeColor = Color.FromArgb(120, 120, 140),
            Font      = new Font("Segoe UI", 9F),
            Size      = new Size(28, 28),
            Location  = new Point(Width - 34, 4),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor    = Cursors.Hand,
        };
        btnX.Click    += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        btnX.MouseEnter += (_, _) => btnX.ForeColor = Color.White;
        btnX.MouseLeave += (_, _) => btnX.ForeColor = Color.FromArgb(120, 120, 140);
        pnlTitle.Controls.Add(btnX);

        // Draggable title bar
        bool dragging = false;
        Point dragStart = Point.Empty;
        pnlTitle.MouseDown += (_, e) => { if (e.Button == MouseButtons.Left) { dragging = true; dragStart = e.Location; } };
        pnlTitle.MouseMove += (_, e) => { if (dragging) { var s = PointToScreen(e.Location); Location = new Point(s.X - dragStart.X, s.Y - dragStart.Y); } };
        pnlTitle.MouseUp   += (_, _) => dragging = false;
        lblTitle.MouseDown += (_, e) => { if (e.Button == MouseButtons.Left) { dragging = true; dragStart = e.Location; } };
        lblTitle.MouseMove += (_, e) => { if (dragging) { var s = PointToScreen(e.Location); Location = new Point(s.X - dragStart.X, s.Y - dragStart.Y); } };
        lblTitle.MouseUp   += (_, _) => dragging = false;

        // ── Icon ───────────────────────────────────────────────────────────
        var picIcon = new PictureBox
        {
            Size     = new Size(48, 48),
            Location = new Point(24, 60),
            SizeMode = PictureBoxSizeMode.StretchImage,
        };

        switch (_type)
        {
            case DialogType.Warning:
            case DialogType.Update:
                picIcon.Image = SystemIcons.Warning.ToBitmap();
                break;
            case DialogType.Error:
                picIcon.Image = SystemIcons.Error.ToBitmap();
                break;
            default:
                picIcon.Image = SystemIcons.Information.ToBitmap();
                break;
        }
        Controls.Add(picIcon);

        // ── Message label ─────────────────────────────────────────────────
        var lblMsg = new Label
        {
            Text      = _message,
            ForeColor = Color.FromArgb(230, 230, 230),
            Font      = new Font("Segoe UI", 10F),
            Location  = new Point(88, 52),
            Size      = new Size(332, 80),
        };
        Controls.Add(lblMsg);

        // ── Border line under title ───────────────────────────────────────
        pnlTitle.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(40, 40, 50));
            e.Graphics.DrawLine(pen, 0, pnlTitle.Height - 1, pnlTitle.Width, pnlTitle.Height - 1);
        };

        // ── Buttons ───────────────────────────────────────────────────────
        int btnY = Height - 50;

        if (_hasCancelBtn)
        {
            var btnCancel = MakeButton("Hayır", Color.FromArgb(40, 40, 50), Color.White);
            btnCancel.Location = new Point(Width - 216, btnY);
            btnCancel.Click += (_, _) => { DialogResult = DialogResult.No; Close(); };
            Controls.Add(btnCancel);

            var btnYes = MakeButton("Evet", Color.FromArgb(255, 200, 0), Color.Black);
            btnYes.Location = new Point(Width - 110, btnY);
            btnYes.Click += (_, _) => { DialogResult = DialogResult.Yes; Close(); };
            Controls.Add(btnYes);
        }
        else
        {
            var btnOk = MakeButton("OK", Color.FromArgb(255, 200, 0), Color.Black);
            btnOk.Location = new Point(Width - 110, btnY);
            btnOk.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
            Controls.Add(btnOk);
        }

        KeyPreview = true;
        KeyDown   += (_, e) => { if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter) { DialogResult = DialogResult.OK; Close(); } };
    }

    private static Button MakeButton(string text, Color bg, Color fg)
    {
        var btn = new Button
        {
            Text      = text,
            Size      = new Size(96, 34),
            FlatStyle = FlatStyle.Flat,
            BackColor = bg,
            ForeColor = fg,
            Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Cursor    = Cursors.Hand,
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Region = RoundedRegion(btn.Width, btn.Height, 6);
        btn.MouseEnter += (_, _) => btn.BackColor = Lighten(bg, 20);
        btn.MouseLeave += (_, _) => btn.BackColor = bg;
        return btn;
    }

    private static Region RoundedRegion(int w, int h, int r)
    {
        var p = new GraphicsPath();
        p.AddArc(0, 0, r*2, r*2, 180, 90);
        p.AddArc(w-r*2, 0, r*2, r*2, 270, 90);
        p.AddArc(w-r*2, h-r*2, r*2, r*2, 0, 90);
        p.AddArc(0, h-r*2, r*2, r*2, 90, 90);
        p.CloseFigure();
        return new Region(p);
    }

    private static Color Lighten(Color c, int amt) =>
        Color.FromArgb(Math.Min(255, c.R + amt), Math.Min(255, c.G + amt), Math.Min(255, c.B + amt));

    // Draw border around the form
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(Color.FromArgb(55, 55, 70), 1.5f);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }
}
