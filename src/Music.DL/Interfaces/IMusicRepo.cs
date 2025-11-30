using Music.DL.Models;

namespace Music.DL.Interfaces;

public interface IMusicRepo
{
    Task<IEnumerable<LibrarySong>> GetLibraryAsync(string userToken, string developerToken);
}
