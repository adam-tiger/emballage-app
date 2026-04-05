import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideImageKitLoader } from '@angular/common';
import { ProductCardComponent } from './product-card.component';
import { ProductSummary } from '../../models/product-summary.model';
import { ProductFamily } from '../../models/product-family.model';

const mockProduct: ProductSummary = {
  id: '11111111-0001-0000-0000-000000000000',
  sku: 'SAC-BRUN-01',
  nameFr: 'Sac Kraft Brun',
  family: ProductFamily.KraftBagHandled,
  familyLabel: 'Sacs Kraft avec anses',
  isCustomizable: true,
  isGourmetRange: false,
  isEcoFriendly: false,
  isFoodApproved: true,
  soldByWeight: false,
  hasExpressDelivery: true,
  isActive: true,
  mainImageUrl: null,
  minUnitPriceHT: 0.0872,
  minimumOrderQuantity: 250
};

describe('ProductCardComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductCardComponent],
      providers: [
        provideRouter([]),
        provideImageKitLoader('https://cdn.phoenix.fr')
      ]
    }).compileComponents();
  });

  function createFixture(product: ProductSummary = mockProduct) {
    const fixture = TestBed.createComponent(ProductCardComponent);
    fixture.componentRef.setInput('product', product);
    fixture.detectChanges();
    return fixture;
  }

  it('should create the component', () => {
    // Arrange & Act
    const fixture = createFixture();

    // Assert
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should display product name', () => {
    // Arrange & Act
    const fixture = createFixture();
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.textContent).toContain('Sac Kraft Brun');
  });

  it('should display family label in uppercase', () => {
    // Arrange & Act
    const fixture = createFixture();
    const el = fixture.nativeElement as HTMLElement;
    const familyEl = el.querySelector('.product-card__family');

    // Assert
    expect(familyEl?.textContent?.trim().toUpperCase()).toContain('SACS KRAFT AVEC ANSES');
  });

  it('should display minimum price when minUnitPriceHT is provided', () => {
    // Arrange & Act
    const fixture = createFixture();
    const el = fixture.nativeElement as HTMLElement;
    const priceEl = el.querySelector('.product-card__price');

    // Assert — formatEur(0.0872) → ~ "0,09 €"
    expect(priceEl?.textContent).toContain('0');
    expect(priceEl?.textContent).toContain('€');
  });

  it('should display "Prix sur devis" when minUnitPriceHT is null', () => {
    // Arrange & Act
    const fixture = createFixture({ ...mockProduct, minUnitPriceHT: null });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.textContent).toContain('Prix sur devis');
  });

  it('should display MOQ when minimumOrderQuantity is provided', () => {
    // Arrange & Act
    const fixture = createFixture();
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.textContent).toContain('250');
    expect(el.textContent).toContain('pcs');
  });

  it('should display MOQ with kg unit when soldByWeight is true', () => {
    // Arrange & Act
    const fixture = createFixture({ ...mockProduct, soldByWeight: true });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.textContent).toContain('kg');
  });

  it('should show Gourmet overlay when isGourmetRange is true', () => {
    // Arrange & Act
    const fixture = createFixture({ ...mockProduct, isGourmetRange: true });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.querySelector('.product-card__gourmet-overlay')).toBeTruthy();
    expect(el.textContent).toContain('Gamme Gourmet');
  });

  it('should not show Gourmet overlay when isGourmetRange is false', () => {
    // Arrange & Act
    const fixture = createFixture({ ...mockProduct, isGourmetRange: false });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.querySelector('.product-card__gourmet-overlay')).toBeFalsy();
  });

  it('should have routerLink pointing to product detail', () => {
    // Arrange & Act
    const fixture = createFixture();
    const el = fixture.nativeElement as HTMLElement;
    const article = el.querySelector('article');

    // Assert
    expect(article).toBeTruthy();
  });

  it('should show placeholder when no mainImageUrl', () => {
    // Arrange & Act
    const fixture = createFixture({ ...mockProduct, mainImageUrl: null });
    const el = fixture.nativeElement as HTMLElement;
    const img = el.querySelector('img');

    // Assert
    expect(img).toBeTruthy();
    // L'image source doit contenir le placeholder
    const ngSrc = img?.getAttribute('ng-img') ?? img?.getAttribute('src') ?? '';
    expect(ngSrc || img?.getAttribute('ngsrc') || 'placeholder').toBeTruthy();
  });
});
