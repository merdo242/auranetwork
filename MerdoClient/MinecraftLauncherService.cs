using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

namespace MerdoClient;

public class MinecraftLauncherService
{
    private readonly SettingsService _settingsService;

    public MinecraftLauncherService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>Minecraft'ı başlatır. Arka planda (Task.Run) çağrılmalıdır.</summary>
    public LauncherResult StartMinecraft(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new LauncherResult(false, "Minecraft'ı başlatmak için kullanıcı adı gereklidir.");

        try
        {
            var path     = new MinecraftPath();
            var launcher = new MinecraftLauncher(path);
            var settings = _settingsService.Settings;

            // --- Sürüm seç ---
            string launchVersion;
            if (!string.IsNullOrEmpty(settings.SelectedVersion))
            {
                launchVersion = settings.SelectedVersion;
            }
            else
            {
                // Yerel sürümleri listele (senkron)
                var allVersions = launcher.GetAllVersionsAsync()
                                          .AsTask()
                                          .ConfigureAwait(false)
                                          .GetAwaiter()
                                          .GetResult()
                                          .ToList();

                if (allVersions.Count == 0)
                    return new LauncherResult(false,
                        "Bilgisayarınızda yüklü bir Minecraft sürümü bulunamadı.\n" +
                        ".minecraft/versions klasöründe kurulu bir sürüm olduğundan emin olun.");

                // OptiFine > Fabric > ilk sürüm
                launchVersion =
                    allVersions.FirstOrDefault(v => v.Name.Contains("OptiFine", StringComparison.OrdinalIgnoreCase))?.Name
                    ?? allVersions.FirstOrDefault(v => v.Name.Contains("Fabric", StringComparison.OrdinalIgnoreCase))?.Name
                    ?? allVersions[0].Name;
            }

            // --- Java yolu ---
            string javaPath = !string.IsNullOrEmpty(settings.JavaPath) && File.Exists(settings.JavaPath)
                ? settings.JavaPath
                : FindSystemJavaPath();

            if (string.IsNullOrEmpty(javaPath))
                return new LauncherResult(false,
                    "Java bulunamadı!\n\n" +
                    "Ayarlar menüsünden Java yolunu manuel olarak seçin.\n" +
                    "Genellikle şu konumdadır:\n" +
                    @"%AppData%\.minecraft\runtime\java-runtime-delta\windows\java-runtime-delta\bin\javaw.exe");

            // --- Oturum ve başlatma seçenekleri ---
            var session = new MSession(username, "offline_token", Guid.NewGuid().ToString("N"));
            var launchOptions = new MLaunchOption
            {
                Session      = session,
                MaximumRamMb = settings.MaxRamMb,
                JavaPath     = javaPath
            };

            // Process'i oluştur (senkron çağrı — Task.Run içinde güvenli)
            var process = launcher.BuildProcessAsync(launchVersion, launchOptions, default)
                                  .AsTask()
                                  .ConfigureAwait(false)
                                  .GetAwaiter()
                                  .GetResult();

            // Konsol göster / gizle
            process.StartInfo.CreateNoWindow  = !settings.ShowConsole;
            process.StartInfo.UseShellExecute = false;

            bool started = process.Start();
            if (!started)
                return new LauncherResult(false, "Minecraft prosesi başlatılamadı (process.Start() false döndürdü).");

            return new LauncherResult(true, $"Minecraft {launchVersion} başarıyla başlatıldı!");
        }
        catch (Exception ex)
        {
            return new LauncherResult(false,
                $"Minecraft başlatılamadı:\n{ex.Message}\n\n" +
                "İpuçları:\n" +
                "• Ayarlar'dan doğru Java yolunu seçin\n" +
                "• Ayarlar'dan oynatmak istediğiniz sürümü seçin\n" +
                "• Minecraft Launcher ile en az bir kez oynamış olduğunuzdan emin olun");
        }
    }

    /// <summary>Sistemde kurulu javaw.exe'yi arar. SettingsForm'dan da çağrılır.</summary>
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

        // 3) Minecraft runtime (Türkçe 'ı' içeren 'wındows' klasörü dahil)
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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

        // 4) Bilinen kurulum dizinleri
        string pf   = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        string[] roots =
        {
            Path.Combine(pf,   "Eclipse Adoptium"),
            Path.Combine(pf,   "Java"),
            Path.Combine(pf,   "Microsoft"),
            Path.Combine(pf,   "BellSoft"),
            Path.Combine(pf,   "Zulu"),
            Path.Combine(pf86, "Java"),
            Path.Combine(pf86, "Eclipse Adoptium"),
        };

        foreach (var root in roots)
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
