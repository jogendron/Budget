#!/bin/bash

docker-compose -p budget stop

docker rm zookeeper
docker rm broker
docker rm mongo
docker rm postgres
docker rm budget_users
docker rm budget_gateway

docker network rm budget_frontend
docker network rm budget_backend