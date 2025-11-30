namespace Music.DL.Models
{
    public class Song
    {
        public string Title { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public string? Album { get; set; }
        public int? TrackNbr { get; set; }
        public string? Genre { get; set; }
        public int DurationInSeconds { get; set; }
    }
}
