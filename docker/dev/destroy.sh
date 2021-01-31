#!/bin/bash

docker-compose -p budget stop

docker rm zookeeper
docker rm broker
docker rm mongo
