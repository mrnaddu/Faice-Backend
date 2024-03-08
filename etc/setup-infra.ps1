$requiredServices = @(    
    'postgres-db'
)
	
foreach ($requiredService in $requiredServices) {	

    $nameParam = -join ("name=", $requiredService)
    $serviceRunningStatus = docker ps --filter $nameParam
    $isDockerImageUp = $serviceRunningStatus -split " " -contains $requiredService
	
    if ( $isDockerImageUp ) {
        Write-Host ($requiredService + " [up]")
    }
    else {
        Invoke-Expression "./etc/docker/up.ps1"
        break;
    }
}