using System.IO;
using System.Text.Json;

namespace MerdoClient;

public class AccountService
{
    private static readonly string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MerdoLauncher"
    );
    private static readonly string AccountsFilePath = Path.Combine(AppDataPath, "accounts.json");
    private static readonly string SavedAccountsFilePath = Path.Combine(AppDataPath, "saved_accounts.json");
    private static readonly string ErrorLogFilePath = Path.Combine(AppDataPath, "error.log");

    private readonly Dictionary<string, string> _accounts = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<SavedAccount> _savedAccounts = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public AccountService()
    {
        LoadData();
    }

    private void LogError(string message, Exception ex)
    {
        try
        {
            Directory.CreateDirectory(AppDataPath);
            File.AppendAllText(ErrorLogFilePath, $"[{DateTime.Now}] {message}: {ex.Message}\n{ex.StackTrace}\n\n");
        }
        catch { }
    }

    private void LoadData()
    {
        try
        {
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }

            if (File.Exists(AccountsFilePath))
            {
                string json = File.ReadAllText(AccountsFilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions);
                if (data != null)
                {
                    _accounts.Clear();
                    foreach (var kvp in data)
                    {
                        _accounts[kvp.Key] = kvp.Value;
                    }
                }
            }

            if (File.Exists(SavedAccountsFilePath))
            {
                string json = File.ReadAllText(SavedAccountsFilePath);
                var data = JsonSerializer.Deserialize<List<SavedAccount>>(json, JsonOptions);
                if (data != null)
                {
                    _savedAccounts.Clear();
                    _savedAccounts.AddRange(data);
                }
            }
        }
        catch (Exception ex)
        {
            LogError("LoadData Hatası", ex);
        }
    }

    private void SaveData()
    {
        try
        {
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }

            string accountsJson = JsonSerializer.Serialize(_accounts, JsonOptions);
            File.WriteAllText(AccountsFilePath, accountsJson);

            string savedJson = JsonSerializer.Serialize(_savedAccounts, JsonOptions);
            File.WriteAllText(SavedAccountsFilePath, savedJson);
        }
        catch (Exception ex)
        {
            LogError("SaveData Hatası", ex);
        }
    }

    public bool Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (_accounts.Count >= 1)
        {
            return false;
        }

        if (_accounts.ContainsKey(username))
        {
            return false;
        }

        _accounts[username] = password;
        SaveData();
        return true;
    }

    public bool HasAccountRegistered()
    {
        return _accounts.Count >= 1;
    }

    public bool Login(string username, string password)
    {
        if (!_accounts.TryGetValue(username, out var storedPassword))
        {
            return false;
        }

        return string.Equals(storedPassword, password, StringComparison.Ordinal);
    }

    public void SaveAccountCredential(string username, string password)
    {
        _savedAccounts.RemoveAll(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        _savedAccounts.Add(new SavedAccount(username, password));
        SaveData();
    }

    public void RemoveSavedAccount(string username)
    {
        _savedAccounts.RemoveAll(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        SaveData();
    }

    public List<SavedAccount> GetSavedAccounts()
    {
        return _savedAccounts;
    }

    public void ResetForTesting()
    {
        _accounts.Clear();
        _savedAccounts.Clear();
        SaveData();
    }
}

public class SavedAccount
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public SavedAccount() { }

    public SavedAccount(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
