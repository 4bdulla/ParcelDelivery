$ErrorActionPreference = "Stop"

$CurrentDir = Get-Location
$EnvFile = Join-Path $CurrentDir ".env"
$ComposeFile = Join-Path $CurrentDir "compose.yml"

docker compose -p "parcel-delivery" --env-file $EnvFile -f $ComposeFile up -d --remove-orphans --build
