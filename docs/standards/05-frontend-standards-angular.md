# Frontend Standards (Angular)

## 1) Architecture feature-first
```txt
src/app/
  core/
    auth/
    http/
    config/
  shared/
    ui/
    pipes/
    directives/
  features/
    customer/
      pages/
      components/
      services/
      models/
      store/
      customer.routes.ts
```

## 2) Règles d’implémentation
- Composants `smart` dans `pages`, `dumb` dans `components`.
- Inputs/Outputs strictement typés.
- Services HTTP par feature.
- Modèles séparés des view-models.
- Chargement/l’erreur/vide gérés explicitement sur toutes les listes.

## 3) Formulaires
- Reactive Forms uniquement.
- Validation sync + async factorisée.
- Messages d’erreurs cohérents et localisables.

## 4) Gestion d’état
- Par défaut: state local composant + RxJS/signals.
- Store global uniquement si état transversal complexe.

## 5) Performance
- `OnPush` par défaut.
- `trackBy` sur boucles.
- Lazy loading des features.
- Eviter les subscriptions manuelles non nettoyées.

## 6) Sécurité front
- Interceptor auth.
- Sanitization templates.
- Jamais de secrets côté front.
- Guards de routes pour permissions.

## 7) UX minimale exigée
- Comportement loading stable.
- Erreur explicite et actionnable.
- Empty state utile.
- Accessibilité de base (labels, focus, contraste).

## 8) Checklist PR front
- [ ] Pas de logique métier lourde en composant.
- [ ] Unité de tests sur composants/services critiques.
- [ ] Routes protégées si nécessaire.
- [ ] Pas de régression responsive sur écrans cibles.