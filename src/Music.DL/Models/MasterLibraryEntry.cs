using System;
using System.Collections.Generic;
using System.Text;

namespace Music.DL.Models;

public class MasterLibraryEntry : Song
{
    public int Id { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime? AppleUpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false;
}
