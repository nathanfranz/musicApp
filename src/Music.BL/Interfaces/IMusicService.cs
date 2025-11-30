using Music.DL.Models;

namespace Music.BL.Interfaces;

public interface IMusicService
{
    string DeveloperToken { get; set; }

    Task<IEnumerable<Song>> GetLibraryAsync(string userToken);
}
