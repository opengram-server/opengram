#!/bin/bash
v=$(head -n 1 ./version.txt)
currentDate=`date +%-m%d`
version=$v.$currentDate

currentDir=$(pwd)
parentFolder=$(dirname "$currentDir")
outputRootFolder="$parentFolder/out/local/$version"
sourceRootFolder="$parentFolder/source/src"

authServerOutputFolder="$outputRootFolder/auth"
gatewayOutputFolder="$outputRootFolder/gateway"
messengerProCommandOutputFolder="$outputRootFolder/command-server"
messengerProQueryOutputFolder="$outputRootFolder/query-server"
messengerGrpcOutputFolder="$outputRootFolder/messenger-grpc"
smsOutputFolder="$outputRootFolder/sms"
dataSeederOutputFolder="$outputRootFolder/data-seeder"

echo "Current dir: $currentDir"
echo "OutputFolder is: $outputRootFolder"

CreateFolderIfNotExists() {
    folder="$1"
    if [ ! -d "$folder" ]; then
        echo "Create new folder: $folder"
        mkdir -p "$folder"
    fi
}

Build-Server() {
    srcFolder="$1"
    outputFolder="$2"
    sourceFolder="$sourceRootFolder/$srcFolder"
    dotnet publish "$sourceFolder" -c Release -o "$outputFolder"
}

Build-Server "./MyTelegram.AuthServer" "$authServerOutputFolder"
Build-Server "./MyTelegram.GatewayServer" "$gatewayOutputFolder"
Build-Server "./MyTelegram.Messenger.CommandServer" "$messengerProCommandOutputFolder"
Build-Server "./MyTelegram.Messenger.QueryServer" "$messengerProQueryOutputFolder"
#Build-Server "./MyTelegram.MessengerServer.GrpcService" "$messengerGrpcOutputFolder"
Build-Server "./MyTelegram.SmsSender" "$smsOutputFolder"
Build-Server "./MyTelegram.DataSeeder" "$dataSeederOutputFolder"

cd "$currentDir"
