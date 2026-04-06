import { CustomerAddressDto } from './customer-address.model';

/**
 * DTO du tableau de bord client — miroir de `CustomerDashboardDto` .NET.
 */
export interface CustomerDashboardDto {
  customerId: string;
  fullName: string;
  totalOrders: number;
  pendingOrders: number;
  totalQuotes: number;
  pendingQuotes: number;
  /** Adresse de livraison par défaut, ou null si aucune n'est enregistrée. */
  defaultAddress: CustomerAddressDto | null;
}
