# AuthSystem - Full-Stack Authentication System

A production-ready full-stack authentication system built with ASP.NET Core 8 (Clean Architecture) and React + Vite.

## Architecture

### Backend (Clean Architecture)
- **AuthSystem.Domain** - Entities and repository interfaces
- **AuthSystem.Application** - DTOs, validators (FluentValidation), service interfaces and implementations
- **AuthSystem.Infrastructure** - EF Core + PostgreSQL, JWT token service, BCrypt password service
- **AuthSystem.API** - ASP.NET Core Web API with controllers, middleware, Swagger
- **AuthSystem.UnitTests** - xUnit unit tests with Moq
- **AuthSystem.IntegrationTests** - xUnit integration tests with WebApplicationFactory

### Frontend (React + Vite)
- React Router DOM for client-side routing
- Axios with interceptors for API communication
- JWT token management in localStorage
- Context API for auth state management

## API Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| POST | /api/auth/register | No | Register a new user |
| POST | /api/auth/login | No | Login and get JWT token |
| GET | /api/auth/me | Yes (Bearer) | Get current user details |
| GET | /health | No | Health check |

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 20+
- PostgreSQL 16 (or Docker)

### Run with Docker Compose
```bash
docker-compose up --build
```

### Run Backend locally
```bash
cd backend
dotnet restore
dotnet run --project AuthSystem.API
```
API available at http://localhost:5000, Swagger at http://localhost:5000/swagger

### Run Frontend locally
```bash
cd frontend
npm install
npm run dev
```
Frontend available at http://localhost:5173

### Run Tests
```bash
cd backend
dotnet test
```

## Configuration

### Backend (appsettings.json)
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection string
- `JwtSettings:Secret` - JWT signing secret (min 32 chars)
- `JwtSettings:Issuer` - JWT issuer
- `JwtSettings:Audience` - JWT audience
- `JwtSettings:ExpirationMinutes` - Token expiry (default: 60)

### Frontend (.env)
- `VITE_API_URL` - Backend API base URL (default: http://localhost:5000)
