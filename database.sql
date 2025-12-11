CREATE TABLE IF NOT EXISTS Products (
    Id TEXT PRIMARY KEY,
    Code TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    Category TEXT,
    Type INTEGER NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Stock INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Transactions (
    Id TEXT PRIMARY KEY,
    ProductCode TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    Type INTEGER NOT NULL,
    PerformedBy TEXT,
    PerformedAt TEXT NOT NULL,
    Notes TEXT,
    FOREIGN KEY (ProductCode) REFERENCES Products(Code)
);
