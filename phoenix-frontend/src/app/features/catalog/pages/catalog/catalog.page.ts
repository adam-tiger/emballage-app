import { ChangeDetectionStrategy, Component, OnInit, computed, inject } from '@angular/core';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { ProductCatalogService } from '../../services/product-catalog.service';
import { ProductCardComponent } from '../../components/product-card/product-card.component';
import { ProductFiltersComponent } from '../../components/product-filters/product-filters.component';
import { ProductFilters } from '../../models/product-filters.model';
import { ProductFamilyDto } from '../../models/product-family.model';

@Component({
  selector: 'app-catalog-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ProductCardComponent, ProductFiltersComponent, PaginatorModule],
  template: `
    <div class="catalog-layout">

      <!-- Filtres latéraux -->
      <aside class="catalog-layout__filters">
        @if (familiesForFilter().length > 0) {
          <app-product-filters
            [families]="familiesForFilter()"
            [currentFilters]="service.filters()"
            (filtersChanged)="onFiltersChanged($event)"
            (filtersReset)="onFiltersReset()" />
        }
      </aside>

      <!-- Contenu principal -->
      <main class="catalog-layout__main">

        <!-- Header résultats -->
        <div class="catalog-header">
          <h1 class="catalog-title">Nos Emballages</h1>
          <span class="catalog-count">
            {{ service.totalCount() }} produit(s)
          </span>
        </div>

        <!-- État loading -->
        @if (service.isLoadingList()) {
          <div class="catalog-loading">
            <div class="skeleton-grid">
              @for (i of skeletons; track i) {
                <div class="skeleton-card"></div>
              }
            </div>
          </div>
        }

        <!-- État erreur -->
        @if (service.listError()) {
          <div class="catalog-error">
            <span class="catalog-error__icon">⚠️</span>
            <p>{{ service.listError()?.message }}</p>
            <button class="btn-retry" (click)="retry()">Réessayer</button>
          </div>
        }

        <!-- État vide -->
        @if (service.isEmpty()) {
          <div class="catalog-empty">
            <span class="catalog-empty__icon">📦</span>
            <p>Aucun produit ne correspond à vos critères.</p>
            <button class="btn-retry" (click)="service.resetFilters()">
              Réinitialiser les filtres
            </button>
          </div>
        }

        <!-- Grille produits -->
        @if (!service.isLoadingList() && service.hasProducts()) {
          <div class="catalog-grid">
            @for (product of service.products(); track product.id) {
              <app-product-card [product]="product" />
            }
          </div>
        }

        <!-- Pagination PrimeNG -->
        @if (service.totalPages() > 1) {
          <p-paginator
            [rows]="service.filters().pageSize"
            [totalRecords]="service.totalCount()"
            [first]="(service.currentPage() - 1) * service.filters().pageSize"
            (onPageChange)="onPageChange($event)" />
        }

      </main>
    </div>
  `,
  styleUrl: './catalog.page.scss'
})
export class CatalogPage implements OnInit {
  readonly service = inject(ProductCatalogService);
  readonly skeletons = Array.from({ length: 8 }, (_, i) => i);

  readonly familiesForFilter = computed(() =>
    this.service.families() as unknown as ProductFamilyDto[]
  );

  ngOnInit(): void {
    this.service.loadFamilies();
    this.service.loadProducts();
  }

  onFiltersChanged(filters: Partial<ProductFilters>): void {
    this.service.applyFilters(filters);
  }

  onFiltersReset(): void {
    this.service.resetFilters();
  }

  onPageChange(event: PaginatorState): void {
    this.service.changePage((event.page ?? 0) + 1);
  }

  retry(): void {
    this.service.loadProducts();
  }
}
