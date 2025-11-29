using Music.BL.Interfaces;
using Music.DL.Models;

namespace Music.BL;

public class Service(MusicServiceType musicServiceType, IMusicServiceFactory musicServiceFactory)
{
    private readonly IMusicService musicService = musicServiceFactory.GetService(musicServiceType);

    public string GetDeveloperToken()
    {
        return musicService.DeveloperToken;
    }

    public async Task<IEnumerable<LibrarySong>> DoStuff(string userToken)
    {
        var library = await musicService.GetLibraryAsync(userToken);

        return library;
    }
}
