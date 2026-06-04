#!/bin/sh
v=$(head -n 1 ../version.txt)
currentDate=`date +%-m%d`
export DEFAULT_VERSION=$v.$currentDate
export DEFAULT_IMAGE_VERSION=$DEFAULT_VERSION
DEFAULT_PLATFORM="linux/amd64"

: ${VERSION:=$DEFAULT_VERSION}
: ${IMAGE_VERSION:=$DEFAULT_IMAGE_VERSION}
: ${TARGETARCH:=$DEFAULT_PLATFORM}
: ${BUILDPLATFORM:=$DEFAULT_PLATFORM}
: ${BUILD_CONFIGURATION:="Release"}
: ${REGISTRY_URL:="mytelegram"}
: ${BUILD_COMMAND:="docker build" }
: ${MYTELEGRAM_SERVER_NAME:="mytelegram-messenger-command-server"}
: ${IMAGE_NAME:=$REGISTRY_URL/$MYTELEGRAM_SERVER_NAME}
: ${DOCKER_FILENAME:="./Dockerfile-$MYTELEGRAM_SERVER_NAME"}

echo Version: $VERSION
echo Docker image version: $IMAGE_VERSION
echo Target architecture: $TARGETARCH
echo Buld configuration: $BUILD_CONFIGURATION

: ${BUILD_ALL_COMMAND:=$BUILD_COMMAND \
--platform $BUILDPLATFORM \
--build-arg BUILD_CONFIGURATION=$BUILD_CONFIGURATION \
--tag $IMAGE_NAME:$IMAGE_VERSION \
--tag $IMAGE_NAME:latest \
--file $DOCKER_FILENAME \
../../source \
--network=host}