# Script d'installation et de configuration de Dapr pour Windows
# Executez ce script en tant qu'administrateur

Write-Host "=== Configuration Dapr pour .NET Aspire ===" -ForegroundColor Cyan
Write-Host ""

# Verifier et configurer la politique d'execution si necessaire
$currentPolicy = Get-ExecutionPolicy -Scope CurrentUser
if ($currentPolicy -eq "Restricted") {
    Write-Host "Configuration de la politique d'execution PowerShell..." -ForegroundColor Yellow
    Set-ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
    Write-Host "Politique d'execution configuree: RemoteSigned" -ForegroundColor Green
}

# Verifier si Dapr est deja installe
Write-Host "Verification de l'installation Dapr..." -ForegroundColor Yellow
$daprInstalled = Get-Command dapr -ErrorAction SilentlyContinue

if ($daprInstalled) {
    Write-Host "Dapr CLI est deja installe" -ForegroundColor Green
    dapr --version
} else {
    Write-Host "Dapr CLI n'est pas installe. Installation en cours..." -ForegroundColor Yellow
    
    try {
        # Installation via le script officiel
        Write-Host "Telechargement et installation de Dapr CLI..." -ForegroundColor Yellow
        $installScript = Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1" -UseBasicParsing
        Invoke-Expression $installScript.Content
        
        Write-Host "Dapr CLI installe avec succes" -ForegroundColor Green
        dapr --version
    } catch {
        Write-Host "Erreur lors de l'installation de Dapr CLI" -ForegroundColor Red
        Write-Host "Erreur: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Vous pouvez installer Dapr manuellement via:" -ForegroundColor Yellow
        Write-Host "  - Chocolatey: choco install dapr-cli" -ForegroundColor Yellow
        Write-Host "  - Scoop: scoop bucket add dapr; scoop install dapr" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Ou telechargez depuis: https://github.com/dapr/cli/releases" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host ""
Write-Host "=== Initialisation de Dapr ===" -ForegroundColor Cyan

# Verifier si Dapr est deja initialise
$daprDir = "$env:USERPROFILE\.dapr"
if (Test-Path $daprDir) {
    Write-Host "Dapr semble deja initialise dans $daprDir" -ForegroundColor Yellow
    $response = Read-Host "Voulez-vous reinitialiser Dapr? (o/N)"
    if ($response -eq "o" -or $response -eq "O") {
        Write-Host "Reinitialisation de Dapr..." -ForegroundColor Yellow
        dapr uninstall --all
        dapr init
    } else {
        Write-Host "Conservation de la configuration existante" -ForegroundColor Green
    }
} else {
    Write-Host "Initialisation de Dapr..." -ForegroundColor Yellow
    dapr init
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors de l'initialisation de Dapr" -ForegroundColor Red
    exit 1
}

Write-Host "Dapr initialise avec succes" -ForegroundColor Green

Write-Host ""
Write-Host "=== Configuration des composants Dapr ===" -ForegroundColor Cyan

# Creer le repertoire components s'il n'existe pas
$componentsDir = "$env:USERPROFILE\.dapr\components"
if (-not (Test-Path $componentsDir)) {
    New-Item -ItemType Directory -Path $componentsDir -Force | Out-Null
    Write-Host "Repertoire components cree: $componentsDir" -ForegroundColor Green
}

# Copier les composants
$sourceDir = Join-Path $PSScriptRoot "components"
if (Test-Path $sourceDir) {
    $components = Get-ChildItem -Path $sourceDir -Filter "*.yaml"
    
    foreach ($component in $components) {
        $destPath = Join-Path $componentsDir $component.Name
        Copy-Item -Path $component.FullName -Destination $destPath -Force
        Write-Host "Composant copie: $($component.Name)" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Composants Dapr configures:" -ForegroundColor Cyan
    Get-ChildItem -Path $componentsDir -Filter "*.yaml" | ForEach-Object {
        Write-Host "  - $($_.Name)" -ForegroundColor White
    }
} else {
    Write-Host "Repertoire components introuvable: $sourceDir" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Verification finale ===" -ForegroundColor Cyan

# Verifier le statut de Dapr
Write-Host "Statut Dapr:" -ForegroundColor Yellow
dapr status

Write-Host ""
Write-Host "=== Configuration terminee! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Prochaines etapes:" -ForegroundColor Cyan
Write-Host "1. Lancez la solution: dotnet run --project src/AppHost" -ForegroundColor White
Write-Host "2. Accedez au dashboard Aspire (generalement http://localhost:15000)" -ForegroundColor White
Write-Host "3. Testez l API via Swagger ou curl" -ForegroundColor White
Write-Host ""
