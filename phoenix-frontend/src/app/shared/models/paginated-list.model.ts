/** Résultat paginé générique retourné par les endpoints de liste. */
export interface PaginatedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
