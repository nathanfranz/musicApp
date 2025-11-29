using Microsoft.Extensions.DependencyInjection;
using Music.BL.Interfaces;
using Music.DL.Models;

namespace Music.BL.Services;

internal class MusicServiceFactory(IServiceProvider provider) : IMusicServiceFactory
{
    public IMusicService GetService(MusicServiceType type)
    {
        return type switch
        {
            MusicServiceType.Apple => provider.GetRequiredService<AppleMusicService>(),
            //MusicServiceType.Spotify => provider.GetRequiredService<SpotifyMusicService>(),
            _ => throw new NotImplementedException()
        };
    }
}