<#
    Script PowerShell pour builder et démarrer le conteneur Docker de l'API LeaveManagement.
#>

# Stoppe et supprime le conteneur existant s'il existe
Write-Host "Arrêt et suppression du conteneur 'leaveapi' (le cas échéant)..." -ForegroundColor Cyan
if (docker ps -a --format "{{.Names}}" | Select-String -Pattern "^leaveapi$") {
    docker stop leaveapi | Out-Null
    docker rm leaveapi   | Out-Null
    Write-Host "Conteneur 'leaveapi' arrêté et supprimé." -ForegroundColor Green
} else {
    Write-Host "Aucun conteneur 'leaveapi' trouvé." -ForegroundColor Yellow
}

# Build de l'image Docker
Write-Host "Construction de l'image Docker 'leavemanagement-api'..." -ForegroundColor Cyan
docker build -t leavemanagement-api .
if ($LASTEXITCODE -ne 0) { 
    Write-Error "Échec de la construction de l'image."; exit 1 
}
Write-Host "Image construite avec succès." -ForegroundColor Green

# Démarrage du conteneur en mode Development pour exposer Swagger
Write-Host "Démarrage du conteneur 'leaveapi'..." -ForegroundColor Cyan
docker run -d -p 5000:80 `
    -e ASPNETCORE_ENVIRONMENT=Development `
    --name leaveapi leavemanagement-api
if ($LASTEXITCODE -ne 0) { 
    Write-Error "Échec du démarrage du conteneur."; exit 1 
}
Write-Host "Conteneur 'leaveapi' démarré et accessible sur http://localhost:5000" -ForegroundColor Green
