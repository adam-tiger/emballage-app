# Backend Standards (.NET)

## 1) Structure de module (vertical slice)
```txt
<Application>/<Feature>/
  Commands/
  Queries/
  Dtos/
  Validators/
  Mappings/
  Services/
```

```txt
<Domain>/<Feature>/
  Entities/
  ValueObjects/
  Events/
  Repositories/
```

```txt
<Infrastructure>/<Feature>/
  Persistence/
  Repositories/
  Configurations/
```

## 2) Règles de conception
- Entité protège ses invariants (constructeurs/factory methods explicites).
- Pas de logique métier dans les contrôleurs.
- Un use case = une responsabilité.
- Validation d’entrée systématique avant exécution métier.

## 3) Persistence & EF Core
- Configurations via `IEntityTypeConfiguration<T>`.
- Migrations versionnées par feature.
- Index sur colonnes de recherche/tri/fréquence d’accès.
- Pagination serveur obligatoire.

## 4) API & sécurité
- Auth via JWT/OAuth2.
- Politiques d’autorisation par endpoint.
- Contrat d’erreur uniforme:
```json
{
  "code": "string",
  "message": "string",
  "details": [],
  "traceId": "string"
}
```
- Idempotency-Key pour endpoints de création sensibles.

## 5) Observabilité
- Logs structurés avec corrélation (`traceId`).
- Health checks (`/health/live`, `/health/ready`).
- Métriques techniques + métier.

## 6) Qualité
- `nullable enable`.
- Warnings as errors en CI.
- Analyse statique activée.
- Couverture minimale recommandée: 70% sur Application/Domain critiques.

## 7) Checklist PR backend
- [ ] Use case testé.
- [ ] Contrat API documenté.
- [ ] Migration revue.
- [ ] Règles d’autorisation validées.
- [ ] Logs + erreurs cohérents.