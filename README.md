# Babbly Like Service

The like/reaction management microservice for the Babbly platform, providing RESTful API endpoints for managing likes on posts.

## Tech Stack

- **Backend**: ASP.NET Core 9.0
- **Database**: Apache Cassandra
- **Message Broker**: Kafka (for event publishing)
- **API Documentation**: Swagger/OpenAPI

## Features

- Add and remove likes from posts
- Get like counts for posts
- Get users who liked a post
- Get posts liked by a user
- Check if a user has liked a specific post
- Event publishing to Kafka for like operations

## Local Development Setup

### Prerequisites

- .NET SDK 9.0 or later
- Docker and Docker Compose
- Apache Cassandra

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/babbly-like-service.git
   cd babbly-like-service
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Configure Cassandra connection:
   ```bash
   # Using user secrets for development
   dotnet user-secrets set "CassandraHosts" "localhost"
   dotnet user-secrets set "CassandraKeyspace" "babbly_likes"
   dotnet user-secrets set "CassandraUsername" "cassandra"
   dotnet user-secrets set "CassandraPassword" "cassandra"
   ```

4. Run the service:
   ```bash
   dotnet run --project babbly-like-service/babbly-like-service.csproj
   ```

The API will be available at `http://localhost:8083`.

### Database Initialization

The Cassandra keyspace and tables are automatically created on startup.

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `CassandraHosts` | Comma-separated Cassandra hosts | `localhost` |
| `CassandraKeyspace` | Keyspace name | `babbly_likes` |
| `CassandraUsername` | Database username | `cassandra` |
| `CassandraPassword` | Database password | `cassandra` |

## API Endpoints

### Like Operations
- `GET /api/likes/{id}` - Get a specific like by ID
- `GET /api/likes/post/{postId}` - Get all likes for a post
- `GET /api/likes/user/{userId}` - Get all likes by a user
- `GET /api/likes/post/{postId}/count` - Get like count for a post
- `GET /api/likes/post/{postId}/users` - Get users who liked a post
- `GET /api/likes/user/{userId}/posts` - Get posts liked by a user
- `GET /api/likes/check?userId={userId}&postId={postId}` - Check if user liked a post
- `POST /api/likes` - Add a like
- `POST /api/likes/unlike` - Remove a like
- `DELETE /api/likes/{id}` - Delete a like by ID

### Health Check
- `GET /api/health` - Service health check

## Database Schema

### Likes Table
```cql
CREATE TABLE likes (
    id uuid PRIMARY KEY,
    post_id uuid,
    user_id text,
    created_at timestamp,
    like_type text
);
```

### Indices
- `post_id` index for querying likes by post
- `user_id` index for querying likes by user
- `(post_id, user_id)` compound index for checking if a user liked a post

## Docker Support

Run the service with Docker Compose:

```bash
# From the root of the Babbly organization
docker-compose up -d like-service
```

Or run with its own Docker Compose (includes Cassandra):

```bash
# From the babbly-like-service directory
docker-compose up -d
```

The service will be available at `http://localhost:8083`.

## Architecture Notes

### Why Cassandra?

Cassandra was chosen for the Like Service because:
- **High write throughput**: Perfect for frequent like/unlike operations
- **Fast lookups**: Efficient queries for like counts and user-post associations
- **Horizontal scalability**: Easy to handle viral content with massive like counts
- **Distributed by design**: Reliable under heavy load

### Kafka Integration

The service publishes events to the `like-events` Kafka topic:
- `LikeAdded` - When a user likes a post
- `LikeRemoved` - When a user unlikes a post

These events can be consumed by other services for real-time notifications or analytics.

### Integration with Babbly Ecosystem

- **API Gateway**: Routes like-related requests to this service
- **Post Service**: Likes are associated with posts via post ID
- **User Service**: Likes are associated with users via user ID
- **Frontend**: Displays like counts and states through the API Gateway

## Testing

API tests are available in `api-tests.http` for use with REST Client extensions.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
