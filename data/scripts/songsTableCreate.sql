CREATE TABLE IF NOT EXISTS Songs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Artist TEXT,
    Album TEXT,
    Genre TEXT,
    DurationInSeconds INTEGER,
    AppleLastUpdated TEXT
);