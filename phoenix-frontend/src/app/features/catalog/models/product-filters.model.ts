import { CustomerSegment, ProductFamily } from './product-family.model';

/** Critères de filtrage, tri et pagination pour la liste des produits. */
export interface ProductFilters {
  page: number;
  pageSize: number;
  sortBy: string;
  sortDir: 'asc' | 'desc';
  family?: ProductFamily;
  segment?: CustomerSegment;
  isCustomizable?: boolean;
  searchText?: string;
}

/** Filtres par défaut appliqués au chargement initial de la liste. */
export const DEFAULT_PRODUCT_FILTERS: ProductFilters = {
  page: 1,
  pageSize: 20,
  sortBy: 'NameFr',
  sortDir: 'asc'
};
