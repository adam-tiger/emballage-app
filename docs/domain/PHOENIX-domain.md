# Domaine Métier — Phoenix Emballages

> Version 2.0 — Mis à jour avec le catalogue réel 2023 + photos produits
> Ce fichier est la source de vérité du vocabulaire métier.
> Tout nom d'entité, de propriété ou de règle métier dans le code DOIT respecter ce glossaire.
> Langue du code : EN — traductions FR indiquées pour référence client/UI.

---

## 1) Glossaire métier (Ubiquitous Language)

| Terme EN (code) | Terme FR (UI/client) | Définition |
|---|---|---|
| `Product` | Produit | Un emballage générique catalogué |
| `ProductFamily` | Famille de produit | Groupe de produits similaires |
| `ProductVariant` | Variante | Format + couleur + poignée d'un produit |
| `CustomizableProduct` | Produit personnalisable | Peut recevoir une impression logo client |
| `BulkProduct` | Produit emballage vrac | Vendu sans personnalisation, grandes séries |
| `GourmetRange` | Gamme Gourmet | Ligne premium boîtes alimentaires personnalisables |
| `CustomizationJob` | Personnalisation | Configuration d'un produit avec logo client |
| `PrintSide` | Impression | `SingleSide` (recto) ou `DoubleSide` (recto-verso) |
| `ColorCount` | Nombre de couleurs | 1, 2, 3 ou 4 couleurs CMJN |
| `BAT` | Bon à Tirer | Validation visuelle obligatoire avant impression |
| `Order` | Commande | Achat validé et payé |
| `OrderLine` | Ligne de commande | Un produit configuré dans une commande |
| `Quote` | Devis | Demande de prix sans paiement immédiat |
| `Customer` | Client | Entreprise ou particulier avec un compte |
| `Employee` | Employé | Collaborateur Phoenix avec accès admin |
| `PriceTier` | Palier de prix | Tranche de quantité avec prix unitaire |
| `MinimumOrderQuantity` | Minimum de commande | Qté minimale par SKU (varie selon le produit) |
| `PromotionRule` | Promotion | Ex: 5 produits achetés = 1 offert |
| `SoldByWeight` | Vendu au KG | Papiers ingraissables — prix au kilo, pas à la pièce |

---

## 2) Informations entreprise réelles

```
Raison sociale  : Phoenix Emballages
Email principal : Phoenix.Emballages@gmail.com
Email contact   : contact.phoenix@gmail.com
Tél 1           : 09.85.22.21.64
Tél 2           : 06.17.66.28.07
Instagram       : @phoenix.emballages
Site actuel     : www.phoenixemballages.fr

Entrepôt principal : 16 rue Adrienne, 92230 Gennevilliers
Dépôt secondaire   : 7 rue Emile Duclaux, 92150 Suresnes

Zone livraison principale : Île-de-France (7j/7, express 24h)
Livraison nationale       : disponible
```

---

## 3) Catalogue produits réel — source de vérité pour le seed DB

> Source : Catalogue_2023_PHOENIX.pdf + 19 photos produits réels
> À utiliser pour le DataSeed et pour les fiches produits de l'application.

### 3.1) ACTIVITÉ PERSONNALISATION (70% CA)

#### Sacs Kraft avec anses (personnalisables)

| SKU | Dimensions | Grammage | Couleur | MOQ | Prix lot |
|---|---|---|---|---|---|
| SAC-BRUN-22x10x28 | 22×10×28cm | 70G | Brun | 250 pcs | 21,80€ |
| SAC-BLANC-22x10x28 | 22×10×28cm | 70G | Blanc | 500 pcs | 52,80€ |
| SAC-BRUN-26x14x33 | 26×14×33cm | 70G | Brun | 250 pcs | 23,90€ |
| SAC-BLANC-26x14x33 | 26×14×33cm | 70G | Blanc | 350 pcs | 42,90€ |
| SAC-BRUN-26x17x29 | 26×17×29cm | 70G | Brun | 250 pcs | 23,90€ |
| SAC-BLANC-26x17x29 | 26×17×29cm | 70G | Blanc | 350 pcs | 42,90€ |
| SAC-BRUN-32x22x25 | 32×22×25cm | 80G | Brun | 250 pcs | 29,90€ |
| SAC-BRUN-32x16x42 | 32×16×42cm | 80G | Brun | 250 pcs | 45,20€ |

**Promotion active : 5 produits achetés = 1 offert**

#### Sacs Kraft SOS sans anses (personnalisables)

| SKU | Dimensions | MOQ | Prix |
|---|---|---|---|
| SAC-SOS-13x8x25 | 13×8×25cm | 500 pcs | 21,90€ |
| SAC-SOS-18x11x34 | 18×11×34cm | 500 pcs | 28,90€ |
| SAC-SOS-20x16x40 | 20×16×40cm | 500 pcs | 35,20€ |

#### Gamme Gourmet (personnalisables) — Produit phare Phoenix

> Photos réelles : boîte burger kraft avec impression multi-couleurs rouge + noir
> Réalisations clients : "Well Done", "Bob's FoodCourt", "Alpha Burger", "Turahe"

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| GOURMET-CREPE | Boîte Crêpe | 400 pcs | 57,90€ |
| GOURMET-CHICKEN-S | Boîte Chicken S | 300 pcs | 36,30€ |
| GOURMET-CHICKEN-M | Boîte Chicken M | 300 pcs | 39,60€ |
| GOURMET-CHICKEN-L | Boîte Chicken L | 300 pcs | 44,40€ |
| GOURMET-WRAP-L | Boîte Wrap L | 800 pcs | 75,40€ |
| GOURMET-WRAP-XL | Boîte Wrap XL | 500 pcs | 56,80€ |
| GOURMET-FRITE-S | Boîte Frite S | 1500 pcs | 69,80€ |
| GOURMET-FRITE-L | Boîte Frite L | 1000 pcs | 59,90€ |
| GOURMET-BURGER-S | Boîte Burger S | 600 pcs | 55,50€ |
| GOURMET-BURGER-L | Boîte Burger L | 500 pcs | 57,90€ |
| GOURMET-BURGER-XL | Boîte Burger XL | 300 pcs | 55,80€ |
| GOURMET-BURGER-XXL | Boîte Burger XXL | 300 pcs | 56,90€ |
| GOURMET-DEMI-SANDWICH | Boîte Demi-Sandwich | 500 pcs | 48,50€ |
| GOURMET-SANDWICH | Boîte Sandwich Gourmet | 200 pcs | 30,90€ |
| GOURMET-SANDWICH-KRAFT | Boîte Sandwich Kraft | 200 pcs | 23,90€ |

#### Bols Kraft + couvercles PP (personnalisables)

| SKU | Volume | MOQ | Prix |
|---|---|---|---|
| BOL-KRAFT-500CC | 500 cc | 300 pcs | 50,70€ |
| BOL-KRAFT-750CC | 750 cc | 300 pcs | 55,90€ |
| BOL-KRAFT-1000CC | 1000 cc | 300 pcs | 58,20€ |
| BOL-KRAFT-1100CC | 1100 cc | 300 pcs | 59,35€ |
| BOL-KRAFT-1200CC | 1200 cc | 300 pcs | 66,99€ |
| BOL-KRAFT-1300CC | 1300 cc | 300 pcs | 67,80€ |

#### Papiers ingraissables (personnalisables AU KG — SoldByWeight = true)

| SKU | Format | MOQ | Prix |
|---|---|---|---|
| PAPER-GREASE-25x35 | Street 25×35cm | 1000 pcs | 18,10€ |
| PAPER-GREASE-33x40 | Street 33×40cm | 1000 pcs | 28,55€ |
| PAPER-FOOD-30x40 | Alimentaire brun 30×40cm | 1000 pcs | 26,50€ |
| PAPER-PLATEAU-32x25 | Plateau 32×25cm | 10 KG | 33,80€ |
| PAPER-PLATEAU-32x50 | Plateau 32×50cm | 10 KG | 36,50€ |

#### Bols Chirashi hermétiques (personnalisables)

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| BOL-CHIRASHI-NOIR-1035ML | Chirashi Noir + couvercle PP | 480 pcs | 89,80€ |
| BOL-SHIRASHI-ROUGE-700ML | Shirashi Rouge + couvercle PP | 300 pcs | 58,50€ |
| BOL-BOBUN-TRANSP-1050ML | Bobun Transparent + couvercle | 300 pcs | 50,10€ |

#### Personnalisation grandes séries (gamme pro catalogue p.18)

```
Sacs personnalisés full-cover  : 1 000 à 10 000 pcs
Papier burger                  : à partir de 100 kg
Boîtes Burger/Frites           : à partir de 25 000 pcs
Boîte sandwich                 : à partir de 20 000 pcs
Étuis Wrap/Tacos               : grandes séries
Boîte Menu enfants             : grandes séries
Sac de Luxe bristol 185g       : à partir de 100 pcs (petite série exceptionnelle)
```

---

### 3.2) ACTIVITÉ EMBALLAGES VRAC (30% CA)

> Produits non personnalisables — vendus tels quels.
> Modèle : `isCustomizable = false`, `isBulkOnly = true`

#### Barquettes à charnière transparentes

| SKU | Volume | MOQ | Prix |
|---|---|---|---|
| BQTE-250ML | 250 ml | 600 pcs | 39,20€ |
| BQTE-375ML | 375 ml | 600 pcs | 41,60€ |
| BQTE-500ML | 500 ml | 900 pcs | 71,50€ |
| BQTE-750ML | 750 ml | 400 pcs | 48,90€ |
| BQTE-1000ML | 1000 ml | 400 pcs | 55,90€ |
| BQTE-1500ML | 1500 ml | 400 pcs | 71,90€ |

#### Barquettes Sushi (noires)

| SKU | Dimensions | MOQ | Prix |
|---|---|---|---|
| SUSHI-SM00 | 140×80×45mm | 800 pcs | 69,50€ |
| SUSHI-SM01 | 150×96×45mm | 600 pcs | 59,60€ |
| SUSHI-SM02 | 230×95×45mm | 400 pcs | 54,25€ |
| SUSHI-SM03 | 170×120×45mm | 400 pcs | 56,85€ |
| SUSHI-SM05 | 190×130×45mm | 400 pcs | 59,50€ |
| SUSHI-SM07 | 220×140×45mm | 400 pcs | 62,60€ |
| SUSHI-SM09 | 245×150×45mm | 300 pcs | 66,60€ |
| SUSHI-SM11 | 265×190×450mm | 200 pcs | 69,25€ |

#### Barquettes micro-ondables (noires 2 compartiments)

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| MARMI-1COMP | 1 compartiment 1000ml + couvercle PP | 400 pcs | 71,20€ |
| MARMI-2COMP | 2 compartiments 1000ml + couvercle | 400 pcs | 70,20€ |

#### Boîtes bio couvercle pliant

| SKU | Volume | MOQ | Prix |
|---|---|---|---|
| BIO-450ML | Bio Pack 450ml | 500 pcs | 54,30€ |
| BIO-780ML | Bio Pack 780ml | 400 pcs | 52,90€ |
| BIO-960ML | Bio Pack 960ml | 300 pcs | 50,25€ |

#### Pots à pâtes / soupes / sauces

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| POT-PATES-480ML | Pot à pâtes 480ml | 500 pcs | 44,92€ |
| POT-PATES-780ML | Pot à pâtes 780ml | 500 pcs | 48,50€ |
| POT-SOUPE-237ML | Soupe 237ml/8oz | 500 pcs | 84,84€ |
| POT-SOUPE-355ML | Soupe 355ml/12oz | 500 pcs | 86,50€ |
| POT-SOUPE-473ML | Soupe 473ml/16oz | 500 pcs | 92,20€ |
| POT-SAUCE-CHARNIERE-30ML | Sauce charnière 30ml | 1000 pcs | 26,90€ |
| POT-SAUCE-30ML | Sauce + couvercle 30ml | 500 pcs | 13,90€ |
| POT-SAUCE-50ML | Sauce + couvercle 50ml | 500 pcs | 16,50€ |

#### Cornets à frites / Bubble Waffle

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| CORNET-163ML | Pot à frites 7,5oz | 1000 pcs | 84,90€ |
| CORNET-660ML | Pot à frites 16oz | 1000 pcs | 89,90€ |
| BARQ-BATEAU-350ML | Barquette kraft bateaux 350ml | 1000 pcs | 54,50€ |
| BARQ-BATEAU-500ML | Barquette kraft bateaux 500ml | 1000 pcs | 58,70€ |

#### Boîtes pizza

| SKU | Taille | MOQ | Prix |
|---|---|---|---|
| PIZZA-T26 | T26 | 100 pcs | 12,50€ |
| PIZZA-T29 | T29 | 100 pcs | 14,50€ |
| PIZZA-T31 | T31 | 100 pcs | 16,20€ |
| PIZZA-T33 | T33 | 100 pcs | 16,80€ |
| PIZZA-T40 | T40 | 50 pcs | 12,90€ |

#### Sachets ingraissables / sandwichs

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| SACHET-POULET | Sachet poulet ingraissable | 500 pcs | 27,52€ |
| SACHET-SMASH-15x15 | Sachet fendu smash 15×15cm | 1000 pcs | 22,15€ |
| SACHET-CREPE-17x18 | Sachet fendu crêpe 17×18cm | 1000 pcs | 28,50€ |
| SACHET-FRITES-11x4x14 | Sachet frites 11×4×14cm | 2000 pcs | 25,90€ |
| SACHET-FRITES-12x7x18 | Sachet frites 12×7×18cm | 500 pcs | 16,90€ |
| SACHET-SANDWICH-FENETRE | Sachet sandwich fenêtre | 1000 pcs | 22,50€ |
| SACHET-SANDWICH | Sachet sandwich | 1000 pcs | 18,50€ |
| SACHET-SANDWICH-IMPRIME | Sachet sandwich imprimé | 1000 pcs | 19,25€ |
| SACHET-FRITES-14x7x22 | Sachet frites 14×7×22cm | 1000 pcs | 26,90€ |

#### Gobelets milkshake + café/eau

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| GOB-MILKSHAKE-355ML | Milkshake 355ml/12oz | 800 pcs | 81,15€ |
| GOB-MILKSHAKE-473ML | Milkshake 473ml/16oz | 800 pcs | 88,30€ |
| PAILLE-CARTON-8OZ | Paille carton 8oz | 5000 pcs | 78,80€ |
| PAILLE-JUS-6OZ | Paille jus carton 6oz | 5000 pcs | 72,30€ |
| GOB-CAFE-120ML | Gobelet café 4oz | 1000 pcs | 23,35€ |
| GOB-CAFE-200ML | Gobelet café 7oz | 1000 pcs | 26,60€ |
| GOB-EAU-237ML | Gobelet eau 8oz | 1000 pcs | 31,90€ |
| GOB-CHOCO-410ML | Gobelet choco 10oz | 1000 pcs | 49,20€ |
| PORTE-GOB-2 | Porte-gobelet 2 compartiments | 800 pcs | 48,92€ |

#### Pots desserts / Boîtes tarte / Donuts

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| POT-TIRAMISU-9OZ | Pot tiramisu + couvercle | 800 pcs | 59,80€ |
| POT-DESSERT-7OZ | Pot dessert + couvercle 7oz | 800 pcs | 56,50€ |
| POT-DESSERT-150ML | Pot dessert + couvercle 150ml | 800 pcs | 53,10€ |
| BOITE-TARTE | Boîte à tarte | 540 pcs | 66,20€ |
| BOITE-TARTE-NOIR | Boîte à tarte fond noir | 720 pcs | 83,20€ |
| BOITE-DONUTS | Boîte à donuts | 720 pcs | 85,30€ |

#### Sacs bretelles réutilisables

| SKU | Dimensions | MOQ | Prix |
|---|---|---|---|
| SAC-BRETELLE-26x45 | T26×45cm | 500 pcs | 12,99€ |
| SAC-BRETELLE-28x50 | T28×50cm | 500 pcs | 18,10€ |
| SAC-BRETELLE-30x55 | T30×55cm | 500 pcs | 24,30€ |
| SAC-BRETELLE-28x60 | T28×60cm | 500 pcs | 25,20€ |

#### Serviettes / Essuie-tout

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| SERV-2PLIS | Serviette 2 plis | 2500 pcs | 26,25€ |
| SERV-1PLIS | Serviette 1 pli | 2000 pcs | 12,90€ |
| SERV-ROUGE-40x40 | Serviette rouge 40×40 | 1500 pcs | 38,70€ |
| SERV-NOIR-40x40 | Serviette noire 40×40 | 4000 pcs | 52,08€ |
| ESSUIE-ROULEAUX | Essuie-tout rouleaux | 6 rlx | 9,70€ |

#### Couverts bois / Bouteilles / Divers

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| COUV-FOURCHETTE | Fourchette bois | 1000 pcs | 20,30€ |
| COUV-COUTEAU | Couteau bois | 1000 pcs | 21,75€ |
| COUV-CUILLERE-16 | Cuillère bois 16cm | 1000 pcs | 21,90€ |
| COUV-CUILLERE-DESSERT | Cuillère dessert bois 11cm | 1000 pcs | 21,30€ |
| KIT-COUV-3X1 | Kit couverts bois CFS 3X1 | 250 pcs | 23,85€ |
| BAGUETTES-CHINOISES | Baguettes chinoises | 3000 pcs | 56,50€ |
| PIQUE-FRITES | Pique frites bois | 10000 pcs | 48,50€ |
| BOUTEILLE-25CL | Bouteille jus 25cl | 336 pcs | 73,90€ |
| BOUTEILLE-50CL | Bouteille jus 50cl | 140 pcs | 42,69€ |

#### Sacs poubelle / Film alimentaire / Hygiène

| SKU | Produit | MOQ | Prix |
|---|---|---|---|
| SAC-POUBELLE-110L | Sac poubelle 110L | 200 pcs | 28,90€ |
| SAC-POUBELLE-130L | Sac poubelle 130L | 100 pcs | 20,55€ |
| SAC-POUBELLE-160L | Sac poubelle 160L | 100 pcs | 25,95€ |
| FILM-30CM | Film alimentaire 30cm-300m | 1 pcs | 8,57€ |
| FILM-45CM | Film alimentaire 45cm-300m | 1 pcs | 11,99€ |
| FILM-VIOLET-30 | Film violet 30cm × 4 | 4 pcs | 30,00€ |
| FILM-VIOLET-45 | Film violet 45cm × 4 | 4 pcs | 32,50€ |
| TABLIER-JETABLE | Tablier jetable | 1000 pcs | 57,20€ |
| GANTS-NITRYLE | Gants noir nitryle | 100 pcs | 4,50€ |
| LIQUIDE-VAISSELLE | Liquide vaisselle 5L | 1 pcs | 5,50€ |
| SAVON-MAIN | Savon main 5L | 1 pcs | 4,89€ |
| BOBINE-CB | Bobine papier thermique CB | 50 pcs | 20,60€ |
| BOITE-ENFANTS | Kid Box — boîte enfants imprimée | 500 pcs | 98,60€ |

---

## 4) Flags produit (Product entity)

```typescript
isCustomizable: boolean    // Peut recevoir impression logo client
isGourmetRange: boolean    // Appartient à la Gamme Gourmet premium
isBulkOnly: boolean        // Uniquement grandes séries, pas personnalisable
isEcoFriendly: boolean     // Certifié biodégradable/écologique
isFoodApproved: boolean    // Agréé Contact Alimentaire
soldByWeight: boolean      // Vendu au KG (papiers ingraissables)
hasExpressDelivery: boolean // Éligible livraison express 24h IDF
```

---

## 5) Règles de pricing

### Structure des PriceTier
Chaque `ProductVariant` a une liste de `PriceTier` :
```
{ MinQuantity, MaxQuantity (nullable), UnitPriceHT }
```

### Coefficients d'impression (sur le prix de base)
```
PrintSide:
  SingleSide  → ×1.00
  DoubleSide  → ×1.15  (+15% — exclusif Phoenix)

ColorCount:
  One         → ×1.00
  Two         → ×1.10  (+10%)
  Three       → ×1.18  (+18%)
  FourCMYK    → ×1.25  (+25%)
```

### Formule de calcul
```
UnitPriceHT = BasePriceTier.UnitPriceHT × PrintSideCoeff × ColorCountCoeff
TotalHT     = UnitPriceHT × Quantity
TVA         = TotalHT × 0.20
TotalTTC    = TotalHT + TVA
```

### Livraison gratuite
```
Livraison offerte dès 200€ HT de commande.
Express 24h : disponible en Île-de-France uniquement (surcharge à définir avec Taieb).
```

### Promotion 5+1
```
if (orderLine.Quantity >= 5 * product.BaseUnitQuantity)
  → 1 unité offerte (PromotionRule configurable en admin)
```

---

## 6) Workflow commande (Order)

### OrderStatus
```
Received       → Payée, reçue
BAT_Pending    → Attente validation BAT (personnalisables uniquement)
BAT_Validated  → BAT validé, impression autorisée
In_Production  → En fabrication/préparation
Shipped        → Expédiée (numéro de suivi disponible)
Delivered      → Livrée
Cancelled      → Annulée (avant BAT_Validated uniquement)
Refunded       → Remboursée (admin uniquement)
```

### Règle BAT
```
if (orderLine.Product.IsCustomizable && orderLine.HasCustomizationJob)
  → Passer par BAT_Pending
else
  → Aller directement à In_Production
```

### Emails automatiques
```
Received       → "Commande confirmée + récapitulatif"
BAT_Pending    → "Votre BAT est prêt" (lien de validation)
BAT_Validated  → "Impression lancée — livraison sous 5-10 jours"
Shipped        → "Votre commande est en route" + tracking
Delivered      → "Commande livrée — laissez un avis"
Cancelled      → "Commande annulée — remboursement en cours"
```

---

## 7) Workflow devis (Quote)

```
Draft → Sent → Accepted → Converted | Rejected | Expired (30 jours)
```

- Délai de traitement Phoenix : < 24h ouvrées
- Relance auto : J+3 si pas de réponse
- Prix garanti : 30 jours après envoi
- Conversion devis → commande en 1 clic côté admin

---

## 8) Personnalisation (CustomizationJob)

### Formats acceptés
```
SVG (recommandé), PDF vectoriel, PNG ≥300 DPI, AI
Taille max : 20 MB
```

### Rendu 2D
```
Bibliothèque : Konva.js
Fonctions    : upload logo, positionnement, redimensionnement, centrage auto
Modes        : vue recto / vue recto-verso
```

---

## 9) RBAC

```
Customer  → Catalogue, configurateur, commande, paiement, espace client
Employee  → Tout Customer + admin (commandes, devis, BAT, tracking, clients)
Admin     → Tout Employee + catalogue CRUD, employés, stats, remboursements, config
```

---

## 10) Segments clients (CustomerSegment)

```
FastFood | BakeryPastry | JapaneseAsian | BubbleTea |
RetailCommerce | FoodTruck | Catering | ChocolateConfectionery |
PizzaShop | Other
```

---

## 11) Contraintes métier non négociables

1. **MOQ variable par SKU** — `Product.MinimumOrderQuantity` est stocké en base, pas de minimum global.
2. **BAT obligatoire** pour tout produit personnalisable avant impression.
3. **Fichiers logos privés** — accès uniquement via Azure Blob SAS tokens (durée 1h).
4. **Prix devis garanti 30 jours** après envoi.
5. **Annulation impossible** après `BAT_Validated`.
6. **SoldByWeight = true** → prix calculé au KG, pas à la pièce (papiers ingraissables).
7. **Promotion 5+1** → `PromotionRule` configurable par l'admin, pas hardcodée.
8. **Express 24h** disponible uniquement en Île-de-France.
9. **Recto-verso** disponible sur TOUS les produits de la Gamme Gourmet — c'est le différenciateur clé.