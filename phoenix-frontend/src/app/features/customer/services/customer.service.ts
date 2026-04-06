import { DestroyRef, Injectable, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, finalize, tap } from 'rxjs';

import { ApiService } from '../../../core/http/api.service';
import { ApiError }   from '../../../shared/models/api-error.model';
import {
  AddAddressRequest,
  CustomerProfileDto,
  UpdateProfileRequest,
} from '../models/customer-address.model';
import { CustomerDashboardDto } from '../models/customer-dashboard.model';

/**
 * Service Angular 21 de l'espace client Phoenix.
 *
 * Gère via Signals :
 * - Le profil client (données personnelles + adresses)
 * - Le tableau de bord (compteurs commandes/devis + adresse par défaut)
 * - Les états de chargement/sauvegarde/erreur
 *
 * Optimistic updates sur les adresses et la mise à jour de profil.
 */
@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly api        = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);

  // ── State privé ──────────────────────────────────────────────────────────

  private readonly _profile   = signal<CustomerProfileDto | null>(null);
  private readonly _dashboard = signal<CustomerDashboardDto | null>(null);
  private readonly _isLoading = signal(false);
  private readonly _isSaving  = signal(false);
  private readonly _error     = signal<ApiError | null>(null);

  // ── State public (readonly) ──────────────────────────────────────────────

  readonly profile   = this._profile.asReadonly();
  readonly dashboard = this._dashboard.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly isSaving  = this._isSaving.asReadonly();
  readonly error     = this._error.asReadonly();

  // ── Computed ─────────────────────────────────────────────────────────────

  /** Vrai si un profil a été chargé. */
  readonly hasProfile = computed(() => this._profile() !== null);

  /** Liste des adresses du client, ou tableau vide. */
  readonly addresses = computed(() => this._profile()?.addresses ?? []);

  /** Adresse de livraison par défaut, ou null. */
  readonly defaultAddress = computed(
    () => this._profile()?.addresses.find(a => a.isDefault) ?? null
  );

  // ── API publique ─────────────────────────────────────────────────────────

  /** Charge le profil complet du client depuis l'API. */
  loadProfile(): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.api.get<CustomerProfileDto>('/api/v1/customer/profile')
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this._isLoading.set(false))
      )
      .subscribe({
        next:  profile => this._profile.set(profile),
        error: (err: ApiError) => this._error.set(err)
      });
  }

  /** Charge le tableau de bord du client depuis l'API. */
  loadDashboard(): void {
    this._isLoading.set(true);

    this.api.get<CustomerDashboardDto>('/api/v1/customer/dashboard')
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this._isLoading.set(false))
      )
      .subscribe({
        next:  dashboard => this._dashboard.set(dashboard),
        error: (err: ApiError) => this._error.set(err)
      });
  }

  /**
   * Met à jour le profil client.
   * Applique un update optimiste immédiat avant confirmation du serveur.
   */
  updateProfile(request: UpdateProfileRequest): Observable<void> {
    this._isSaving.set(true);

    return this.api.put<void>('/api/v1/customer/profile', request).pipe(
      tap(() => {
        const current = this._profile();
        if (current) {
          this._profile.set({
            ...current,
            firstName:   request.firstName,
            lastName:    request.lastName,
            fullName:    `${request.firstName} ${request.lastName}`,
            companyName: request.companyName ?? null,
            segment:     request.segment
          });
        }
      }),
      finalize(() => this._isSaving.set(false))
    );
  }

  /**
   * Ajoute une nouvelle adresse de livraison et recharge le profil.
   * Retourne l'identifiant de la nouvelle adresse.
   */
  addAddress(request: AddAddressRequest): Observable<string> {
    this._isSaving.set(true);

    return this.api.post<string>('/api/v1/customer/addresses', request).pipe(
      tap(() => this.loadProfile()),
      finalize(() => this._isSaving.set(false))
    );
  }

  /**
   * Définit une adresse comme adresse par défaut.
   * Applique un update optimiste immédiat.
   */
  setDefaultAddress(addressId: string): Observable<void> {
    return this.api.put<void>(
      `/api/v1/customer/addresses/${addressId}/default`,
      {}
    ).pipe(
      tap(() => {
        const current = this._profile();
        if (current) {
          this._profile.set({
            ...current,
            addresses: current.addresses.map(a => ({
              ...a,
              isDefault: a.id === addressId
            }))
          });
        }
      })
    );
  }

  /** Efface l'erreur courante. */
  clearError(): void {
    this._error.set(null);
  }
}
