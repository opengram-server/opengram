#!/bin/sh
# For more information,please visit https://github.com/loyldg/mytelegram
export App__QueryServerEventStoreDatabaseName=tg-1
export App__QueryServerReadModelDatabaseName=tg-1
export App__DatabaseName=tg
export App__BotDatabaseName=tg

export App__FixedVerifyCode=22222

export Serilog__MinimumLevel__Default=Information
export Serilog__MinimumLevel__Override__Microsoft=Information

# Use separate media server
export App__Servers__2__Enabled=True
export App__Servers__3__Enabled=True

# RabbitMQ
# export RabbitMQ__Connections__Default__HostName=localhost
# export RabbitMQ__Connections__Default__Password=test
# export RabbitMQ__Connections__Default__Port=5672
# export RabbitMQ__Connections__Default__UserName=test
# export RabbitMQ__EventBus__ExchangeName=MyTelegramExchange

# Redis
# export Redis__Configuration=localhost:6379

# MongoDB
# export ConnectionStrings__Default=mongodb://localhost:27017

# Minio
# export Minio__AccessKey=test
# export Minio__BucketName=tg-files
# export Minio__CreateBucketIfNotExists=True
# export Minio__Endpoint=localhost:9000
# export Minio__SecretKey=12345678

# TwilioSms
# export TwilioSms__AccountSId=
# export TwilioSms__AuthToken=
# export TwilioSms__Enabled=False
# export TwilioSms__FromNumber=
# VonageSms
# export VonageSms__ApiKey=
# export VonageSms__ApiSecret=
# export VonageSms__BrandName=
# export VonageSms__Enabled=False

# App
# export App__AllowedUserIds__0=2000001
# export App__AutoCreateSuperGroup=True
# export App__BotDatabaseName=tg
# export App__ChannelGetDifferenceIntervalSeconds=60
# export App__CommandServerObjectIds=
# export App__DatabaseName=tg
# export App__DcOptions__0__Cdn=False
# export App__DcOptions__0__Enabled=True
# export App__DcOptions__0__Id=1
# export App__DcOptions__0__IpAddress=192.168.1.100
# export App__DcOptions__0__Ipv6=False
# export App__DcOptions__0__MediaOnly=False
# export App__DcOptions__0__Port=20443
# export App__DcOptions__0__Static=False
# export App__DcOptions__0__TcpoOnly=True
# export App__DcOptions__0__ThisPortOnly=True
# export App__DcOptions__1__Cdn=False
# export App__DcOptions__1__Enabled=True
# export App__DcOptions__1__Id=2
# export App__DcOptions__1__IpAddress=192.168.1.100
# export App__DcOptions__1__Ipv6=False
# export App__DcOptions__1__MediaOnly=False
# export App__DcOptions__1__Port=20543
# export App__DcOptions__1__Static=False
# export App__DcOptions__1__TcpoOnly=True
# export App__DcOptions__1__ThisPortOnly=True
# export App__DcOptions__2__Cdn=False
# export App__DcOptions__2__Enabled=True
# export App__DcOptions__2__Id=2
# export App__DcOptions__2__IpAddress=192.168.1.100
# export App__DcOptions__2__Ipv6=False
# export App__DcOptions__2__MediaOnly=True
# export App__DcOptions__2__Port=20643
# export App__DcOptions__2__Static=False
# export App__DcOptions__2__TcpoOnly=True
# export App__DcOptions__2__ThisPortOnly=True
# export App__DcOptions__3__Cdn=False
# export App__DcOptions__3__Enabled=True
# export App__DcOptions__3__Id=2
# export App__DcOptions__3__IpAddress=192.168.1.100
# export App__DcOptions__3__Ipv6=False
# export App__DcOptions__3__MediaOnly=True
# export App__DcOptions__3__Port=20644
# export App__DcOptions__3__Static=False
# export App__DcOptions__3__TcpoOnly=True
# export App__DcOptions__3__ThisPortOnly=True
# export App__DispatchUnknownObjectToMessengerServer=True
# export App__EnableFutureAuthToken=True
# export App__FileServerGrpcServiceUrl=http://localhost:10001
# export App__FixedVerifyCode=
# export App__IdGeneratorGrpcServiceUrl=http://localhost:10002
# export App__IsMediaDc=False
# export App__JoinChatDomain=https://t.me
# export App__LoadAllStickersIntoMemory=True
# export App__MediaDcId=2
# export App__MediaOnly=False
# export App__MessengerServerGrpcServiceUrl=http://localhost:10003
# export App__OfficialTelegramBotOptions__ProxyUrl=socks5://localhost:10808
# export App__OfficialTelegramBotOptions__Token=
# export App__OfficialTelegramBotOptions__UseProxy=True
# export App__PrivateKeyFilePath=private.pkcs8.key
# export App__QueryServerEventStoreDatabaseName=tg-1
# export App__QueryServerReadModelDatabaseName=tg-1
# export App__SendWelcomeMessageAfterUserSignIn=False
# export App__Servers__0__CertPemFilePath=
# export App__Servers__0__Enabled=True
# export App__Servers__0__EnableProxyProtocolV2=False
# export App__Servers__0__Ip=
# export App__Servers__0__Ipv6=True
# export App__Servers__0__KeyPemFilePath=
# export App__Servers__0__MediaOnly=False
# export App__Servers__0__Port=20443
# export App__Servers__0__ServerType=0
# export App__Servers__0__Ssl=False
# export App__Servers__1__CertPemFilePath=
# export App__Servers__1__Enabled=True
# export App__Servers__1__EnableProxyProtocolV2=False
# export App__Servers__1__Ip=
# export App__Servers__1__Ipv6=True
# export App__Servers__1__KeyPemFilePath=
# export App__Servers__1__MediaOnly=False
# export App__Servers__1__Port=20543
# export App__Servers__1__ServerType=0
# export App__Servers__1__Ssl=False
# export App__Servers__2__CertPemFilePath=
# export App__Servers__2__Enabled=False
# export App__Servers__2__EnableProxyProtocolV2=False
# export App__Servers__2__Ip=
# export App__Servers__2__Ipv6=True
# export App__Servers__2__KeyPemFilePath=
# export App__Servers__2__MediaOnly=True
# export App__Servers__2__Port=20643
# export App__Servers__2__ServerType=0
# export App__Servers__2__Ssl=False
# export App__Servers__3__CertPemFilePath=
# export App__Servers__3__Enabled=False
# export App__Servers__3__EnableProxyProtocolV2=False
# export App__Servers__3__Ip=
# export App__Servers__3__Ipv6=True
# export App__Servers__3__KeyPemFilePath=
# export App__Servers__3__MediaOnly=True
# export App__Servers__3__Port=20644
# export App__Servers__3__ServerType=0
# export App__Servers__3__Ssl=False
# export App__Servers__4__CertPemFilePath=_wildcard.telegram2.com.pem
# export App__Servers__4__Enabled=True
# export App__Servers__4__EnableProxyProtocolV2=False
# export App__Servers__4__Ip=
# export App__Servers__4__Ipv6=True
# export App__Servers__4__KeyPemFilePath=_wildcard.telegram2.com-key.pem
# export App__Servers__4__MediaOnly=False
# export App__Servers__4__Port=30443
# export App__Servers__4__ServerType=1
# export App__Servers__4__Ssl=True
# export App__Servers__5__CertPemFilePath=
# export App__Servers__5__Enabled=True
# export App__Servers__5__EnableProxyProtocolV2=False
# export App__Servers__5__Ip=
# export App__Servers__5__Ipv6=True
# export App__Servers__5__KeyPemFilePath=
# export App__Servers__5__MediaOnly=False
# export App__Servers__5__Port=30444
# export App__Servers__5__ServerType=1
# export App__Servers__5__Ssl=False
# export App__SetPremiumToTrueAfterUserCreated=True
# export App__SubscribeLocalRequest=True
# export App__SubscribeRemoteRequest=True
# export App__TempAuthKeyExpirationMinutes=720
# export App__ThisDcId=1
# export App__UploadRootPath=uploads2
# export App__UseExternalWebHookSender=False
# export App__UseInMemoryFilters=True
# export App__VerificationCodeExpirationSeconds=300
# export App__VerificationCodeLength=5
# export App__WebHookSenderUrl=
# export App__WebRtcConnections__0__Ip=192.168.1.100
# export App__WebRtcConnections__0__Ipv6=
# export App__WebRtcConnections__0__Password=b
# export App__WebRtcConnections__0__Port=20444
# export App__WebRtcConnections__0__Stun=True
# export App__WebRtcConnections__0__Turn=True
# export App__WebRtcConnections__0__UserName=a



cd ./command-server
./MyTelegram.Messenger.CommandServer &

cd ../query-server
./MyTelegram.Messenger.QueryServer &

cd ../gateway
./MyTelegram.GatewayServer &

# ./MyTelegram.GatewayServer \
# RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly \
# RabbitMQ:EventBus:ClientName=MyTelegramGatewayServer.MediaOnly \
# RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly \
# App:Servers:0:Enabled=false \
# App:Servers:1:Enabled=false \
# App:Servers:2:Enabled=true \
# App:Servers:3:Enabled=true \
# App:Servers:4:Enabled=false \
# App:Servers:5:Enabled=false &

cd ../auth
./MyTelegram.AuthServer &

# ./MyTelegram.AuthServer \
# RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly \
# RabbitMQ:EventBus:ClientName=MyTelegramAuthServer.MediaOnly \
# RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly &

cd ../session
./MyTelegram.SessionServer &
# ./MyTelegram.SessionServer \
# RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly \
# App:MediaOnly=true \
# RabbitMQ:EventBus:ClientName=MyTelegramSessionServer.MediaOnly \
# RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly &

cd ../file
./MyTelegram.FileServer urls=http://.+:10001 &
# ./MyTelegram.FileServer.exe urls=http://.+:10007 \
# RabbitMQ:EventBus:ClientName=MyTelegramFileServer.MediaOnly \
# RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly &

cd ../messenger-grpc
./MyTelegram.MessengerServer.GrpcService urls=http://.+:10003 &

cd ../sms
./MyTelegram.SmsSender &
