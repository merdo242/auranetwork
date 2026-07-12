using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuraNetwork
{
    public class FirebaseService
    {
        private static readonly string DatabaseUrl = "https://auranw-c3bf4-default-rtdb.firebaseio.com/";
        private static readonly HttpClient client = new HttpClient();

        public async Task SetUserOnline(string username, bool isOnline)
        {
            try
            {
                var payload = new { isOnline = isOnline, lastSeen = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
                string json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PutAsync($"{DatabaseUrl}users/{username.ToLower()}.json", content);
            }
            catch { }
        }

        public async Task SendFriendRequest(string fromUser, string toUser)
        {
            try
            {
                var content = new StringContent("true", Encoding.UTF8, "application/json");
                await client.PutAsync($"{DatabaseUrl}friendRequests/{toUser.ToLower()}/{fromUser.ToLower()}.json", content);
            }
            catch { }
        }

        public async Task AcceptFriendRequest(string myUsername, string requesterUsername)
        {
            try
            {
                var content = new StringContent("true", Encoding.UTF8, "application/json");
                // Istegi sil
                await client.DeleteAsync($"{DatabaseUrl}friendRequests/{myUsername.ToLower()}/{requesterUsername.ToLower()}.json");
                // Iki tarafa da ekle
                await client.PutAsync($"{DatabaseUrl}friends/{myUsername.ToLower()}/{requesterUsername.ToLower()}.json", content);
                await client.PutAsync($"{DatabaseUrl}friends/{requesterUsername.ToLower()}/{myUsername.ToLower()}.json", content);
            }
            catch { }
        }

        public async Task RejectFriendRequest(string myUsername, string requesterUsername)
        {
            try
            {
                await client.DeleteAsync($"{DatabaseUrl}friendRequests/{myUsername.ToLower()}/{requesterUsername.ToLower()}.json");
            }
            catch { }
        }

        public async Task RemoveFriend(string myUsername, string targetUsername)
        {
            try
            {
                await client.DeleteAsync($"{DatabaseUrl}friends/{myUsername.ToLower()}/{targetUsername.ToLower()}.json");
                await client.DeleteAsync($"{DatabaseUrl}friends/{targetUsername.ToLower()}/{myUsername.ToLower()}.json");
            }
            catch { }
        }

        public async Task<List<string>> GetFriendRequests(string username)
        {
            try
            {
                var response = await client.GetStringAsync($"{DatabaseUrl}friendRequests/{username.ToLower()}.json");
                if (response != "null")
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, bool>>(response);
                    if (dict != null) return new List<string>(dict.Keys);
                }
            }
            catch { }
            return new List<string>();
        }

        public async Task<Dictionary<string, bool>> GetFriendsWithStatus(string username)
        {
            var result = new Dictionary<string, bool>();
            try
            {
                var response = await client.GetStringAsync($"{DatabaseUrl}friends/{username.ToLower()}.json");
                if (response != "null")
                {
                    var friendsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(response);
                    if (friendsDict != null)
                    {
                        foreach (var friend in friendsDict.Keys)
                        {
                            var statusResponse = await client.GetStringAsync($"{DatabaseUrl}users/{friend.ToLower()}/isOnline.json");
                            bool isOnline = statusResponse != "null" && statusResponse.Trim().ToLower() == "true";
                            result[friend] = isOnline;
                        }
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
