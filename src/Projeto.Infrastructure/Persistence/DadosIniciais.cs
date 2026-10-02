namespace Projeto.Infrastructure.Persistence;

/// <summary>
/// Ids fixos dos dados iniciais (HasData). Precisam ser constantes para que a migration
/// gerada seja sempre a mesma.
/// </summary>
internal static class DadosIniciais
{
    public static readonly Guid CategoriaEletronicosId = new("0b1d6c2e-5a1f-4c3a-9f10-000000000001");
    public static readonly Guid CategoriaLivrosId = new("0b1d6c2e-5a1f-4c3a-9f10-000000000002");

    public static readonly Guid ProdutoMouseId = new("5e7a9d4c-2b3f-4e8a-8c20-000000000001");
    public static readonly Guid ProdutoEbookId = new("5e7a9d4c-2b3f-4e8a-8c20-000000000002");
}
