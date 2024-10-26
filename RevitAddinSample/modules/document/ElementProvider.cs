using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Json;

static class ElementProvider
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<T> CreateElementAsync<T>(string url, T element)
    {
        var response = await client.PostAsJsonAsync(url, element);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public static async Task<T> ReadElementAsync<T>(string url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public static async Task<T> UpdateElementAsync<T>(string url, T element)
    {
        var response = await client.PutAsJsonAsync(url, element);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public static async Task<bool> DeleteElementAsync(string url)
    {
        var response = await client.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}
