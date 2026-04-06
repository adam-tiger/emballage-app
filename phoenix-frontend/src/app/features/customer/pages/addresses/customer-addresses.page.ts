import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
  signal,
} from '@angular/core';

import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ToastModule }  from 'primeng/toast';
import { MessageService } from 'primeng/api';

import { ApiError }          from '../../../../shared/models/api-error.model';
import { CustomerService }   from '../../services/customer.service';
import { AddressCardComponent }    from '../../components/address-card/address-card.component';
import { AddAddressFormComponent } from '../../components/add-address-form/add-address-form.component';
import { AddAddressRequest }       from '../../models/customer-address.model';

/**
 * Page de gestion des adresses de livraison du client.
 *
 * - Liste des adresses via AddressCardComponent
 * - Ajout d'une adresse via Dialog + AddAddressFormComponent
 * - Définir par défaut via CustomerService.setDefaultAddress()
 * - Maximum 5 adresses par client (bouton désactivé au-delà)
 *
 * Note : showAddFormVisible est un getter/setter qui wrappe le signal
 * `showAddForm` pour la compatibilité avec [(visible)] de p-dialog.
 */
@Component({
  selector: 'app-customer-addresses-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ButtonModule,
    DialogModule,
    ToastModule,
    AddressCardComponent,
    AddAddressFormComponent,
  ],
  template: `
    <p-toast />

    <div class="addresses-page">

      <div class="addresses-page__header">
        <div>
          <h2 class="addresses-page__title">Mes adresses de livraison</h2>
          <p class="addresses-page__sub">
            Vous pouvez enregistrer jusqu'à 5 adresses de livraison.
          </p>
        </div>
        <p-button
          label="+ Ajouter une adresse"
          severity="secondary"
          [outlined]="true"
          [disabled]="customerService.addresses().length >= 5"
          (onClick)="showAddForm.set(true)" />
      </div>

      <!-- État vide -->
      @if (customerService.addresses().length === 0 && !customerService.isLoading()) {
        <div class="addresses-empty">
          <span class="addresses-empty__icon">📍</span>
          <p>Vous n'avez pas encore d'adresse de livraison enregistrée.</p>
          <p-button
            label="Ajouter ma première adresse"
            (onClick)="showAddForm.set(true)" />
        </div>
      }

      <!-- Grille d'adresses -->
      <div class="addresses-grid">
        @for (address of customerService.addresses(); track address.id) {
          <app-address-card
            [address]="address"
            (setDefault)="onSetDefault($event)" />
        }
      </div>

      <!-- Dialog ajout adresse -->
      <p-dialog
        header="Nouvelle adresse de livraison"
        [(visible)]="showAddFormVisible"
        [modal]="true"
        [style]="{ width: '480px' }"
        [draggable]="false"
        [closable]="true">
        <app-add-address-form
          [isSaving]="customerService.isSaving()"
          (addressSubmitted)="onAddressSubmitted($event)"
          (cancelled)="showAddForm.set(false)" />
      </p-dialog>

    </div>
  `,
  styleUrl: './customer-addresses.page.scss'
})
export class CustomerAddressesPage implements OnInit {
  protected readonly customerService = inject(CustomerService);
  private   readonly messageService  = inject(MessageService);

  /** Signal interne contrôlant la visibilité du dialog. */
  protected readonly showAddForm = signal(false);

  /**
   * Getter/setter wrappant le signal pour la compatibilité
   * avec [(visible)] de p-dialog (two-way binding PrimeNG).
   */
  get showAddFormVisible(): boolean {
    return this.showAddForm();
  }
  set showAddFormVisible(val: boolean) {
    this.showAddForm.set(val);
  }

  ngOnInit(): void {
    this.customerService.loadProfile();
  }

  protected onAddressSubmitted(request: AddAddressRequest): void {
    this.customerService.addAddress(request).subscribe({
      next: () => {
        this.showAddForm.set(false);
        this.messageService.add({
          severity: 'success',
          summary:  'Adresse ajoutée avec succès'
        });
      },
      error: (err: ApiError) => {
        this.messageService.add({
          severity: 'error',
          summary:  'Erreur',
          detail:   err.message
        });
      }
    });
  }

  protected onSetDefault(addressId: string): void {
    this.customerService.setDefaultAddress(addressId).subscribe({
      next: () => this.messageService.add({
        severity: 'success',
        summary:  'Adresse par défaut mise à jour'
      }),
      error: () => this.messageService.add({
        severity: 'error',
        summary:  'Erreur lors de la mise à jour'
      })
    });
  }
}
