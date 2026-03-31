# Database Standards & Strategy

## 1) Choix de base
- Type DB: [PostgreSQL / PostgreSQL / MySQL]
- Stratégie migration: EF Core migrations versionnées.
- Politique backup/restauration: RPO/RTO documentés.

## 2) Modélisation
- Normalisation jusqu’à 3NF par défaut.
- Dénormalisation seulement justifiée performance.
- Clés techniques + contraintes d’unicité métier.

## 3) Conventions schéma
- PK: `Id`
- FK: `<EntityName>Id`
- Audit: `CreatedAtUtc`, `UpdatedAtUtc`, `CreatedBy`, `UpdatedBy`
- Soft delete (si requis): `IsDeleted`, `DeletedAtUtc`

## 4) Intégrité
- Contraintes NOT NULL partout où possible.
- CHECK constraints pour bornes métier stables.
- FK obligatoires sauf cas explicitement justifiés.

## 5) Performance
- Index:
  - colonnes de recherche fréquente
  - FKs
  - colonnes de tri/pagination
- Requêtes N+1 interdites.
- Requêtes longues surveillées et optimisées.

## 6) Sécurité & conformité
- Chiffrement au repos/en transit.
- Données sensibles pseudonymisées/anonymisées si possible.
- Politique de rétention/suppression conforme réglementaire.

## 7) Seed & environnements
- Seeds minimaux pour dev/tests.
- Données de test non sensibles.
- Scripts reproductibles par environnement.

## 8) Checklist DB
- [ ] Migrations idempotentes.
- [ ] Index revus.
- [ ] Contraintes métier appliquées.
- [ ] Plan rollback documenté.