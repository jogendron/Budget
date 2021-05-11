#!/bin/bash

function createKafkaTopic() {
    TOPIC_NAME=$1

    echo -n "Creating $TOPIC_NAME kafka topic... "
    docker exec broker kafka-topics --create --bootstrap-server broker:9092 --replication-factor 1 --partitions 1 --topic $TOPIC_NAME >/dev/null 
    echo "Done !"
}

function configurePostgres() {
    echo -n "Configuring postgres... "
    docker cp ./postgres-budget-setup.sql postgres:/tmp >/dev/null 
    docker exec -u postgres postgres psql -f /tmp/postgres-budget-setup.sql >/dev/null 
    echo "Done !"
}

# Create containers
docker-compose -p budget up -d

# Setup Kafka
echo -n "Waiting 30 seconds for zookeeper and Kafka to be up... "
sleep 30
echo "Done !"
createKafkaTopic "User.EventSourcing"
createKafkaTopic "User.PublicEvents"

# Setup Postgres
configurePostgres

echo "Restarting users api"
docker container restart budget_users
