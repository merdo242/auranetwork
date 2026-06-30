using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// ----- BFS background remover -----
static void RemoveDarkBg(Bitmap bmp)
{
    int W = bmp.Width, H = bmp.Height;
    bool[,] visited = new bool[W, H];
    var queue = new Queue<Point>();

    void TryEnqueue(int x, int y)
    {
        if (x < 0 || y < 0 || x >= W || y >= H || visited[x, y]) return;
        Color c = bmp.GetPixel(x, y);
        if (c.R + c.G + c.B < 120 && c.A > 10)
        {
            visited[x, y] = true;
            queue.Enqueue(new Point(x, y));
        }
    }

    for (int x = 0; x < W; x++) { TryEnqueue(x, 0); TryEnqueue(x, H - 1); }
    for (int y = 0; y < H; y++) { TryEnqueue(0, y); TryEnqueue(W - 1, y); }

    while (queue.Count > 0)
    {
        var p = queue.Dequeue();
        bmp.SetPixel(p.X, p.Y, Color.Transparent);
        TryEnqueue(p.X + 1, p.Y);
        TryEnqueue(p.X - 1, p.Y);
        TryEnqueue(p.X, p.Y + 1);
        TryEnqueue(p.X, p.Y - 1);
    }
}

// ----- Proper multi-size ICO writer -----
static void WritePngIco(Bitmap src, string icoPath, int[] sizes)
{
    var pngs = new List<byte[]>();
    foreach (int sz in sizes)
    {
        using var ms = new MemoryStream();
        using var resized = new Bitmap(sz, sz, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(resized))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(src, 0, 0, sz, sz);
        }
        resized.Save(ms, ImageFormat.Png);
        pngs.Add(ms.ToArray());
    }

    using var ico = new FileStream(icoPath, FileMode.Create);
    using var bw = new BinaryWriter(ico);

    // ICONDIR
    bw.Write((short)0); // Reserved
    bw.Write((short)1); // Type = ICO
    bw.Write((short)pngs.Count);

    int offset = 6 + pngs.Count * 16;
    for (int i = 0; i < pngs.Count; i++)
    {
        int sz = sizes[i];
        bw.Write((byte)(sz >= 256 ? 0 : sz));
        bw.Write((byte)(sz >= 256 ? 0 : sz));
        bw.Write((byte)0);
        bw.Write((byte)0);
        bw.Write((short)1);
        bw.Write((short)32);
        bw.Write(pngs[i].Length);
        bw.Write(offset);
        offset += pngs[i].Length;
    }
    foreach (var png in pngs) bw.Write(png);
}

// ----- MAIN -----
string pngPath = @"C:\Users\muhar\MerdoClient\MerdoClient\Resources\logo_large.png";
string icoPath = @"C:\Users\muhar\MerdoClient\MerdoClient\icon.ico";

using var src = new Bitmap(pngPath);

// Rebuild transparent PNG from scratch (re-do background removal)
using var bmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
using (var g = Graphics.FromImage(bmp))
    g.DrawImage(src, 0, 0);

RemoveDarkBg(bmp);

// Write to a fresh file name (avoid locking issues with open launcher)
string newPngPath = Path.Combine(Path.GetDirectoryName(pngPath)!, "logo_large_new.png");
string tmpPath = newPngPath + ".tmp";
using (var ms = new MemoryStream())
{
    bmp.Save(ms, ImageFormat.Png);
    File.WriteAllBytes(tmpPath, ms.ToArray());
}
if (File.Exists(newPngPath)) File.Delete(newPngPath);
File.Move(tmpPath, newPngPath);
Console.WriteLine("PNG saved: " + newPngPath);

WritePngIco(bmp, icoPath, new int[] { 16, 32, 48, 64, 128, 256 });
Console.WriteLine("Done: PNG + ICO written.");
