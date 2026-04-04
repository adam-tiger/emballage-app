// Enums string — valeurs identiques aux enums .NET sérialisés par JsonStringEnumConverter

/** Familles de produits du catalogue Phoenix (27 familles). */
export enum ProductFamily {
  KraftBagHandled     = 'KraftBagHandled',
  KraftBagSOS         = 'KraftBagSOS',
  GourmetRange        = 'GourmetRange',
  KraftBowl           = 'KraftBowl',
  GreaseproofPaper    = 'GreaseproofPaper',
  ChirashiBowl        = 'ChirashiBowl',
  HingedTray          = 'HingedTray',
  SushiTray           = 'SushiTray',
  MicroTray           = 'MicroTray',
  BioPack             = 'BioPack',
  PastaPouch          = 'PastaPouch',
  SoupPouch           = 'SoupPouch',
  SaucePouch          = 'SaucePouch',
  FriesCone           = 'FriesCone',
  MilkshakeCup        = 'MilkshakeCup',
  CoffeeCup           = 'CoffeeCup',
  DessertPot          = 'DessertPot',
  PizzaBox            = 'PizzaBox',
  GreaseproofBag      = 'GreaseproofBag',
  SandwichBag         = 'SandwichBag',
  ReusableBag         = 'ReusableBag',
  Napkin              = 'Napkin',
  WoodenCutlery       = 'WoodenCutlery',
  Bottle              = 'Bottle',
  GarbageBag          = 'GarbageBag',
  FoodWrap            = 'FoodWrap',
  HygieneMisc         = 'HygieneMisc'
}

/** Segments client cibles. */
export enum CustomerSegment {
  FastFood               = 'FastFood',
  BakeryPastry           = 'BakeryPastry',
  JapaneseAsian          = 'JapaneseAsian',
  BubbleTea              = 'BubbleTea',
  RetailCommerce         = 'RetailCommerce',
  FoodTruck              = 'FoodTruck',
  Catering               = 'Catering',
  ChocolateConfectionery = 'ChocolateConfectionery',
  PizzaShop              = 'PizzaShop',
  Other                  = 'Other'
}

/** Face d'impression d'une variante. */
export enum PrintSide {
  SingleSide = 'SingleSide',
  DoubleSide = 'DoubleSide'
}

/** Nombre de couleurs d'impression d'une variante. */
export enum ColorCount {
  One      = 'One',
  Two      = 'Two',
  Three    = 'Three',
  FourCMYK = 'FourCMYK'
}

/** DTO famille retourné par GET /api/v1/products/families. */
export interface ProductFamilyDto {
  value: string;
  labelFr: string;
}

// ── Labels FR pour l'UI ──────────────────────────────────────────────────────

/** Libellés français des familles de produits. */
export const PRODUCT_FAMILY_LABELS: Record<string, string> = {
  KraftBagHandled:     'Sacs Kraft avec anses',
  KraftBagSOS:         'Sacs Kraft SOS sans anses',
  GourmetRange:        'Gamme Gourmet',
  KraftBowl:           'Bols Kraft',
  GreaseproofPaper:    'Papiers Ingraissables',
  ChirashiBowl:        'Bols Chirashi',
  HingedTray:          'Barquettes à Charnière',
  SushiTray:           'Barquettes Sushi',
  MicroTray:           'Barquettes Micro-ondables',
  BioPack:             'Boîtes Bio',
  PastaPouch:          'Pots à Pâtes',
  SoupPouch:           'Pots à Soupes',
  SaucePouch:          'Pots à Sauce',
  FriesCone:           'Cornets à Frites',
  MilkshakeCup:        'Gobelets Milkshake',
  CoffeeCup:           'Gobelets Carton',
  DessertPot:          'Pots à Desserts',
  PizzaBox:            'Boîtes Pizza',
  GreaseproofBag:      'Sachets Ingraissables',
  SandwichBag:         'Sachets Sandwich',
  ReusableBag:         'Sacs Bretelles Réutilisables',
  Napkin:              'Serviettes',
  WoodenCutlery:       'Couverts Bois',
  Bottle:              'Bouteilles',
  GarbageBag:          'Sacs Poubelle',
  FoodWrap:            'Film Alimentaire',
  HygieneMisc:         'Hygiène & Divers'
};

/** Libellés français des segments client. */
export const CUSTOMER_SEGMENT_LABELS: Record<string, string> = {
  FastFood:               'Fast Food & Burger',
  BakeryPastry:           'Boulangerie & Pâtisserie',
  JapaneseAsian:          'Japonais & Asiatique',
  BubbleTea:              'Bubble Tea',
  RetailCommerce:         'Commerce & Retail',
  FoodTruck:              'Food Truck',
  Catering:               'Traiteur & Événementiel',
  ChocolateConfectionery: 'Chocolaterie & Confiserie',
  PizzaShop:              'Pizzéria',
  Other:                  'Autre activité'
};

/** Emoji par segment pour l'UI. */
export const CUSTOMER_SEGMENT_EMOJI: Record<string, string> = {
  FastFood:               '🍔',
  BakeryPastry:           '🥐',
  JapaneseAsian:          '🍣',
  BubbleTea:              '🧋',
  RetailCommerce:         '🛍️',
  FoodTruck:              '🚚',
  Catering:               '🎉',
  ChocolateConfectionery: '🍫',
  PizzaShop:              '🍕',
  Other:                  '📦'
};
