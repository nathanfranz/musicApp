using Music.DL.Interfaces;
using Music.DL.Models;
using System.Text.Json.Nodes;
using System.Linq;

namespace Music.DL.Repos;

internal class AppleMusicRepo(IHttpClientFactory httpClientFactory) : IMusicRepo
{
    internal const string ClientName = "AppleMusic";

    internal const string ClientBaseUrl = "https://api.music.apple.com";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ClientName);

    public async Task<IEnumerable<LibrarySong>> GetLibraryAsync(string userToken, string developerToken)
    {
        var allSongsData = await FetchAllSongsParallelAsync(developerToken, userToken);

        var allSongs = new List<LibrarySong>();
        foreach (var songItem in allSongsData)
        {
            var attributes = songItem["attributes"];

            if (attributes == null)
                continue;

            var song = new LibrarySong
            {
                Name = attributes["name"]?.GetValue<string>() ?? string.Empty,
                Artist = attributes["artistName"]?.GetValue<string>() ?? string.Empty
            };

            allSongs.Add(song);
        }

        return allSongs;
    }

    private async Task<List<JsonNode>> FetchAllSongsParallelAsync(string developerToken, string userToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {developerToken}");
        _httpClient.DefaultRequestHeaders.Add("Music-User-Token", userToken);

        var firstResponse = await _httpClient.GetStringAsync(
            $"{ClientBaseUrl}/v1/me/library/songs?limit=100");

        var firstJson = JsonNode.Parse(firstResponse);

        var dataNode = firstJson?["data"];
        if (dataNode is null || dataNode.AsArray() is null)
            return [];

        var data = dataNode.AsArray()
            .Where(item => item is not null)
            .Select(item => item!)
            .ToList();

        int total = firstJson?["meta"]?["total"]?.GetValue<int>() ?? data.Count;
        int limit = 100;

        int totalPages = (int)Math.Ceiling((double)total / limit);

        var urls = Enumerable.Range(0, totalPages)
            .Select(i =>
                $"{ClientBaseUrl}/v1/me/library/songs?limit={limit}&offset={i * limit}"
            )
            .ToList();

        const int batchSize = 5;
        var allTasks = new List<Task<string>>();

        var results = new List<string>();

        foreach (var batch in urls.Chunk(batchSize))
        {
            var tasks = batch.Select(url => _httpClient.GetStringAsync(url));
            var batchResults = await Task.WhenAll(tasks);
            results.AddRange(batchResults);
        }

        foreach (var jsonString in results.Skip(1))
        {
            var json = JsonNode.Parse(jsonString);

            var jsonDataNode = json?["data"];
            if (jsonDataNode is null || jsonDataNode.AsArray() is null)
                continue;

            foreach (var item in jsonDataNode.AsArray())
            {
                if (item is not null)
                    data.Add(item!);
            }
        }

        return data;
    }
}
