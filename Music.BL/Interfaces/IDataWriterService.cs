using Music.DL.Models;

namespace Music.BL.Interfaces
{
    internal interface IDataWriterService
    {
        void WriteSongs(IEnumerable<LibrarySong> songs);
    }
}
