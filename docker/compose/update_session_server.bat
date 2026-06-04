@echo off
echo Stopping session-server container...
docker-compose stop session-server

echo Removing session-server container...
docker-compose rm -f session-server

echo Removing old session server image (0.36.220.1231)...
docker rmi mytelegram/mytelegram-session-server:0.36.220.1231

echo Pulling session server image v0.32.206.802...
docker pull mytelegram/mytelegram-session-server:0.32.206.802

echo Starting session-server with version 0.32.206.802...
set MyTelegramVersion=0.32.206.802
docker-compose up -d session-server

echo Done.
pause
