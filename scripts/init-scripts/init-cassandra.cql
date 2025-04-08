-- Create keyspace
CREATE KEYSPACE IF NOT EXISTS babbly_likes
WITH REPLICATION = { 
  'class' : 'SimpleStrategy', 
  'replication_factor' : 1 
};

-- Use the keyspace
USE babbly_likes;

-- Create likes table
CREATE TABLE IF NOT EXISTS likes (
  id uuid PRIMARY KEY,
  post_id uuid,
  user_id text,
  created_at timestamp,
  like_type text
);

-- Create indices for common queries
CREATE INDEX IF NOT EXISTS ON likes (post_id);
CREATE INDEX IF NOT EXISTS ON likes (user_id);
CREATE INDEX IF NOT EXISTS ON likes (post_id, user_id);

-- Create user credentials if not using default Cassandra auth
-- CREATE USER IF NOT EXISTS babbly_user WITH PASSWORD 'babbly_password' SUPERUSER;
-- GRANT ALL PERMISSIONS ON KEYSPACE babbly_likes TO babbly_user; 