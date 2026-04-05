import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  computed,
  effect,
  inject,
  input,
  OnInit,
  output,
  signal
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, timer } from 'rxjs';
import { debounce } from 'rxjs/operators';
import {
  CustomerSegment,
  ProductFamilyDto,
  CUSTOMER_SEGMENT_LABELS,
  CUSTOMER_SEGMENT_EMOJI
} from '../../models/product-family.model';
import { ProductFilters } from '../../models/product-filters.model';

interface ActiveChip {
  key: string;
  label: string;
}

@Component({
  selector: 'app-product-filters',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],
  template: `
    <div class="filters-panel">

      <!-- En-tête -->
      <div class="filters-header">
        <span class="filters-header__title">Filtres</span>
        @if (activeChips().length > 0) {
          <span class="filters-header__count">{{ activeChips().length }}</span>
        }
        @if (activeChips().length > 0) {
          <button class="filters-header__clear" (click)="resetFilters()">
            Tout effacer
          </button>
        }
      </div>

      <!-- Chips actifs -->
      @if (activeChips().length > 0) {
        <div class="filters-active">
          @for (chip of activeChips(); track chip.key) {
            <button class="filters-active__chip" (click)="removeChip(chip.key)">
              {{ chip.label }} ×
            </button>
          }
        </div>
      }

      <!-- Recherche -->
      <div class="filters-section">
        <div class="filters-search-wrap">
          <span class="filters-search-icon">🔍</span>
          <input
            class="filters-search"
            type="text"
            placeholder="Rechercher un produit..."
            [value]="searchText()"
            (input)="onSearchInput($event)" />
        </div>
      </div>

      <!-- Catégorie -->
      <div class="filters-group">
        <button class="filters-group__header" (click)="openCategories.set(!openCategories())">
          <span>📦 Catégorie</span>
          <span class="filters-group__arrow" [class.open]="openCategories()">›</span>
        </button>
        @if (openCategories()) {
          <div class="filters-group__content">
            @for (family of families(); track family.value) {
              <button
                class="filters-chip"
                [class.filters-chip--active]="selectedFamily() === family.value"
                (click)="toggleFamily(family.value)">
                <span class="filters-chip__dot"
                  [class.filters-chip__dot--active]="selectedFamily() === family.value"></span>
                {{ family.labelFr }}
              </button>
            }
          </div>
        }
      </div>

      <!-- Usage métier -->
      <div class="filters-group">
        <button class="filters-group__header" (click)="openSegments.set(!openSegments())">
          <span>🍽️ Usage métier</span>
          <span class="filters-group__arrow" [class.open]="openSegments()">›</span>
        </button>
        @if (openSegments()) {
          <div class="filters-group__content">
            @for (seg of segments; track seg.value) {
              <button
                class="filters-chip"
                [class.filters-chip--active]="selectedSegment() === seg.value"
                (click)="toggleSegment(seg.value)">
                <span class="filters-chip__dot"
                  [class.filters-chip__dot--active]="selectedSegment() === seg.value"></span>
                {{ seg.emoji }} {{ seg.label }}
              </button>
            }
          </div>
        }
      </div>

      <!-- Options -->
      <div class="filters-group">
        <button class="filters-group__header" (click)="openOptions.set(!openOptions())">
          <span>✨ Options</span>
          <span class="filters-group__arrow" [class.open]="openOptions()">›</span>
        </button>
        @if (openOptions()) {
          <div class="filters-group__content filters-group__content--options">

            <label class="filters-toggle">
              <div class="filters-toggle__track" [class.filters-toggle__track--on]="isCustomizableOnly()">
                <input
                  type="checkbox"
                  [checked]="isCustomizableOnly()"
                  (change)="toggleCustomizable($event)" />
                <div class="filters-toggle__thumb"></div>
              </div>
              <div class="filters-toggle__info">
                <span class="filters-toggle__label">🎨 Personnalisable</span>
                <span class="filters-toggle__sub">Impression logo & couleurs</span>
              </div>
            </label>

            <label class="filters-toggle">
              <div class="filters-toggle__track" [class.filters-toggle__track--on]="isEcoFriendly()">
                <input
                  type="checkbox"
                  [checked]="isEcoFriendly()"
                  (change)="toggleEco($event)" />
                <div class="filters-toggle__thumb"></div>
              </div>
              <div class="filters-toggle__info">
                <span class="filters-toggle__label">🌿 Éco-responsable</span>
                <span class="filters-toggle__sub">Matières certifiées FSC</span>
              </div>
            </label>

            <label class="filters-toggle">
              <div class="filters-toggle__track" [class.filters-toggle__track--on]="hasExpressDelivery()">
                <input
                  type="checkbox"
                  [checked]="hasExpressDelivery()"
                  (change)="toggleExpress($event)" />
                <div class="filters-toggle__thumb"></div>
              </div>
              <div class="filters-toggle__info">
                <span class="filters-toggle__label">🚀 Express 24h</span>
                <span class="filters-toggle__sub">Livraison prioritaire</span>
              </div>
            </label>

          </div>
        }
      </div>

    </div>
  `,
  styleUrl: './product-filters.component.scss'
})
export class ProductFiltersComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  families       = input.required<ProductFamilyDto[]>();
  currentFilters = input.required<ProductFilters>();

  filtersChanged = output<Partial<ProductFilters>>();
  filtersReset   = output<void>();

  // Section open/close
  readonly openCategories = signal(true);
  readonly openSegments   = signal(true);
  readonly openOptions    = signal(true);

  // Filter state
  readonly selectedFamily     = signal<string | undefined>(undefined);
  readonly selectedSegment    = signal<string | undefined>(undefined);
  readonly isCustomizableOnly = signal<boolean>(false);
  readonly isEcoFriendly      = signal<boolean>(false);
  readonly hasExpressDelivery = signal<boolean>(false);
  readonly searchText         = signal<string>('');

  private readonly searchSubject = new Subject<string>();

  readonly segments = Object.values(CustomerSegment).map(value => ({
    value,
    label: CUSTOMER_SEGMENT_LABELS[value],
    emoji: CUSTOMER_SEGMENT_EMOJI[value]
  }));

  // Active filter chips for display
  readonly activeChips = computed<ActiveChip[]>(() => {
    const chips: ActiveChip[] = [];
    const fam = this.selectedFamily();
    const seg = this.selectedSegment();
    const txt = this.searchText();
    if (fam) {
      const found = this.families().find(f => f.value === fam);
      chips.push({ key: 'family', label: found?.labelFr ?? fam });
    }
    if (seg) {
      chips.push({
        key: 'segment',
        label: `${CUSTOMER_SEGMENT_EMOJI[seg]} ${CUSTOMER_SEGMENT_LABELS[seg]}`
      });
    }
    if (txt) chips.push({ key: 'search', label: `"${txt}"` });
    if (this.isCustomizableOnly()) chips.push({ key: 'customizable', label: '🎨 Personnalisable' });
    if (this.isEcoFriendly()) chips.push({ key: 'eco', label: '🌿 Éco-responsable' });
    if (this.hasExpressDelivery()) chips.push({ key: 'express', label: '🚀 Express 24h' });
    return chips;
  });

  constructor() {
    // Sync internal state when parent changes filters (e.g. via quick chips)
    effect(() => {
      const f = this.currentFilters();
      this.selectedFamily.set(f.family as string | undefined);
      this.selectedSegment.set(f.segment as string | undefined);
      this.isCustomizableOnly.set(f.isCustomizable ?? false);
      this.isEcoFriendly.set(f.isEcoFriendly ?? false);
      this.hasExpressDelivery.set(f.hasExpressDelivery ?? false);
      this.searchText.set(f.searchText ?? '');
    });
  }

  ngOnInit(): void {
    this.searchSubject
      .pipe(debounce(() => timer(300)), takeUntilDestroyed(this.destroyRef))
      .subscribe(text => {
        this.searchText.set(text);
        this.applyFilters();
      });
  }

  applyFilters(): void {
    this.filtersChanged.emit({
      family:             this.selectedFamily() as any,
      segment:            this.selectedSegment() as any,
      isCustomizable:     this.isCustomizableOnly() || undefined,
      isEcoFriendly:      this.isEcoFriendly() || undefined,
      hasExpressDelivery: this.hasExpressDelivery() || undefined,
      searchText:         this.searchText() || undefined
    });
  }

  resetFilters(): void {
    this.selectedFamily.set(undefined);
    this.selectedSegment.set(undefined);
    this.isCustomizableOnly.set(false);
    this.isEcoFriendly.set(false);
    this.hasExpressDelivery.set(false);
    this.searchText.set('');
    this.filtersReset.emit();
  }

  removeChip(key: string): void {
    switch (key) {
      case 'family':       this.selectedFamily.set(undefined); break;
      case 'segment':      this.selectedSegment.set(undefined); break;
      case 'search':       this.searchText.set(''); break;
      case 'customizable': this.isCustomizableOnly.set(false); break;
      case 'eco':          this.isEcoFriendly.set(false); break;
      case 'express':      this.hasExpressDelivery.set(false); break;
    }
    this.applyFilters();
  }

  onSearchInput(event: Event): void {
    this.searchSubject.next((event.target as HTMLInputElement).value);
  }

  toggleFamily(value: string): void {
    this.selectedFamily.set(this.selectedFamily() === value ? undefined : value);
    this.applyFilters();
  }

  toggleSegment(value: string): void {
    this.selectedSegment.set(this.selectedSegment() === value ? undefined : value);
    this.applyFilters();
  }

  toggleCustomizable(event: Event): void {
    this.isCustomizableOnly.set((event.target as HTMLInputElement).checked);
    this.applyFilters();
  }

  toggleEco(event: Event): void {
    this.isEcoFriendly.set((event.target as HTMLInputElement).checked);
    this.applyFilters();
  }

  toggleExpress(event: Event): void {
    this.hasExpressDelivery.set((event.target as HTMLInputElement).checked);
    this.applyFilters();
  }
}
