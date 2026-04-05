import { TestBed } from '@angular/core/testing';
import {
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { ProductCatalogService } from './product-catalog.service';
import { errorInterceptor } from '../../../core/http/error.interceptor';
import { ProductSummary } from '../models/product-summary.model';
import { ProductFamily } from '../models/product-family.model';

const mockProductSummary: ProductSummary = {
  id: '11111111-0001-0000-0000-000000000000',
  sku: 'SAC-BRUN-01',
  nameFr: 'Sac Kraft Brun',
  family: ProductFamily.KraftBagHandled,
  familyLabel: 'Sac kraft avec anses',
  isCustomizable: true,
  isGourmetRange: false,
  isEcoFriendly: false,
  isFoodApproved: true,
  soldByWeight: false,
  hasExpressDelivery: true,
  isActive: true,
  mainImageUrl: 'https://cdn.phoenix.fr/products/01/main.webp',
  minUnitPriceHT: 0.0872,
  minimumOrderQuantity: 250
};

const pagedResponse = (items: ProductSummary[], total = items.length) => ({
  items,
  page: 1,
  pageSize: 20,
  totalCount: total,
  totalPages: Math.ceil(total / 20)
});

describe('ProductCatalogService', () => {
  let service: ProductCatalogService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ProductCatalogService,
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting()
      ]
    });

    service  = TestBed.inject(ProductCatalogService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  // ── État initial ───────────────────────────────────────────────────────────

  it('should initialize with empty products signal', () => {
    // Assert
    expect(service.products()).toEqual([]);
    expect(service.isLoadingList()).toBe(false);
    expect(service.listError()).toBeNull();
    expect(service.hasProducts()).toBe(false);
  });

  it('should initialize with default filters', () => {
    // Assert
    expect(service.filters().page).toBe(1);
    expect(service.filters().pageSize).toBe(20);
    expect(service.filters().sortDir).toBe('asc');
  });

  // ── loadProducts ──────────────────────────────────────────────────────────

  it('should set isLoadingList to true during loadProducts', () => {
    // Act
    service.loadProducts();

    // Assert — avant flush, isLoading doit être true
    expect(service.isLoadingList()).toBe(true);

    // Cleanup
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));
    expect(service.isLoadingList()).toBe(false);
  });

  it('should update products signal after successful load', () => {
    // Arrange
    service.loadProducts();

    // Act
    const req = httpMock.expectOne(req => req.url.includes('/api/v1/products'));
    req.flush(pagedResponse([mockProductSummary]));

    // Assert
    expect(service.products()).toHaveLength(1);
    expect(service.products()[0].sku).toBe('SAC-BRUN-01');
    expect(service.hasProducts()).toBe(true);
    expect(service.isLoadingList()).toBe(false);
  });

  it('should update totalCount and totalPages after successful load', () => {
    // Arrange
    service.loadProducts();

    // Act
    const req = httpMock.expectOne(req => req.url.includes('/api/v1/products'));
    req.flush(pagedResponse([mockProductSummary, { ...mockProductSummary, id: '2', sku: 'PROD-02' }], 45));

    // Assert
    expect(service.totalCount()).toBe(45);
    expect(service.totalPages()).toBe(3); // ceil(45 / 20) = 3
  });

  it('should set listError signal on HTTP error', () => {
    // Arrange
    service.loadProducts();

    // Act — flush avec statut 404 et corps ApiErrorResponse
    const req = httpMock.expectOne(req => req.url.includes('/api/v1/products'));
    req.flush(
      { code: 'NOT_FOUND', message: 'Ressource introuvable', traceId: 'trace-123' },
      { status: 404, statusText: 'Not Found' }
    );

    // Assert
    expect(service.listError()).not.toBeNull();
    expect(service.listError()?.code).toBe('NOT_FOUND');
    expect(service.listError()?.statusCode).toBe(404);
    expect(service.isLoadingList()).toBe(false);
  });

  it('should clear listError on next successful load', () => {
    // Arrange — provoquer d'abord une erreur
    service.loadProducts();
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(
      { code: 'SERVER_ERROR', message: 'Erreur serveur' },
      { status: 500, statusText: 'Internal Server Error' }
    );
    expect(service.listError()).not.toBeNull();

    // Act — charger à nouveau avec succès
    service.loadProducts();
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));

    // Assert
    expect(service.listError()).toBeNull();
  });

  // ── isEmpty ───────────────────────────────────────────────────────────────

  it('should compute isEmpty correctly when no products and no error', () => {
    // Act
    service.loadProducts();
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));

    // Assert
    expect(service.isEmpty()).toBe(true);
    expect(service.hasProducts()).toBe(false);
  });

  it('should not be isEmpty when products are loaded', () => {
    // Act
    service.loadProducts();
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([mockProductSummary]));

    // Assert
    expect(service.isEmpty()).toBe(false);
  });

  it('should not be isEmpty when loading is in progress', () => {
    // Act
    service.loadProducts(); // isLoadingList = true

    // Assert — durant le chargement, isEmpty doit rester false (listError=null mais isLoading=true)
    expect(service.isEmpty()).toBe(false);

    // Cleanup
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));
  });

  // ── applyFilters ──────────────────────────────────────────────────────────

  it('should reset page to 1 when applying filters', () => {
    // Arrange — aller à la page 3
    service.changePage(3);
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));

    // Act
    service.applyFilters({ searchText: 'kraft' });
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));

    // Assert
    expect(service.filters().page).toBe(1);
    expect(service.filters().searchText).toBe('kraft');
  });

  it('should pass filter params in the HTTP request', () => {
    // Act
    service.applyFilters({ searchText: 'sac', isCustomizable: true });
    const req = httpMock.expectOne(req => req.url.includes('/api/v1/products'));

    // Assert
    expect(req.request.params.get('searchText')).toBe('sac');
    expect(req.request.params.get('isCustomizable')).toBe('true');

    req.flush(pagedResponse([]));
  });

  // ── resetFilters ──────────────────────────────────────────────────────────

  it('should reset filters to defaults and reload', () => {
    // Arrange
    service.applyFilters({ searchText: 'test', page: 5 });
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));

    // Act
    service.resetFilters();
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(pagedResponse([]));

    // Assert
    expect(service.filters().page).toBe(1);
    expect(service.filters().searchText).toBeUndefined();
  });

  // ── loadProductById ───────────────────────────────────────────────────────

  it('should update selectedProduct signal after loadProductById', () => {
    // Arrange
    const productId = mockProductSummary.id;

    // Act
    service.loadProductById(productId);
    const req = httpMock.expectOne(req => req.url.includes(`/api/v1/products/${productId}`));
    req.flush({ ...mockProductSummary, descriptionFr: 'Description détaillée', variants: [], images: [] });

    // Assert
    expect(service.selectedProduct()).not.toBeNull();
    expect(service.selectedProduct()?.id).toBe(productId);
  });

  // ── clearError ────────────────────────────────────────────────────────────

  it('should clear both errors via clearError()', () => {
    // Arrange
    service.loadProducts();
    httpMock.expectOne(req => req.url.includes('/api/v1/products')).flush(
      { code: 'ERR', message: 'Erreur' },
      { status: 500, statusText: 'Error' }
    );

    // Act
    service.clearError();

    // Assert
    expect(service.listError()).toBeNull();
    expect(service.detailError()).toBeNull();
  });
});
