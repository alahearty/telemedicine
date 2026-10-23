-- Telemedicine database setup (run with psql as a PostgreSQL superuser).
CREATE DATABASE telemedicine;
CREATE DATABASE telemedicine_test;

-- The API applies EF Core migrations and seeds the roles/admin automatically.
-- Development connection:
--   Host=localhost;Database=telemedicine;Username=postgres;Password=worisoft@123
-- Change the password before any deployment.
