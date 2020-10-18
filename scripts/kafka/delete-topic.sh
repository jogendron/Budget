#!/bin/bash

kafka-topics.sh --delete --zookeeper localhost:2181 --topic $1
