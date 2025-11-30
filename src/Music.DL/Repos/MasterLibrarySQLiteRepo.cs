using Microsoft.Data.Sqlite;
using Music.DL.Interfaces;
using Music.DL.Models;

namespace Music.DL.Repos;

internal class MasterLibrarySQLiteRepo(string connectionString) : IMasterLibraryRepo
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    private SqliteConnection GetConnection() => new(_connectionString);

    // Single source of truth for columns
    private static readonly string[] Columns =
    [
        "Id", "Title", "Artist", "Album", "TrackNbr", "Genre", "DurationInSeconds", "AddedDate", "AppleUpdatedDate", "IsDeleted"
    ];

    public async Task<List<MasterLibraryEntry>> GetAllSongsAsync()
    {
        var songs = new List<MasterLibraryEntry>();
        var sql = $"SELECT {string.Join(", ", Columns)} FROM Songs";

        await using var connection = GetConnection();
        await connection.OpenAsync();
        await using var command = new SqliteCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        // Map column names to indexes
        var indexes = Columns.ToDictionary(c => c, c => reader.GetOrdinal(c));

        while (await reader.ReadAsync())
        {
            var song = new MasterLibraryEntry();

            foreach (var col in Columns)
            {
                var prop = typeof(MasterLibraryEntry).GetProperty(col);
                if (prop == null) continue;

                object? value = reader.IsDBNull(indexes[col])
                    ? null
                    : reader.GetValue(indexes[col]);

                // Handle nullable and non-nullable types
                if (value != null && prop.PropertyType != value.GetType())
                {
                    value = Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                prop.SetValue(song, value);
            }

            songs.Add(song);
        }

        return songs;
    }

    public async Task InsertSongsAsync(IEnumerable<MasterLibraryEntry> entries)
    {
        if (entries == null || !entries.Any()) return;

        var insertColumns = Columns.Where(c => c != "Id").ToArray();
        var sql = $"INSERT INTO Songs ({string.Join(", ", insertColumns)}) VALUES ({string.Join(", ", insertColumns.Select(c => "@" + c))})";

        await using var connection = GetConnection();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await using var command = new SqliteCommand(sql, connection, (SqliteTransaction?)transaction);

        foreach (var entry in entries)
        {
            command.Parameters.Clear();

            foreach (var col in insertColumns)
            {
                var prop = typeof(MasterLibraryEntry).GetProperty(col);
                if (prop == null) continue;

                var value = prop.GetValue(entry) ?? DBNull.Value;
                command.Parameters.AddWithValue("@" + col, value);
            }

            await command.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
    }

    public async Task<int> GetSongsCountAsync()
    {
        var sql = "SELECT COUNT(*) FROM Songs";

        await using var connection = GetConnection();
        await connection.OpenAsync();

        await using var command = new SqliteCommand(sql, connection);
        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
}
