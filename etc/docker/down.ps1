$MyInvocation.MyCommand.Path | Split-Path | Push-Location

docker-compose down

Pop-Location