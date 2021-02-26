#!/bin/bash

docker-compose -p budget up -d

echo Waiting 10 seconds for zookeeper and kafka to become online...
sleep 10

echo Creating User.EventSourcing kafka topic...
docker exec broker kafka-topics --create --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1 --topic User.EventSourcing

echo Creating User.PublicEvents kafka topic...
docker exec broker kafka-topics --create --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1 --topic User.PublicEvents

echo Configuring postgres...
docker cp ./postgres-budget-setup.sql postgres:/tmp
docker exec -u postgres postgres psql -f /tmp/postgres-budget-setup.sql
