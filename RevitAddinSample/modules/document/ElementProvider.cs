using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

static class ElementProvider
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly string apiUrl = "http://eq-api-dev-alb-1162581781.us-east-2.elb.amazonaws.com/elements";
    private const string sessionId = "1";

    public static async Task<bool> CreateElementAsync(RoomData room)
    {
        try
        {
            var response = await client.PostAsJsonAsync($"{apiUrl}?sessionId={sessionId}", room);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"CreateElementAsync Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"CreateElementAsync Exception: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> CreateElementsAsync(List<RoomData> rooms)
    {
        try
        {
            var response = await client.PostAsJsonAsync($"{apiUrl}?sessionId={sessionId}", rooms);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"CreateElementsAsync Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"CreateElementsAsync Exception: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> UpdateElementAsync(RoomData room)
    {
        try
        {
            var response = await client.PatchAsJsonAsync($"{apiUrl}/{room.ApplicationId}?sessionId={sessionId}", room);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"UpdateElementAsync Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UpdateElementAsync Exception: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> UpdateElementsAsync(List<RoomData> rooms)
    {
        try
        {
            var response = await client.PatchAsJsonAsync($"{apiUrl}?sessionId={sessionId}", rooms);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"UpdateElementsAsync Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UpdateElementsAsync Exception: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> DeleteElementAsync(string id)
    {
        try
        {
            var requestUri = $"{apiUrl}/{id}?sessionId={sessionId}";
            var response = await client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri)
            });
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"DeleteElementAsync Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DeleteElementAsync Exception: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> DeleteElementsAsync(List<string> ids)
    {
        try
        {
            var requestUri = $"{apiUrl}?sessionId={sessionId}";
            var response = await client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri),
                Content = JsonContent.Create(ids)
            });
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"DeleteElementsAsync Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DeleteElementsAsync Exception: {ex.Message}");
            return false;
        }
    }
}
