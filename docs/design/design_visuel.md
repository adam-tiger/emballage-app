Tu es un expert senior en Product Design, UX/UI, e-commerce B2B, conversion CRO, Angular 21, SCSS design systems, et interfaces premium inspirées des meilleures SPA du marché (Stripe, Shopify, Linear, Netflix pour la fluidité et les mises en avant produits).

rappel : Contexte projet
Je développe une application web premium pour une entreprise d’emballages personnalisés / impression packaging. L’objectif n’est pas seulement de faire un beau site, mais de construire une machine de conversion qui donne envie de demander un devis, personnaliser un emballage, commander rapidement, et inspirer confiance aux professionnels.

Stack technique :
- Frontend : Angular 21
- Backend : .NET 10
- Styling : prime ng, SCSS global + composants Angular
- Design actuel : dark theme premium
- Modules déjà existants :
  - homepage
  - catalogue produits
  - fiche produit
  - admin produits
  - admin devis / commandes / clients ( prévus)

Ce que j’attends de toi
Je veux que tu refondes/améliores l’expérience utilisateur et le design visuel pour atteindre un niveau “wow”, premium, moderne, vendeur, crédible et orienté conversion.

Je veux que tu appliques toutes les recommandations ci-dessous directement dans les composants, styles SCSS, structure de layout, hiérarchie visuelle, boutons, cartes, contrastes, labels, filtres, animations, etc.

Objectif business
Le site doit :
1. inspirer immédiatement confiance
2. paraître haut de gamme
3. être plus émotionnel et plus vendeur
4. pousser l’utilisateur à cliquer rapidement sur les CTA
5. mieux présenter les produits comme des solutions business, pas juste des articles
6. donner à l’entreprise une image de leader sérieux du packaging
7. augmenter fortement les demandes de devis et les personnalisations

Positionnement UX à viser
Ne pense pas “catalogue technique”.
Pense :
- “machine à vendre”
- “expérience premium”
- “friction minimale”
- “guidage invisible”
- “business réel + conversion”
- “Shopify / Stripe / Netflix-like browsing experience”

Diagnostic du design actuel à corriger
Le design actuel est déjà propre et premium, mais il souffre encore de plusieurs limites :
- dark theme trop uniforme, trop plat, trop froid
- manque de contraste émotionnel
- manque de rythme visuel entre sections
- catalogue trop statique
- cards produits trop “catalogue propre” et pas assez “achat / envie”
- fiches produit trop rationnelles et pas assez orientées projection métier
- CTA pas encore assez puissants
- sidebar catalogue un peu trop lourde et “admin-like”
- manque d’éléments de réassurance, d’urgence et de preuve sociale
- images produits pas encore assez valorisées
- manque de micro-interactions premium

Décision design importante
Je veux conserver une base dark premium, MAIS la rendre plus chaude, plus vivante, plus contrastée, plus commerciale et plus attrayante.
Le dark n’est pas supprimé, il est “humanisé”.

Palette à mettre en place
Remplace/ajuste le design system global avec une palette plus vibrante, plus contrastée et plus adaptée à un univers packaging / alimentaire / commerce B2B premium.

Utilise cette base :

:root {
  --phoenix-bg: #0B0D12;
  --phoenix-surface: #151922;
  --phoenix-surface2: #1F2433;
  --phoenix-surface3: #262D3D;
  --phoenix-border: #2C3447;

  --phoenix-accent: #FF5A2F;
  --phoenix-accent-hover: #FF6A45;
  --phoenix-accent2: #FFB020;
  --phoenix-accent-glow: rgba(255, 90, 47, 0.18);
  --phoenix-gradient: linear-gradient(135deg, #FF5A2F 0%, #FFB020 100%);

  --phoenix-text: #F5F1E8;
  --phoenix-text-soft: #D8D2C8;
  --phoenix-muted: #8A93A5;
  --phoenix-muted-2: #667085;

  --phoenix-success: #22C55E;
  --phoenix-warning: #F59E0B;
  --phoenix-danger: #EF4444;
  --phoenix-info: #38BDF8;

  --phoenix-card-shadow: 0 10px 30px rgba(0, 0, 0, 0.35);
  --phoenix-hover-shadow: 0 16px 40px rgba(255, 90, 47, 0.18);

  --phoenix-radius-sm: 12px;
  --phoenix-radius-md: 16px;
  --phoenix-radius-lg: 22px;

  --phoenix-container-max: 1440px;
}

Règles couleurs / contraste
- le fond global reste dark
- les CTA principaux doivent utiliser le gradient accent
- les hover des cartes et boutons doivent avoir un léger glow orange
- les zones importantes doivent ressortir par contraste
- les textes secondaires doivent rester lisibles, pas trop gris
- éviter tout rendu trop terne ou “dashboard froid”
- les images produits doivent idéalement être présentées sur fonds clairs ou légèrement chauds, pas sur gris fade

Direction visuelle globale
Je veux une expérience visuelle premium, moderne et orientée vente :
- plus de relief
- plus de profondeur
- plus de clarté dans les CTA
- plus de projection métier
- plus de sensation “produit désirable”
- plus de respiration visuelle

Style à appliquer globalement
- coins arrondis premium
- spacing généreux
- hiérarchie typographique forte
- badges plus visibles
- hover states raffinés
- animations courtes et élégantes
- transitions fluides
- ombres subtiles mais premium
- bordures fines, propres
- accent orange maîtrisé et haut de gamme
- éviter l’effet “template bootstrap sombre”
- viser un rendu premium propriétaire

Typographie
Conserver DM Sans ou une police équivalente premium.
Appliquer une vraie hiérarchie :
- titres section : forts, larges, premium
- noms produits : très lisibles, plus affirmés
- prix : très visibles, plus impactants
- textes secondaires : bien lisibles mais non dominants
- labels et catégories : petits caps ou styles de sur-titres élégants

Règles UX majeures à implémenter
1. Réduire la friction
- rendre les parcours plus évidents
- mieux guider l’utilisateur
- faire ressortir l’action principale à chaque écran

2. Renforcer la projection
- chaque produit doit apparaître comme une solution pour un métier / un besoin
- sur fiche produit, ajouter des blocs de réassurance et d’usage

3. Renforcer la conversion
- CTA plus forts
- badges plus utiles
- prix plus lisibles
- éléments de réassurance visibles
- messages d’urgence et de confiance

4. Créer plus de rythme visuel
- casser la monotonie dark
- utiliser des surfaces différenciées
- ajouter des zones plus respirantes
- éventuellement certaines sections claires ou plus contrastées sur homepage

Demandes précises par écran

==================================================
1. HOMEPAGE
==================================================

Objectif :
Créer une homepage premium, plus vendeuse, plus claire, plus business.

Le hero doit être retravaillé pour :
- plus d’impact émotionnel
- meilleur copywriting
- CTA plus forts
- meilleure hiérarchie

Recommandations homepage :
- conserver un hero dark premium
- rendre le texte plus vendeur
- CTA principal en gradient : “Voir le catalogue”
- CTA secondaire sobre : “Demander un devis”
- ajouter sous le hero ou dans le hero :
  - preuves de confiance
  - livraison rapide
  - made in France
  - petites séries possibles
  - personnalisation simple

Ajouter après le hero des sections structurées :
1. “Ils nous font confiance” / logos clients (même placeholders si besoin)
2. “Nos best-sellers” avec cards horizontales ou carrousel
3. “Comment ça marche” en 3 étapes :
   - je choisis mon emballage
   - j’envoie mon logo / ma demande
   - nous imprimons et livrons
4. “Nos réalisations” avec visuels concrets ou placeholders premium
5. “Pourquoi choisir Phoenix” avec arguments business
6. bloc final CTA très fort

Je veux une homepage qui donne envie d’aller plus loin immédiatement.

==================================================
2. CATALOGUE PRODUITS
==================================================

Objectif :
Faire passer le catalogue de “grid statique propre” à “expérience de browsing premium et vendeuse”.

À faire :
- améliorer la sidebar ou la transformer partiellement en filtres plus modernes
- rendre le catalogue plus vivant
- rendre les cards plus désirables
- améliorer la lecture du prix et des tags
- ajouter des actions plus visibles

Recommandations catalogue :
- rendre les cards plus premium
- hover state :
  - légère élévation
  - glow subtil
  - bordure accentuée
  - image légèrement zoomée
- ajouter boutons ou CTA rapides au hover :
  - “Voir le produit”
  - “Personnaliser”
- badges plus explicites :
  - Best seller
  - Nouveau
  - Gourmet
  - Personnalisable
  - Express 24h
  - Éco-responsable

Les prix doivent être plus visibles et plus vendeurs.
La structure d’une card produit doit mieux hiérarchiser :
- famille
- nom produit
- badges
- prix
- minimum de commande
- CTA

Le catalogue doit aussi mieux guider :
- proposer sections ou regroupements type :
  - top ventes
  - restauration rapide
  - sushi / asiatique
  - boulangerie
  - boissons chaudes

Si c’est cohérent avec l’architecture existante, ajoute des rangées type carrousel ou sections mises en avant inspirées de Netflix / e-commerce premium.

Les filtres doivent être plus sexy :
- pills / chips plus visuels
- meilleure lisibilité
- meilleure interaction hover/selected
- moins de sensation “panneau d’admin”
- conserver la puissance du filtre, mais avec une UX plus moderne

==================================================
3. FICHE PRODUIT
==================================================

Objectif :
Transformer la fiche produit en page qui donne envie d’acheter ou de demander un devis immédiatement.

Constat actuel :
- rationnelle
- propre
- mais trop froide
- manque de projection
- placeholder visuel pas assez séduisant

À faire absolument :
1. améliorer énormément le visuel produit
2. augmenter la réassurance
3. rendre les CTA plus convaincants
4. créer plus d’émotion et de projection

Structure attendue :
- gros visuel produit premium
- si possible fond clair ou beige très léger derrière le produit
- mockup plus valorisant
- possibilité d’afficher une zone “Votre logo ici” si pertinent
- galerie ou miniatures si possible
- à défaut, meilleur composant placeholder premium

Dans la colonne info :
- nom produit fort
- badges plus premium
- description plus lisible
- bloc prix très lisible
- tableau tarifaire plus élégant
- CTA principal ultra visible
- CTA secondaire clair
- infos de réassurance visibles

Modifier les CTA actuels :
- remplacer “Personnaliser mon emballage” par quelque chose de plus orienté action et conversion
Propositions :
- “Créer mon emballage maintenant”
- “Je personnalise en 2 min”

CTA secondaire :
- “Demander un devis gratuit”

Sous les CTA, afficher de manière plus claire :
- devis sans engagement
- réponse rapide
- livraison 5 à 10 jours
- fabrication française
- paiement sécurisé

Ajouter des blocs utiles type :
- “Idéal pour”
- “Pourquoi choisir ce produit”
- “Disponible en personnalisation”
- “Adapté aux métiers suivants”

Exemples de contenu UX :
Idéal pour :
- sushi
- poke
- dark kitchen
- restauration premium

Pourquoi choisir ce produit :
- excellente présentation
- compatible alimentaire
- bonne tenue
- impression possible

Le pricing block doit être plus premium :
- meilleur espacement
- contraste plus fort
- total plus mis en avant
- ligne active mieux visible

==================================================
4. ADMIN PRODUITS
==================================================

Objectif :
Conserver le côté pro, mais améliorer encore le polish premium.

Constat :
L’admin est déjà très bon et constitue une vraie force produit.

À faire :
- améliorer légèrement le raffinement visuel
- conserver la lisibilité maximale
- rendre les tableaux plus élégants
- améliorer les formulaires
- harmoniser avec le design system premium

Recommandations admin :
- plus de contraste maîtrisé
- inputs plus raffinés
- meilleures bordures
- meilleurs états focus
- tableaux plus respirants
- boutons d’action plus propres
- badges statut mieux designés
- meilleure gestion du vide / alignements / spacing

Important :
L’admin doit rester efficace avant tout.
Ne pas sacrifier l’ergonomie au profit du visuel.

==================================================
5. MICRO-INTERACTIONS ET MOTION
==================================================

Ajouter des micro-interactions premium :
- hover cartes
- hover boutons
- transitions sur badges
- focus states sur inputs
- petits effets de montée ou glow
- animations rapides, sobres, modernes
- pas de gadgets inutiles
- pas de lenteur
- pas d’exagération

Transitions recommandées :
- 180ms à 250ms
- ease-out ou cubic-bezier premium
- animation légère seulement

==================================================
6. COPYWRITING UX
==================================================

Je veux que le texte dans les composants soit aussi amélioré pour mieux convertir.

Ligne éditoriale :
- premium
- claire
- rassurante
- rapide
- business
- orientée résultats

Exemples :
- “Vos emballages à votre image”
- “Petites séries, livraison rapide”
- “Personnalisez en quelques clics”
- “Recevez votre devis rapidement”
- “Pensé pour les professionnels exigeants”

Évite les formulations fades ou trop génériques.

==================================================
7. ACCESSIBILITÉ ET LISIBILITÉ
==================================================

- vérifier les contrastes de texte
- éviter les gris trop faibles
- CTA bien visibles
- navigation claire
- focus visible clavier
- états hover/selected nets
- taille des textes cohérente
- responsive propre

==================================================
8. MOBILE / RESPONSIVE
==================================================

Je veux que les améliorations restent premium sur mobile.
Le catalogue doit rester agréable sur téléphone.
Les CTA ne doivent jamais être perdus.
La fiche produit doit rester très lisible.

==================================================
9. LIVRABLE ATTENDU
==================================================

Je veux que tu modifies directement le code du projet pour produire :
- un design system SCSS plus premium
- une homepage plus vendeuse
- un catalogue plus vivant
- une fiche produit plus persuasive
- un admin légèrement plus raffiné
- de meilleurs hover / contrastes / CTA / badges / spacing

Méthode de travail attendue
1. Audite la structure Angular existante
2. Identifie les composants impactés
3. Propose une stratégie claire de refonte
4. Implémente progressivement
5. Mets à jour les SCSS globaux et composants
6. Harmonise les styles
7. Améliore les textes si nécessaire
8. Garde le code propre, maintenable et cohérent

Si certaines images réelles n’existent pas encore :
- utilise placeholders premium crédibles
- structure le composant pour accueillir de vrais médias ensuite

Contraintes
- ne casse pas l’architecture existante
- reste cohérent avec Angular 21
- reste cohérent avec la logique métier existante
- ne fais pas une refonte gadget
- optimise pour la conversion et le rendu premium
- code propre, clair et réutilisable

Priorité absolue
Le site doit donner l’impression d’une marque sérieuse, haut de gamme, moderne, rapide, fiable, faite pour les professionnels, et donner envie d’acheter ou demander un devis immédiatement.

Commence par :
1. analyser la structure actuelle
2. proposer un plan de refonte écran par écran
3. puis implémenter les changements les plus impactants visuellement et commercialement
4. en priorité : design system, homepage, catalogue, fiche produit