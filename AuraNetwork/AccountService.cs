using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuraNetwork;

public class AccountService
{
    private readonly string AppDataPath;
    private readonly string SavedAccountsFilePath;
    private readonly string ErrorLogFilePath;

    private readonly List<SavedAccount> _savedAccounts = new();
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string FirebaseBaseUrl = "https://auranw-c3bf4-default-rtdb.firebaseio.com/accounts";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public AccountService(string? customPath = null)
    {
        AppDataPath = customPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AuraNWLauncher"
        );
        SavedAccountsFilePath = Path.Combine(AppDataPath, "saved_accounts.json");
        ErrorLogFilePath = Path.Combine(AppDataPath, "error.log");

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

            string savedJson = JsonSerializer.Serialize(_savedAccounts, JsonOptions);
            File.WriteAllText(SavedAccountsFilePath, savedJson);
        }
        catch (Exception ex)
        {
            LogError("SaveData Hatası", ex);
        }
    }

    private class AccountData
    {
        public string password { get; set; } = string.Empty;
        public bool isBanned { get; set; } = false;
    }

    public async Task<bool> IsBanned(string username)
    {
        try
        {
            string url = $"{FirebaseBaseUrl}/{username}.json";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                if (json != "null")
                {
                    var data = JsonSerializer.Deserialize<AccountData>(json, JsonOptions);
                    return data != null && data.isBanned;
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Firebase Ban Check Error", ex);
        }
        return false;
    }

    public async Task<bool> IsRegistered(string username)
    {
        try
        {
            string url = $"{FirebaseBaseUrl}/{username}.json?shallow=true";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return json != "null";
            }
        }
        catch (Exception ex)
        {
            LogError("Firebase Check Error", ex);
        }
        return false;
    }

    public async Task<bool> Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        bool isExist = await IsRegistered(username);
        if (isExist) return false;

        try
        {
            var data = new AccountData { password = password };
            string json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{FirebaseBaseUrl}/{username}.json";
            var response = await _httpClient.PutAsync(url, content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            LogError("Firebase Register Error", ex);
            return false;
        }
    }

    public async Task<bool> Login(string username, string password)
    {
        try
        {
            string url = $"{FirebaseBaseUrl}/{username}.json";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                if (json != "null")
                {
                    var data = JsonSerializer.Deserialize<AccountData>(json);
                    return data != null && data.password == password;
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Firebase Login Error", ex);
        }
        return false;
    }

    public bool HasReachedRegisterLimit()
    {
        return _savedAccounts.Count >= 3;
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