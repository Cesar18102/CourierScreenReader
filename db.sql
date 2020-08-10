CREATE TABLE orders(
    id INTEGER PRIMARY KEY IDENTITY(1, 1),
    account_id INTEGER NOT NULL,
    from_a NVARCHAR(1024) NOT NULL,
    to_b NVARCHAR(1024) NOT NULL,
	init_date_time DATETIME NOT NULL,
    gain FLOAT NOT NULL,
    paid BIT NOT NULL DEFAULT 0,
    FOREIGN KEY(account_id) REFERENCES account(id)
);

CREATE TABLE pwd_changes(
    id INTEGER PRIMARY KEY IDENTITY(1, 1),
    account_id INTEGER NOT NULL,
    change_date_time DATETIME NOT NULL,
    new_value NVARCHAR(512),
    FOREIGN KEY(account_id) REFERENCES account(id)
);