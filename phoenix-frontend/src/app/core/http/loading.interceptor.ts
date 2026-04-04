import { Injectable, computed, signal } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.headers.has('X-Skip-Loading')) {
    return next(req);
  }
  LoadingService.increment();
  return next(req).pipe(
    finalize(() => LoadingService.decrement())
  );
};

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private static readonly _count = signal(0);
  static readonly isLoading = computed(() => LoadingService._count() > 0);
  static readonly count = LoadingService._count.asReadonly();

  static increment(): void {
    LoadingService._count.update(c => c + 1);
  }

  static decrement(): void {
    LoadingService._count.update(c => Math.max(0, c - 1));
  }
}
