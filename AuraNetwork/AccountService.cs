using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuraNetwork;

public class AccountService
{
    private readonly string AppDataPath;
    private readonly string SavedAccountsFilePath;
    private readonly string ErrorLogFilePath;

    private readonly List<SavedAccount> _savedAccounts = new();
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string ApiBaseUrl = "http://auranetwork.com.tr/launcher_api.php";
    private const string FirebaseBansUrl = "https://auranw-c3bf4-default-rtdb.firebaseio.com/bans";

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

    public async Task<bool> IsBanned(string username)
    {
        try
        {
            string url = $"{FirebaseBansUrl}/{Uri.EscapeDataString(username.ToLower())}.json?shallow=true";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return json != "null";
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
            string url = $"{ApiBaseUrl}?action=check&username={Uri.EscapeDataString(username)}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("registered", out var regProp))
                {
                    return regProp.GetBoolean();
                }
            }
        }
        catch (Exception ex)
        {
            LogError("API Check Error", ex);
        }
        return false;
    }

    public async Task<bool> Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        try
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            string url = $"{ApiBaseUrl}?action=register";
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("success", out var successProp))
                {
                    return successProp.GetBoolean();
                }
            }
        }
        catch (Exception ex)
        {
            LogError("API Register Error", ex);
        }
        return false;
    }

    public async Task<bool> Login(string username, string password)
    {
        try
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            string url = $"{ApiBaseUrl}?action=login";
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("success", out var successProp))
                {
                    return successProp.GetBoolean();
                }
            }
        }
        catch (Exception ex)
        {
            LogError("API Login Error", ex);
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