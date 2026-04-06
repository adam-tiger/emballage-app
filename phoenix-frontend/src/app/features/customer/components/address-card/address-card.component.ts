import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';

import { CustomerAddressDto } from '../../models/customer-address.model';

/**
 * Composant dumb — carte d'une adresse de livraison.
 *
 * Affiche les informations de l'adresse et un bouton pour la définir par défaut
 * (masqué si elle l'est déjà). Émet `setDefault` avec l'id de l'adresse.
 */
@Component({
  selector: 'app-address-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ButtonModule],
  template: `
    <div class="address-card"
         [class.address-card--default]="address().isDefault">

      <div class="address-card__header">
        <span class="address-card__label">{{ address().label }}</span>
        @if (address().isDefault) {
          <span class="address-card__badge">Par défaut</span>
        }
      </div>

      <div class="address-card__body">
        <p>{{ address().street }}</p>
        <p>{{ address().postalCode }} {{ address().city }}</p>
        <p>{{ address().country }}</p>
      </div>

      @if (!address().isDefault) {
        <p-button
          label="Définir par défaut"
          severity="secondary"
          [text]="true"
          size="small"
          (onClick)="setDefault.emit(address().id)" />
      }

    </div>
  `,
  styleUrl: './address-card.component.scss'
})
export class AddressCardComponent {
  /** Adresse à afficher. */
  readonly address = input.required<CustomerAddressDto>();

  /** Émis avec l'id de l'adresse quand l'utilisateur clique "Définir par défaut". */
  readonly setDefault = output<string>();
}
