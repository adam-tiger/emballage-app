import { ColorCount, PrintSide } from '../../../catalog/models/product-family.model';

export interface AddVariantRequest {
  sku: string;
  nameFr: string;
  minimumOrderQuantity: number;
  printSide: PrintSide;
  colorCount: ColorCount;
}
