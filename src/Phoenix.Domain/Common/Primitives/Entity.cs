namespace Phoenix.Domain.Common.Primitives;

/// <summary>
/// Classe de base pour toutes les entités du domaine.
/// Une entité est identifiée par son <see cref="Id"/> et non par l'ensemble de ses attributs.
/// Elle peut accumuler des événements de domaine à dispatcher en fin de transaction.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Constructeur principal — utilisé par les factory methods ou les constructeurs des sous-classes.
    /// </summary>
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("L'identifiant d'une entité ne peut pas être vide.", nameof(id));

        Id = id;
    }

    /// <summary>
    /// Constructeur sans paramètre réservé à EF Core (mapping).
    /// Ne jamais appeler directement dans le code métier.
    /// </summary>
    protected Entity() { }

    /// <summary>Identifiant immuable de l'entité.</summary>
    public Guid Id { get; private init; }

    /// <summary>
    /// Liste en lecture seule des événements de domaine accumulés depuis le dernier dispatch.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Ajoute un événement à la file des événements en attente de dispatch.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>
    /// Vide la liste des événements après que l'Infrastructure les a dispatchés.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    // ----- Égalité basée sur l'Id -----

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Entity? left, Entity? right) =>
        !(left == right);
}
