# Guide d’utilisation A à Z du starter kit (Angular + .NET)

Ce document explique **comment utiliser le starter kit de bout en bout**, depuis la définition du besoin jusqu’à la mise en production, avec un focus pratique VS Code.

---

## 0) Démarrage du projet (Jour 0)

### Objectif
Poser un cadre propre avant d’écrire du code.

### Actions
1. Créer le repository cible.
2. Copier le dossier `ai-starter-kit/` à la racine du nouveau projet.
3. Ouvrir le projet dans VS Code.
4. Installer les extensions recommandées:
   - C# / .NET
   - Angular Language Service
   - ESLint
   - Prettier
   - GitLens
5. Créer/adapter `copilot-instructions.md` à partir de `11-copilot-instructions-template.md`.

### Résultat attendu
- Les standards sont explicites dès le départ.
- L’IA produit du code conforme au cadre d’équipe.

---

## 1) Définition du besoin (Produit)

### Document à remplir
- `01-productvision.md`

### Actions
1. Définir le problème principal.
2. Décrire personas et cas d’usage clés.
3. Fixer le scope MVP (In / Out).
4. Définir KPIs de succès.
5. Écrire les user stories avec critères Given/When/Then.

### Règle
Aucun dev tant que le MVP n’est pas cadré.

### Résultat attendu
Backlog priorisé, testable, compréhensible par équipe + IA.

---

## 2) Cadrage architecture (Lead Tech)

### Documents à utiliser
- `02-architecture.md`
- `03-naming-conventions.md`
- `04-backend-standards-dotnet.md`
- `05-frontend-standards-angular.md`
- `06-api-contract-standards.md`
- `07-database.md`

### Actions
1. Valider le style d’architecture (couches, dépendances, découpage feature).
2. Choisir et figer les conventions de nommage.
3. Définir les règles API (routes, erreurs, pagination, versioning).
4. Définir stratégie DB (migrations, index, contraintes).
5. Choisir la bibliothèque UI frontend (PrimeNG, Material ou Custom) avec la matrice `17-ui-library-decision-matrix.md`.
6. Documenter les décisions importantes via ADR (`12-adr-template.md`).

### Résultat attendu
Architecture partagée par tous, stable, avec décisions traçables.

### Moment exact où lancer la création technique
Lancer la génération de l’architecture et du code uniquement **après validation des documents `01` à `07`**.

Séquence recommandée:
1. **D’abord**: cadrage produit + architecture + conventions + choix UI library.
2. **Ensuite**: génération du squelette projet (folders, config, pipelines, standards).
3. **Enfin**: génération du premier module vertical complet via prompt master.

Règle: ne pas demander à l’IA de générer un module complet tant que la UI library n’est pas figée.

---

## 3) Décision UI library (Frontend)

### Document à utiliser
- `17-ui-library-decision-matrix.md`

### Actions
1. Scorer PrimeNG, Material, Custom avec la matrice pondérée.
2. Choisir une seule bibliothèque UI pour tout le MVP.
3. Reporter le choix dans:
   - `02-architecture.md` (frontend stack)
   - `05-frontend-standards-angular.md` (composants autorisés)
   - `11-copilot-instructions-template.md` (contrainte IA)
4. En cas d’exception (mix de libs), créer un ADR obligatoire.

### Recommandation rapide
- Back-office/data-heavy: PrimeNG.
- Produit sobre standard: Angular Material.
- Produit premium fortement brandé: Custom design system.

### Résultat attendu
Choix UI clair, cohérent et reproductible sur tous les modules.

---

## 4) Planification et gouvernance delivery

### Documents à utiliser
- `08-roadmap.md`
- `09-definition-of-done.md`

### Actions
1. Planifier Sprint 0 (cadrage + socle technique).
2. Planifier Sprint 1 (module vertical de référence).
3. Fixer DoR/DoD équipe.
4. Définir les indicateurs engineering (lead time, cycle time, defect rate).

### Résultat attendu
Un rythme de delivery prévisible, avec critères de qualité communs.

---

## 5) Génération du premier module (Golden Path)

### Document à utiliser
- `10-prompt-master-module.md`

### Actions
1. Dupliquer le prompt master.
2. Remplir les placeholders (`[MODULE_NAME]`, règles métier, rôles, stack).
3. Générer un module complet (backend + API + frontend + migration + tests).
4. Revue lead: architecture, nommage, sécurité, contrats API.
5. Corriger jusqu’à conformité DoD.

### Bonnes pratiques
- Commencer par un module représentatif, pas le plus complexe.
- Exiger du code complet (pas de pseudo-code).
- Vérifier cohérence métier avant optimisation technique.

### Résultat attendu
Un module “référence” réutilisable comme modèle pour les suivants.

---

## 6) Setup CI/CD dès le début

### Documents à utiliser
- `13-quality-gates-ci.md`
- `ci-templates/README.md`
- `ci-templates/github-actions/ci.yml`
- `ci-templates/azure-devops/azure-pipelines.yml`

### Actions
1. Copier un pipeline template (GitHub Actions ou Azure DevOps).
2. Adapter versions, chemins et scripts (backend/frontend).
3. Activer branch protection sur `main`.
4. Rendre les checks obligatoires (build/test/lint/security/gate).

### Résultat attendu
Pas de merge de code non conforme.

---

## 7) Routine quotidienne dans VS Code (mode opératoire)

### Avant de coder
1. Lire la user story + critères d’acceptation.
2. Identifier impacts API/DB/front.
3. Vérifier conventions de nommage/archi.

### Pendant le dev
1. Backend: implémenter use case métier puis endpoint puis persistence.
2. Frontend: implémenter service API puis pages `list/detail/create/edit`.
3. Ajouter validations backend + frontend.
4. Gérer systématiquement `loading/error/empty`.

### Avant PR
1. Build local backend/frontend.
2. Lint + tests unitaires + tests intégration critiques.
3. Vérifier checklists DoD.
4. Vérifier migration DB et compatibilité contrat API.

### Pendant PR review
- Revue structure, sécurité, erreurs, performance, maintenabilité.
- Refus de merge si écart standards.

### Après merge
- Vérifier pipeline.
- Surveiller erreurs et métriques.

---

## 8) Passage préproduction et production

### Document à utiliser
- `14-release-runbook.md`

### Procédure type
1. Déployer en staging.
2. Exécuter smoke tests.
3. Vérifier monitoring (logs, latence, erreurs).
4. Déployer prod (canary/rolling recommandé).
5. Vérifier endpoints santé et parcours critiques.

### En cas d’incident
- Déclencher rollback vers N-1.
- Vérifier intégrité DB.
- Communiquer statut.
- Faire post-mortem avec actions préventives.

### Résultat attendu
Mises en production maîtrisées, traçables et réversibles.

---

## 9) Timeline recommandée (premières semaines)

### Semaine 1
- Vision produit
- Architecture
- Conventions
- CI/CD

### Semaine 2
- Module de référence complet
- Validation DoD

### Semaine 3+
- Industrialisation par modules
- Renforcement qualité/perf/sécurité

---

## 10) Anti-patterns à éviter

- Générer du code sans cadre produit clair.
- Mélanger les conventions de nommage.
- Retarder la CI/CD.
- Coder sans contrats API explicites.
- Oublier les états UX `loading/error/empty`.
- Sur-ingénierie hors MVP.

---

## 11) Checklist exécutable ultra-courte

### Kickoff
- [ ] `01-productvision.md` complété
- [ ] `02` à `07` validés
- [ ] UI library choisie avec `17-ui-library-decision-matrix.md`
- [ ] ADR rédigés pour décisions structurantes

### Build du socle
- [ ] Prompt master adapté
- [ ] 1 module référence livré
- [ ] DoD validée

### Industrialisation
- [ ] CI gates actifs
- [ ] Branch protection active
- [ ] Runbook release opérationnel

### Go-live
- [ ] Staging validé
- [ ] Smoke tests OK
- [ ] Monitoring + rollback prêts

---

## Références du kit
- `README.md`
- `01-productvision.md`
- `02-architecture.md`
- `03-naming-conventions.md`
- `04-backend-standards-dotnet.md`
- `05-frontend-standards-angular.md`
- `06-api-contract-standards.md`
- `07-database.md`
- `08-roadmap.md`
- `09-definition-of-done.md`
- `10-prompt-master-module.md`
- `11-copilot-instructions-template.md`
- `12-adr-template.md`
- `13-quality-gates-ci.md`
- `14-release-runbook.md`
- `17-ui-library-decision-matrix.md`
- `ci-templates/README.md`

Ce guide est conçu comme document de référence vivant: adapte-le à chaque nouveau contexte (réglementaire, équipe, scale, architecture cible).