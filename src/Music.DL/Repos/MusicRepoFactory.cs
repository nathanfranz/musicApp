using Microsoft.Extensions.DependencyInjection;
using Music.DL.Interfaces;
using Music.DL.Models;

namespace Music.DL.Repos;

internal class MusicRepoFactory(IServiceProvider provider) : IMusicRepoFactory
{
    public IMusicRepo GetService(MusicServiceType type)
    {
        return type switch
        {
            MusicServiceType.Apple => provider.GetRequiredService<AppleMusicRepo>(),
            //MusicServiceType.Spotify => provider.GetRequiredService<SpotifyMusicService>(),
            _ => throw new NotImplementedException()
        };
    }
}