# Babbly Like Service

## Overview

This is the like service for the Babbly social media platform. It's built with ASP.NET Core, providing RESTful API endpoints for managing post likes.

## Tech Stack

* **Backend**: ASP.NET Core 9.0
* **Database**: Apache Cassandra
* **Containerization**: Docker
* **API Documentation**: Swagger/OpenAPI

## Features

* RESTful API endpoints for like management
* Add/remove likes from posts
* Get like counts for posts
* Get users who liked a post
* Get posts liked by a user
* Check if a user liked a post

## Database Schema

### Likes Table

```sql
CREATE TABLE likes (
    id uuid PRIMARY KEY,
    post_id uuid,
    user_id text,
    created_at timestamp,
    like_type text
);
```

### Indices

* `post_id` index for querying likes by post
* `user_id` index for querying likes by user
* `post_id, user_id` compound index for checking if a user liked a post

## Getting Started

### Prerequisites

* .NET SDK 9.0 or later
* Docker and Docker Compose
* Apache Cassandra

### Installation

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/babbly-like-service.git
   cd babbly-like-service
   ```

2. Restore dependencies:
   ```
   dotnet restore
   ```

3. Set up the Cassandra connection in your environment variables or user secrets:
   ```
   # For development, you can use user secrets
   dotnet user-secrets set "CassandraHosts" "localhost"
   dotnet user-secrets set "CassandraKeyspace" "babbly_likes"
   dotnet user-secrets set "CassandraUsername" "cassandra"
   dotnet user-secrets set "CassandraPassword" "cassandra"
   ```

4. Run the application:
   ```
   dotnet run --project babbly-like-service/babbly-like-service.csproj
   ```

5. The API will be available at http://localhost:5003.

## Docker Setup

1. Build and start the containers:
   ```
   docker-compose up -d
   ```

2. The services will be available at:
   * Like Service API: http://localhost:5003
   * Cassandra: localhost:9042

3. To stop the containers:
   ```
   docker-compose down
   ```

## API Endpoints

* `GET /api/likes/{id}` - Get a specific like
* `GET /api/likes/post/{postId}` - Get all likes for a post
* `GET /api/likes/user/{userId}` - Get all likes by a user
* `GET /api/likes/post/{postId}/count` - Get like count for a post
* `GET /api/likes/post/{postId}/users` - Get users who liked a post
* `GET /api/likes/user/{userId}/posts` - Get posts liked by a user
* `GET /api/likes/check?userId={userId}&postId={postId}` - Check if a user liked a post
* `POST /api/likes` - Add a like
* `POST /api/likes/unlike` - Remove a like
* `DELETE /api/likes/{id}` - Delete a like
* `GET /api/health` - Health check endpoint

## Testing

The API can be tested using the included `api-tests.http` file with REST Client extension in Visual Studio Code or similar tools.

## Schema Management

See [CASSANDRA-MIGRATIONS.md](CASSANDRA-MIGRATIONS.md) for details on how to manage the Cassandra schema. 