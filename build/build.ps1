$version=Get-Content "./version.txt"
$date = Get-Date -Format "Mdd"
$version = $version+$date

$currentDir=(Get-Item .).FullName
$parentFolder=(Get-Item $currentDir).Parent
$outputRootFolder=Join-Path $parentFolder "out/local" $version 
$sourceRootFolder=Join-Path $parentFolder "./source/src"

$authServerOutputFolder=Join-Path $outputRootFolder "auth"
$gatewayOutputFolder=Join-Path $outputRootFolder "gateway"
$messengerProCommandOutputFolder=Join-Path $outputRootFolder "command-server"
$messengerProQueryOutputFolder=Join-Path $outputRootFolder "query-server"
$messengerGrpcOutputFolder=Join-Path $outputRootFolder "messenger-grpc"
$smsOutputFolder=Join-Path $outputRootFolder "sms"
$dataSeederOutputFolder=Join-Path $outputRootFolder "data-seeder"

Write-Output "Current dir:$currentDir"
Write-Output "OutputFolder is:$outputRootFolder"

function CreateFolderIfNotExists([System.String] $folder){
    if(![System.IO.Directory]::Exists($folder)){
        Write-Output "Create new folder:$folder"
        [System.IO.Directory]::CreateDirectory($folder)
    }
}

function Build-Server([System.String]$srcFolder,[System.String] $outputFolder) {
    $sourceFolder=Join-Path $sourceRootFolder $srcFolder
    dotnet publish $sourceFolder -c Release -o $outputFolder
}

# Set-Location ../source/
#dotnet restore ./MyTelegram.sln
Build-Server "./MyTelegram.AuthServer" $authServerOutputFolder
Build-Server "./MyTelegram.GatewayServer" $gatewayOutputFolder
Build-Server "./MyTelegram.Messenger.CommandServer" $messengerProCommandOutputFolder
Build-Server "./MyTelegram.Messenger.QueryServer" $messengerProQueryOutputFolder
#Build-Server "./MyTelegram.MessengerServer.GrpcService" $messengerGrpcOutputFolder
Build-Server "./MyTelegram.SmsSender" $smsOutputFolder
Build-Server "./MyTelegram.DataSeeder" $dataSeederOutputFolder

Set-Location $currentDir


