# Product Vision — Phoenix Emballages

> Document vivant — Version 1.0 — Sprint 0
> Statut : À valider avec Taieb avant démarrage Sprint 1

---

## 1) Contexte

- **Produit** : Application web SPA full-stack permettant la vente en ligne, la personnalisation d'emballages et la gestion opérationnelle de l'entreprise Phoenix Emballages.
- **Problème principal à résoudre** : Taieb reçoit seulement 2 commandes en ligne depuis la création de ses sites. 100% de ses nouvelles commandes passent par téléphone. Son positionnement (petites séries, 5 jours, recto-verso, multi-couleurs) est invisible sur le web alors que c'est son avantage compétitif majeur sur Kedypack.
- **Cibles utilisateurs (personas)** :
  - **Pedro** — Restaurateur qui lance son food truck. Budget serré, pas de stockage, veut 100 sacs avec son logo rapidement. Ne peut pas commander 500 à 10 000 pièces en grande série ailleurs.
  - **Sophie** — Gérante de boulangerie artisanale. Réapprovisionne tous les 2 mois. Veut commander sans appeler, voir ses factures, récommander en 1 clic.
  - **Taieb** — Propriétaire Phoenix. Veut gérer son entreprise depuis son téléphone (ou la plage), déléguer à ses 2 employés et suivre en live sans être dans la boucle opérationnelle.
  - **Employé Phoenix** — Gère la production au quotidien. A besoin d'un outil clair pour traiter les commandes et les devis sans dépendre de Taieb.
- **Contraintes réglementaires / sécurité** :
  - RGPD : données clients (emails, adresses, logos) stockées sur Azure EU.
  - PCI-DSS : aucune donnée de carte stockée côté application — délégué à Stripe.
  - CGV / mentions légales obligatoires sur le site public.
  - Logos clients uploadés : validation format + taille, stockage Azure Blob privé.

---

## 2) Objectifs business (12 mois)

- **Objectif 1** : Passer de 2 commandes en ligne/mois à 50+ commandes en ligne/mois dans les 6 mois suivant le lancement.
- **Objectif 2** : Éliminer le téléphone comme canal principal de prise de commande — objectif 80% des nouvelles commandes en self-service.
- **Objectif 3** : Approcher le million d'euros de CA annuel grâce à l'acquisition de nouveaux clients B2B via le SEO, le configurateur et le bouche-à-oreille de Taieb.

---

## 3) KPIs (mesurables)

- **Activation** : Nombre de premières commandes en ligne par semaine (cible : 10/sem à M+3).
- **Rétention** : Taux de réachat à 60 jours (cible : > 40% des clients ayant commandé).
- **Time-to-value** : Temps entre arrivée sur le site et soumission d'une commande ou d'un devis (cible : < 5 minutes).
- **Taux de conversion devis → commande** : (cible : > 35%).
- **Taux d'erreur** : Commandes bloquées ou annulées pour problème technique (cible : < 1%).
- **Satisfaction utilisateur** : Note NPS collectée post-livraison (cible : > 50).

---

## 4) Scope MVP

### In ✅
- Site public responsive : homepage, catalogue par métier, fiches produits
- Configurateur de personnalisation 2D (upload logo + aperçu sur l'emballage)
- Calcul de prix dynamique par palier de quantité
- Formulaire de devis en ligne avec accusé de réception automatique
- Tunnel de commande complet avec paiement Stripe (carte) + virement
- Authentification client (inscription / connexion / mot de passe oublié)
- Espace client : historique commandes, suivi statut, factures PDF, récommande 1 clic
- Back-office admin : Kanban commandes, gestion devis, catalogue produits, fiche clients
- Tableau de bord admin : KPIs CA, commandes actives, devis à traiter
- Emails transactionnels automatisés (confirmation commande, statut, devis)
- SEO technique : SSR Angular, sitemap, meta tags par page, landing pages par métier
- Hébergement Azure, CI/CD, HTTPS, backups automatiques

### Out ❌ (V2 ou plus)
- Paiement fractionné Alma (V2 — intégration après validation Stripe)
- Configurateur 3D (trop complexe pour MVP — 2D suffisant)
- Application mobile native (PWA suffisant pour MVP)
- Multi-langue (FR uniquement MVP)
- Marketplace / revendeurs
- Gestion des stocks en temps réel avec alertes
- Intégration ERP / comptabilité (Sage, Pennylane)
- Module marketing / emailing campagnes
- Chat en direct support client

---

## 5) Principes produit

- **Clarté avant exhaustivité** : chaque page doit avoir un seul objectif clair — convertir ou informer.
- **Mobile-first** : 60%+ du trafic food truck / restauration arrive sur mobile.
- **Self-service total** : un client doit pouvoir commander sans jamais appeler Taieb.
- **Taieb doit pouvoir déléguer** : toute action opérationnelle doit être réalisable par un employé depuis l'admin.
- **Sécurité et conformité dès le départ** : RGPD, HTTPS, validation inputs, pas de secret côté front.
- **Performance visible** : temps de chargement < 2s — critique pour le SEO et la conversion.

---

## 6) User journeys clés

1. **Pedro (nouveau client)** : Arrive via TikTok → voit "dès 50 pièces (boîtes pizza) à 250+ pcs selon produit, 5 jours" → clique sur Fast Food → choisit sac kraft → configure son logo → voit l'aperçu → calcule le prix pour 100 pièces → commande et paie en ligne → reçoit confirmation email → suit sa commande depuis son espace client.

2. **Sophie (cliente fidèle)** : Se connecte → va dans "mes commandes" → clique "récommander" sur sa dernière commande → vérifie la quantité → paie → reçoit confirmation. Durée cible : moins de 2 minutes.

3. **Prospect hésitant** : Arrive sur le site → configure un emballage → ne veut pas payer directement → clique "Demander un devis" → reçoit un devis PDF par email sous 24h → accepte → la commande est créée automatiquement.

4. **Taieb (admin, depuis son téléphone)** : Reçoit une notification nouvelle commande → ouvre l'admin mobile → voit le Kanban → assigne la commande à un employé → change le statut "En production" → le client reçoit une notification automatique. Taieb n'a rien eu à faire manuellement.

5. **Employé Phoenix** : Se connecte à l'admin → voit ses commandes assignées → télécharge le fichier logo client → valide le BAT → change le statut → génère le bon de livraison PDF.

---

## 7) Contraintes techniques

- **Stack imposée** : Angular 21+ (SSR), ASP.NET Core 10, EF Core 10, PostgreSQL
- **Hébergement** : Azure App Service (backend), Azure Static Web Apps ou App Service (frontend SSR), Azure Blob Storage (logos, PDFs), Azure SQL Database
- **Intégrations externes** :
  - Stripe (paiement carte, webhooks)
  - SendGrid (emails transactionnels)
  - Azure Blob Storage (fichiers clients)
  - Alma (paiement fractionné — V2)
- **SLA/SLO** :
  - Disponibilité cible : 99.5% (hors maintenance planifiée)
  - Temps de réponse API p95 : < 500ms
  - Chargement page : < 2s sur 4G

---

## 8) Risques & mitigations

| Risque | Impact | Mitigation |
|---|---|---|
| Taieb change d'avis sur le scope en cours de dev | Élevé | Scope MVP figé et signé avant Sprint 1. Toute demande hors scope = V2. |
| Configurateur 2D trop complexe à implémenter | Moyen | Commencer par un aperçu simplifié (placement logo sur image statique du produit) — Konva.js en V1. |
| SEO lent à produire des résultats | Moyen | Démarrer SSR dès Sprint 1. Lancer Google Ads en parallèle au lancement. |
| Stripe refus de compte (délais validation) | Moyen | Créer le compte Stripe dès Sprint 0 — délais peuvent prendre 1-2 semaines. |
| Taieb ne forme pas ses employés | Faible | Formation incluse dans la livraison. Guide utilisateur admin généré. |
| Qualité fichiers logos clients insuffisante | Faible | Validation format obligatoire à l'upload (SVG/PDF/PNG HD). Message d'aide clair. |

---

## 9) Critères d'acceptation produit

- [x] Chaque user story a un critère Given/When/Then documenté dans le backlog.
- [ ] Chaque KPI est instrumenté (Google Analytics 4 + événements custom).
- [x] Le périmètre MVP est figé — toute évolution passe par une validation explicite.
- [ ] Taieb a validé les wireframes des 5 écrans clés avant démarrage du dev.
- [ ] Le compte Stripe est créé et validé avant Sprint 2.
- [ ] Les fichiers de logos de Taieb pour les produits existants sont fournis avant Sprint 1.

---

## 10) Corrections et précisions issues du catalogue réel 2023

> Section ajoutée après réception du catalogue PDF et des photos produits.

### Minimums de commande réels (ne pas mettre 50 pcs comme minimum global)
- Sacs kraft avec anses : **250 à 500 pcs** selon format
- Gamme Gourmet : **200 à 1500 pcs** selon référence
- Boîtes pizza : **50 à 100 pcs** (seul produit à 50 pcs min)
- Bols kraft : **300 pcs**
- Papiers ingraissables : **1000 pcs** ou **10 kg**

### Délais réels
- Standard : **5 à 10 jours** (catalogue annonce "délais max 10 jours")
- Express : **24h** disponible (Île-de-France)
- Livraison **7j/7**

### Deux adresses physiques réelles
- Entrepôt Gennevilliers : 16 rue Adrienne, 92230
- Dépôt Suresnes : 7 rue Emile Duclaux, 92150

### Galerie de réalisations réelles (à mettre en avant sur le site)
Photos de réalisations clients visibles dans le catalogue :
- "Well Done" (burger box + sac kraft)
- "Bob's FoodCourt" (sac kraft personnalisé)
- "Alpha Burger" (papier ingraissable + burger box full-cover bleu)
- "Turahe" (burger box couleur or)
→ Ces réalisations sont un argument de vente fort — créer une section galerie.

### Certifications à afficher sur le site
- ✓ Agréé Contact Alimentaire
- ✓ Produit 100% Biodégradable (sur gamme éco)
- ✓ Région Île-de-France
- ✓ Promotion : 5 produits achetés = 1 offert