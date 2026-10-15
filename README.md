# dbeb — certificate management web service

Full-stack web service for generating and moderating certificates ("gramota"):
users upload document scans, moderators review them (with built-in image
rotation tooling), approved documents produce score based on document type,
and a ranking table is built from user scores.

Stack: **ASP.NET Core, Blazor Server, EF Core + MariaDB (Pomelo), SkiaSharp, .NET 9**

## Architecture

| Project | Role |
|---|---|
| `dserver` | REST API: controllers, EF Core + MariaDB, auth engine, background scoring |
| `DiplomBeb` | Blazor Server frontend: service layer, role-based UI, moderation pages |
| `dshared` | Shared utilities (`ListElement` data container) |

Notable design points:

- **Background scoring** — score recalculation is offloaded from the request
  path into a `Channel<T>` + `BackgroundService` producer-consumer pipeline.
- **Moderation tooling** — document photos can be rotated server-side during
  review (SkiaSharp, canvas recalculation for arbitrary angles).
- **Role model** — separate roles for the API (`ApiRole`) and the web UI
  (`WebRole`), token auth enforced by a custom `AuthFilter` attribute.

## Build & run

1. .NET 9 SDK and MariaDB
2. Set the connection string in `dserver/appsettings.json`
3. Run `dserver` (API), then `DiplomBeb` (web UI)

## Known issues / what I'd do differently now

- Auth uses client-side unsalted SHA-256 — the hash effectively becomes the
  password (pass-the-hash), and token expiry is stored but not enforced.
  Today: server-side salted BCrypt/Argon2 + signed JWT with `exp`.
- EF Core query logging left enabled (debug leftover).
- Score recalculation materializes rows in memory instead of a SQL-side `SUM`.
- A few endpoints return 500 instead of 400 on malformed input.
