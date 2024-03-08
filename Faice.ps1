#./pokemon.ps1 install
#./pokemon.ps1 infra up
#./pokemon.ps1 infra down

$action = $args[0]
$subaction = $args[1]

if (!$action) {
    Write-Host("`nPlease mention what action to perform`n")
    exit;
}

$action = $action.ToLower();

if ($action -eq "install") {
    Invoke-Expression "./etc/setup.ps1"
}
elseif ($action -eq "infra") {
    if ($subaction -eq "up") {
        Invoke-Expression "./etc/docker/up.ps1"
    }
    elseif ($subaction -eq "down") {

        Invoke-Expression "./etc/docker/down.ps1"
    }
}
