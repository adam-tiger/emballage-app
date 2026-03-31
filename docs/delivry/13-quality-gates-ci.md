# Quality Gates CI (Angular + .NET)

## Objectif
Empêcher la régression qualité/sécurité/performance avant merge et release.

## 1) Pipeline minimal obligatoire
1. Restore dependencies
2. Build backend/frontend
3. Lint + format check
4. Unit tests
5. Integration tests (API critiques)
6. Security scan dépendances
7. Publish artifacts

## 2) Gates de merge (PR)
- Build: ✅ obligatoire
- Tests unitaires: ✅ 100% pass
- Tests intégration critiques: ✅ pass
- Lint: ✅ zero error
- Couverture minimale:
  - Domain/Application: >= 70%
  - Global repo: >= 60%
- Security (SAST/dependency):
  - 0 vulnérabilité Critique
  - 0 vulnérabilité Haute non justifiée
- Taille PR recommandée: < 800 lignes nettes (hors snapshots)

## 3) Gates de release
- Tag version semver
- Changelog généré
- Migration DB testée en environnement de staging
- Smoke tests post-deploy validés
- Rollback procedure validée

## 4) Standards outillage recommandés
- .NET:
  - `dotnet restore`
  - `dotnet build -warnaserror`
  - `dotnet test --collect:"XPlat Code Coverage"`
- Angular:
  - `npm ci`
  - `npm run lint`
  - `npm run test -- --watch=false --browsers=ChromeHeadless`
  - `npm run build -- --configuration production`

## 5) Politique de branches
- `main`: protégée, merge via PR uniquement
- `develop` (option): intégration continue
- `feature/*`, `hotfix/*`, `release/*`

## 6) Checklist PR rapide
- [ ] Story liée
- [ ] Tests ajoutés/ajustés
- [ ] Contrat API impacté documenté
- [ ] Migration DB validée
- [ ] Capture des preuves (logs/tests)

## 7) Exemples de policy
- Required checks:
  - `backend-build`
  - `backend-tests`
  - `frontend-build`
  - `frontend-tests`
  - `security-scan`
- Required reviewers: 1 à 2 selon criticité
- Dismiss stale approvals: activé
