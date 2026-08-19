# GameVault — Game Database Browser

Full-stack **game database browser** built on the **[RAWG API](https://rawg.io/apidocs)**.

Search and browse games, view details, and keep a personal **Wishlist / Playing / Completed** library.

## Quick Highlights

| Area          | Details                                                                 |
| ------------- | ------------------------------------------------------------------------ |
| Game Data     | Live from RAWG — no local sync, the API is a thin authenticated proxy   |
| Auth to RAWG  | Server-side API key; it never reaches the browser                       |
| Library       | Personal Wishlist / Playing / Completed list, persisted in Postgres     |
| Architecture  | Clean Architecture (Api / Services / Data / Infrastructure / Contracts) |
| Deployment    | Vercel (client) · Render (API) · Neon (Postgres)                        |

## Tech Stack

| Area     | Technologies                                              |
| -------- | ----------------------------------------------------------- |
| Client   | React (JavaScript) · Vite · Tailwind CSS v4 · React Router · Axios · lucide-react |
| API      | ASP.NET Core · .NET 8                                       |
| Data     | PostgreSQL (Neon) · EF Core · Mapster                        |
| External | RAWG API (games)                                              |
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
  GameVault.Infrastructure     RAWG HTTP client (GameVault.Infrastructure.ExternalApis.Rawg)
  GameVault.Contracts          Interfaces implemented by Data/Infrastructure
  GameVault.Common             Shared exceptions and constants
  *.Tests                      xUnit test projects
```

> [!NOTE]
> The API never exposes the RAWG API key to the browser. The client only ever calls
> `GameVault.Api`, which fetches from RAWG server-side.

## Main API Routes

| Method | Route                  | Description                                         |
| ------ | ----------------------- | ---------------------------------------------------- |
| GET    | `/api/health`            | Health check                                          |
| GET    | `/api/games/search`      | Search games by name (`query`, `limit`, `offset`)    |
| GET    | `/api/games/popular`     | Most-added games on RAWG (its own popularity signal)  |
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
> - [x] **A RAWG API key** (see below)

### 1. Get a RAWG API key

1. Go to [rawg.io/apidocs](https://rawg.io/apidocs) and click **GET API KEY**.
2. Sign up with just an email/password (no phone verification or 2FA required).
3. Your API key appears immediately on your [RAWG API account page](https://rawg.io/login?forward=developer).

The free tier covers roughly 20,000 requests/month, which is comfortably enough for a
personal project.

### 2. Clone and configure the API

```bash
git clone https://github.com/hristianivanov/IGDB.git
cd IGDB
```

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=gamevault;Username=postgres;Password=YOUR_PASSWORD" --project api/GameVault.Api

dotnet user-secrets set "Rawg:ApiKey" "your_rawg_api_key" --project api/GameVault.Api
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
| API    | [Render](https://render.com) | `render.yaml` (Docker-based web service) at the repo root; set `Rawg__ApiKey`, `ConnectionStrings__DefaultConnection`, and `Frontend__AllowedOrigins__0` as env vars in the Render dashboard |
| DB     | [Neon](https://neon.tech) | Free-tier Postgres; use its pooled connection string (`sslmode=require`) as `ConnectionStrings__DefaultConnection` |

`GitHub Actions` (`.github/workflows/workflow.yml`) builds and tests both projects on
every push/PR, then (on `main`) pings `RENDER_DEPLOY_HOOK_URL` and
`VERCEL_DEPLOY_HOOK_URL` repo secrets to trigger deploys.

## Give a Star ⭐

If you find this project useful, please consider giving it a star!
