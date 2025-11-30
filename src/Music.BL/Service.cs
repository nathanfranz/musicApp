using Music.BL.Interfaces;
using Music.DL.Models;

namespace Music.BL;

public class Service(IMusicServiceFactory musicServiceFactory, IDataWriterService dataWriter)
{
    private readonly IMusicService musicService = musicServiceFactory.GetService(MusicServiceType.Apple);

    public string GetDeveloperToken()
    {
        return musicService.DeveloperToken;
    }

    public async Task<IEnumerable<Song>> DoStuff(string userToken)
    {
        var library = await musicService.GetLibraryAsync(userToken);

        dataWriter.WriteSongs(library);

        return library;
    }
}
