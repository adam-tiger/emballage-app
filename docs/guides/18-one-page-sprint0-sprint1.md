# One-Page Opérationnelle — Sprint 0 / Sprint 1

Usage: checklist ultra-courte pour lancer un nouveau projet Angular + .NET avec le starter kit.

---

## Sprint 0 — Cadrage + socle (objectif: être prêt à développer proprement)

### 1) Produit (max 1 journée)
- [ ] Remplir `01-productvision.md`
- [ ] Fixer MVP In/Out
- [ ] Définir 3 à 5 KPIs
- [ ] Écrire user stories avec critères Given/When/Then

### 2) Architecture (max 1 journée)
- [ ] Valider `02-architecture.md`
- [ ] Figer conventions `03-naming-conventions.md`
- [ ] Valider standards `04` à `07`
- [ ] Choisir UI library via `17-ui-library-decision-matrix.md`
- [ ] Documenter décisions structurantes en ADR (`12-adr-template.md`)

### 3) Setup technique (max 1 journée)
- [ ] Créer/adapter `copilot-instructions.md` depuis `11-copilot-instructions-template.md`
- [ ] Installer pipeline CI depuis `ci-templates/`
- [ ] Activer branch protection sur `main`
- [ ] Exiger checks obligatoires (build/test/lint/security)

### Gate de sortie Sprint 0
- [ ] Docs `01` à `07` validés
- [ ] UI library figée
- [ ] CI verte sur branche principale
- [ ] DoD validée (`09-definition-of-done.md`)

---

## Sprint 1 — Module référence (objectif: golden path industrialisable)

### 1) Génération contrôlée
- [ ] Adapter `10-prompt-master-module.md`
- [ ] Générer 1 module complet (backend + API + frontend + migration + tests)
- [ ] Interdire la sur-ingénierie (MVP strict)

### 2) Qualité d’implémentation
- [ ] Contrats API conformes (`06-api-contract-standards.md`)
- [ ] Validations backend + frontend
- [ ] États UX `loading/error/empty` présents
- [ ] Nommage et structure conformes

### 3) Vérification et merge
- [ ] Build backend/frontend OK
- [ ] Tests unitaires + intégration critiques OK
- [ ] PR review lead (architecture/sécurité/perf)
- [ ] CI gates pass

### Gate de sortie Sprint 1
- [ ] Module de référence livré en qualité prod
- [ ] Pattern validé pour duplication features
- [ ] Runbook release prêt (`14-release-runbook.md`)

---

## Séquence de commande à donner à l’IA (copier/coller)

1. "Aide-moi à compléter `01-productvision.md` pour ce contexte: [X]."
2. "Propose et finalise `02` à `07` selon ce MVP et ces contraintes: [X]."
3. "Applique la matrice `17-ui-library-decision-matrix.md` et tranche la UI library."
4. "Génère le squelette projet (folders/config/pipeline) sans coder de feature métier."
5. "Avec `10-prompt-master-module.md`, génère le premier module complet `[MODULE_NAME]`."
6. "Vérifie la conformité à `09-definition-of-done.md` et corrige les écarts."

---

## Timebox recommandée
- Sprint 0: 2 à 3 jours
- Sprint 1: 3 à 5 jours (selon complexité domaine)

---

## Anti-dérives (rappel)
- [ ] Ne pas générer de module avant validation `01` à `07`
- [ ] Ne pas changer de UI library en cours de MVP
- [ ] Ne pas merger si CI rouge
- [ ] Ne pas accepter de pseudo-code en livraison
