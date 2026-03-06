CREATE TABLE IF NOT EXISTS users (
                                     id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    first_name TEXT NOT NULL,
    second_name TEXT NOT NULL,
    birthdate DATE NOT NULL,
    gender TEXT,
    biography TEXT,
    city TEXT,
    password_hash TEXT NOT NULL
    );
