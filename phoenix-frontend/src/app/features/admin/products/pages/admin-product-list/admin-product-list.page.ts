import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  inject,
  signal
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime } from 'rxjs';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TooltipModule } from 'primeng/tooltip';
import { AdminProductService } from '../../services/admin-product.service';
import { ProductSummary } from '../../../../catalog/models/product-summary.model';

@Component({
  selector: 'app-admin-product-list-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    ToastModule,
    ConfirmDialogModule,
    IconFieldModule,
    InputIconModule,
    ProgressSpinnerModule,
    TooltipModule
  ],
  providers: [ConfirmationService, MessageService],
  template: `
    <!-- Header page -->
    <div class="admin-page-header">
      <div>
        <h2 class="admin-page-title">Produits</h2>
        <span class="admin-page-sub">
          {{ service.totalCount() }} produit(s) au total
        </span>
      </div>
      <p-button
        label="Nouveau produit"
        icon="pi pi-plus"
        (onClick)="router.navigate(['/admin/products/new'])" />
    </div>

    <p-toast />
    <p-confirmDialog />

    <!-- Loading overlay -->
    @if (service.isLoading() && service.products().length === 0) {
      <div class="admin-loading">
        <p-progressSpinner strokeWidth="3" />
      </div>
    }

    <!-- DataTable -->
    <p-table
      [value]="service.products()"
      [lazy]="true"
      [paginator]="true"
      [rows]="20"
      [totalRecords]="service.totalCount()"
      [loading]="service.isLoading()"
      (onLazyLoad)="onLazyLoad($event)"
      [globalFilterFields]="['nameFr', 'sku']"
      styleClass="p-datatable-striped p-datatable-sm"
      [tableStyle]="{ 'min-width': '60rem' }">

      <ng-template #caption>
        <div class="table-caption">
          <p-iconField iconPosition="left">
            <p-inputIcon styleClass="pi pi-search" />
            <input
              pInputText
              type="text"
              placeholder="Rechercher (nom, SKU)..."
              (input)="onSearch($event)" />
          </p-iconField>
        </div>
      </ng-template>

      <ng-template #header>
        <tr>
          <th pSortableColumn="sku">SKU <p-sortIcon field="sku" /></th>
          <th pSortableColumn="nameFr">Nom <p-sortIcon field="nameFr" /></th>
          <th>Famille</th>
          <th>Type</th>
          <th pSortableColumn="isActive">Statut <p-sortIcon field="isActive" /></th>
          <th>Actions</th>
        </tr>
      </ng-template>

      <ng-template #body let-product>
        <tr>
          <td><code>{{ product.sku }}</code></td>
          <td>{{ product.nameFr }}</td>
          <td>
            <p-tag [value]="product.familyLabel" severity="secondary" />
          </td>
          <td>
            @if (product.isCustomizable) {
              <p-tag value="Personnalisable" severity="info" />
            } @else {
              <p-tag value="Vrac" severity="secondary" />
            }
            @if (product.isGourmetRange) {
              <p-tag value="Gourmet" severity="warn" styleClass="ml-1" />
            }
          </td>
          <td>
            <p-tag
              [value]="product.isActive ? 'Actif' : 'Inactif'"
              [severity]="product.isActive ? 'success' : 'danger'" />
          </td>
          <td>
            <div class="table-actions">
              <p-button
                icon="pi pi-pencil"
                severity="secondary"
                size="small"
                [rounded]="true"
                [outlined]="true"
                (onClick)="editProduct(product.id)"
                pTooltip="Modifier" />
              <p-button
                icon="pi pi-trash"
                severity="danger"
                size="small"
                [rounded]="true"
                [outlined]="true"
                (onClick)="confirmDeactivate(product)"
                pTooltip="Désactiver"
                [disabled]="!product.isActive" />
            </div>
          </td>
        </tr>
      </ng-template>

      <ng-template #emptymessage>
        <tr>
          <td colspan="6" class="table-empty">
            Aucun produit trouvé.
          </td>
        </tr>
      </ng-template>

    </p-table>
  `,
  styleUrl: './admin-product-list.page.scss'
})
export class AdminProductListPage implements OnInit {
  readonly service            = inject(AdminProductService);
  readonly router             = inject(Router);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly messageService      = inject(MessageService);
  private readonly destroyRef          = inject(DestroyRef);

  private readonly searchSubject = new Subject<string>();
  private readonly currentParams = signal<Record<string, unknown>>({ page: 1, pageSize: 20 });

  ngOnInit(): void {
    this.service.loadProducts({ page: 1, pageSize: 20 });

    this.searchSubject
      .pipe(debounceTime(300), takeUntilDestroyed(this.destroyRef))
      .subscribe(text => {
        const params = { ...this.currentParams(), page: 1, searchText: text || undefined };
        this.currentParams.set(params);
        this.service.loadProducts(params);
      });
  }

  onLazyLoad(event: TableLazyLoadEvent): void {
    const first    = event.first ?? 0;
    const rows     = event.rows  ?? 20;
    const page     = Math.floor(first / rows) + 1;
    const sortBy   = event.sortField ? String(event.sortField) : 'NameFr';
    const sortDir  = event.sortOrder === -1 ? 'desc' : 'asc';

    const params: Record<string, unknown> = {
      ...this.currentParams(),
      page,
      pageSize: rows,
      sortBy,
      sortDir
    };
    this.currentParams.set(params);
    this.service.loadProducts(params);
  }

  onSearch(event: Event): void {
    const text = (event.target as HTMLInputElement).value;
    this.searchSubject.next(text);
  }

  editProduct(id: string): void {
    this.router.navigate(['/admin/products', id, 'edit']);
  }

  confirmDeactivate(product: ProductSummary): void {
    this.confirmationService.confirm({
      message: `Désactiver "${product.nameFr}" ?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => this.deactivate(product.id)
    });
  }

  deactivate(id: string): void {
    this.service.deactivateProduct(id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Produit désactivé'
        });
        this.service.loadProducts(this.currentParams());
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erreur',
          detail: 'Impossible de désactiver ce produit.'
        });
      }
    });
  }
}
