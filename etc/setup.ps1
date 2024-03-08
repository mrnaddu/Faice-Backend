$ErrorActionPreference = "Stop"

$nodeExist = [bool] (Get-Command -ErrorAction Ignore -Type Application node)
$dockerExist = [bool] (Get-Command -ErrorAction Ignore -Type Application docker)

if(!$dockerExist){
    Write-Host("`nPlease install docker`n")
    exit
}

$docker = docker ps 2>&1

Write-Host("`nAdding dotnet-ef tool`n")
dotnet tool install --global dotnet-ef

Write-Host("`nBuilding Solution`n")
dotnet build /graphBuild

Invoke-Expression "./pokemon.ps1 infra up"


Write-Host("`nCongrats! Project setup is complete'`n")