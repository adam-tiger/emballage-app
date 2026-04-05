import { ProductFamily } from '../../../catalog/models/product-family.model';

export interface CreateProductRequest {
  sku: string;
  nameFr: string;
  descriptionFr?: string;
  family: ProductFamily;
  isCustomizable: boolean;
  isGourmetRange: boolean;
  isBulkOnly: boolean;
  isEcoFriendly: boolean;
  isFoodApproved: boolean;
  soldByWeight: boolean;
  hasExpressDelivery: boolean;
}
