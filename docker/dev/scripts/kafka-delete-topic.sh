#!/bin/bash

docker exec -it broker kafka-topics --delete bootstrap-server localhost:9092 --topic $1
