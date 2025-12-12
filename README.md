# Solution .NET Aspire avec Dapr

Solution de démonstration illustrant l'orchestration de microservices avec .NET Aspire et Dapr.

## Architecture

```
Gateway.Api → Orders.Api → Dapr Pub/Sub → Billing.Worker → Dapr State Store (Redis)
```

## Prérequis

- .NET 8 ou .NET 9 SDK
- Dapr CLI installé et initialisé
- Docker (pour Redis via Aspire)

### Installation de Dapr CLI (Windows)

Si Dapr CLI n'est pas installé, utilisez PowerShell en tant qu'administrateur :

```powershell
# Télécharger et installer Dapr CLI
powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
```

Ou utilisez Chocolatey :

```powershell
choco install dapr-cli
```

Ou utilisez Scoop :

```powershell
scoop bucket add dapr
scoop install dapr
```

Après l'installation, redémarrez votre terminal et vérifiez :

```bash
dapr --version
```

## Structure

- `src/AppHost/` - Projet AppHost .NET Aspire orchestrant tous les services
- `src/Gateway.Api/` - Point d'entrée HTTP pour les clients
- `src/Orders.Api/` - API de gestion des commandes (publie des événements Dapr)
- `src/Billing.Worker/` - Worker consommant les événements et gérant la facturation
- `components/` - Composants Dapr (Pub/Sub et State Store)

## Démarrage

1. **Installez Dapr CLI** (si ce n'est pas déjà fait) - voir section Prérequis ci-dessus

2. **Initialisez Dapr** :
   ```bash
   dapr init
   ```
   Cette commande installe Dapr runtime et crée le répertoire `~/.dapr/` avec les composants par défaut.

3. **Configurez les composants Dapr** :
   
   Copiez les composants de ce projet dans le répertoire Dapr :
   
   **Windows (PowerShell)** :
   ```powershell
   Copy-Item -Path "components\*.yaml" -Destination "$env:USERPROFILE\.dapr\components\" -Force
   ```
   
   **Linux/Mac** :
   ```bash
   cp components/*.yaml ~/.dapr/components/
   ```
   
   Ou utilisez l'option `--components-path` lors du démarrage de vos services avec Dapr.

4. **Lancez la solution via AppHost** :
   ```bash
   dotnet run --project src/AppHost
   ```

3. Accédez au dashboard Aspire (généralement sur `http://localhost:15000`)

4. Testez l'API :
   - Via Gateway : `POST http://localhost:5XXX/orders` (port visible dans Aspire Dashboard)
   - Via Orders.Api directement : `POST http://localhost:5XXX/orders`
   - Consultez la facturation : `GET http://localhost:5XXX/billing/{orderId}`

## Flux de données

1. Un client envoie une commande via `POST /orders` sur `Gateway.Api`
2. `Gateway.Api` délègue à `Orders.Api`
3. `Orders.Api` génère un `orderId` et publie un événement `order.created` dans Dapr Pub/Sub
4. `Billing.Worker` consomme l'événement via son abonnement Dapr
5. Le worker simule une facturation et sauvegarde l'état dans Dapr State Store (Redis)
6. L'état peut être consulté via `GET /billing/{orderId}`

## Composants Dapr

Les composants Dapr sont configurés dans le dossier `components/` :
- `pubsub.yaml` - Pub/Sub basé sur Redis
- `statestore.yaml` - State Store basé sur Redis

Ces composants utilisent l'instance Redis orchestrée par Aspire.

