# GameVault — Game Database Browser

Full-stack **game database browser** built on the **IGDB API** (Twitch/IGDB.com).

Search and browse games, view details, and keep a personal **Wishlist / Playing / Completed** library.

## Quick Highlights

| Area          | Details                                                                 |
| ------------- | ------------------------------------------------------------------------ |
| Game Data     | Live from IGDB — no local sync, the API is a thin authenticated proxy   |
| Auth to IGDB  | Server-side Twitch OAuth (client-credentials); the token never reaches the browser |
| Library       | Personal Wishlist / Playing / Completed list, persisted in Postgres     |
| Architecture  | Clean Architecture (Api / Services / Data / Infrastructure / Contracts) |
| Deployment    | Vercel (client) · Render (API) · Neon (Postgres)                        |

## Tech Stack

| Area     | Technologies                                              |
| -------- | ----------------------------------------------------------- |
| Client   | React (JavaScript) · Vite · Tailwind CSS v4 · React Router · Axios · lucide-react |
| API      | ASP.NET Core · .NET 8                                       |
| Data     | PostgreSQL (Neon) · EF Core · Mapster                        |
| External | IGDB API (games) · Twitch OAuth2 (client-credentials)        |
| Testing  | xUnit                                                        |
| Delivery | GitHub Actions · Vercel · Render                             |

## Project Architecture

```
client/                     React SPA (Vite)
  src/api/                  Axios wrappers — the only thing that talks to the backend
  src/context/               LibraryProvider — shared saved-games state
  src/pages/                  BrowsePage · GameDetailsPage · LibraryPage
  src/components/             GameCard · Navbar · StatusSelect

api/                         .NET 8 solution (GameVault.slnx)
  GameVault.Api               Controllers, Program.cs, appsettings
  GameVault.Services          GameService, LibraryService, Mapster config
  GameVault.Services.Models   DTOs shared between Api and Services
  GameVault.Data               EF Core DbContext, migrations, repositories
  GameVault.Data.Models        Persisted entities (SavedGame)
  GameVault.Infrastructure     IGDB HTTP client + Twitch token provider
  GameVault.Contracts          Interfaces implemented by Data/Infrastructure
  GameVault.Common             Shared exceptions and constants
  *.Tests                      xUnit test projects
```

> [!NOTE]
> The API never exposes IGDB/Twitch credentials to the browser. The client only ever
> calls `GameVault.Api`, which fetches from IGDB server-side and caches its Twitch
> access token in memory.

## Main API Routes

| Method | Route                  | Description                                         |
| ------ | ----------------------- | ---------------------------------------------------- |
| GET    | `/api/health`            | Health check                                          |
| GET    | `/api/games/search`      | Search games by name (`query`, `limit`, `offset`)    |
| GET    | `/api/games/popular`     | Highest-rated games with enough ratings to be notable |
| GET    | `/api/games/{id}`        | Full game details (summary, genres, platforms, screenshots, companies) |
| GET    | `/api/library`           | List your saved games                                 |
| POST   | `/api/library`           | Add a game to your library                             |
| PUT    | `/api/library/{id}`      | Change a saved game's status                           |
| DELETE | `/api/library/{id}`      | Remove a saved game                                    |

Swagger UI is available in Development at `http://localhost:8080/swagger`.

## Local Setup

> [!IMPORTANT]
> - [x] **.NET 8 SDK**
> - [x] **Node.js 20+**
> - [x] **PostgreSQL** running locally (or a [Neon](https://neon.tech) connection string)
> - [x] **A Twitch Developer app** (see below) for IGDB access

### 1. Register a Twitch Developer app (needed for IGDB)

IGDB's API is operated by Twitch and requires an app registration to get a Client ID
and Client Secret:

1. Go to the [Twitch Developer Console](https://dev.twitch.tv/console/apps) and log in
   with (or create) a Twitch account.
2. Click **Register Your Application**.
3. Fill in:
   - **Name**: anything unique, e.g. `GameVault Local`
   - **OAuth Redirect URLs**: `http://localhost` (not used for the client-credentials
     flow this project uses, but Twitch requires a value)
   - **Category**: `Application Integration`
4. Save, then open the app and click **New Secret** to generate a Client Secret.
5. Copy the **Client ID** and **Client Secret** — you'll need both below.

No further IGDB-specific signup is required; IGDB authenticates purely through this
Twitch app.

### 2. Clone and configure the API

```bash
git clone https://github.com/hristianivanov/IGDB.git
cd IGDB
```

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=gamevault;Username=postgres;Password=YOUR_PASSWORD" --project api/GameVault.Api

dotnet user-secrets set "Igdb:ClientId" "your_twitch_client_id" --project api/GameVault.Api
dotnet user-secrets set "Igdb:ClientSecret" "your_twitch_client_secret" --project api/GameVault.Api
```

### 3. Apply migrations and run the backend

```powershell
dotnet ef database update --project api/GameVault.Data --startup-project api/GameVault.Api
dotnet run --project api/GameVault.Api
```

### 4. Install client dependencies and start the dev server

```bash
cd client
npm install
npm run dev
```

The client reads its API URL from `client/.env` (`VITE_API_BASE_URL`, defaults to
`http://localhost:8080/api`).

## Running Tests

```bash
# API
cd api && dotnet restore && dotnet build --configuration Release && dotnet test
```

```bash
# Client
cd client && npm install && npm run lint && npm run build
```

## Deployment (free tier)

| Piece  | Service | Notes |
| ------ | ------- | ----- |
| Client | [Vercel](https://vercel.com) | `client/vercel.json` sets the build/output config and CSP headers |
| API    | [Render](https://render.com) | `render.yaml` (Docker-based web service) at the repo root; set `Igdb__ClientId`, `Igdb__ClientSecret`, `ConnectionStrings__DefaultConnection`, and `Frontend__AllowedOrigins__0` as env vars in the Render dashboard |
| DB     | [Neon](https://neon.tech) | Free-tier Postgres; use its pooled connection string (`sslmode=require`) as `ConnectionStrings__DefaultConnection` |

`GitHub Actions` (`.github/workflows/workflow.yml`) builds and tests both projects on
every push/PR, then (on `main`) pings `RENDER_DEPLOY_HOOK_URL` and
`VERCEL_DEPLOY_HOOK_URL` repo secrets to trigger deploys.

## Give a Star ⭐

If you find this project useful, please consider giving it a star!
