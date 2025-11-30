using Microsoft.Data.Sqlite;
using Music.DL.Models;
using Music.DL.Repos;

namespace DL.Tests.Integration.Database;

[TestFixture, Category("Integration")]
public class MasterLibrarySQLiteRepoTests
{
    private string _testDbPath;
    private string _connectionString;
    private MasterLibrarySQLiteRepo _repo;

    [SetUp]
    public void Setup()
    {
        var testsDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "..", "..", "..", "..");
        _testDbPath = Path.Combine(testsDir, "data", "masterLibrary_test.db");
        _testDbPath = Path.GetFullPath(_testDbPath);

        _connectionString = $"Data Source={_testDbPath}";

        _repo = new MasterLibrarySQLiteRepo(_connectionString);
    }

    //[TearDown]
    public void TearDown()
    {
        // Optionally clean up all rows after each test
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var cmd = new SqliteCommand("DELETE FROM Songs;", connection);
        cmd.ExecuteNonQuery();
    }

    [Test]
    public async Task InsertSongsAsync_ShouldInsertSongs()
    {
        // Arrange
        var songs = new List<MasterLibraryEntry>
        {
            new() {
                Title = "Song 1",
                Artist = "Artist 1",
                Album = "Album 1",
                Genre = "Pop",
                TrackNbr = 1,
                DurationInSeconds = 180,
                AddedDate = DateTime.UtcNow,
                AppleUpdatedDate = null,
                IsDeleted = false
            },
            new() {
                Title = "Song 2",
                Artist = "Artist 2",
                Album = "Album 2",
                Genre = "Rock",
                TrackNbr = 13,
                DurationInSeconds = 210,
                AddedDate = DateTime.UtcNow,
                AppleUpdatedDate = null,
                IsDeleted = false
            }
        };

        // Act
        var allSongsBeforeCount = await _repo.GetSongsCountAsync();

        await _repo.InsertSongsAsync(songs);
        var allSongs = await _repo.GetAllSongsAsync();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(allSongs, Has.Count.EqualTo(allSongsBeforeCount + 2));
            Assert.That(allSongs[0].Title, Is.EqualTo("Song 1"));
            Assert.That(allSongs[1].Title, Is.EqualTo("Song 2"));
        }
    }
}