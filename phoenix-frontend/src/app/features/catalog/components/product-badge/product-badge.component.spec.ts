import { TestBed } from '@angular/core/testing';
import { ProductBadgeComponent } from './product-badge.component';

describe('ProductBadgeComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductBadgeComponent]
    }).compileComponents();
  });

  function createFixture(inputs: {
    isGourmetRange?: boolean;
    isCustomizable?: boolean;
    hasExpressDelivery?: boolean;
    isEcoFriendly?: boolean;
    isFoodApproved?: boolean;
    soldByWeight?: boolean;
    size?: 'sm' | 'md';
  }) {
    const fixture = TestBed.createComponent(ProductBadgeComponent);
    if (inputs.isGourmetRange   !== undefined) fixture.componentRef.setInput('isGourmetRange',   inputs.isGourmetRange);
    if (inputs.isCustomizable   !== undefined) fixture.componentRef.setInput('isCustomizable',   inputs.isCustomizable);
    if (inputs.hasExpressDelivery !== undefined) fixture.componentRef.setInput('hasExpressDelivery', inputs.hasExpressDelivery);
    if (inputs.isEcoFriendly    !== undefined) fixture.componentRef.setInput('isEcoFriendly',    inputs.isEcoFriendly);
    if (inputs.isFoodApproved   !== undefined) fixture.componentRef.setInput('isFoodApproved',   inputs.isFoodApproved);
    if (inputs.soldByWeight     !== undefined) fixture.componentRef.setInput('soldByWeight',     inputs.soldByWeight);
    if (inputs.size             !== undefined) fixture.componentRef.setInput('size',             inputs.size);
    fixture.detectChanges();
    return fixture;
  }

  it('should show Gourmet badge when isGourmetRange is true', () => {
    // Arrange & Act
    const fixture = createFixture({ isGourmetRange: true });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.textContent).toContain('Gamme Gourmet');
    expect(el.querySelector('.badge--gourmet')).toBeTruthy();
  });

  it('should show Personnalisable badge when isCustomizable is true', () => {
    // Arrange & Act
    const fixture = createFixture({ isCustomizable: true });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.textContent).toContain('Personnalisable');
    expect(el.querySelector('.badge--custom')).toBeTruthy();
  });

  it('should show Express badge when hasExpressDelivery is true', () => {
    // Arrange & Act
    const fixture = createFixture({ hasExpressDelivery: true });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.textContent).toContain('Express 24h');
    expect(el.querySelector('.badge--express')).toBeTruthy();
  });

  it('should not show Eco badge when isEcoFriendly is false', () => {
    // Arrange & Act
    const fixture = createFixture({ isEcoFriendly: false });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.querySelector('.badge--eco')).toBeFalsy();
    expect(el.textContent).not.toContain('Éco-responsable');
  });

  it('should show all badges when all flags are true', () => {
    // Arrange & Act
    const fixture = createFixture({
      isGourmetRange: true,
      isCustomizable: true,
      hasExpressDelivery: true,
      isEcoFriendly: true,
      isFoodApproved: true,
      soldByWeight: true
    });
    const el = fixture.nativeElement as HTMLElement;
    const badges = el.querySelectorAll('.badge');

    // Assert
    expect(badges.length).toBe(6);
    expect(el.textContent).toContain('Gamme Gourmet');
    expect(el.textContent).toContain('Personnalisable');
    expect(el.textContent).toContain('Express 24h');
    expect(el.textContent).toContain('Éco-responsable');
    expect(el.textContent).toContain('Agréé alimentaire');
    expect(el.textContent).toContain('Vendu au KG');
  });

  it('should show no badges when all flags are false', () => {
    // Arrange & Act
    const fixture = createFixture({
      isGourmetRange: false,
      isCustomizable: false,
      hasExpressDelivery: false,
      isEcoFriendly: false,
      isFoodApproved: false,
      soldByWeight: false
    });
    const el = fixture.nativeElement as HTMLElement;
    const badges = el.querySelectorAll('.badge');

    // Assert
    expect(badges.length).toBe(0);
  });

  it('should apply sm class when size is sm', () => {
    // Arrange & Act
    const fixture = createFixture({ isCustomizable: true, size: 'sm' });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.querySelector('.badge-list--sm')).toBeTruthy();
  });

  it('should not apply sm class when size is md (default)', () => {
    // Arrange & Act
    const fixture = createFixture({ isCustomizable: true, size: 'md' });
    const el = fixture.nativeElement as HTMLElement;

    // Assert
    expect(el.querySelector('.badge-list--sm')).toBeFalsy();
  });
});
