import { ProductFamily } from './product-family.model';

/** Représentation allégée d'un produit pour les listes catalogue. */
export interface ProductSummary {
  id: string;
  sku: string;
  nameFr: string;
  family: ProductFamily;
  familyLabel: string;
  isCustomizable: boolean;
  isGourmetRange: boolean;
  isEcoFriendly: boolean;
  isFoodApproved: boolean;
  soldByWeight: boolean;
  hasExpressDelivery: boolean;
  isActive: boolean;
  mainImageUrl: string | null;
  minUnitPriceHT: number | null;
  minimumOrderQuantity: number | null;
}
