# Naming Conventions (Obligatoires)

## 1) Principes
- Le vocabulaire suit l’UBIQUITOUS LANGUAGE métier.
- Un concept = un nom stable dans tout le système.
- Pas d’abréviations ambiguës.

## 2) Backend .NET
- Classes/Enums/Records: `PascalCase`.
- Méthodes: `PascalCase`.
- Variables locales/paramètres: `camelCase`.
- Interfaces: préfixe `I` (`ICustomerRepository`).
- Async: suffixe `Async` (`GetByIdAsync`).
- DTOs: suffixes `Request`, `Response`, `Dto`.
- Exceptions métier: suffixe `Exception`.
- Handlers/UseCases: suffixe `Handler` ou verbe explicite (`CreateOrderHandler`).

## 3) Angular
- Fichiers: `kebab-case`.
- Classes TS: `PascalCase`.
- Variables/fonctions/propriétés: `camelCase`.
- Sélecteurs composants: préfixe app (`app-`) + feature (`app-customer-list`).
- Suffixes:
  - `.component.ts`
  - `.service.ts`
  - `.model.ts`
  - `.validator.ts`
  - `.guard.ts`
  - `.interceptor.ts`

## 4) Base de données
- Tables: singulier `PascalCase` ou snake_case (choisir 1 convention pour tout le projet).
- Colonnes FK: `<EntityName>Id`.
- PK: `Id` (GUID recommandé pour distribution; int possible si mono-instance).
- Timestamps: `CreatedAtUtc`, `UpdatedAtUtc`.
- Soft delete: `IsDeleted`, `DeletedAtUtc`.

## 5) API REST
- Routes: pluriel, `kebab-case`.
- Ressources: `/api/customers`.
- Identifiant: `/api/customers/{customerId}`.
- Query params: `camelCase` (`page`, `pageSize`, `sortBy`, `sortDir`).

## 6) Interdits
- Noms génériques (`Manager`, `Helper`, `Utils`) sans contexte.
- Acronymes non documentés.
- Mélange FR/EN dans les noms techniques (choisir une langue).

## À personnaliser
Choisir langue canonique du code: [EN recommandé / FR].