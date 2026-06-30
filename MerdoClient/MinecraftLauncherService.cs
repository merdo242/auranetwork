using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

namespace MerdoClient;

public class MinecraftLauncherService
{
    private readonly SettingsService _settingsService;

    // Sunucuyla uyumlu sabit sürüm
    public const string FixedVersion = "1.21.8";

    public MinecraftLauncherService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>Minecraft'ı başlatır. Arka planda (Task.Run içinde) çağrılmalıdır.</summary>
    public LauncherResult StartMinecraft(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new LauncherResult(false, "Minecraft'ı başlatmak için kullanıcı adı gereklidir.");

        try
        {
            var path     = new MinecraftPath();
            var launcher = new MinecraftLauncher(path);
            var settings = _settingsService.Settings;

            // --- Sabit sürüm 1.21.8 ---
            string launchVersion = FixedVersion;

            // --- Java yolunu bul ---
            string javaPath = !string.IsNullOrEmpty(settings.JavaPath) && File.Exists(settings.JavaPath)
                ? settings.JavaPath
                : FindSystemJavaPath();

            if (string.IsNullOrEmpty(javaPath))
                return new LauncherResult(false,
                    "Java bulunamadı!\n\n" +
                    "Ayarlar menüsünden Java yolunu manuel olarak seçin.\n\n" +
                    "Tipik konum:\n" +
                    @"%AppData%\.minecraft\runtime\java-runtime-delta\windows\java-runtime-delta\bin\javaw.exe");

            // --- Session ve başlatma seçenekleri ---
            var session = new MSession(username, "offline_token", Guid.NewGuid().ToString("N"));
            var launchOptions = new MLaunchOption
            {
                Session      = session,
                MaximumRamMb = settings.MaxRamMb,
                JavaPath     = javaPath
            };

            // Process'i oluştur (Task.Run içinde çalıştığından deadlock yok)
            var process = launcher.BuildProcessAsync(launchVersion, launchOptions, default)
                                  .AsTask()
                                  .ConfigureAwait(false)
                                  .GetAwaiter()
                                  .GetResult();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow  = !settings.ShowConsole;

            bool started = process.Start();
            if (!started)
                return new LauncherResult(false, "Minecraft prosesi başlatılamadı.");

            return new LauncherResult(true, $"Minecraft {launchVersion} başarıyla başlatıldı!");
        }
        catch (Exception ex)
        {
            return new LauncherResult(false,
                $"Minecraft başlatılamadı:\n{ex.Message}\n\n" +
                "• Minecraft Launcher ile en az bir kez oynamış olduğunuzdan emin olun\n" +
                "• Ayarlar'dan Java yolunu manuel seçin");
        }
    }

    /// <summary>Sistemde kurulu javaw.exe'yi arar.</summary>
    public static string FindSystemJavaPath()
    {
        // 1) JAVA_HOME
        string? javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
        if (!string.IsNullOrEmpty(javaHome))
        {
            string c = Path.Combine(javaHome, "bin", "javaw.exe");
            if (File.Exists(c)) return c;
        }

        // 2) PATH
        string? pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrEmpty(pathEnv))
        {
            foreach (var dir in pathEnv.Split(';'))
            {
                string c = Path.Combine(dir.Trim(), "javaw.exe");
                if (File.Exists(c)) return c;
            }
        }

        // 3) Minecraft'ın kendi runtime dizini — ÖNCE ARANIR
        //    (Türkçe 'ı' içeren 'wındows' klasörü dahil EnumerateFiles ile bulunur)
        string appData   = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string mcRuntime = Path.Combine(appData, ".minecraft", "runtime");
        if (Directory.Exists(mcRuntime))
        {
            try
            {
                foreach (var found in Directory.EnumerateFiles(mcRuntime, "javaw.exe", SearchOption.AllDirectories))
                    return found;
            }
            catch { }
        }

        // 4) Bilinen Java kurulum dizinleri
        string pf   = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        foreach (var root in new[]
        {
            Path.Combine(pf,   "Eclipse Adoptium"),
            Path.Combine(pf,   "Java"),
            Path.Combine(pf,   "Microsoft"),
            Path.Combine(pf,   "BellSoft"),
            Path.Combine(pf,   "Zulu"),
            Path.Combine(pf86, "Java"),
            Path.Combine(pf86, "Eclipse Adoptium"),
        })
        {
            if (!Directory.Exists(root)) continue;
            try
            {
                foreach (var found in Directory.EnumerateFiles(root, "javaw.exe", SearchOption.AllDirectories))
                    return found;
            }
            catch { }
        }

        return string.Empty;
    }
}

public sealed record LauncherResult(bool Success, string Message);
