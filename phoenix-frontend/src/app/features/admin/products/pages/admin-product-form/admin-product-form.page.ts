import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  effect,
  inject
} from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators
} from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { MessageService } from 'primeng/api';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { DividerModule } from 'primeng/divider';
import { AdminProductService } from '../../services/admin-product.service';
import { ProductCatalogService } from '../../../../catalog/services/product-catalog.service';
import { VariantManagerComponent } from '../../components/variant-manager/variant-manager.component';
import { ProductImageUploaderComponent } from '../../components/product-image-uploader/product-image-uploader.component';
import { CreateProductRequest } from '../../models/create-product.request';
import { UpdateProductRequest } from '../../models/update-product.request';
import { AddVariantRequest } from '../../models/add-variant.request';
import { AddPriceTierRequest, UploadProductImageResponse } from '../../models/add-price-tier.request';
import { ApiError } from '../../../../../shared/models/api-error.model';
import { ProductFamily, ProductFamilyDto, PRODUCT_FAMILY_LABELS } from '../../../../catalog/models/product-family.model';

@Component({
  selector: 'app-admin-product-form-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    NgIf,
    RouterLink,
    InputTextModule,
    TextareaModule,
    SelectModule,
    CheckboxModule,
    ButtonModule,
    ToastModule,
    DividerModule,
    VariantManagerComponent,
    ProductImageUploaderComponent
  ],
  providers: [MessageService],
  template: `
    <!-- Breadcrumb -->
    <div class="admin-breadcrumb">
      <a routerLink="/admin/products">← Retour aux produits</a>
    </div>

    <h2 class="admin-page-title">
      {{ isEditMode() ? 'Modifier le produit' : 'Nouveau produit' }}
    </h2>

    <p-toast />

    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="admin-form">

      <!-- Section Informations générales -->
      <div class="admin-form__section">
        <h3 class="admin-form__section-title">Informations générales</h3>
        <p-divider />

        <div class="admin-form__grid">

          <div class="admin-form__field">
            <label for="sku">SKU *</label>
            <input
              pInputText
              id="sku"
              formControlName="sku"
              placeholder="ex: SAC-BRUN-22x10x28"
              [class.ng-invalid]="isInvalid('sku')" />
            @if (isInvalid('sku')) {
              <small class="error">{{ getError('sku') }}</small>
            }
          </div>

          <div class="admin-form__field">
            <label for="family">Famille de produit *</label>
            <p-select
              formControlName="family"
              [options]="familyOptions()"
              optionLabel="labelFr"
              optionValue="value"
              placeholder="Sélectionner une famille"
              inputId="family" />
          </div>

          <div class="admin-form__field admin-form__field--full">
            <label for="nameFr">Nom du produit (FR) *</label>
            <input
              pInputText
              id="nameFr"
              formControlName="nameFr"
              placeholder="ex: Sac Kraft Brun 22×10×28cm"
              [class.ng-invalid]="isInvalid('nameFr')" />
            @if (isInvalid('nameFr')) {
              <small class="error">{{ getError('nameFr') }}</small>
            }
          </div>

          <div class="admin-form__field admin-form__field--full">
            <label for="descriptionFr">Description (FR)</label>
            <textarea
              pTextarea
              id="descriptionFr"
              formControlName="descriptionFr"
              rows="3"
              placeholder="Description détaillée du produit...">
            </textarea>
          </div>

        </div>
      </div>

      <!-- Section Caractéristiques -->
      <div class="admin-form__section">
        <h3 class="admin-form__section-title">Caractéristiques</h3>
        <p-divider />

        <div class="admin-form__checkboxes">
          <div class="admin-form__checkbox-item">
            <p-checkbox formControlName="isCustomizable"
                        [binary]="true" inputId="isCustomizable" />
            <label for="isCustomizable">🎨 Personnalisable (impression logo)</label>
          </div>
          <div class="admin-form__checkbox-item">
            <p-checkbox formControlName="isGourmetRange"
                        [binary]="true" inputId="isGourmetRange" />
            <label for="isGourmetRange">★ Gamme Gourmet premium</label>
          </div>
          <div class="admin-form__checkbox-item">
            <p-checkbox formControlName="isBulkOnly"
                        [binary]="true" inputId="isBulkOnly" />
            <label for="isBulkOnly">📦 Grandes séries uniquement</label>
          </div>
          <div class="admin-form__checkbox-item">
            <p-checkbox formControlName="isEcoFriendly"
                        [binary]="true" inputId="isEcoFriendly" />
            <label for="isEcoFriendly">♻️ Éco-responsable / Biodégradable</label>
          </div>
          <div class="admin-form__checkbox-item">
            <p-checkbox formControlName="isFoodApproved"
                        [binary]="true" inputId="isFoodApproved" />
            <label for="isFoodApproved">✓ Agréé Contact Alimentaire</label>
          </div>
          <div class="admin-form__checkbox-item">
            <p-checkbox formControlName="soldByWeight"
                        [binary]="true" inputId="soldByWeight" />
            <label for="soldByWeight">⚖️ Vendu au KG</label>
          </div>
          <div class="admin-form__checkbox-item">
            <p-checkbox formControlName="hasExpressDelivery"
                        [binary]="true" inputId="hasExpressDelivery" />
            <label for="hasExpressDelivery">⚡ Express 24h (Île-de-France)</label>
          </div>
        </div>
      </div>

      <!-- Section Image (mode édition uniquement) -->
      @if (isEditMode() && productId()) {
        <div class="admin-form__section">
          <h3 class="admin-form__section-title">Image produit</h3>
          <p-divider />
          <app-product-image-uploader
            [productId]="productId()!"
            [currentImageUrl]="adminService.selectedProduct()?.mainImageUrl ?? null"
            (imageUploaded)="onImageUploaded($event)" />
        </div>
      }

      <!-- Section Variantes (mode édition uniquement) -->
      @if (isEditMode() && adminService.selectedProduct()) {
        <div class="admin-form__section">
          <h3 class="admin-form__section-title">Variantes & Prix</h3>
          <p-divider />
          <app-variant-manager
            [productId]="productId()!"
            [variants]="adminService.selectedProduct()!.variants"
            (variantAdded)="onVariantAdded($event)"
            (priceTierAdded)="onPriceTierAdded($event)" />
        </div>
      }

      <!-- Boutons -->
      <div class="admin-form__actions">
        <p-button
          label="Annuler"
          severity="secondary"
          [outlined]="true"
          routerLink="/admin/products" />
        <p-button
          [label]="isEditMode() ? 'Enregistrer les modifications' : 'Créer le produit'"
          type="submit"
          [loading]="adminService.isSaving()"
          [disabled]="form.invalid" />
      </div>

    </form>
  `,
  styleUrl: './admin-product-form.page.scss'
})
export class AdminProductFormPage implements OnInit {
  readonly adminService   = inject(AdminProductService);
  private readonly catalogService    = inject(ProductCatalogService);
  private readonly route             = inject(ActivatedRoute);
  private readonly router            = inject(Router);
  private readonly messageService    = inject(MessageService);

  readonly isEditMode = computed(() => !!this.route.snapshot.paramMap.get('id'));
  readonly productId  = computed(() => this.route.snapshot.paramMap.get('id'));

  readonly form = new FormGroup({
    sku:               new FormControl({ value: '', disabled: false }, [
                         Validators.required,
                         Validators.maxLength(50),
                         Validators.pattern('^[A-Z0-9\\-]+$')
                       ]),
    nameFr:            new FormControl('', [Validators.required, Validators.maxLength(200)]),
    descriptionFr:     new FormControl('', [Validators.maxLength(1000)]),
    family:            new FormControl<ProductFamily | null>(null, Validators.required),
    isCustomizable:    new FormControl(false),
    isGourmetRange:    new FormControl(false),
    isBulkOnly:        new FormControl(false),
    isEcoFriendly:     new FormControl(false),
    isFoodApproved:    new FormControl(false),
    soldByWeight:      new FormControl(false),
    hasExpressDelivery: new FormControl(false)
  });

  // Patch form when selected product loads (edit mode)
  private readonly _patchEffect = effect(() => {
    const p = this.adminService.selectedProduct();
    if (p && this.isEditMode()) {
      this.patchForm();
    }
  });

  readonly familyOptions = computed<ProductFamilyDto[]>(() => {
    const raw = this.catalogService.families() as unknown as ProductFamilyDto[];
    if (raw.length > 0) return raw;
    return Object.values(ProductFamily).map(value => ({
      value,
      labelFr: PRODUCT_FAMILY_LABELS[value] ?? value
    }));
  });

  ngOnInit(): void {
    this.catalogService.loadFamilies();
    if (this.isEditMode() && this.productId()) {
      this.adminService.loadProductById(this.productId()!);
    }
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl?.invalid && ctrl?.touched);
  }

  getError(field: string): string {
    const ctrl = this.form.get(field);
    if (!ctrl?.errors) return '';
    if (ctrl.errors['required'])   return 'Ce champ est obligatoire.';
    if (ctrl.errors['maxlength'])  return 'Valeur trop longue.';
    if (ctrl.errors['pattern'])    return 'Format invalide (ex: SAC-BRUN-01).';
    return '';
  }

  patchForm(): void {
    const p = this.adminService.selectedProduct();
    if (!p) return;
    this.form.patchValue({
      nameFr:            p.nameFr,
      descriptionFr:     p.descriptionFr ?? '',
      family:            p.family,
      isCustomizable:    p.isCustomizable,
      isGourmetRange:    p.isGourmetRange,
      isEcoFriendly:     p.isEcoFriendly,
      isFoodApproved:    p.isFoodApproved,
      soldByWeight:      p.soldByWeight,
      hasExpressDelivery: p.hasExpressDelivery
    });
    this.form.get('sku')?.setValue(p.sku);
    this.form.get('sku')?.disable();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (this.isEditMode()) {
      this.updateProduct();
    } else {
      this.createProduct();
    }
  }

  private createProduct(): void {
    const request = this.form.getRawValue() as CreateProductRequest;
    this.adminService.createProduct(request).subscribe({
      next: (id: string) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Produit créé',
          detail: 'Vous pouvez maintenant ajouter des variantes.'
        });
        this.router.navigate(['/admin/products', id, 'edit']);
      },
      error: (err: ApiError) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erreur',
          detail: err.message
        });
      }
    });
  }

  private updateProduct(): void {
    const { nameFr, descriptionFr } = this.form.getRawValue();
    const request: UpdateProductRequest = {
      nameFr: nameFr!,
      descriptionFr: descriptionFr ?? undefined
    };
    this.adminService.updateProduct(this.productId()!, request).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Produit mis à jour'
        });
        this.adminService.loadProductById(this.productId()!);
      },
      error: (err: ApiError) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Erreur',
          detail: err.message
        });
      }
    });
  }

  onImageUploaded(response: UploadProductImageResponse): void {
    this.messageService.add({ severity: 'success', summary: 'Image uploadée' });
    this.adminService.loadProductById(this.productId()!);
  }

  onVariantAdded(request: AddVariantRequest): void {
    this.adminService.addVariant(this.productId()!, request).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Variante ajoutée' });
        this.adminService.loadProductById(this.productId()!);
      },
      error: (err: ApiError) => {
        this.messageService.add({ severity: 'error', summary: 'Erreur', detail: err.message });
      }
    });
  }

  onPriceTierAdded(data: { variantId: string; tier: AddPriceTierRequest }): void {
    this.adminService
      .addPriceTier(this.productId()!, data.variantId, data.tier)
      .subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Palier ajouté' });
          this.adminService.loadProductById(this.productId()!);
        },
        error: (err: ApiError) => {
          this.messageService.add({ severity: 'error', summary: 'Erreur', detail: err.message });
        }
      });
  }
}
