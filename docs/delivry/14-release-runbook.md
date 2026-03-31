# Release Runbook (Production)

## Objectif
Standardiser un déploiement fiable, traçable, réversible.

## 1) Pré-requis
- CI verte sur commit release
- Version validée (`major.minor.patch`)
- Changelog prêt
- Migrations DB revues
- Fenêtre de déploiement validée
- Responsable release désigné

## 2) Pre-release checklist
- [ ] Tag release créé
- [ ] Artefacts signés/publiés
- [ ] Variables d’environnement vérifiées
- [ ] Secrets/config à jour
- [ ] Plan de communication prêt

## 3) Procédure de release
1. Geler merges non critiques
2. Déployer en staging
3. Lancer smoke tests staging
4. Valider monitoring (logs, métriques, erreurs)
5. Déployer en production (canary/rolling recommandé)
6. Vérifier santé applicative (`/health/live`, `/health/ready`)
7. Vérifier parcours métier critiques
8. Annoncer disponibilité release

## 4) Smoke tests minimaux
- Authentification utilisateur
- Création et consultation d’une ressource clé
- Recherche/pagination d’une liste
- Vérification permission role-based
- Vérification traces/logs corrélés

## 5) Monitoring post-release (J+0)
- Taux erreurs (5xx/4xx)
- Latence p95/p99
- Saturation CPU/RAM
- Exceptions métier nouvelles
- KPI produit critique (activation/transaction)

## 6) Rollback
### Déclencheurs
- Erreurs critiques non maîtrisées
- Régression métier bloquante
- Dégradation performance majeure durable

### Procédure rollback
1. Stopper rollout
2. Revenir à la version N-1
3. Vérifier intégrité DB (rollback migration si applicable)
4. Relancer smoke tests
5. Communiquer incident + statut

## 7) Post-mortem (si incident)
- Timeline
- Root cause
- Correctifs immédiats
- Actions préventives (owner + deadline)

## 8) Templates de communication
### Message interne (pré-release)
"Release [VERSION] prévue à [HEURE], scope: [MODULES], risque: [FAIBLE/MOYEN/ÉLEVÉ], rollback prêt: [OUI/NON]."

### Message interne (post-release)
"Release [VERSION] déployée avec succès à [HEURE]. Monitoring renforcé jusqu’à [HEURE]."
