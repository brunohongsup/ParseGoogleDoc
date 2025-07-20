// See https://aka.ms/new-console-template for more information
using System.Net.Http;
using System.Threading.Tasks;

Console.WriteLine("Hello, World!");


async Task<string> GetGoogleDocText(string docId)
{
    var url = $"https://docs.google.com/document/d/{docId}/export?format=txt";
    using (var client = new HttpClient())
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
