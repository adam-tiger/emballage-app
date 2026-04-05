import { CanActivateFn } from '@angular/router';

export const adminGuard: CanActivateFn = () => {
  // TODO Tour Auth : vérifier le rôle Admin/Employee via AuthService
  return true;
};
