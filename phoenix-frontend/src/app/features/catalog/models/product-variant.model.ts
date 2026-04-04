import { PriceTier } from './price-tier.model';
import { ColorCount, PrintSide } from './product-family.model';

/** Variante d'impression d'un produit. */
export interface ProductVariant {
  id: string;
  sku: string;
  nameFr: string;
  minimumOrderQuantity: number;
  printSide: PrintSide;
  colorCount: ColorCount;
  printCoefficient: number;
  priceTiers: PriceTier[];
}

// ── Labels FR ────────────────────────────────────────────────────────────────

/** Libellés français des faces d'impression. */
export const PRINT_SIDE_LABELS: Record<PrintSide, string> = {
  [PrintSide.SingleSide]: 'Recto uniquement',
  [PrintSide.DoubleSide]: 'Recto-verso ✦ Exclusif Phoenix'
};

/** Libellés français du nombre de couleurs. */
export const COLOR_COUNT_LABELS: Record<ColorCount, string> = {
  [ColorCount.One]:      '1 couleur',
  [ColorCount.Two]:      '2 couleurs',
  [ColorCount.Three]:    '3 couleurs',
  [ColorCount.FourCMYK]: '4 couleurs CMJN'
};

// ── Coefficients d'impression (miroir du domaine .NET) ───────────────────────

/** Coefficients multiplicateurs liés à la face d'impression. */
export const PRINT_COEFFICIENTS: Record<PrintSide, number> = {
  [PrintSide.SingleSide]: 1.00,
  [PrintSide.DoubleSide]: 1.15
};

/** Coefficients multiplicateurs liés au nombre de couleurs. */
export const COLOR_COEFFICIENTS: Record<ColorCount, number> = {
  [ColorCount.One]:      1.00,
  [ColorCount.Two]:      1.10,
  [ColorCount.Three]:    1.18,
  [ColorCount.FourCMYK]: 1.25
};
