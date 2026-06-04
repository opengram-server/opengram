:: For more information,please visit https://github.com/loyldg/mytelegram
set App__QueryServerEventStoreDatabaseName=tg-1
set App__QueryServerReadModelDatabaseName=tg-1
set App__DatabaseName=tg
set App__BotDatabaseName=tg

set App__FixedVerifyCode=22222

set Serilog__MinimumLevel__Default=Information
set Serilog__MinimumLevel__Override__Microsoft=Information

:: Use separate media server
set App__Servers__2__Enabled=True
set App__Servers__3__Enabled=True

:: RabbitMQ
:: set RabbitMQ__Connections__Default__HostName=localhost
:: set RabbitMQ__Connections__Default__Password=test
:: set RabbitMQ__Connections__Default__Port=5672
:: set RabbitMQ__Connections__Default__UserName=test
:: set RabbitMQ__EventBus__ExchangeName=MyTelegramExchange

:: Redis
:: set Redis__Configuration=localhost:6379

:: MongoDB
:: set ConnectionStrings__Default=mongodb://localhost:27017

:: Minio
:: set Minio__AccessKey=test
:: set Minio__BucketName=tg-files
:: set Minio__CreateBucketIfNotExists=True
:: set Minio__Endpoint=localhost:9000
:: set Minio__SecretKey=12345678

:: TwilioSms
:: set TwilioSms__AccountSId=
:: set TwilioSms__AuthToken=
:: set TwilioSms__Enabled=False
:: set TwilioSms__FromNumber=
:: VonageSms
:: set VonageSms__ApiKey=
:: set VonageSms__ApiSecret=
:: set VonageSms__BrandName=
:: set VonageSms__Enabled=False

:: App
:: set App=
:: set App__AllowedUserIds__0=2000001
:: set App__AutoCreateSuperGroup=True
:: set App__BotDatabaseName=tg
:: set App__ChannelGetDifferenceIntervalSeconds=60
:: set App__CommandServerObjectIds=
:: set App__DatabaseName=tg
:: set App__DcOptions__0__Cdn=False
:: set App__DcOptions__0__Enabled=True
:: set App__DcOptions__0__Id=1
:: set App__DcOptions__0__IpAddress=192.168.1.100
:: set App__DcOptions__0__Ipv6=False
:: set App__DcOptions__0__MediaOnly=False
:: set App__DcOptions__0__Port=20443
:: set App__DcOptions__0__Static=False
:: set App__DcOptions__0__TcpoOnly=True
:: set App__DcOptions__0__ThisPortOnly=True
:: set App__DcOptions__1__Cdn=False
:: set App__DcOptions__1__Enabled=True
:: set App__DcOptions__1__Id=2
:: set App__DcOptions__1__IpAddress=192.168.1.100
:: set App__DcOptions__1__Ipv6=False
:: set App__DcOptions__1__MediaOnly=False
:: set App__DcOptions__1__Port=20543
:: set App__DcOptions__1__Static=False
:: set App__DcOptions__1__TcpoOnly=True
:: set App__DcOptions__1__ThisPortOnly=True
:: set App__DcOptions__2__Cdn=False
:: set App__DcOptions__2__Enabled=True
:: set App__DcOptions__2__Id=2
:: set App__DcOptions__2__IpAddress=192.168.1.100
:: set App__DcOptions__2__Ipv6=False
:: set App__DcOptions__2__MediaOnly=True
:: set App__DcOptions__2__Port=20643
:: set App__DcOptions__2__Static=False
:: set App__DcOptions__2__TcpoOnly=True
:: set App__DcOptions__2__ThisPortOnly=True
:: set App__DcOptions__3__Cdn=False
:: set App__DcOptions__3__Enabled=True
:: set App__DcOptions__3__Id=2
:: set App__DcOptions__3__IpAddress=192.168.1.100
:: set App__DcOptions__3__Ipv6=False
:: set App__DcOptions__3__MediaOnly=True
:: set App__DcOptions__3__Port=20644
:: set App__DcOptions__3__Static=False
:: set App__DcOptions__3__TcpoOnly=True
:: set App__DcOptions__3__ThisPortOnly=True
:: set App__DispatchUnknownObjectToMessengerServer=True
:: set App__EnableFutureAuthToken=True
:: set App__FileServerGrpcServiceUrl=http://localhost:10001
:: set App__FixedVerifyCode=
:: set App__IdGeneratorGrpcServiceUrl=http://localhost:10002
:: set App__IsMediaDc=False
:: set App__JoinChatDomain=https://t.me
:: set App__LoadAllStickersIntoMemory=True
:: set App__MediaDcId=2
:: set App__MediaOnly=False
:: set App__MessengerServerGrpcServiceUrl=http://localhost:10003
:: set App__OfficialTelegramBotOptions__ProxyUrl=socks5://localhost:10808
:: set App__OfficialTelegramBotOptions__Token=
:: set App__OfficialTelegramBotOptions__UseProxy=True
:: set App__PrivateKeyFilePath=private.pkcs8.key
:: set App__QueryServerEventStoreDatabaseName=tg-1
:: set App__QueryServerReadModelDatabaseName=tg-1
:: set App__SendWelcomeMessageAfterUserSignIn=False
:: set App__Servers__0__CertPemFilePath=
:: set App__Servers__0__Enabled=True
:: set App__Servers__0__EnableProxyProtocolV2=False
:: set App__Servers__0__Ip=
:: set App__Servers__0__Ipv6=True
:: set App__Servers__0__KeyPemFilePath=
:: set App__Servers__0__MediaOnly=False
:: set App__Servers__0__Port=20443
:: set App__Servers__0__ServerType=0
:: set App__Servers__0__Ssl=False
:: set App__Servers__1__CertPemFilePath=
:: set App__Servers__1__Enabled=True
:: set App__Servers__1__EnableProxyProtocolV2=False
:: set App__Servers__1__Ip=
:: set App__Servers__1__Ipv6=True
:: set App__Servers__1__KeyPemFilePath=
:: set App__Servers__1__MediaOnly=False
:: set App__Servers__1__Port=20543
:: set App__Servers__1__ServerType=0
:: set App__Servers__1__Ssl=False
:: set App__Servers__2__CertPemFilePath=
:: set App__Servers__2__Enabled=False
:: set App__Servers__2__EnableProxyProtocolV2=False
:: set App__Servers__2__Ip=
:: set App__Servers__2__Ipv6=True
:: set App__Servers__2__KeyPemFilePath=
:: set App__Servers__2__MediaOnly=True
:: set App__Servers__2__Port=20643
:: set App__Servers__2__ServerType=0
:: set App__Servers__2__Ssl=False
:: set App__Servers__3__CertPemFilePath=
:: set App__Servers__3__Enabled=False
:: set App__Servers__3__EnableProxyProtocolV2=False
:: set App__Servers__3__Ip=
:: set App__Servers__3__Ipv6=True
:: set App__Servers__3__KeyPemFilePath=
:: set App__Servers__3__MediaOnly=True
:: set App__Servers__3__Port=20644
:: set App__Servers__3__ServerType=0
:: set App__Servers__3__Ssl=False
:: set App__Servers__4__CertPemFilePath=_wildcard.telegram2.com.pem
:: set App__Servers__4__Enabled=True
:: set App__Servers__4__EnableProxyProtocolV2=False
:: set App__Servers__4__Ip=
:: set App__Servers__4__Ipv6=True
:: set App__Servers__4__KeyPemFilePath=_wildcard.telegram2.com-key.pem
:: set App__Servers__4__MediaOnly=False
:: set App__Servers__4__Port=30443
:: set App__Servers__4__ServerType=1
:: set App__Servers__4__Ssl=True
:: set App__Servers__5__CertPemFilePath=
:: set App__Servers__5__Enabled=True
:: set App__Servers__5__EnableProxyProtocolV2=False
:: set App__Servers__5__Ip=
:: set App__Servers__5__Ipv6=True
:: set App__Servers__5__KeyPemFilePath=
:: set App__Servers__5__MediaOnly=False
:: set App__Servers__5__Port=30444
:: set App__Servers__5__ServerType=1
:: set App__Servers__5__Ssl=False
:: set App__SetPremiumToTrueAfterUserCreated=True
:: set App__SubscribeLocalRequest=True
:: set App__SubscribeRemoteRequest=True
:: set App__TempAuthKeyExpirationMinutes=720
:: set App__ThisDcId=1
:: set App__UploadRootPath=uploads2
:: set App__UseExternalWebHookSender=False
:: set App__UseInMemoryFilters=True
:: set App__VerificationCodeExpirationSeconds=300
:: set App__VerificationCodeLength=5
:: set App__WebHookSenderUrl=
:: set App__WebRtcConnections__0__Ip=192.168.1.100
:: set App__WebRtcConnections__0__Ipv6=
:: set App__WebRtcConnections__0__Password=b
:: set App__WebRtcConnections__0__Port=20444
:: set App__WebRtcConnections__0__Stun=True
:: set App__WebRtcConnections__0__Turn=True
:: set App__WebRtcConnections__0__UserName=a



cd ./command-server
start MyTelegram.Messenger.CommandServer.exe

cd ../query-server
start MyTelegram.Messenger.QueryServer.exe

cd ../gateway
start MyTelegram.GatewayServer.exe

:: start MyTelegram.GatewayServer.exe ^
:: RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly ^
:: RabbitMQ:EventBus:ClientName=MyTelegramGatewayServer.MediaOnly ^
:: RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly ^
:: App:Servers:0:Enabled=false ^
:: App:Servers:1:Enabled=false ^
:: App:Servers:2:Enabled=true ^
:: App:Servers:3:Enabled=true ^
:: App:Servers:4:Enabled=false ^
:: App:Servers:5:Enabled=false

cd ../auth
start MyTelegram.AuthServer.exe

:: start MyTelegram.AuthServer.exe ^
:: RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly ^
:: RabbitMQ:EventBus:ClientName=MyTelegramAuthServer.MediaOnly ^
:: RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly

cd ../session
start MyTelegram.SessionServer.exe
:: start MyTelegram.SessionServer.exe ^
:: RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly ^
:: App:MediaOnly=true ^
:: RabbitMQ:EventBus:ClientName=MyTelegramSessionServer.MediaOnly ^
:: RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly

cd ../file
start MyTelegram.FileServer.exe urls=http://.+:10001
:: start MyTelegram.FileServer.exe urls=http://.+:10007 ^
:: RabbitMQ:EventBus:ClientName=MyTelegramFileServer.MediaOnly ^
:: RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly

cd ../messenger-grpc-service
start MyTelegram.MessengerServer.GrpcService.exe urls=http://.+:10003

cd ../sms
start MyTelegram.SmsSender.exe