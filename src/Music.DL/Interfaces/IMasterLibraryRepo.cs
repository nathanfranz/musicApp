using Music.DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.DL.Interfaces;

public interface IMasterLibraryRepo
{
    Task<List<MasterLibraryEntry>> GetAllSongsAsync();
    Task InsertSongsAsync(IEnumerable<MasterLibraryEntry> entries);
    Task<int> GetSongsCountAsync();
}
