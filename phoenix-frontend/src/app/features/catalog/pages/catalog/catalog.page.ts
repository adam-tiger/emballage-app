import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { ProductCatalogService } from '../../services/product-catalog.service';
import { ProductCardComponent } from '../../components/product-card/product-card.component';
import { ProductFiltersComponent } from '../../components/product-filters/product-filters.component';
import { ProductFilters } from '../../models/product-filters.model';
import { ProductFamilyDto } from '../../models/product-family.model';

interface QuickChip {
  id: string;
  label: string;
  emoji: string;
  filter: Partial<ProductFilters>;
}

interface SortOption {
  label: string;
  sortBy: string;
  sortDir: 'asc' | 'desc';
}

@Component({
  selector: 'app-catalog-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, ProductCardComponent, ProductFiltersComponent, PaginatorModule],
  template: `
    <div class="catalog-page">

      <!-- ── TOPBAR ──────────────────────────────────────────────────────────── -->
      <div class="catalog-topbar">
        <div class="catalog-topbar__inner">

          <!-- Quick chips -->
          <div class="catalog-topbar__chips">
            @for (chip of quickChips; track chip.id) {
              <button
                class="quick-chip"
                [class.quick-chip--active]="isQuickChipActive(chip)"
                (click)="applyQuickChip(chip)">
                {{ chip.emoji }} {{ chip.label }}
              </button>
            }
          </div>

          <!-- Count + Sort -->
          <div class="catalog-topbar__controls">
            <span class="catalog-count">
              {{ service.totalCount() }} produit{{ service.totalCount() > 1 ? 's' : '' }}
            </span>
            <div class="catalog-sort">
              <label class="catalog-sort__label">Trier :</label>
              <select class="catalog-sort__select" (change)="onSortChange($event)">
                @for (opt of sortOptions; track opt.label) {
                  <option [value]="opt.label"
                    [selected]="service.filters().sortBy === opt.sortBy && service.filters().sortDir === opt.sortDir">
                    {{ opt.label }}
                  </option>
                }
              </select>
            </div>
          </div>

        </div>
      </div>

      <!-- ── LAYOUT ──────────────────────────────────────────────────────────── -->
      <div class="catalog-layout">

        <!-- Filtres latéraux -->
        <aside class="catalog-layout__sidebar">
          @if (familiesForFilter().length > 0) {
            <app-product-filters
              [families]="familiesForFilter()"
              [currentFilters]="service.filters()"
              (filtersChanged)="onFiltersChanged($event)"
              (filtersReset)="onFiltersReset()" />
          }

          <!-- Assistant block -->
          <div class="assistant-block">
            <div class="assistant-block__icon">🤔</div>
            <h4 class="assistant-block__title">Vous ne savez pas quoi choisir ?</h4>
            <p class="assistant-block__text">
              Répondez à 3 questions et recevez une sélection personnalisée pour votre activité.
            </p>
            <a routerLink="/devis" class="assistant-block__btn">
              Démarrer le guide →
            </a>
          </div>
        </aside>

        <!-- Contenu principal -->
        <main class="catalog-layout__main">

          <!-- État loading -->
          @if (service.isLoadingList()) {
            <div class="skeleton-grid">
              @for (i of skeletons; track i) {
                <div class="skeleton-card">
                  <div class="skeleton-card__image"></div>
                  <div class="skeleton-card__body">
                    <div class="skeleton-line skeleton-line--short"></div>
                    <div class="skeleton-line"></div>
                    <div class="skeleton-line skeleton-line--medium"></div>
                  </div>
                </div>
              }
            </div>
          }

          <!-- État erreur -->
          @if (service.listError()) {
            <div class="catalog-empty-state">
              <div class="catalog-empty-state__icon">⚠️</div>
              <h3 class="catalog-empty-state__title">Une erreur est survenue</h3>
              <p class="catalog-empty-state__text">{{ service.listError()?.message }}</p>
              <button class="btn-retry" (click)="retry()">Réessayer</button>
            </div>
          }

          <!-- État vide -->
          @if (service.isEmpty()) {
            <div class="catalog-empty-state">
              <div class="catalog-empty-state__icon">📦</div>
              <h3 class="catalog-empty-state__title">Aucun produit trouvé</h3>
              <p class="catalog-empty-state__text">
                Essayez d'élargir vos critères ou de réinitialiser les filtres.
              </p>
              <div class="catalog-empty-state__actions">
                <button class="btn-retry" (click)="service.resetFilters()">
                  Réinitialiser les filtres
                </button>
                <a routerLink="/devis" class="btn-devis">
                  Demander un devis personnalisé
                </a>
              </div>
              <div class="catalog-empty-state__suggestions">
                <p class="catalog-empty-state__suggest-label">Vous cherchez peut-être :</p>
                @for (chip of quickChips.slice(0, 4); track chip.id) {
                  <button class="quick-chip" (click)="applyQuickChip(chip)">
                    {{ chip.emoji }} {{ chip.label }}
                  </button>
                }
              </div>
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
    </div>
  `,
  styleUrl: './catalog.page.scss'
})
export class CatalogPage implements OnInit {
  readonly service = inject(ProductCatalogService);
  readonly skeletons = Array.from({ length: 8 }, (_, i) => i);

  readonly activeQuickChipId = signal<string | null>(null);

  readonly familiesForFilter = computed(() =>
    this.service.families() as unknown as ProductFamilyDto[]
  );

  readonly quickChips: QuickChip[] = [
    { id: 'sushi',   label: 'Sushi & Asiatique', emoji: '🍣',  filter: { segment: 'JapaneseAsian' as any } },
    { id: 'fastfood',label: 'Fast Food',          emoji: '🍔',  filter: { segment: 'FastFood' as any } },
    { id: 'bakery',  label: 'Boulangerie',        emoji: '🥐',  filter: { segment: 'BakeryPastry' as any } },
    { id: 'eco',     label: 'Éco-responsable',    emoji: '🌿',  filter: { isEcoFriendly: true } },
    { id: 'express', label: 'Express 24h',        emoji: '🚀',  filter: { hasExpressDelivery: true } },
    { id: 'custom',  label: 'Personnalisable',    emoji: '🎨',  filter: { isCustomizable: true } },
  ];

  readonly sortOptions: SortOption[] = [
    { label: 'Nom A–Z',         sortBy: 'NameFr',          sortDir: 'asc'  },
    { label: 'Prix croissant',  sortBy: 'MinUnitPriceHT',  sortDir: 'asc'  },
    { label: 'Prix décroissant',sortBy: 'MinUnitPriceHT',  sortDir: 'desc' },
    { label: 'Nouveautés',      sortBy: 'CreatedAt',       sortDir: 'desc' },
  ];

  ngOnInit(): void {
    this.service.loadFamilies();
    this.service.loadProducts();
  }

  isQuickChipActive(chip: QuickChip): boolean {
    return this.activeQuickChipId() === chip.id;
  }

  applyQuickChip(chip: QuickChip): void {
    if (this.activeQuickChipId() === chip.id) {
      this.activeQuickChipId.set(null);
      this.service.resetFilters();
    } else {
      this.activeQuickChipId.set(chip.id);
      this.service.applyFilters(chip.filter);
    }
  }

  onFiltersChanged(filters: Partial<ProductFilters>): void {
    this.activeQuickChipId.set(null);
    this.service.applyFilters(filters);
  }

  onFiltersReset(): void {
    this.activeQuickChipId.set(null);
    this.service.resetFilters();
  }

  onPageChange(event: PaginatorState): void {
    this.service.changePage((event.page ?? 0) + 1);
  }

  onSortChange(event: Event): void {
    const label = (event.target as HTMLSelectElement).value;
    const opt = this.sortOptions.find(o => o.label === label);
    if (opt) {
      this.service.applyFilters({ sortBy: opt.sortBy, sortDir: opt.sortDir });
    }
  }

  retry(): void {
    this.service.loadProducts();
  }
}
