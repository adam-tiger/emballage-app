import { ProductSummary } from './product-summary.model';
import { ProductVariant } from './product-variant.model';
import { ProductImage } from './product-image.model';

/** Représentation complète d'un produit pour la fiche détail. */
export interface ProductDetail extends ProductSummary {
  descriptionFr: string | null;
  variants: ProductVariant[];
  images: ProductImage[];
}
