# Guide de démarrage de la solution

## Étape 1 : Lancer AppHost

La solution est lancée via :

```bash
dotnet run --project src/AppHost
```

Cela démarre :
- Redis (via Aspire)
- Gateway.Api
- Orders.Api
- Billing.Worker
- Dashboard Aspire (généralement sur `http://localhost:15000`)

## Étape 2 : Accéder au Dashboard Aspire

1. Ouvrez votre navigateur sur l'URL affichée dans la console (généralement `http://localhost:15000`)
2. Vous verrez tous les services et leurs ports assignés
3. Vous pouvez consulter les logs, métriques et dépendances de chaque service

## Étape 3 : Démarrer Dapr pour chaque service

**Important** : Pour que Dapr fonctionne correctement (Pub/Sub et State Store), chaque service doit être démarré avec un sidecar Dapr.

### Option A : Démarrer manuellement avec Dapr (recommandé pour les tests)

Dans des terminaux séparés, démarrez chaque service avec Dapr :

```powershell
# Terminal 1 : Orders.Api avec Dapr
$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
dapr run --app-id orders-api --app-port 5001 --dapr-http-port 3500 --components-path "$env:USERPROFILE\.dapr\components" -- dotnet run --project src/Orders.Api

# Terminal 2 : Billing.Worker avec Dapr
$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
dapr run --app-id billing-worker --app-port 5002 --dapr-http-port 3501 --components-path "$env:USERPROFILE\.dapr\components" -- dotnet run --project src/Billing.Worker

# Terminal 3 : Gateway.Api (optionnel avec Dapr)
$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
dapr run --app-id gateway-api --app-port 5000 --dapr-http-port 3502 --components-path "$env:USERPROFILE\.dapr\components" -- dotnet run --project src/Gateway.Api
```

**Note** : Les ports (`--app-port`) doivent correspondre aux ports assignés par Aspire. Vérifiez les ports dans le dashboard Aspire.

### Option B : Utiliser Aspire uniquement (sans Dapr sidecar)

Si vous utilisez uniquement Aspire sans sidecar Dapr, les services démarreront mais :
- Les événements Pub/Sub ne fonctionneront pas
- Le State Store ne fonctionnera pas
- Les appels Dapr échoueront

## Étape 4 : Tester la solution

### Via Swagger

1. Accédez à Swagger sur `Gateway.Api` ou `Orders.Api` (port visible dans Aspire Dashboard)
2. Testez l'endpoint `POST /orders` avec :
   ```json
   {
     "customerId": "customer-123",
     "amount": 99.99
   }
   ```

### Via curl/PowerShell

```powershell
# Créer une commande via Gateway.Api
$body = @{
    customerId = "customer-123"
    amount = 99.99
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5XXX/orders" -Method Post -Body $body -ContentType "application/json"
```

Remplacez `5XXX` par le port réel de Gateway.Api (visible dans Aspire Dashboard).

### Vérifier la facturation

```powershell
# Récupérer l'état de facturation (remplacez {orderId} par l'ID retourné)
Invoke-RestMethod -Uri "http://localhost:5XXX/billing/{orderId}" -Method Get
```

## Vérification du flux complet

1. **Créer une commande** → `POST /orders` sur Gateway.Api
2. **Vérifier les logs** de Orders.Api → doit publier l'événement
3. **Vérifier les logs** de Billing.Worker → doit recevoir l'événement et facturer
4. **Consulter l'état** → `GET /billing/{orderId}` doit retourner l'état de facturation

## Dépannage

### Les services ne démarrent pas

- Vérifiez que .NET SDK est installé : `dotnet --version`
- Vérifiez que tous les packages NuGet sont restaurés : `dotnet restore`
- Consultez les logs dans Aspire Dashboard

### Dapr ne fonctionne pas

- Vérifiez que Dapr CLI est installé : `dapr --version`
- Vérifiez que les composants sont copiés : `Get-ChildItem "$env:USERPROFILE\.dapr\components\*.yaml"`
- Vérifiez les logs Dapr : `dapr logs -a orders-api`

### Erreur de connexion Redis

- Vérifiez que Redis est démarré (via Aspire Dashboard)
- Vérifiez le port Redis dans les composants Dapr (`components/pubsub.yaml` et `components/statestore.yaml`)
- Le port doit correspondre au port Redis utilisé par Aspire (généralement `localhost:6379`)

### Les événements ne sont pas reçus

- Vérifiez que Billing.Worker est démarré avec Dapr sidecar
- Vérifiez que Orders.Api publie bien l'événement (logs)
- Vérifiez les logs Dapr pour les erreurs de Pub/Sub

