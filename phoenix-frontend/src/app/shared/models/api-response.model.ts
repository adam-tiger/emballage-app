/** Contrat d'erreur uniforme retourné par l'API Phoenix. */
export interface ApiErrorResponse {
  code: string;
  message: string;
  details?: Record<string, string[]>;
  traceId: string;
}
