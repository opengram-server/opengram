#!/bin/sh
export REGISTRY_URL="mytelegram"

./1.build-messenger-command-server.sh
./2.build-messenger-query-server.sh
./4.build-sms-sender.sh
./5.build-gateway-server.sh
./6.build-auth-server.sh