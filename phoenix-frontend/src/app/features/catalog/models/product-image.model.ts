/** Image produit stockée dans Azure Blob Storage et servie via CDN. */
export interface ProductImage {
  id: string;
  publicUrl: string;
  thumbPublicUrl: string | null;
  isMain: boolean;
}
