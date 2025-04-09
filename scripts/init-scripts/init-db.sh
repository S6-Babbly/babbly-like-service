#!/bin/bash

set -e

MAX_ATTEMPTS=30
DELAY_SECONDS=5

# Get the container's own IP address - more reliable than hardcoding 0.0.0.0
CASSANDRA_HOST=$(hostname -i)

# Wait for Cassandra to be ready
for i in $(seq 1 $MAX_ATTEMPTS); do
  echo "Attempt $i: Checking if Cassandra is up..."
  if cqlsh $CASSANDRA_HOST -e "describe keyspaces" > /dev/null 2>&1; then
    echo "Cassandra is up - executing schema"
    break
  fi
  
  if [ $i -eq $MAX_ATTEMPTS ]; then
    echo "Failed to connect to Cassandra after $MAX_ATTEMPTS attempts. Exiting."
    exit 1
  fi
  
  echo "Cassandra is unavailable - sleeping $DELAY_SECONDS seconds"
  sleep $DELAY_SECONDS
done

# Execute the initialization script
echo "Executing init-cassandra.cql..."
cqlsh $CASSANDRA_HOST -f /docker-entrypoint-initdb.d/init-cassandra.cql

# Verify keyspace was created
if cqlsh $CASSANDRA_HOST -e "describe keyspace babbly_likes" > /dev/null 2>&1; then
  echo "Keyspace babbly_likes created successfully"
else
  echo "Failed to create keyspace babbly_likes"
  exit 1
fi

echo "Schema initialization completed" 