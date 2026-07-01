using System.Net.NetworkInformation;

namespace MerdoClient;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // --- 1) Splash (loading) ekranını göster ---
        using var splash = new SplashForm();
        splash.ShowDialog(); // Kendi zamanlayıcısı tamamlanınca kapanır

        // --- 2) İnternet bağlantısı kontrolü ---
        if (!IsInternetAvailable())
        {
            MessageBox.Show(
                "Merdo Launcher'a erişmek için internet bağlantısı gereklidir.\n\nLütfen bağlantınızı kontrol edip tekrar deneyin.",
                "Bağlantı Yok",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return; // Uygulama kapanır, Form1 açılmaz
        }

        // --- 3) Ana ekranı aç ---
        Application.Run(new Form1());
    }

    private static bool IsInternetAvailable()
    {
        try
        {
            using var ping = new Ping();
            var reply = ping.Send("8.8.8.8", 2000); // Google DNS — 2 saniyelik zaman aşımı
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }
}