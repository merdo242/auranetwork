using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;

namespace MerdoClient;

public class UpdateCheckerService
{
    // Mevcut launcher sürümü
    public const string CurrentVersion = "3.6";

    // Güncelleme kontrolü için doğrudan bu GitHub deposundaki update.json dosyasını kullanıyoruz (100% ücretsiz & hızlı)
    private const string UpdateUrl = "https://raw.githubusercontent.com/merdo242/merdoclient/main/update.json";

    public static void CheckForUpdates(Form parentForm)
    {
        Task.Run(async () =>
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MerdoLauncher/2.0");

                var response = await client.GetStringAsync(UpdateUrl);
                var data = JsonSerializer.Deserialize<UpdateResponse>(response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data == null || !IsNewerVersion(data.LatestVersion, CurrentVersion))
                    return;

                parentForm.Invoke(() => ShowUpdateDialog(parentForm, data));
            }
            catch
            {
                // Sunucu kapalı veya internet yok — sessizce geç
            }
        });
    }

    private static void ShowUpdateDialog(Form parentForm, UpdateResponse data)
    {
        var msg = $"⚡ Merdo Launcher için yeni bir sürüm mevcut!\n\n" +
                  $"   Mevcut sürümünüz : v{CurrentVersion}\n" +
                  $"   Yeni sürüm       : v{data.LatestVersion}\n\n" +
                  $"📋 Değişiklikler:\n{data.Changelog}\n\n" +
                  $"Devam etmek için güncellemeyi yüklemelisiniz. Şimdi yüklensin mi?";

        var result = MerdoDialog.ShowYesNo(parentForm, msg);

        if (result != DialogResult.Yes)
        {
            Application.Exit();
            Environment.Exit(0);
            return;
        }

        // Otomatik indir ve kur
        DownloadAndInstall(parentForm, data);
    }

    private static void DownloadAndInstall(Form parentForm, UpdateResponse data)
    {
        if (string.IsNullOrEmpty(data.DownloadUrl))
        {
            MerdoDialog.ShowWarning(parentForm, "İndirme bağlantısı bulunamadı. Lütfen web sitesini ziyaret edin.");
            return;
        }

        // İndirme penceresi
        var dlgForm = new Form
        {
            Text            = "Merdo Launcher Güncelleniyor...",
            Size            = new System.Drawing.Size(420, 130),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox     = false,
            MinimizeBox     = false,
            StartPosition   = FormStartPosition.CenterParent,
            BackColor       = System.Drawing.Color.FromArgb(12, 12, 15),
            ControlBox      = false
        };

        var lblInfo = new Label
        {
            Text      = "Yeni sürüm indiriliyor, lütfen bekleyin...",
            ForeColor = System.Drawing.Color.White,
            Font      = new System.Drawing.Font("Segoe UI", 9.5F),
            Location  = new System.Drawing.Point(20, 18),
            AutoSize  = true
        };

        var pb = new ProgressBar
        {
            Location = new System.Drawing.Point(20, 45),
            Size     = new System.Drawing.Size(370, 20),
            Style    = ProgressBarStyle.Continuous
        };

        var lblPercent = new Label
        {
            Text      = "%0",
            ForeColor = System.Drawing.Color.FromArgb(255, 204, 0),
            Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
            Location  = new System.Drawing.Point(20, 70),
            AutoSize  = true
        };

        dlgForm.Controls.AddRange(new Control[] { lblInfo, pb, lblPercent });
        dlgForm.Show(parentForm);

        Task.Run(async () =>
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "MerdoLauncher_Update.exe");

                using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MerdoLauncher/2.0");

                using var response = await client.GetAsync(data.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long totalBytes  = response.Content.Headers.ContentLength ?? -1;
                long downloaded  = 0;

                using var stream = await response.Content.ReadAsStreamAsync();
                using var file   = File.Create(tempPath);

                byte[] buffer = new byte[81920];
                int read;

                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    await file.WriteAsync(buffer.AsMemory(0, read));
                    downloaded += read;

                    if (totalBytes > 0)
                    {
                        int pct = (int)(downloaded * 100 / totalBytes);
                        dlgForm.Invoke(() =>
                        {
                            pb.Value         = pct;
                            lblPercent.Text  = $"%{pct}  ({downloaded / 1024 / 1024:0.0} / {totalBytes / 1024 / 1024:0.0} MB)";
                        });
                    }
                }

                file.Close();

                dlgForm.Invoke(() =>
                {
                    lblInfo.Text    = "İndirme tamamlandı! Kurulum başlatılıyor...";
                    lblPercent.Text = "%100";
                    pb.Value        = 100;
                });

                await Task.Delay(800);

                // Inno Setup /SILENT bayrağıyla sessizce kur, ardından yeni launcher'ı aç
                Process.Start(new ProcessStartInfo
                {
                    FileName  = tempPath,
                    Arguments = "/SILENT /NORESTART",
                    UseShellExecute = true
                });

                // Mevcut launcher'ı kapat
                dlgForm.Invoke(() =>
                {
                    dlgForm.Close();
                    Application.Exit();
                });
            }
            catch (Exception ex)
            {
                dlgForm.Invoke(() =>
                {
                    dlgForm.Close();
                    MerdoDialog.ShowError(parentForm,
                        $"İndirme sırasında hata oluştu:\n{ex.Message}\n\nEl ile güncellemek için:\n{data.DownloadUrl}");
                });
            }
        });
    }

    private static bool IsNewerVersion(string latest, string current)
    {
        if (Version.TryParse(latest, out var l) && Version.TryParse(current, out var c))
            return l > c;
        return false;
    }
}

public class UpdateResponse
{
    public string LatestVersion { get; set; } = string.Empty;
    public string DownloadUrl   { get; set; } = string.Empty;
    public string Changelog     { get; set; } = string.Empty;
}
