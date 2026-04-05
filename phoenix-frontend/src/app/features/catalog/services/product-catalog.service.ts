import { inject, Injectable, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ApiService } from '../../../core/http/api.service';
import { ApiError } from '../../../shared/models/api-error.model';
import { PaginatedList } from '../../../shared/models/paginated-list.model';
import { ProductDetail } from '../models/product-detail.model';
import { ProductSummary } from '../models/product-summary.model';
import { ProductFilters, DEFAULT_PRODUCT_FILTERS } from '../models/product-filters.model';
import { ProductFamily } from '../models/product-family.model';

@Injectable({ providedIn: 'root' })
export class ProductCatalogService {
  private readonly api = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);

  // --- Private state signals ---
  private readonly _products = signal<ProductSummary[]>([]);
  private readonly _selectedProduct = signal<ProductDetail | null>(null);
  private readonly _filters = signal<ProductFilters>({ ...DEFAULT_PRODUCT_FILTERS });
  private readonly _totalCount = signal<number>(0);
  private readonly _totalPages = signal<number>(0);
  private readonly _isLoadingList = signal<boolean>(false);
  private readonly _isLoadingDetail = signal<boolean>(false);
  private readonly _listError = signal<ApiError | null>(null);
  private readonly _detailError = signal<ApiError | null>(null);
  private readonly _families = signal<ProductFamily[]>([]);

  // --- Public readonly signals ---
  readonly products = this._products.asReadonly();
  readonly selectedProduct = this._selectedProduct.asReadonly();
  readonly filters = this._filters.asReadonly();
  readonly totalCount = this._totalCount.asReadonly();
  readonly totalPages = this._totalPages.asReadonly();
  readonly isLoadingList = this._isLoadingList.asReadonly();
  readonly isLoadingDetail = this._isLoadingDetail.asReadonly();
  readonly listError = this._listError.asReadonly();
  readonly detailError = this._detailError.asReadonly();
  readonly families = this._families.asReadonly();

  // --- Computed signals ---
  readonly hasProducts = computed(() => this._products().length > 0);
  readonly currentPage = computed(() => this._filters().page);
  readonly hasPreviousPage = computed(() => this._filters().page > 1);
  readonly hasNextPage = computed(() => this._filters().page < this._totalPages());
  readonly isEmpty = computed(() => !this._isLoadingList() && this._products().length === 0 && !this._listError());

  loadProducts(): void {
    this._isLoadingList.set(true);
    this._listError.set(null);

    const f = this._filters();
    this.api
      .get<PaginatedList<ProductSummary>>('/api/v1/products', {
        page: f.page,
        pageSize: f.pageSize,
        sortBy: f.sortBy,
        sortDir: f.sortDir,
        family: f.family,
        segment: f.segment,
        isCustomizable: f.isCustomizable,
        isEcoFriendly: f.isEcoFriendly,
        hasExpressDelivery: f.hasExpressDelivery,
        searchText: f.searchText
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => {
          this._products.set(result.items);
          this._totalCount.set(result.totalCount);
          this._totalPages.set(result.totalPages);
          this._isLoadingList.set(false);
        },
        error: (err: ApiError) => {
          this._listError.set(err);
          this._isLoadingList.set(false);
        }
      });
  }

  loadProductById(id: string): void {
    this._isLoadingDetail.set(true);
    this._detailError.set(null);
    this._selectedProduct.set(null);

    this.api
      .get<ProductDetail>(`/api/v1/products/${id}`)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: product => {
          this._selectedProduct.set(product);
          this._isLoadingDetail.set(false);
        },
        error: (err: ApiError) => {
          this._detailError.set(err);
          this._isLoadingDetail.set(false);
        }
      });
  }

  loadFamilies(): void {
    this.api
      .get<ProductFamily[]>('/api/v1/products/families')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: families => this._families.set(families),
        error: () => { /* non-critical */ }
      });
  }

  changePage(page: number): void {
    this._filters.update(f => ({ ...f, page }));
    this.loadProducts();
  }

  applyFilters(partial: Partial<ProductFilters>): void {
    this._filters.update(f => ({ ...f, ...partial, page: 1 }));
    this.loadProducts();
  }

  resetFilters(): void {
    this._filters.set({ ...DEFAULT_PRODUCT_FILTERS });
    this.loadProducts();
  }

  clearError(): void {
    this._listError.set(null);
    this._detailError.set(null);
  }

  clearSelectedProduct(): void {
    this._selectedProduct.set(null);
    this._detailError.set(null);
  }
}
