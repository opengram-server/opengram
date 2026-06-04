# For more information,please visit https://github.com/loyldg/mytelegram
$env:App__QueryServerEventStoreDatabaseName = "tg-1"
$env:App__QueryServerReadModelDatabaseName = "tg-1"
$env:App__DatabaseName = "tg"
$env:App__BotDatabaseName = "tg"

$env:App__FixedVerifyCode = "22222"

$env:Serilog__MinimumLevel__Default = "Information"
$env:Serilog__MinimumLevel__Override__Microsoft = "Information"

# Use separate media server
$env:App__Servers__2__Enabled = "True"
$env:App__Servers__3__Enabled = "True"


# RabbitMQ
# $env:RabbitMQ__Connections__Default__HostName=localhost
# $env:RabbitMQ__Connections__Default__Password=test
# $env:RabbitMQ__Connections__Default__Port=5672
# $env:RabbitMQ__Connections__Default__UserName=test
# $env:RabbitMQ__EventBus__ExchangeName=MyTelegramExchange

# Redis
# $env:Redis__Configuration=localhost:6379

# MongoDB
# $env:ConnectionStrings__Default=mongodb://localhost:27017

# Minio
# $env:Minio__AccessKey=test
# $env:Minio__BucketName=tg-files
# $env:Minio__CreateBucketIfNotExists=True
# $env:Minio__Endpoint=localhost:9000
# $env:Minio__SecretKey=12345678

# TwilioSms
# $env:TwilioSms__AccountSId=
# $env:TwilioSms__AuthToken=
# $env:TwilioSms__Enabled=False
# $env:TwilioSms__FromNumber=
# VonageSms
# $env:VonageSms__ApiKey=
# $env:VonageSms__ApiSecret=
# $env:VonageSms__BrandName=
# $env:VonageSms__Enabled=False

# App
# $env:App=
# $env:App__AllowedUserIds__0=2000001
# $env:App__AutoCreateSuperGroup=True
# $env:App__BotDatabaseName=tg
# $env:App__ChannelGetDifferenceIntervalSeconds=60
# $env:App__CommandServerObjectIds=
# $env:App__DatabaseName=tg
# $env:App__DcOptions__0__Cdn=False
# $env:App__DcOptions__0__Enabled=True
# $env:App__DcOptions__0__Id=1
# $env:App__DcOptions__0__IpAddress=192.168.1.100
# $env:App__DcOptions__0__Ipv6=False
# $env:App__DcOptions__0__MediaOnly=False
# $env:App__DcOptions__0__Port=20443
# $env:App__DcOptions__0__Static=False
# $env:App__DcOptions__0__TcpoOnly=True
# $env:App__DcOptions__0__ThisPortOnly=True
# $env:App__DcOptions__1__Cdn=False
# $env:App__DcOptions__1__Enabled=True
# $env:App__DcOptions__1__Id=2
# $env:App__DcOptions__1__IpAddress=192.168.1.100
# $env:App__DcOptions__1__Ipv6=False
# $env:App__DcOptions__1__MediaOnly=False
# $env:App__DcOptions__1__Port=20543
# $env:App__DcOptions__1__Static=False
# $env:App__DcOptions__1__TcpoOnly=True
# $env:App__DcOptions__1__ThisPortOnly=True
# $env:App__DcOptions__2__Cdn=False
# $env:App__DcOptions__2__Enabled=True
# $env:App__DcOptions__2__Id=2
# $env:App__DcOptions__2__IpAddress=192.168.1.100
# $env:App__DcOptions__2__Ipv6=False
# $env:App__DcOptions__2__MediaOnly=True
# $env:App__DcOptions__2__Port=20643
# $env:App__DcOptions__2__Static=False
# $env:App__DcOptions__2__TcpoOnly=True
# $env:App__DcOptions__2__ThisPortOnly=True
# $env:App__DcOptions__3__Cdn=False
# $env:App__DcOptions__3__Enabled=True
# $env:App__DcOptions__3__Id=2
# $env:App__DcOptions__3__IpAddress=192.168.1.100
# $env:App__DcOptions__3__Ipv6=False
# $env:App__DcOptions__3__MediaOnly=True
# $env:App__DcOptions__3__Port=20644
# $env:App__DcOptions__3__Static=False
# $env:App__DcOptions__3__TcpoOnly=True
# $env:App__DcOptions__3__ThisPortOnly=True
# $env:App__DispatchUnknownObjectToMessengerServer=True
# $env:App__EnableFutureAuthToken=True
# $env:App__FileServerGrpcServiceUrl=http://localhost:10001
# $env:App__FixedVerifyCode=
# $env:App__IdGeneratorGrpcServiceUrl=http://localhost:10002
# $env:App__IsMediaDc=False
# $env:App__JoinChatDomain=https://t.me
# $env:App__LoadAllStickersIntoMemory=True
# $env:App__MediaDcId=2
# $env:App__MediaOnly=False
# $env:App__MessengerServerGrpcServiceUrl=http://localhost:10003
# $env:App__OfficialTelegramBotOptions__ProxyUrl=socks5://localhost:10808
# $env:App__OfficialTelegramBotOptions__Token=
# $env:App__OfficialTelegramBotOptions__UseProxy=True
# $env:App__PrivateKeyFilePath=private.pkcs8.key
# $env:App__QueryServerEventStoreDatabaseName=tg-1
# $env:App__QueryServerReadModelDatabaseName=tg-1
# $env:App__SendWelcomeMessageAfterUserSignIn=False
# $env:App__Servers__0__CertPemFilePath=
# $env:App__Servers__0__Enabled=True
# $env:App__Servers__0__EnableProxyProtocolV2=False
# $env:App__Servers__0__Ip=
# $env:App__Servers__0__Ipv6=True
# $env:App__Servers__0__KeyPemFilePath=
# $env:App__Servers__0__MediaOnly=False
# $env:App__Servers__0__Port=20443
# $env:App__Servers__0__ServerType=0
# $env:App__Servers__0__Ssl=False
# $env:App__Servers__1__CertPemFilePath=
# $env:App__Servers__1__Enabled=True
# $env:App__Servers__1__EnableProxyProtocolV2=False
# $env:App__Servers__1__Ip=
# $env:App__Servers__1__Ipv6=True
# $env:App__Servers__1__KeyPemFilePath=
# $env:App__Servers__1__MediaOnly=False
# $env:App__Servers__1__Port=20543
# $env:App__Servers__1__ServerType=0
# $env:App__Servers__1__Ssl=False
# $env:App__Servers__2__CertPemFilePath=
# $env:App__Servers__2__Enabled=False
# $env:App__Servers__2__EnableProxyProtocolV2=False
# $env:App__Servers__2__Ip=
# $env:App__Servers__2__Ipv6=True
# $env:App__Servers__2__KeyPemFilePath=
# $env:App__Servers__2__MediaOnly=True
# $env:App__Servers__2__Port=20643
# $env:App__Servers__2__ServerType=0
# $env:App__Servers__2__Ssl=False
# $env:App__Servers__3__CertPemFilePath=
# $env:App__Servers__3__Enabled=False
# $env:App__Servers__3__EnableProxyProtocolV2=False
# $env:App__Servers__3__Ip=
# $env:App__Servers__3__Ipv6=True
# $env:App__Servers__3__KeyPemFilePath=
# $env:App__Servers__3__MediaOnly=True
# $env:App__Servers__3__Port=20644
# $env:App__Servers__3__ServerType=0
# $env:App__Servers__3__Ssl=False
# $env:App__Servers__4__CertPemFilePath=_wildcard.telegram2.com.pem
# $env:App__Servers__4__Enabled=True
# $env:App__Servers__4__EnableProxyProtocolV2=False
# $env:App__Servers__4__Ip=
# $env:App__Servers__4__Ipv6=True
# $env:App__Servers__4__KeyPemFilePath=_wildcard.telegram2.com-key.pem
# $env:App__Servers__4__MediaOnly=False
# $env:App__Servers__4__Port=30443
# $env:App__Servers__4__ServerType=1
# $env:App__Servers__4__Ssl=True
# $env:App__Servers__5__CertPemFilePath=
# $env:App__Servers__5__Enabled=True
# $env:App__Servers__5__EnableProxyProtocolV2=False
# $env:App__Servers__5__Ip=
# $env:App__Servers__5__Ipv6=True
# $env:App__Servers__5__KeyPemFilePath=
# $env:App__Servers__5__MediaOnly=False
# $env:App__Servers__5__Port=30444
# $env:App__Servers__5__ServerType=1
# $env:App__Servers__5__Ssl=False
# $env:App__SetPremiumToTrueAfterUserCreated=True
# $env:App__SubscribeLocalRequest=True
# $env:App__SubscribeRemoteRequest=True
# $env:App__TempAuthKeyExpirationMinutes=720
# $env:App__ThisDcId=1
# $env:App__UploadRootPath=uploads2
# $env:App__UseExternalWebHookSender=False
# $env:App__UseInMemoryFilters=True
# $env:App__VerificationCodeExpirationSeconds=300
# $env:App__VerificationCodeLength=5
# $env:App__WebHookSenderUrl=
# $env:App__WebRtcConnections__0__Ip=192.168.1.100
# $env:App__WebRtcConnections__0__Ipv6=
# $env:App__WebRtcConnections__0__Password=b
# $env:App__WebRtcConnections__0__Port=20444
# $env:App__WebRtcConnections__0__Stun=True
# $env:App__WebRtcConnections__0__Turn=True
# $env:App__WebRtcConnections__0__UserName=a

Set-Location -Path ".\command-server"
Start-Process -FilePath "MyTelegram.Messenger.CommandServer.exe"

Set-Location -Path "..\query-server"
Start-Process -FilePath "MyTelegram.Messenger.QueryServer.exe"

Set-Location -Path "..\gateway"
Start-Process -FilePath "MyTelegram.GatewayServer.exe"

<#
Start-Process -FilePath "MyTelegram.GatewayServer.exe" -ArgumentList `
    "RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly", `
    "RabbitMQ:EventBus:ClientName=MyTelegramGatewayServer.MediaOnly", `
    "RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly", `
    "App:Servers:0:Enabled=false", `
    "App:Servers:1:Enabled=false", `
    "App:Servers:2:Enabled=true", `
    "App:Servers:3:Enabled=true", `
    "App:Servers:4:Enabled=false", `
    "App:Servers:5:Enabled=false"
#>

Set-Location -Path "..\auth"
Start-Process -FilePath "MyTelegram.AuthServer.exe"

<#
Start-Process -FilePath "MyTelegram.AuthServer.exe" -ArgumentList `
    "RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly", `
    "RabbitMQ:EventBus:ClientName=MyTelegramAuthServer.MediaOnly", `
    "RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly"
#>

Set-Location -Path "..\session"
Start-Process -FilePath "MyTelegram.SessionServer.exe"

<#
Start-Process -FilePath "MyTelegram.SessionServer.exe" -ArgumentList `
    "RabbitMQ:EventBus:ExchangeName=MyTelegramExchange.MediaOnly", `
    "App:MediaOnly=true", `
    "RabbitMQ:EventBus:ClientName=MyTelegramSessionServer.MediaOnly", `
    "RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly"
#>

Set-Location -Path "..\file"
Start-Process -FilePath "MyTelegram.FileServer.exe" -ArgumentList "urls=http://.+:10001"

<#
Start-Process -FilePath "MyTelegram.FileServer.exe" -ArgumentList `
    "urls=http://.+:10007", `
    "RabbitMQ:EventBus:ClientName=MyTelegramFileServer.MediaOnly", `
    "RabbitMQ:EventBus:TopicExchangeName=MyTelegramTopicExchange.MediaOnly"
#>

Set-Location -Path "..\messenger-grpc-service"
Start-Process -FilePath "MyTelegram.MessengerServer.GrpcService.exe" -ArgumentList "urls=http://.+:10003"

Set-Location -Path "..\sms"
Start-Process -FilePath "MyTelegram.SmsSender.exe"


Set-Location ../