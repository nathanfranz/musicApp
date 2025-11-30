using Music.DL.Models;

namespace Music.DL.Interfaces;

public interface IMusicRepo
{
    Task<IEnumerable<Song>> GetLibraryAsync(string userToken, string developerToken);
}
