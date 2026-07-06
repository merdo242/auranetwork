using System.Text.Json;

namespace AuraNetwork;

public class ChatMessage
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public long Timestamp { get; set; }
}

public class ChatMessagesResponse
{
    public List<ChatMessage> Messages { get; set; } = new();
}

public class ChatSendResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
    public long Id { get; set; }
}

public class ChatService
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private long _lastMessageId;
    private readonly CancellationTokenSource _cts = new();

    public event Action<ChatMessage>? OnNewMessage;
    public event Action<string>? OnError;

    public ChatService(string baseUrl = "http://91.132.49.16:8880")
    {
        _baseUrl = baseUrl;
        _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("AuraNetwork/2.0");
    }

    public void StartPolling()
    {
        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var url = $"{_baseUrl}/chat/messages?since={_lastMessageId}";
                    var response = await _client.GetStringAsync(url, _cts.Token);
                    var data = JsonSerializer.Deserialize<ChatMessagesResponse>(response,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (data?.Messages != null)
                    {
                        foreach (var msg in data.Messages)
                        {
                            if (msg.Id > _lastMessageId)
                            {
                                _lastMessageId = msg.Id;
                                OnNewMessage?.Invoke(msg);
                            }
                        }
                    }
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    OnError?.Invoke($"Sohbet bağlantı hatası: {ex.Message}");
                }

                try
                {
                    await Task.Delay(1500, _cts.Token); // 1.5 saniyede bir polling
                }
                catch (TaskCanceledException) { break; }
            }
        }, _cts.Token);
    }

    public async Task<bool> SendMessage(string username, string password, string message)
    {
        try
        {
            var url = $"{_baseUrl}/chat/send?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}&message={Uri.EscapeDataString(message)}";
            var response = await _client.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<ChatSendResponse>(response,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data?.Success ?? false;
        }
        catch (Exception ex)
        {
            OnError?.Invoke($"Mesaj gönderilemedi: {ex.Message}");
            return false;
        }
    }

    public void Stop()
    {
        _cts.Cancel();
        _client.Dispose();
    }
}