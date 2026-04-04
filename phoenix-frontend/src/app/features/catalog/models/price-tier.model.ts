/** Palier tarifaire d'une variante d'impression. */
export interface PriceTier {
  id: string;
  minQuantity: number;
  maxQuantity: number | null;
  unitPriceHT: number;
}

/**
 * Retourne le palier tarifaire correspondant à la quantité demandée,
 * ou `null` si aucun palier ne couvre cette quantité.
 */
export function getPriceForQuantity(
  tiers: PriceTier[],
  quantity: number
): PriceTier | null {
  return (
    tiers.find(
      (t) =>
        quantity >= t.minQuantity &&
        (t.maxQuantity === null || quantity <= t.maxQuantity)
    ) ?? null
  );
}

/**
 * Formate un montant en euros selon la locale française.
 * Ex : 1234.5 → "1 234,50 €"
 */
export function formatEur(amount: number): string {
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'EUR'
  }).format(amount);
}
