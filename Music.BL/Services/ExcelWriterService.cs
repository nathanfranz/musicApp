using System.IO;
using Music.BL.Interfaces;
using Music.DL.Models;
using OfficeOpenXml;

namespace Music.BL.Services
{
    internal class ExcelWriterService : IDataWriterService
    {
        public void WriteSongs(IEnumerable<LibrarySong> songs)
        {
            string folderPath = @"D:\Documents\MusicLibrary";

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"appleLibrary_{timestamp}.xlsx";

            string filePath = Path.Combine(folderPath, fileName);

            using var stream = CreateExcelStream(songs);
            using var fileStream = File.Create(filePath);
            stream.CopyTo(fileStream);
        }

        public static MemoryStream CreateExcelStream(IEnumerable<LibrarySong> songs)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Nate");

            var stream = new MemoryStream();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("LibrarySongs");

                // Headers
                ws.Cells[1, 1].Value = "Name";
                ws.Cells[1, 2].Value = "Artist";
                ws.Cells[1, 3].Value = "Album";

                int row = 2;
                foreach (var song in songs)
                {
                    ws.Cells[row, 1].Value = song.Name;
                    ws.Cells[row, 2].Value = song.Artist;
                    ws.Cells[row, 3].Value = song.Album;
                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                package.SaveAs(stream);
            }

            stream.Position = 0;
            return stream;
        }
    }
}
