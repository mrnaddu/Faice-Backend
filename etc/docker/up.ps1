$MyInvocation.MyCommand.Path | Split-Path | Push-Location

docker-compose up -d

Pop-Location