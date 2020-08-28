#!/bin/bash

# ---------
# Functions
# ---------

is_kafka_started() {
	systemdOutput=`systemctl status kafka | grep active`

	if [[ -z $systemdOutput ]]; then
		result=false
	else
		result=true
	fi

	echo $result
}

create_topic() {

	serverString="$server:$port"
	topic=$1
	replicationFactor=$2
	partitions=$3

	if [  $# -eq 3 ]; then
		kafka-topics.sh \
			--create \
			--bootstrap-server $serverString \
			--replication-factor $replicationFactor \
			--partitions $partitions \
			--topic $topic
	else
		echo [Error] Missing or invalid arguments sent to create_topic function
		echo No topic has been created
		echo Usage is "create_topic topic replication_factor number_of_partitions"
		exit 1
	fi

}

# ----------------
# Global variables
# ----------------

server=localhost
port=9092

# ---------------
# Read parameters
# ---------------

while [ $# -gt 0 ]; do
	key=$1

	case $key in 
		-s|--server)
			server=$2
			shift
			shift
			;;

		-p|--port)
			port=$2
			shift
			shift
			;;

		-h|--help)
			echo Supported parameters are : 
			echo "-s|--server server"
			echo "-p|--port port"
			shift
			exit
			;;

		*)
			echo [Warning] Skipped unknown parameter $1 
			shift
			;;
	esac
done

echo Using server $server
echo Using port $port

# --------------------------
# Make sure kafka is started
# --------------------------

echo -n "Checking if kafka is started... "

if [ $(is_kafka_started) = true ]; then
	echo Ok !
else
	echo Not started !
	echo Please start kafka and try again
	exit 1
fi

# -------------
# Create topics
# -------------

create_topic User.EventSourcing 1 1
create_topic User.Subscriptions 1 1

exit 0
