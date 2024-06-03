#!/bin/bash
set -e

# Wait for the database to be available
until dotnet ef database update; do
  >&2 echo "Postgres is unavailable - sleeping"
  sleep 1
done

# Run the migrations
dotnet ef migrations add dockerFix --project iuca.Infrastructure --startup-project iuca.Web/iuca.Web.csproj
dotnet ef database update --startup-project iuca.Web/iuca.Web.csproj

# Start the application
exec "$@"
