# Composants Dapr

Ce dossier contient les composants Dapr utilisés par la solution.

## Configuration

Les composants sont configurés pour utiliser Redis sur `localhost:6379`.

### Note importante

Si vous utilisez Docker avec Aspire, vous devrez peut-être ajuster la configuration Redis dans les fichiers YAML pour pointer vers le bon hôte Redis :

- Si Redis est dans un conteneur Docker : utilisez le nom du service (ex: `redis:6379`)
- Si Redis est sur votre machine locale : utilisez `localhost:6379`

Pour utiliser ces composants avec Dapr, copiez-les dans le répertoire de composants Dapr :

```bash
# Sur Windows
copy components\*.yaml %USERPROFILE%\.dapr\components\

# Sur Linux/Mac
cp components/*.yaml ~/.dapr/components/
```

Ou utilisez l'option `--components-path` lors du démarrage de Dapr.

