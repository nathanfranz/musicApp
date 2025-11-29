using Music.DL.Models;

namespace Music.DL.Interfaces;

public interface IMusicRepoFactory
{
    IMusicRepo GetService(MusicServiceType type);
}