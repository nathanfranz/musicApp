-- Drop the table if it exists
DROP TABLE IF EXISTS Songs;

-- Create the table from scratch
CREATE TABLE Songs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Artist TEXT NOT NULL,
    Album TEXT,
    TrackNbr INTEGER,
    Genre TEXT,
    DurationInSeconds INTEGER NOT NULL,
    AddedDate TEXT NOT NULL,
    AppleUpdatedDate TEXT,
    IsDeleted INTEGER NOT NULL DEFAULT 0
);