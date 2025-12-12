# Guide de configuration - Dapr et .NET Aspire

Ce guide vous accompagne étape par étape pour configurer et lancer la solution.

## Étape 1 : Installation de Dapr CLI

### Windows (PowerShell en tant qu'administrateur)

**Option 1 : Script d'installation officiel**
```powershell
powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
```

**Option 2 : Chocolatey**
```powershell
choco install dapr-cli
```

**Option 3 : Scoop**
```powershell
scoop bucket add dapr
scoop install dapr
```

### Vérification

Après l'installation, redémarrez votre terminal et vérifiez :

```bash
dapr --version
```

Vous devriez voir quelque chose comme : `CLI version: 1.x.x`

## Étape 2 : Initialisation de Dapr

Exécutez la commande suivante :

```bash
dapr init
```

Cette commande :
- Installe le runtime Dapr
- Crée le répertoire `~/.dapr/` (ou `%USERPROFILE%\.dapr\` sur Windows)
- Configure les composants par défaut
- Optionnellement, installe Docker Desktop si nécessaire

### Vérification de l'initialisation

Vérifiez que Dapr est bien initialisé :

```bash
dapr --version
dapr status
```

## Étape 3 : Configuration des composants Dapr

Les composants Dapr de ce projet doivent être copiés dans le répertoire Dapr.

### Windows (PowerShell)

```powershell
# Créer le répertoire s'il n'existe pas
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\.dapr\components"

# Copier les composants
Copy-Item -Path "components\*.yaml" -Destination "$env:USERPROFILE\.dapr\components\" -Force
```

### Linux/Mac

```bash
# Créer le répertoire s'il n'existe pas
mkdir -p ~/.dapr/components

# Copier les composants
cp components/*.yaml ~/.dapr/components/
```

### Vérification

Vérifiez que les fichiers sont bien copiés :

**Windows** :
```powershell
Get-ChildItem "$env:USERPROFILE\.dapr\components\"
```

**Linux/Mac** :
```bash
ls ~/.dapr/components/
```

Vous devriez voir `pubsub.yaml` et `statestore.yaml`.

## Étape 4 : Ajustement de la configuration Redis (si nécessaire)

Si Redis est géré par Aspire dans un conteneur Docker, vous devrez peut-être ajuster les composants Dapr.

Par défaut, les composants pointent vers `localhost:6379`. Si Redis est dans un conteneur Docker avec Aspire, vous pouvez :

1. **Option 1** : Utiliser `localhost:6379` si le port est exposé
2. **Option 2** : Utiliser le nom du service Docker (ex: `redis:6379`) si Dapr s'exécute dans le même réseau Docker
3. **Option 3** : Utiliser l'IP du conteneur Redis

Pour vérifier le port Redis utilisé par Aspire, consultez le dashboard Aspire après le démarrage.

## Étape 5 : Lancement de la solution

1. **Lancez AppHost** :
   ```bash
   dotnet run --project src/AppHost
   ```

2. **Accédez au dashboard Aspire** :
   - Ouvrez votre navigateur sur l'URL affichée dans la console (généralement `http://localhost:15000`)
   - Vous verrez tous les services et leurs ports

3. **Démarrez Dapr pour chaque service** (si nécessaire) :
   
   Pour une utilisation complète avec Dapr, vous devrez démarrer chaque service avec Dapr sidecar :
   
   ```bash
   # Terminal 1 : Orders.Api avec Dapr
   dapr run --app-id orders-api --app-port 5001 --dapr-http-port 3500 -- dotnet run --project src/Orders.Api
   
   # Terminal 2 : Billing.Worker avec Dapr
   dapr run --app-id billing-worker --app-port 5002 --dapr-http-port 3501 -- dotnet run --project src/Billing.Worker
   
   # Terminal 3 : Gateway.Api avec Dapr
   dapr run --app-id gateway-api --app-port 5000 --dapr-http-port 3502 -- dotnet run --project src/Gateway.Api
   ```

   **Note** : Avec .NET Aspire, les services peuvent être démarrés automatiquement. Vérifiez la configuration Aspire pour l'intégration Dapr.

## Étape 6 : Test de la solution

1. **Créer une commande** via Gateway.Api :
   ```bash
   curl -X POST http://localhost:5000/orders \
     -H "Content-Type: application/json" \
     -d '{"customerId": "customer-123", "amount": 99.99}'
   ```

2. **Vérifier les logs** de Billing.Worker pour voir la facturation

3. **Consulter l'état** de la facturation :
   ```bash
   curl http://localhost:5002/billing/{orderId}
   ```

## Dépannage

### Dapr n'est pas reconnu

- Vérifiez que Dapr CLI est dans votre PATH
- Redémarrez votre terminal
- Sur Windows, vérifiez que le répertoire Dapr est dans votre PATH utilisateur

### Erreur de connexion Redis

- Vérifiez que Redis est bien démarré (via Aspire ou Docker)
- Vérifiez le port Redis dans les composants Dapr
- Vérifiez les logs Dapr : `dapr logs -a orders-api`

### Les événements ne sont pas reçus

- Vérifiez que les composants Dapr sont bien copiés
- Vérifiez les logs Dapr pour les erreurs
- Vérifiez que le topic `order.created` est bien configuré dans `pubsub.yaml`

## Ressources

- [Documentation Dapr](https://docs.dapr.io/)
- [Documentation .NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Dapr CLI Reference](https://docs.dapr.io/reference/cli/)

