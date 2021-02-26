CREATE DATABASE budget WITH ENCODING UTF8;

CREATE USER budget_user WITH PASSWORD 'budget_user';
GRANT CONNECT ON DATABASE budget TO budget_user;

\c budget

CREATE SCHEMA users;

CREATE TABLE users.users (
    id UUID PRIMARY KEY,
    username VARCHAR(25) NOT NULL,
    firstname VARCHAR(50) NOT NULL,
    lastname VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL
);

ALTER TABLE users.users ADD CONSTRAINT unique_username UNIQUE(username);

GRANT USAGE ON SCHEMA users TO budget_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA users TO budget_user;