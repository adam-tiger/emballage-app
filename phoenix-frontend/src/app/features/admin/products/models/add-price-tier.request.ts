export interface AddPriceTierRequest {
  minQuantity: number;
  maxQuantity?: number;
  unitPriceHT: number;
}

export interface UploadProductImageResponse {
  id: string;
  publicUrl: string;
  thumbPublicUrl: string | null;
  isMain: boolean;
}
