namespace Phoenix.IntegrationTests;

/// <summary>
/// Définition de la collection xUnit partagée entre tous les tests d'intégration.
/// Garantit que <see cref="PhoenixWebAppFactory"/> (et son container PostgreSQL)
/// est instancié une seule fois par run de test, pour toutes les classes de la collection.
/// </summary>
[CollectionDefinition("Integration")]
public sealed class IntegrationTestCollection : ICollectionFixture<Phoenix.IntegrationTests.Common.PhoenixWebAppFactory>
{
    // Classe marqueur — pas de code nécessaire.
}
