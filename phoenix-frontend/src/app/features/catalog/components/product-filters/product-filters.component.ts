import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  inject,
  input,
  OnInit,
  output,
  signal
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule, NgClass } from '@angular/common';
import { Subject, timer } from 'rxjs';
import { debounce } from 'rxjs/operators';
import { CustomerSegment, ProductFamilyDto } from '../../models/product-family.model';
import { ProductFilters } from '../../models/product-filters.model';
import { CUSTOMER_SEGMENT_LABELS, CUSTOMER_SEGMENT_EMOJI } from '../../models/product-family.model';

@Component({
  selector: 'app-product-filters',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, NgClass],
  template: `
    <div class="filters-panel">

      <!-- Recherche -->
      <div class="filters-section">
        <div class="filters-section__title">Recherche</div>
        <input
          class="filters-search"
          type="text"
          placeholder="Rechercher un produit..."
          [value]="searchText()"
          (input)="onSearchInput($event)" />
      </div>

      <!-- Famille de produit -->
      <div class="filters-section">
        <div class="filters-section__title">Famille de produit</div>
        <div class="filters-chips">
          @for (family of families(); track family.value) {
            <button
              class="filters-chip"
              [class.filters-chip--active]="selectedFamily() === family.value"
              (click)="toggleFamily(family.value)">
              {{ family.labelFr }}
            </button>
          }
        </div>
      </div>

      <!-- Personnalisables uniquement -->
      <div class="filters-section">
        <label class="filters-toggle">
          <input
            type="checkbox"
            [checked]="isCustomizableOnly()"
            (change)="toggleCustomizable($event)" />
          <span>Personnalisables uniquement</span>
        </label>
      </div>

      <!-- Secteur d'activité -->
      <div class="filters-section">
        <div class="filters-section__title">Secteur d'activité</div>
        <div class="filters-chips">
          @for (segment of segments; track segment.value) {
            <button
              class="filters-chip"
              [class.filters-chip--active]="selectedSegment() === segment.value"
              (click)="toggleSegment(segment.value)">
              {{ segment.emoji }} {{ segment.label }}
            </button>
          }
        </div>
      </div>

      <!-- Reset -->
      <div class="filters-section filters-section--reset">
        <button class="filters-reset" (click)="resetFilters()">
          Réinitialiser les filtres
        </button>
      </div>

    </div>
  `,
  styleUrl: './product-filters.component.scss'
})
export class ProductFiltersComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  families        = input.required<ProductFamilyDto[]>();
  currentFilters  = input.required<ProductFilters>();

  filtersChanged = output<Partial<ProductFilters>>();
  filtersReset   = output<void>();

  readonly selectedFamily      = signal<string | undefined>(undefined);
  readonly selectedSegment     = signal<string | undefined>(undefined);
  readonly isCustomizableOnly  = signal<boolean>(false);
  readonly searchText          = signal<string>('');

  private readonly searchSubject = new Subject<string>();

  readonly segments = Object.values(CustomerSegment).map(value => ({
    value,
    label: CUSTOMER_SEGMENT_LABELS[value],
    emoji: CUSTOMER_SEGMENT_EMOJI[value]
  }));

  ngOnInit(): void {
    this.searchSubject
      .pipe(
        debounce(() => timer(300)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(text => {
        this.searchText.set(text);
        this.applyFilters();
      });
  }

  applyFilters(): void {
    this.filtersChanged.emit({
      family:         this.selectedFamily() as any,
      segment:        this.selectedSegment() as any,
      isCustomizable: this.isCustomizableOnly() || undefined,
      searchText:     this.searchText() || undefined
    });
  }

  resetFilters(): void {
    this.selectedFamily.set(undefined);
    this.selectedSegment.set(undefined);
    this.isCustomizableOnly.set(false);
    this.searchText.set('');
    this.filtersReset.emit();
  }

  onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  toggleFamily(value: string): void {
    this.selectedFamily.set(
      this.selectedFamily() === value ? undefined : value
    );
    this.applyFilters();
  }

  toggleSegment(value: string): void {
    this.selectedSegment.set(
      this.selectedSegment() === value ? undefined : value
    );
    this.applyFilters();
  }

  toggleCustomizable(event: Event): void {
    this.isCustomizableOnly.set((event.target as HTMLInputElement).checked);
    this.applyFilters();
  }
}
