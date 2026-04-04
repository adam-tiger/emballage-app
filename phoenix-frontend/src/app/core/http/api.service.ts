import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../config/environment';

/**
 * Service HTTP de base utilisé par tous les services métier de l'application.
 * Préfixe automatiquement les URLs avec `environment.apiBaseUrl`.
 */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  /** Effectue une requête GET. */
  get<T>(path: string, params?: Record<string, unknown>): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}${path}`, {
      params: buildParams(params)
    });
  }

  /** Effectue une requête POST avec un corps JSON. */
  post<T>(path: string, body: unknown): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${path}`, body);
  }

  /** Effectue une requête PUT avec un corps JSON. */
  put<T>(path: string, body: unknown): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${path}`, body);
  }

  /** Effectue une requête DELETE. */
  delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${path}`);
  }

  /** Effectue une requête POST avec un corps `multipart/form-data`. */
  postForm<T>(path: string, formData: FormData): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${path}`, formData);
  }
}

/**
 * Construit un objet `HttpParams` en ignorant les valeurs `null` et `undefined`.
 */
function buildParams(params?: Record<string, unknown>): HttpParams {
  if (!params) return new HttpParams();

  return Object.entries(params).reduce((acc, [key, value]) => {
    if (value !== null && value !== undefined) {
      return acc.set(key, String(value));
    }
    return acc;
  }, new HttpParams());
}
