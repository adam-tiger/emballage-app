using Microsoft.Extensions.Logging;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Infrastructure.Email;

/// <summary>
/// Implémentation bouchon de <see cref="IEmailService"/> pour les environnements
/// de développement et de test.
/// Logue les e-mails au lieu de les envoyer réellement.
/// </summary>
/// <remarks>
/// À remplacer par une implémentation SendGrid (ou autre) en production.
/// </remarks>
public sealed class StubEmailService(ILogger<StubEmailService> logger) : IEmailService
{
    /// <inheritdoc />
    public Task SendWelcomeEmailAsync(string to, string firstName, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL STUB] Bienvenue → {To} | Prénom : {FirstName}", to, firstName);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL STUB] Réinitialisation MDP → {To} | Lien : {ResetLink}", to, resetLink);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendOrderConfirmationEmailAsync(string to, string orderRef, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL STUB] Confirmation commande → {To} | Ref : {OrderRef}", to, orderRef);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendBatReadyEmailAsync(string to, string orderRef, string batLink, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL STUB] BAT prêt → {To} | Ref : {OrderRef} | Lien : {BatLink}",
            to, orderRef, batLink);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendOrderShippedEmailAsync(string to, string orderRef, string trackingUrl, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL STUB] Commande expédiée → {To} | Ref : {OrderRef} | Suivi : {TrackingUrl}",
            to, orderRef, trackingUrl);
        return Task.CompletedTask;
    }
}
