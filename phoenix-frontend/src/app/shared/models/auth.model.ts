/**
 * Interfaces TypeScript miroir des DTOs .NET du module Auth.
 */

/** Réponse d'authentification reçue du serveur (register, login, refresh). */
export interface AuthResponseDto {
  /** JWT access token signé (durée de vie : 15 minutes). À conserver en mémoire / sessionStorage. */
  accessToken: string;
  /** Durée de validité de l'access token en secondes (ex : 900). */
  expiresIn: number;
  /** Profil complet de l'utilisateur authentifié. */
  user: UserProfileDto;
}

/** Requête de connexion. */
export interface LoginRequest {
  email: string;
  password: string;
  /** Si true, prolonge la durée de vie du cookie refresh token. */
  rememberMe?: boolean;
}

/** Requête d'inscription d'un nouveau client. */
export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  companyName?: string;
  /** Valeur de l'enum CustomerSegment (ex : "FastFood", "BakeryPastry"). */
  segment: string;
}

/** Requête d'envoi du lien de réinitialisation de mot de passe. */
export interface ForgotPasswordRequest {
  email: string;
}

/** Requête de réinitialisation du mot de passe via le lien de reset. */
export interface ResetPasswordRequest {
  email: string;
  /** Token de reset extrait du lien envoyé par e-mail. */
  token: string;
  newPassword: string;
  confirmNewPassword: string;
}

// Import circulaire évité — UserProfileDto est défini dans user-profile.model.ts
// et importé ici pour typer AuthResponseDto.
import type { UserProfileDto } from './user-profile.model';
