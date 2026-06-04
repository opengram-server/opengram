#!/bin/sh
export BUILDPLATFORM="linux/arm64"
export TARGETARCH="linux/arm64"
./1.build-messenger-command-server.sh
./2.build-messenger-query-server.sh
./4.build-sms-sender.sh
./5.build-gateway-server.sh
./6.build-auth-server.sh
./7.build-data-seeder.sh