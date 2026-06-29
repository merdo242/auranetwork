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

    public LauncherResult StartMinecraft(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new LauncherResult(false, "Minecraft'ı başlatmak için kullanıcı adı gereklidir.");

        try
        {
            var path     = new MinecraftPath();
            var launcher = new MinecraftLauncher(path);
            var settings = _settingsService.Settings;

            // Versiyon seç
            string launchVersion;
            if (!string.IsNullOrEmpty(settings.SelectedVersion))
            {
                launchVersion = settings.SelectedVersion;
            }
            else
            {
                var allVersions = launcher.GetAllVersionsAsync().AsTask().GetAwaiter().GetResult().ToList();
                if (allVersions.Count == 0)
                    return new LauncherResult(false, "Bilgisayarınızda yüklü bir Minecraft sürümü bulunamadı.\n.minecraft/versions klasöründe kurulu bir sürüm olduğundan emin olun.");

                launchVersion = allVersions.FirstOrDefault(v =>
                    v.Name.Contains("OptiFine", StringComparison.OrdinalIgnoreCase) ||
                    v.Name.Contains("Fabric", StringComparison.OrdinalIgnoreCase))?.Name
                    ?? allVersions[0].Name;
            }

            // Java yolu
            string javaPath = !string.IsNullOrEmpty(settings.JavaPath) && File.Exists(settings.JavaPath)
                ? settings.JavaPath
                : FindSystemJavaPath();

            var session      = new MSession(username, "offline_token", Guid.NewGuid().ToString("N"));
            var launchOptions = new MLaunchOption
            {
                Session      = session,
                MaximumRamMb = settings.MaxRamMb,
                JavaPath     = string.IsNullOrEmpty(javaPath) ? null : javaPath
            };

            var process = launcher.BuildProcessAsync(launchVersion, launchOptions, default).AsTask().GetAwaiter().GetResult();

            // Konsol göster / gizle
            if (!settings.ShowConsole)
                process.StartInfo.CreateNoWindow = true;

            process.Start();

            return new LauncherResult(true, $"Minecraft {launchVersion} başarıyla başlatıldı!");
        }
        catch (Exception ex)
        {
            return new LauncherResult(false, "Minecraft başlatılamadı: " + ex.Message);
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

        // 3) Bilinen kurulum dizinleri
        string pf   = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        string[] roots =
        {
            Path.Combine(pf,   "Eclipse Adoptium"),
            Path.Combine(pf,   "Java"),
            Path.Combine(pf,   "Microsoft"),
            Path.Combine(pf,   "BellSoft"),
            Path.Combine(pf,   "Zulu"),
            Path.Combine(pf86, "Java"),
            Path.Combine(pf86, "Eclipse Adoptium"),
            Path.Combine(appData, ".minecraft", "runtime")
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
