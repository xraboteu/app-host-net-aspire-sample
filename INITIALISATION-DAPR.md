# Guide d'initialisation Dapr - Étapes manuelles

Si le script automatique ne fonctionne pas, suivez ces étapes manuelles :

## Étape 1 : Configurer la politique d'exécution PowerShell

Ouvrez PowerShell en tant qu'administrateur et exécutez :

```powershell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## Étape 2 : Installer Dapr CLI

### Option A : Script d'installation officiel

```powershell
powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
```

### Option B : Chocolatey

```powershell
choco install dapr-cli
```

### Option C : Scoop

```powershell
scoop bucket add dapr
scoop install dapr
```

### Option D : Téléchargement manuel

1. Allez sur https://github.com/dapr/cli/releases
2. Téléchargez la dernière version pour Windows
3. Extrayez l'archive
4. Ajoutez le répertoire au PATH

## Étape 3 : Vérifier l'installation

```powershell
dapr --version
```

Vous devriez voir quelque chose comme : `CLI version: 1.x.x`

## Étape 4 : Initialiser Dapr

```powershell
dapr init
```

Cette commande :
- Installe le runtime Dapr
- Crée le répertoire `%USERPROFILE%\.dapr\`
- Configure les composants par défaut

## Étape 5 : Copier les composants Dapr

Depuis le répertoire du projet, exécutez :

```powershell
# Créer le répertoire s'il n'existe pas
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\.dapr\components"

# Copier les composants
Copy-Item -Path "components\*.yaml" -Destination "$env:USERPROFILE\.dapr\components\" -Force
```

## Étape 6 : Vérifier la configuration

```powershell
# Vérifier le statut de Dapr
dapr status

# Lister les composants
Get-ChildItem "$env:USERPROFILE\.dapr\components\"
```

Vous devriez voir `pubsub.yaml` et `statestore.yaml`.

## Prochaines étapes

Une fois Dapr initialisé et configuré :

1. Lancez la solution : `dotnet run --project src/AppHost`
2. Accédez au dashboard Aspire (généralement `http://localhost:15000`)
3. Testez l'API via Swagger ou curl

## Dépannage

### Dapr n'est pas reconnu après l'installation

- Redémarrez votre terminal PowerShell
- Vérifiez que Dapr est dans votre PATH : `$env:PATH -split ';' | Select-String dapr`

### Erreur lors de `dapr init`

- Assurez-vous que Docker est installé et démarré (si vous utilisez le mode conteneur)
- Vérifiez les permissions d'écriture dans `%USERPROFILE%\.dapr\`

### Les composants ne sont pas trouvés

- Vérifiez que les fichiers YAML sont bien dans `%USERPROFILE%\.dapr\components\`
- Vérifiez les permissions de lecture des fichiers

