/**
 * DTO du profil utilisateur authentifié — miroir de `UserProfileDto` .NET.
 */
export interface UserProfileDto {
  /** Identifiant unique de l'ApplicationUser (GUID sous forme de string). */
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  /** Nom complet calculé (prénom + nom). */
  fullName: string;
  companyName: string | null;
  /** Valeur string du segment professionnel (ex : "FastFood"). */
  segment: string;
  /** Rôles applicatifs (ex : ["Customer"], ["Admin", "Employee"]). */
  roles: string[];
  isActive: boolean;
  /** Date de création du compte en ISO 8601 UTC. */
  createdAtUtc: string;
}

/** Retourne true si l'utilisateur a le rôle Admin. */
export function isAdmin(user: UserProfileDto): boolean {
  return user.roles.includes('Admin');
}

/** Retourne true si l'utilisateur a le rôle Employee ou Admin. */
export function isEmployee(user: UserProfileDto): boolean {
  return user.roles.includes('Employee') || isAdmin(user);
}

/** Retourne true si l'utilisateur a le rôle Customer. */
export function isCustomer(user: UserProfileDto): boolean {
  return user.roles.includes('Customer');
}
