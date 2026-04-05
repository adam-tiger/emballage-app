namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Port (interface) du service d'envoi d'e-mails transactionnels.
/// Implémenté dans la couche Infrastructure (ex : Azure Communication Services, SendGrid).
/// Tous les envois sont non-bloquants et gèrent les erreurs en interne (logs + retry).
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envoie l'e-mail de bienvenue au nouveau client après son inscription.
    /// </summary>
    /// <param name="to">Adresse e-mail du destinataire.</param>
    /// <param name="firstName">Prénom du client pour personnaliser le message.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task SendWelcomeEmailAsync(
        string to,
        string firstName,
        CancellationToken ct = default);

    /// <summary>
    /// Envoie l'e-mail de réinitialisation de mot de passe.
    /// </summary>
    /// <param name="to">Adresse e-mail du destinataire.</param>
    /// <param name="resetLink">Lien de réinitialisation incluant le token temporaire.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task SendPasswordResetEmailAsync(
        string to,
        string resetLink,
        CancellationToken ct = default);

    /// <summary>
    /// Envoie la confirmation de commande après validation du paiement.
    /// </summary>
    /// <param name="to">Adresse e-mail du client.</param>
    /// <param name="orderRef">Référence lisible de la commande (ex : "PHX-2025-00042").</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task SendOrderConfirmationEmailAsync(
        string to,
        string orderRef,
        CancellationToken ct = default);

    /// <summary>
    /// Notifie le client que son BAT (Bon À Tirer) est prêt pour validation.
    /// </summary>
    /// <param name="to">Adresse e-mail du client.</param>
    /// <param name="orderRef">Référence de la commande concernée.</param>
    /// <param name="batLink">URL publique signée permettant de visualiser et valider le BAT.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task SendBatReadyEmailAsync(
        string to,
        string orderRef,
        string batLink,
        CancellationToken ct = default);

    /// <summary>
    /// Notifie le client que sa commande a été expédiée avec les informations de suivi.
    /// </summary>
    /// <param name="to">Adresse e-mail du client.</param>
    /// <param name="orderRef">Référence de la commande expédiée.</param>
    /// <param name="trackingUrl">URL de suivi du colis chez le transporteur.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task SendOrderShippedEmailAsync(
        string to,
        string orderRef,
        string trackingUrl,
        CancellationToken ct = default);
}
