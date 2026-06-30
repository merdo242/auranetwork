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
    public LauncherResult StartMinecraft(string username, Action<string>? onProgress = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new LauncherResult(false, "Minecraft'ı başlatmak için kullanıcı adı gereklidir.");

        try
        {
            var path     = new MinecraftPath();
            var launcher = new MinecraftLauncher(path);
            var settings = _settingsService.Settings;

            // --- İndirme durumunu bildiren olaylar ---
            launcher.FileProgressChanged += (sender, e) =>
            {
                onProgress?.Invoke("Oyun dosyaları indiriliyor ve doğrulanıyor...");
            };

            // --- OptiFine'ı otomatik indir ve kur ---
            string optifineName = "ForgeOptiFine 1.21.8";
            string optiPath = Path.Combine(path.BasePath, "versions", optifineName);
            string optiLibs = Path.Combine(path.BasePath, "libraries", "optifine");
            
            if (!Directory.Exists(optiPath) || !Directory.Exists(optiLibs))
            {
                onProgress?.Invoke("OptiFine Full Paketi indiriliyor (Eksik kütüphaneler dahil)... Lütfen bekleyin.");
                try 
                {
                    using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromMinutes(10) };
                    var zipBytes = client.GetByteArrayAsync("https://github.com/merdo242/merdoclient/raw/main/installer/ForgeOptiFine_Full.zip").GetAwaiter().GetResult();
                    var tempZip = Path.Combine(Path.GetTempPath(), "merdo_optifine_full.zip");
                    System.IO.File.WriteAllBytes(tempZip, zipBytes);
                    System.IO.Compression.ZipFile.ExtractToDirectory(tempZip, path.BasePath, true);
                    System.IO.File.Delete(tempZip);
                    onProgress?.Invoke("OptiFine ve gerekli kütüphaneler başarıyla kuruldu.");
                } 
                catch (Exception ex) 
                {
                    return new LauncherResult(false, "OptiFine indirilemedi: " + ex.Message + "\nİnternet bağlantınızı kontrol edin.");
                }
            }

            // --- OptiFine sürümünü otomatik bul (yoksa 1.21.8 vanilla) ---
            string launchVersion = FindOptifineVersion(path.BasePath) ?? FixedVersion;

            // --- Java yolunu bul ---
            onProgress?.Invoke("Java kontrol ediliyor...");
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
            onProgress?.Invoke("Minecraft dosyaları hazırlanıyor...");
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
        // 1) Minecraft'ın kendi runtime dizini — ÖNCE ARANIR (Java 21/17 Öncelikli)
        string appData   = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string mcRuntime = Path.Combine(appData, ".minecraft", "runtime");
        if (Directory.Exists(mcRuntime))
        {
            try
            {
                // Önce java-runtime-delta (Java 21 - MC 1.20.5+ için) veya gamma (Java 17) ara
                string[] preferredRuntimes = { "java-runtime-delta", "java-runtime-gamma", "java-runtime-beta" };
                foreach (var pref in preferredRuntimes)
                {
                    var prefDirs = Directory.GetDirectories(mcRuntime, pref, SearchOption.AllDirectories);
                    foreach (var pDir in prefDirs)
                    {
                        var jPath = Path.Combine(pDir, "bin", "javaw.exe");
                        if (File.Exists(jPath)) return jPath;
                    }
                }

                // Bulunamazsa herhangi bir javaw.exe (legacy vb.)
                foreach (var found in Directory.EnumerateFiles(mcRuntime, "javaw.exe", SearchOption.AllDirectories))
                    return found;
            }
            catch { }
        }

        // 2) JAVA_HOME
        string? javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
        if (!string.IsNullOrEmpty(javaHome))
        {
            string c = Path.Combine(javaHome, "bin", "javaw.exe");
            if (File.Exists(c)) return c;
        }

        // 3) PATH
        string? pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrEmpty(pathEnv))
        {
            foreach (var dir in pathEnv.Split(';'))
            {
                string c = Path.Combine(dir.Trim(), "javaw.exe");
                if (File.Exists(c)) return c;
            }
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

    /// <summary>.minecraft/versions klasöründeki 1.21.8 Optifine sürümlerini arar</summary>
    private static string? FindOptifineVersion(string basePath)
    {
        string versionsDir = Path.Combine(basePath, "versions");
        if (!Directory.Exists(versionsDir)) return null;

        try
        {
            var dirs = Directory.GetDirectories(versionsDir);
            // ForgeOptiFine 1.21.8 veya OptiFine 1.21.8 gibi klasörleri bul
            foreach (var d in dirs)
            {
                string name = Path.GetFileName(d);
                if (name.Contains("1.21.8") && name.IndexOf("OptiFine", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return name;
                }
            }
        }
        catch { }

        return null;
    }
}

public sealed record LauncherResult(bool Success, string Message);
