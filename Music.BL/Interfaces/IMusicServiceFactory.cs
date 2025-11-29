using Music.DL.Models;

namespace Music.BL.Interfaces;

public interface IMusicServiceFactory
{
    IMusicService GetService(MusicServiceType type);
}