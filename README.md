# 🎬 MovieTracker API

> A production-ready RESTful API for tracking movies and TV series with TMDB integration, built with .NET 10 and Clean Architecture.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=flat&logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat&logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Quick Start](#-quick-start)
- [API Endpoints Summary](#-api-endpoints-summary)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Database Schema](#-database-schema)
- [Full API Documentation](#-full-api-documentation)
- [Smart Episode Marking](#-smart-episode-marking)
- [Postman Collection](#-postman-collection)
- [Configuration](#-configuration)
- [Deployment](#-deployment)

---

## 🎯 Overview

**MovieTracker API** is a comprehensive backend solution for managing personal movie and TV series watchlists with intelligent tracking features.

### Key Highlights

✨ **Smart Episode Tracking** - Automatically marks previous episodes as watched  
🔐 **Secure Authentication** - JWT with refresh token mechanism  
🎬 **TMDB Integration** - Automatic metadata fetching (62 episodes in seconds!)  
🗑️ **Soft Delete** - Remove and restore movies/series  
🏗️ **Clean Architecture** - Production-ready, scalable, maintainable  
📦 **Docker Ready** - One-command SQL Server setup  

---

## ✨ Features

### 🔐 Authentication
- JWT access tokens (15 min expiry)
- Refresh tokens (7 days, one-time use with auto-revocation)
- BCrypt password hashing
- Role-based authorization

### 🎬 Movie Management
- Search & add from TMDB
- Mark as watched/unwatched
- Rate (1-10 scale)
- Personal notes
- **Soft delete with restore**

### 📺 Series Management
- Auto-fetch ALL episodes (Breaking Bad = 62 episodes in 2 seconds!)
- **Smart Episode Marking** - Mark S02E05 → auto-marks S01 + S02E01-E04
- Episode-level progress tracking
- Rate & notes
- **Soft delete with restore**

### 🔍 Search
- Real-time TMDB movie search
- Real-time TMDB series search
- Rich metadata (posters, descriptions, dates)

---

## 🚀 Quick Start

### 1. Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)


### 2. Clone & Setup
```bash
git clone https://github.com/yourusername/MovieTracker.git
cd MovieTracker
docker-compose up -d
```

### 3. Configure
Edit `src/MovieTracker.API/appsettings.json`:
```json
{
  "Jwt": {
    "Secret": "your-32-char-secret-here",
    "Issuer": "MovieTrackerAPI",
    "Audience": "MovieTrackerClient"
  },
  "Tmdb": {
    "ApiKey": "your-tmdb-api-key"
  }
}
```

### 4. Run
```bash
dotnet ef database update --project src/MovieTracker.Infrastructure --startup-project src/MovieTracker.API
dotnet run --project src/MovieTracker.API
```

**API**: `http://localhost:5076`  


---

## 📊 API Endpoints Summary

| Category | Endpoint | Method | Description |
|----------|----------|--------|-------------|
| **Auth** | `/api/Auth/register` | POST | Create account |
| | `/api/Auth/login` | POST | Get tokens |
| | `/api/Auth/refresh` | POST | Refresh access token |
| **Search** | `/api/Search/movies?query=` | GET | Search TMDB movies |
| | `/api/Search/series?query=` | GET | Search TMDB series |
| **Movies** | `/api/Movies` | POST | Add movie to watchlist |
| | `/api/Movies` | GET | Get my movies |
| | `/api/Movies/{id}/watched` | PUT | Mark watched/unwatched |
| | `/api/Movies/{id}/rating` | PUT | Rate movie (1-10) |
| | `/api/Movies/{id}/notes` | PUT | Add/update notes |
| | `/api/Movies/{id}` | DELETE | **Soft delete movie** |
| **Series** | `/api/Series` | POST | Add series + all episodes |
| | `/api/Series` | GET | Get my series |
| | `/api/Series/{id}/episodes` | GET | Get episodes with progress |
| | `/api/Series/episodes/{id}/watched` | PUT | Smart mark episode |
| | `/api/Series/{id}/rating` | PUT | Rate series (1-10) |
| | `/api/Series/{id}/notes` | PUT | Add/update notes |
| | `/api/Series/{id}` | DELETE | **Soft delete series** |

**Total**: 16 endpoints

---

## 🛠 Tech Stack

**Backend**: .NET 10, ASP.NET Core Web API, C# 13  
**Architecture**: Clean Architecture (4 layers), CQRS, MediatR  
**Database**: SQL Server 2022, Entity Framework Core 10  
**Auth**: JWT, Refresh Tokens, BCrypt  
**External**: TMDB API v3, HttpClient  
**Logging**: Serilog  
**Tools**: Docker, EF Migrations, OpenAPI  

---

## 🏗 Architecture

```
MovieTracker/
├── Domain/              # Entities, Enums (pure business logic)
├── Application/         # Commands, Queries, DTOs, Interfaces
├── Infrastructure/      # DbContext, Services (TMDB, Tokens)
└── API/                 # Controllers, Startup, Configuration
```

**Dependencies**:
```
API → Application → Domain
API → Infrastructure → Application
```

**Patterns**: CQRS, MediatR, Repository, Dependency Injection

---

## 💾 Database Schema

```
User
├── Movies (1:N)
├── Series (1:N)
│   └── Episodes (1:N)
└── RefreshTokens (1:N)
```

**Entities**:
- **User**: Id, Username, Email, PasswordHash, Role
- **Movie**: Id, TmdbId, Title, IsWatched, Rating, Notes, **IsDeleted**
- **Series**: Id, TmdbId, Title, NumberOfSeasons, NumberOfEpisodes, **IsDeleted**
- **Episode**: Id, SeriesId, SeasonNumber, EpisodeNumber, IsWatched
- **RefreshToken**: Id, UserId, Token, ExpiresAt, RevokedAt

**Soft Deletes**: Movies & Series use `IsDeleted` flag  
**Restore Logic**: Re-adding a soft-deleted item restores it  
**Cascading Deletes**: User → all related entities  

---

## 📚 Full API Documentation

### Response Format

**Success**:
```json
{
  "success": true,
  "data": {...},
  "message": "Optional message",
  "errors": null
}
```

**Error**:
```json
{
  "success": false,
  "data": null,
  "message": "Error description",
  "errors": null
}
```

---

### 🔐 Authentication

#### 1. Register
**POST** `/api/Auth/register`

```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response**:
```json
{
  "success": true,
  "data": {
    "userId": "uuid",
    "username": "johndoe",
    "email": "john@example.com",
    "accessToken": "eyJ...",
    "refreshToken": "dGh..."
  },
  "message": "User registered successfully"
}
```

---

#### 2. Login
**POST** `/api/Auth/login`

```json
{
  "username": "johndoe",
  "password": "SecurePass123!"
}
```

**Response**: Same as Register

**Token Expiry**:
- Access Token: **15 minutes**
- Refresh Token: **7 days**

---

#### 3. Refresh Token
**POST** `/api/Auth/refresh`

```json
{
  "refreshToken": "dGh..."
}
```

**Response**:
```json
{
  "success": true,
  "data": {
    "userId": "uuid",
    "username": "johndoe",
    "email": "john@example.com",
    "accessToken": "NEW_TOKEN",
    "refreshToken": "NEW_REFRESH_TOKEN"
  },
  "message": "Tokens refreshed successfully"
}
```

**Important**:
- ✅ Old refresh token is **revoked** (one-time use)
- ✅ New tokens issued
- ❌ Reusing old token returns error

---

### 🔍 Search

#### Search Movies
**GET** `/api/Search/movies?query=inception`

**Headers**: `Authorization: Bearer {accessToken}`

**Response**:
```json
{
  "success": true,
  "data": [
    {
      "id": 27205,
      "title": "Inception",
      "overview": "Cobb, a skilled thief...",
      "posterPath": "/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
      "releaseDate": "2010-07-16",
      "year": 2010
    }
  ]
}
```

**Image URLs**: `https://image.tmdb.org/t/p/w500{posterPath}`

---

#### Search Series
**GET** `/api/Search/series?query=breaking+bad`

**Headers**: `Authorization: Bearer {accessToken}`

**Response**: Similar to movies

---

### 🎬 Movies

#### Add Movie
**POST** `/api/Movies`

```json
{
  "tmdbId": 27205
}
```

**Response**:
```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "tmdbId": 27205,
    "title": "Inception",
    "overview": "...",
    "posterUrl": "https://...",
    "releaseYear": 2010,
    "isWatched": false,
    "rating": null,
    "notes": null,
    "addedAt": "2026-01-04T00:00:00Z"
  },
  "message": "Movie added to your list"
}
```

---

#### Get My Movies
**GET** `/api/Movies?isWatched=true`

**Query Params** (optional):
- `isWatched=true` - Only watched
- `isWatched=false` - Only unwatched
- Omit - All movies (excludes soft-deleted)

---

#### Mark as Watched
**PUT** `/api/Movies/{movieId}/watched`

```json
{
  "isWatched": true
}
```

---

#### Rate Movie
**PUT** `/api/Movies/{movieId}/rating`

```json
{
  "rating": 9.5
}
```

**Validation**: 1.0 - 10.0

---

#### Add Notes
**PUT** `/api/Movies/{movieId}/notes`

```json
{
  "notes": "Amazing plot!"
}
```

**Validation**: Max 1000 characters

---

#### Delete Movie (Soft Delete) ⭐
**DELETE** `/api/Movies/{movieId}`

**Headers**: `Authorization: Bearer {accessToken}`

**Response**:
```json
{
  "success": true,
  "data": true,
  "message": "Movie removed from your list"
}
```

**What happens**:
- ✅ Sets `IsDeleted = true`, `DeletedAt = DateTime.UtcNow`
- ✅ Movie won't appear in `GET /api/Movies`
- ✅ Re-adding same movie (same tmdbId) will **restore** it automatically

**Error Response** (404):
```json
{
  "success": false,
  "data": null,
  "message": "Movie not found"
}
```

---

### 📺 Series

#### Add Series
**POST** `/api/Series`

```json
{
  "tmdbId": 1396
}
```

**Response**:
```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "tmdbId": 1396,
    "title": "Breaking Bad",
    "numberOfSeasons": 5,
    "numberOfEpisodes": 62,
    ...
  },
  "message": "Series added with 62 episodes"
}
```

---

#### Get My Series
**GET** `/api/Series`

**Response**: Array of series (excludes soft-deleted)

---

#### Get Series Episodes
**GET** `/api/Series/{seriesId}/episodes`

**Response**:
```json
{
  "success": true,
  "data": {
    "seriesId": "uuid",
    "title": "Breaking Bad",
    "totalEpisodes": 62,
    "watchedEpisodes": 12,
    "episodes": [...]
  }
}
```

---

#### Mark Episode as Watched (Smart)
**PUT** `/api/Series/episodes/{episodeId}/watched`

```json
{
  "isWatched": true
}
```

**Response**:
```json
{
  "success": true,
  "data": true,
  "message": "Marked 12 episodes as watched (including previous episodes)"
}
```

See [Smart Episode Marking](#-smart-episode-marking) for details.

---

#### Rate Series
**PUT** `/api/Series/{seriesId}/rating`

```json
{
  "rating": 9.5
}
```

---

#### Add Notes to Series
**PUT** `/api/Series/{seriesId}/notes`

```json
{
  "notes": "Best series ever!"
}
```

---

#### Delete Series (Soft Delete) ⭐
**DELETE** `/api/Series/{seriesId}`

**Headers**: `Authorization: Bearer {accessToken}`

**Response**:
```json
{
  "success": true,
  "data": true,
  "message": "Series removed from your list"
}
```

**What happens**:
- ✅ Sets `IsDeleted = true`, `DeletedAt = DateTime.UtcNow`
- ✅ Series + all episodes won't appear in queries
- ✅ Re-adding same series (same tmdbId) will **restore** it with all episodes

**Error Response** (404):
```json
{
  "success": false,
  "data": null,
  "message": "Series not found"
}
```

---

## 🧠 Smart Episode Marking

### The Problem
Manually marking 12+ episodes when binge-watching is tedious.

### The Solution

#### Marking as Watched
```
User marks: S02E05
System automatically marks:
├── S01E01 ✅
├── S01E02 ✅
├── S01E03 ✅
├── S01E04 ✅
├── S01E05 ✅
├── S01E06 ✅
├── S01E07 ✅
├── S02E01 ✅
├── S02E02 ✅
├── S02E03 ✅
├── S02E04 ✅
└── S02E05 ✅
Total: 12 episodes
```

#### Unmarking
```
User unmarks: S01E05
System unmarks:
└── S01E05 ❌ (only this one!)

Still watched:
├── S01E01-E04 ✅
├── S01E06-E07 ✅
└── S02E01-05 ✅
```

---

## 🗑️ Soft Delete & Restore

### How It Works

**Soft Delete**:
- Movie/Series not physically deleted from database
- `IsDeleted = true`, `DeletedAt = timestamp`
- Excluded from all GET queries automatically (via EF Core query filters)

**Restore**:
```bash
# User accidentally deleted Breaking Bad
DELETE /api/Series/{seriesId}
# Response: "Series removed from your list"

# User wants it back - just re-add it!
POST /api/Series
{
  "tmdbId": 1396
}
# Response: "Series restored to your list with 62 episodes"
```

**Benefits**:
- ✅ Undo accidental deletes
- ✅ Preserves watch history, ratings, notes
- ✅ No need to re-mark episodes as watched
- ✅ Data never truly lost

---

## 📮 Postman Collection

A complete Postman collection is included in the repository with all 16 endpoints pre-configured.

### Features
✅ **Auto-saves tokens** after login/register/refresh  
✅ **Auto-saves IDs** (movieId, seriesId, episodeId)  
✅ **All 16 endpoints** with example requests  
✅ **Test scripts** for automation  
✅ **Organized folders** (Auth, Search, Movies, Series)  

### Import to Postman
1. Open Postman
2. Click **Import**
3. Select `MovieTracker.postman_collection.json` from the repository
4. Collection ready to use!

### Collection Variables (Auto-populated)
- `baseUrl` - API base URL (`http://localhost:5076/api`)
- `accessToken` - Automatically updated on login/refresh
- `refreshToken` - Automatically updated on login/refresh
- `movieId` - Auto-saved when adding a movie
- `seriesId` - Auto-saved when adding a series
- `episodeId` - Auto-saved when fetching episodes

### Quick Test Workflow
1. Run **Register** or **Login** → Tokens auto-saved
2. Run **Search Movies** → Find tmdbId
3. Run **Add Movie** → movieId auto-saved
4. Run **Mark as Watched** → Uses saved movieId
5. Run **Rate Movie** → Uses saved movieId
6. Run **Delete Movie** → Soft deletes movie
7. Run **Add Movie** again → Restores deleted movie!

---

## ⚙️ Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MovieTrackerDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "minimum-32-characters-secret-key-here",
    "Issuer": "MovieTrackerAPI",
    "Audience": "MovieTrackerClient"
  },
  "Tmdb": {
    "ApiKey": "your-tmdb-api-key"
  }
}
```

### Docker Compose
```yaml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
volumes:
  sqlserver-data:
```

**Commands**:
```bash
docker-compose up -d      # Start
docker-compose down       # Stop
docker-compose logs -f    # View logs
```

---

## 🚀 Deployment

### Publish
```bash
dotnet publish -c Release -o ./publish
```

### Run
```bash
dotnet MovieTracker.API.dll
```

### Docker (Recommended)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY publish/ .
ENTRYPOINT ["dotnet", "MovieTracker.API.dll"]
```

```bash
docker build -t movietracker-api .
docker run -p 5076:5076 movietracker-api
```

---

## 📊 Project Stats

- **Endpoints**: 16 total
- **Entities**: 5 (User, Movie, Series, Episode, RefreshToken)
- **Architecture**: 4-layer Clean Architecture
- **Patterns**: CQRS, MediatR, Repository, DI
- **Lines of Code**: ~4000+
- **Development Time**: 5+ hours

---

## 🗺️ Roadmap

- [ ] User profiles & avatars
- [ ] Social features (friends, sharing)
- [ ] Recommendations engine
- [ ] Mobile app
- [ ] Streaming platform integration
- [ ] Statistics dashboard
- [ ] Export data (CSV/JSON)
- [ ] Hard delete option (permanent)

---


## 🙏 Credits

- [TMDB API](https://www.themoviedb.org/)
- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)

---

**Built with ❤️ using .NET 10 and Clean Architecture**
