import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { ApiService } from '../../../../core/http/api.service';
import { ApiError } from '../../../../shared/models/api-error.model';
import { PaginatedList } from '../../../../shared/models/paginated-list.model';
import { ProductDetail } from '../../../catalog/models/product-detail.model';
import { ProductSummary } from '../../../catalog/models/product-summary.model';
import { CreateProductRequest } from '../models/create-product.request';
import { UpdateProductRequest } from '../models/update-product.request';
import { AddVariantRequest } from '../models/add-variant.request';
import { AddPriceTierRequest, UploadProductImageResponse } from '../models/add-price-tier.request';

@Injectable({ providedIn: 'root' })
export class AdminProductService {
  private readonly api       = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly _products        = signal<ProductSummary[]>([]);
  private readonly _selectedProduct = signal<ProductDetail | null>(null);
  private readonly _isLoading       = signal(false);
  private readonly _isSaving        = signal(false);
  private readonly _error           = signal<ApiError | null>(null);
  private readonly _totalCount      = signal(0);
  private readonly _totalPages      = signal(0);

  readonly products        = this._products.asReadonly();
  readonly selectedProduct = this._selectedProduct.asReadonly();
  readonly isLoading       = this._isLoading.asReadonly();
  readonly isSaving        = this._isSaving.asReadonly();
  readonly error           = this._error.asReadonly();
  readonly totalCount      = this._totalCount.asReadonly();
  readonly totalPages      = this._totalPages.asReadonly();

  loadProducts(params?: Record<string, unknown>): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.api
      .get<PaginatedList<ProductSummary>>('/api/v1/admin/products', params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => {
          this._products.set(result.items);
          this._totalCount.set(result.totalCount);
          this._totalPages.set(result.totalPages);
          this._isLoading.set(false);
        },
        error: (err: ApiError) => {
          this._error.set(err);
          this._isLoading.set(false);
        }
      });
  }

  loadProductById(id: string): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.api
      .get<ProductDetail>(`/api/v1/admin/products/${id}`)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: product => {
          this._selectedProduct.set(product);
          this._isLoading.set(false);
        },
        error: (err: ApiError) => {
          this._error.set(err);
          this._isLoading.set(false);
        }
      });
  }

  createProduct(request: CreateProductRequest): Observable<string> {
    this._isSaving.set(true);
    this._error.set(null);
    return this.api.post<string>('/api/v1/admin/products', request).pipe(
      tap(() => this._isSaving.set(false)),
      catchError(err => {
        this._isSaving.set(false);
        this._error.set(err as ApiError);
        return throwError(() => err);
      })
    );
  }

  updateProduct(id: string, request: UpdateProductRequest): Observable<void> {
    this._isSaving.set(true);
    this._error.set(null);
    return this.api.put<void>(`/api/v1/admin/products/${id}`, request).pipe(
      tap(() => this._isSaving.set(false)),
      catchError(err => {
        this._isSaving.set(false);
        this._error.set(err as ApiError);
        return throwError(() => err);
      })
    );
  }

  deactivateProduct(id: string): Observable<void> {
    return this.api.delete<void>(`/api/v1/admin/products/${id}`);
  }

  addVariant(productId: string, request: AddVariantRequest): Observable<string> {
    return this.api.post<string>(
      `/api/v1/admin/products/${productId}/variants`,
      request
    );
  }

  addPriceTier(
    productId: string,
    variantId: string,
    request: AddPriceTierRequest
  ): Observable<string> {
    return this.api.post<string>(
      `/api/v1/admin/products/${productId}/variants/${variantId}/price-tiers`,
      request
    );
  }

  uploadImage(
    productId: string,
    file: File,
    setAsMain: boolean = true
  ): Observable<UploadProductImageResponse> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('setAsMain', String(setAsMain));
    return this.api.postForm<UploadProductImageResponse>(
      `/api/v1/admin/products/${productId}/image`,
      formData
    );
  }

  clearError(): void {
    this._error.set(null);
  }

  clearSelected(): void {
    this._selectedProduct.set(null);
  }
}
