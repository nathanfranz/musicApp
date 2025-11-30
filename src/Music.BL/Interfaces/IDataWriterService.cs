using Music.DL.Models;

namespace Music.BL.Interfaces
{
    public interface IDataWriterService
    {
        void WriteSongs(IEnumerable<Song> songs);
    }
}
