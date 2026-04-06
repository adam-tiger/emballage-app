/**
 * DTO d'une adresse de livraison client — miroir de `CustomerAddressDto` .NET.
 */
export interface CustomerAddressDto {
  id: string;
  label: string;
  street: string;
  city: string;
  postalCode: string;
  country: string;
  isDefault: boolean;
}

/**
 * DTO du profil client complet — miroir de `CustomerProfileDto` .NET.
 */
export interface CustomerProfileDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  companyName: string | null;
  /** Valeur string du segment (ex : "FastFood"). */
  segment: string;
  /** Libellé français du segment (ex : "Fast Food & Burger"). */
  segmentLabel: string;
  addresses: CustomerAddressDto[];
  createdAtUtc: string;
}

/** Requête d'ajout d'une nouvelle adresse de livraison. */
export interface AddAddressRequest {
  label: string;
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

/** Requête de mise à jour du profil client. */
export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  companyName?: string;
  segment: string;
}
