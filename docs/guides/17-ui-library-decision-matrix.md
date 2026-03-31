# Décision UI Library — Phoenix Emballages

> Décision prise : Sprint 0
> Statut : ✅ Validé — PrimeNG retenu pour tout le MVP
> Ne pas modifier sans ADR.

---

## Contexte de la décision

Phoenix Emballages est une application avec **deux faces distinctes** :

1. **Site public** (catalogue, configurateur, tunnel commande) — orienté conversion, design brandé orange/dark Phoenix.
2. **Back-office admin** (Kanban commandes, gestion devis, catalogue, stats) — orienté efficacité opérationnelle, data-heavy.

La librairie UI doit servir les deux faces sans créer une dette technique.

---

## Matrice de décision — scores pondérés

| Critère | Poids | PrimeNG | Material | Custom |
|---|---:|---:|---:|---:|
| Time-to-market | 5 | **5 → 25** | 4 → 20 | 2 → 10 |
| Richesse DataTable/Form | 5 | **5 → 25** | 3 → 15 | 3 → 15 |
| Besoin branding fort | 4 | 3 → 12 | 2 → 8 | **5 → 20** |
| Maintenabilité long terme | 5 | **4 → 20** | 4 → 20 | 2 → 10 |
| Courbe apprentissage (Abdel) | 4 | **4 → 16** | 3 → 12 | 3 → 12 |
| Accessibilité out-of-the-box | 4 | **4 → 16** | 4 → 16 | 2 → 8 |
| Coût de personnalisation | 3 | 3 → 9 | 3 → 9 | **5 → 15** |
| Cohérence avec stack Angular | 5 | **5 → 25** | 4 → 20 | 3 → 15 |
| **Score total pondéré** | | **148** | 120 | 105 |

---

## Décision : ✅ PrimeNG

### Justification

- **Admin back-office** : PrimeNG Table, Kanban (OrderList), Charts, Dropdowns, DatePicker — tout est prêt. Gain de temps énorme sur les 40h estimées d'admin.
- **Site public** : Les composants PrimeNG sont entièrement thémables via CSS variables — le design system Phoenix (orange #E8552A, dark #0C0E13) s'applique globalement.
- **Configurateur** : PrimeNG ne touche pas au canvas Konva.js — cohabitation parfaite.
- **Formulaires** : PrimeNG + Reactive Forms = validation riche avec messages d'erreur cohérents.
- **Time-to-market** : Le MVP doit être livré en ~13 semaines. PrimeNG réduit le dev front d'environ 30%.

### Contre-arguments assumés (trade-offs)

- PrimeNG est plus lourd qu'un custom design system épuré — le theming CSS variables compense via tree-shaking.
- Le look "PrimeNG par défaut" doit être systématiquement overridé pour le site public — convention dans `05-frontend-standards-angular.md`.

---

## Règles d'utilisation dans le projet

### Site public (features: home, catalog, configurator, cart, payment, quotes)
```
- PrimeNG utilisé pour : Button, InputText, Select, Dialog, Toast, ProgressBar
- PrimeNG ÉVITÉ pour : layouts, grilles, cards visuelles (CSS custom Phoenix)
- Thème : PrimeNG Lara Dark + overrides CSS variables Phoenix
- Jamais de style PrimeNG par défaut visible côté client — tout doit être brandé
```

### Back-office admin (features: admin/*)
```
- PrimeNG utilisé librement : Table, Kanban (OrderList dragdrop), Chart, 
  Calendar, MultiSelect, FileUpload, Tag, Badge, Menu, Sidebar
- Thème : PrimeNG Lara Dark (cohérent avec le reste)
- Efficacité avant esthétique — l'admin est pour les employés, pas les clients
```

### Interdictions globales
```
- Interdiction d'importer Angular Material en parallèle
- Interdiction d'importer Bootstrap ou Tailwind en parallèle
- Pas de CDN externe de composants — tout via npm
- Exception autorisée : Konva.js (canvas configurateur uniquement)
```

---

## Installation et configuration

```bash
# Installation
npm install primeng @primeng/themes primeicons

# Dans angular.json — styles
"styles": [
  "node_modules/primeicons/primeicons.css",
  "src/styles.scss"
]
```

```typescript
// app.config.ts
import { providePrimeNG } from 'primeng/config';
import { definePreset } from '@primeng/themes';
import Lara from '@primeng/themes/lara';

const PhoenixTheme = definePreset(Lara, {
  semantic: {
    primary: {
      50:  '#fff1ec',
      100: '#ffd5c4',
      200: '#ffb99c',
      300: '#ff9d74',
      400: '#f0724c',
      500: '#E8552A',  // accent Phoenix
      600: '#c94420',
      700: '#aa3316',
      800: '#8b220c',
      900: '#6c1102'
    }
  }
});

export const appConfig: ApplicationConfig = {
  providers: [
    providePrimeNG({
      theme: { preset: PhoenixTheme, options: { darkModeSelector: '.dark' } }
    })
  ]
};
```

---

## Mise à jour des fichiers impactés

- [x] `02-architecture.md` → Frontend stack : PrimeNG ✅
- [x] `05-frontend-standards-angular.md` → Composants autorisés : PrimeNG ✅
- [x] `11-copilot-instructions-template.md` → Contrainte IA : PrimeNG uniquement ✅
- [ ] ADR-002 à rédiger via `12-adr-template.md`

---

## Prompt à donner à Claude pour tout module front

```
Frontend Angular avec bibliothèque UI imposée : PrimeNG.
Thème Phoenix : primary color #E8552A, dark mode par défaut.
Interdiction d'utiliser Material, Bootstrap, Tailwind ou autre lib UI.
Respecter strictement 05-frontend-standards-angular.md.
Produire pages list/detail/create/edit avec états loading/error/empty.
Utiliser PrimeNG Table pour les listes, PrimeNG Form pour les formulaires.
Pour le back-office uniquement — utiliser librement tous les composants PrimeNG.
Pour le site public — utiliser PrimeNG avec overrides CSS Phoenix systématiques.
```