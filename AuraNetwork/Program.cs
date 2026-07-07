using System.Net.NetworkInformation;

namespace AuraNetwork;

static class Program
{
    // Kurulum dizini: %LOCALAPPDATA%\AuraNetwork\
    private static readonly string InstallDir  = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AuraNetwork");
    private static readonly string InstalledExe = Path.Combine(InstallDir, "AuraNetwork.exe");

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // --- 0) Kendi kendine kurulum ---
        string currentExe = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
        bool isInstalled  = string.Equals(currentExe, InstalledExe, StringComparison.OrdinalIgnoreCase);

        if (!isInstalled)
        {
            SelfInstall(currentExe);
            return; // Kurulumdan sonra yeni konumdan yeniden başlar
        }

        // --- 1) Splash (loading) ekranını göster ---
        using var splash = new SplashForm();
        splash.ShowDialog(); // Kendi zamanlayıcısı tamamlanınca kapanır

        // --- 2) İnternet bağlantısı kontrolü ---
        if (!IsInternetAvailable())
        {
            MessageBox.Show(
                "AuraNW Launcher'a erişmek için internet bağlantısı gereklidir.\n\nLütfen bağlantınızı kontrol edip tekrar deneyin.",
                "Bağlantı Yok",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return; // Uygulama kapanır, Form1 açılmaz
        }

        // --- 3) Ana ekranı aç ---
        Application.Run(new Form1());
    }

    private static void SelfInstall(string sourceExe)
    {
        try
        {
            // Klasörü oluştur
            Directory.CreateDirectory(InstallDir);

            // EXE'yi kopyala
            File.Copy(sourceExe, InstalledExe, overwrite: true);

            // Masaüstü kısayolu oluştur
            string desktop    = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcut   = Path.Combine(desktop, "Aura Network.lnk");
            CreateShortcut(shortcut, InstalledExe);

            // Başlat menüsü kısayolu
            string startMenu  = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                "Programs");
            Directory.CreateDirectory(startMenu);
            CreateShortcut(Path.Combine(startMenu, "Aura Network.lnk"), InstalledExe);

            MessageBox.Show(
                "AuraNW Launcher başarıyla kuruldu!\n\nMasaüstünüzdeki 'Aura Network' kısayolundan açabilirsiniz.",
                "Kurulum Tamamlandı",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Kurulu konumdan başlat
            System.Diagnostics.Process.Start(InstalledExe);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Kurulum sırasında hata oluştu:\n{ex.Message}",
                "Kurulum Hatası",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static void CreateShortcut(string shortcutPath, string targetPath)
    {
        // WScript.Shell COM nesnesi ile kısayol oluştur
        try
        {
            dynamic shell   = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell")!)!;
            dynamic lnk     = shell.CreateShortcut(shortcutPath);
            lnk.TargetPath  = targetPath;
            lnk.WorkingDirectory = Path.GetDirectoryName(targetPath);
            lnk.Description = "Aura Network Launcher";
            lnk.Save();
        }
        catch { }
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
