import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { ProductCatalogService } from '../../services/product-catalog.service';
import { ProductBadgeComponent } from '../../components/product-badge/product-badge.component';
import { VariantSelectorComponent } from '../../components/variant-selector/variant-selector.component';
import { ProductVariant } from '../../models/product-variant.model';
import { getPlaceholderImageUrl } from '../../../../shared/services/images.service';

@Component({
  selector: 'app-product-detail-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, ProductBadgeComponent, VariantSelectorComponent],
  template: `
    <!-- Breadcrumb -->
    <nav class="breadcrumb">
      <a routerLink="/">Accueil</a>
      <span class="breadcrumb__sep">›</span>
      <a routerLink="/catalogue">Catalogue</a>
      <span class="breadcrumb__sep">›</span>
      <span>{{ product()?.nameFr }}</span>
    </nav>

    @if (service.isLoadingDetail()) {
      <div class="detail-skeleton">
        <div class="detail-skeleton__image"></div>
        <div class="detail-skeleton__content">
          <div class="detail-skeleton__line detail-skeleton__line--short"></div>
          <div class="detail-skeleton__line"></div>
          <div class="detail-skeleton__line detail-skeleton__line--medium"></div>
        </div>
      </div>
    }

    @if (service.detailError()) {
      <div class="detail-error">
        <span>⚠️</span>
        <p>{{ service.detailError()?.message }}</p>
        <a routerLink="/catalogue" class="btn-back">Retour au catalogue</a>
      </div>
    }

    @if (product()) {
      <div class="detail-layout">

        <!-- Galerie images -->
        <div class="detail-gallery">
          <div class="detail-gallery__main-wrap">
            <img
              [src]="mainImageUrl()"
              [alt]="product()!.nameFr"
              class="detail-gallery__main" />
          </div>
          @if (product()!.images.length > 1) {
            <div class="detail-gallery__thumbs">
              @for (img of product()!.images; track img.id) {
                <img
                  [src]="img.thumbPublicUrl ?? img.publicUrl"
                  [alt]="product()!.nameFr"
                  class="detail-gallery__thumb"
                  [class.detail-gallery__thumb--active]="selectedImageId() === img.id"
                  (click)="selectImage(img.id)" />
              }
            </div>
          }
        </div>

        <!-- Infos produit -->
        <div class="detail-info">
          <div class="detail-family">{{ product()!.familyLabel }}</div>
          <h1 class="detail-name">{{ product()!.nameFr }}</h1>

          <app-product-badge
            [isCustomizable]="product()!.isCustomizable"
            [isGourmetRange]="product()!.isGourmetRange"
            [isEcoFriendly]="product()!.isEcoFriendly"
            [isFoodApproved]="product()!.isFoodApproved"
            [soldByWeight]="product()!.soldByWeight"
            [hasExpressDelivery]="product()!.hasExpressDelivery"
            size="md" />

          @if (product()!.descriptionFr) {
            <p class="detail-description">{{ product()!.descriptionFr }}</p>
          }

          <!-- Sélecteur variante + prix -->
          @if (product()!.variants.length > 0) {
            <div class="detail-variants">
              <app-variant-selector
                [variants]="product()!.variants"
                [soldByWeight]="product()!.soldByWeight"
                (variantSelected)="onVariantSelected($event)" />
            </div>
          }

          <!-- CTAs -->
          <div class="detail-ctas">
            @if (product()!.isCustomizable) {
              <button
                class="btn-primary"
                [routerLink]="['/configurateur', product()!.id]">
                🎨 Personnaliser mon emballage
              </button>
            }
            <button class="btn-outline" (click)="requestQuote()">
              📋 Demander un devis gratuit
            </button>
          </div>

          <!-- Garanties -->
          <div class="detail-guarantees">
            <span class="guarantee">⚡ Livraison 5-10 jours</span>
            <span class="guarantee">🔒 Paiement sécurisé</span>
            <span class="guarantee">🇫🇷 Made in France</span>
          </div>
        </div>

      </div>
    }
  `,
  styleUrl: './product-detail.page.scss'
})
export class ProductDetailPage implements OnInit {
  readonly service = inject(ProductCatalogService);
  private readonly route  = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly selectedImageId = signal<string | null>(null);

  readonly product = computed(() => this.service.selectedProduct());

  readonly mainImageUrl = computed(() => {
    const p = this.product();
    if (!p) return getPlaceholderImageUrl();
    const sid = this.selectedImageId();
    if (sid) {
      return p.images.find(i => i.id === sid)?.publicUrl
        ?? p.mainImageUrl
        ?? getPlaceholderImageUrl();
    }
    return p.mainImageUrl ?? getPlaceholderImageUrl();
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.service.loadProductById(id);
    }
  }

  selectImage(id: string): void {
    this.selectedImageId.set(id);
  }

  onVariantSelected(data: { variant: ProductVariant; quantity: number }): void {
    // Panier = Tour futur — log uniquement pour l'instant
    console.log('[ProductDetailPage] Variant selected:', data.variant.sku, 'qty:', data.quantity);
  }

  requestQuote(): void {
    this.router.navigate(['/devis'], {
      queryParams: { productId: this.product()?.id }
    });
  }
}
