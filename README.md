# LibraryService
C# API microservice for BookWorm application that manages users' libraries.

## Features
- Manage personal library (add, remove books and view book collections)
- Track reading status (Want to Read, Currently Reading, Read)
- Filter books by status
- Pagination support for library listings
- User authentication and authorization
- RESTful API for external integrations

## Tech Stack
- **Backend**: ASP.NET Core
- **Database**: PostgreSQL
- **Authentication**: JSON Web Tokens (JWT)
- **Documentation**: Swagger
- **Other**: Docker, Entity Framework Core

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Docker (optional, for containerized setup)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/LibraryService.git
   cd LibraryService
   ```

2. Set up the database:
   - Update the connection string in `appsettings.json` to match your PostgreSQL setup.

3. Run the application:
   ```bash
   dotnet run
   ```

4. Access the application:
   - API: `http://localhost:<port>`
   - Swagger UI: `http://localhost:<port>/swagger/index.html`

### Running Tests
```bash
dotnet test
```

### Docker Setup (Optional)
1. Build the Docker image:
   ```bash
   docker build -t libraryservice .
   ```

2. Run the container:
   ```bash
   docker run -p <port>:<port> libraryservice
   ```

3. Access the application at `http://localhost:<port>`.

## API Endpoints

### User Endpoints (Authenticated)
- `GET /library`: Retrieve books from the user's library with optional pagination and status filtering.
- `POST /library`: Add a book to the user's library with a specified status.
- `DELETE /library/{bookId}`: Remove a book from the user's library.

## Dependencies
- **BookService**: Used to verify book existence when adding to library, returns information about the books when GET method is called.
