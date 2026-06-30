using System.Text.Json;
using CmlLib.Core;

namespace MerdoClient;

public class SettingsService
{
    private static readonly string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MerdoLauncher");

    private static readonly string SettingsFilePath = Path.Combine(AppDataPath, "settings.json");

    public LauncherSettings Settings { get; private set; } = new();

    public SettingsService()
    {
        Load();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                Settings = JsonSerializer.Deserialize<LauncherSettings>(json) ?? new();
            }
        }
        catch { Settings = new(); }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(AppDataPath);
            File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }

    public List<string> GetAvailableVersions()
    {
        var versions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 1. Yerel yüklü sürümleri oku
        try
        {
            string versionsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft", "versions");

            if (Directory.Exists(versionsPath))
            {
                foreach (var dir in Directory.GetDirectories(versionsPath))
                {
                    string name = Path.GetFileName(dir);
                    if (Directory.GetFiles(dir, "*.jar").Length > 0)
                        versions.Add(name);
                }
            }
        }
        catch { }

        // 2. Çevrimiçi Mojang ana sürümlerini (Release) ekle — sadece güncel (1.16+)
        try
        {
            var path = new MinecraftPath();
            var launcher = new CmlLib.Core.MinecraftLauncher(path);
            var onlineVersions = launcher.GetAllVersionsAsync().AsTask().GetAwaiter().GetResult();
            foreach (var v in onlineVersions)
            {
                if (string.Equals(v.Type, "release", StringComparison.OrdinalIgnoreCase)
                    && IsModernVersion(v.Name))
                {
                    versions.Add(v.Name);
                }
            }
        }
        catch { }

        return versions.OrderByDescending(v => v).ToList();
    }

    /// <summary>
    /// Sadece 1.16+ ve yeni versiyonlama şemasındaki (26.x+) sürümleri kabul eder.
    /// Fabric, OptiFine, Forge gibi mod yükleyici sürümleri her zaman dahil edilir.
    /// </summary>
    private static bool IsModernVersion(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        // Mod yükleyici sürümleri (Fabric, OptiFine, Forge, vb.) — hep göster
        if (!char.IsDigit(name[0])) return true;

        var parts = name.Split('.');
        if (parts.Length < 1) return false;

        if (!int.TryParse(parts[0], out int major)) return false;

        // Yeni versiyonlama şeması: 26.x, 27.x... (1.21+ sonrası Mojang formatı)
        if (major >= 20) return true;

        // Klasik 1.x.x versiyonlama — 1.16 ve sonrası
        if (major == 1 && parts.Length >= 2 && int.TryParse(parts[1], out int minor))
            return minor >= 16;

        return false;
    }
}

public class LauncherSettings
{
    public int MaxRamMb       { get; set; } = 4096;
    public int MusicVolume    { get; set; } = 25;   // 0-100
    public bool CloseOnLaunch { get; set; } = false;
    public bool ShowConsole   { get; set; } = false;
    public string JavaPath    { get; set; } = string.Empty;
}
