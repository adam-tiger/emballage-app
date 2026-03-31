# API Contract Standards

## 1) Principes
- Contract-first: définir requests/responses avant implémentation.
- Versioning explicite: `/api/v1/...`.
- Rétrocompatibilité: pas de breaking change sans nouvelle version.

## 2) Endpoints REST
- `GET /api/v1/resources`
- `GET /api/v1/resources/{id}`
- `POST /api/v1/resources`
- `PUT /api/v1/resources/{id}`
- `PATCH /api/v1/resources/{id}` (optionnel)
- `DELETE /api/v1/resources/{id}`

## 3) Conventions de réponse
### Success
- `200 OK` lecture/maj
- `201 Created` création (+ `Location`)
- `204 No Content` suppression

### Erreurs
- `400` validation
- `401` non authentifié
- `403` non autorisé
- `404` introuvable
- `409` conflit métier
- `422` règle métier invalide
- `500` erreur inattendue

## 4) Enveloppe pagination standard
```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalCount": 120,
  "totalPages": 6
}
```

## 5) Recherche / tri / filtre
- Query params standard:
  - `page`, `pageSize`
  - `sortBy`, `sortDir`
  - `q` (search fulltext légère)
  - filtres explicites (`status`, `fromDate`, `toDate`)

## 6) Documentation
- OpenAPI/Swagger obligatoire.
- Exemples de payloads pour chaque endpoint.
- Décrire erreurs métier spécifiques (`code`).

## 7) Checklist API
- [ ] Schémas request/response validés.
- [ ] Codes HTTP cohérents.
- [ ] Erreurs standardisées.
- [ ] AuthN/AuthZ documentées.
- [ ] Scénarios happy path + edge cases couverts.