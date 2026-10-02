using System.Linq.Expressions;

namespace Projeto.Application;

/// <summary>
/// Contrato generico de acesso a dados. Fica na Application para que nenhuma camada
/// interna dependa do EF Core; a implementacao concreta mora na Infrastructure.
/// </summary>
public interface IRepositorio<TEntidade> where TEntidade : class
{
    Task<TEntidade?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntidade>> ListarAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntidade>> BuscarAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(TEntidade entidade, CancellationToken cancellationToken = default);

    void Atualizar(TEntidade entidade);

    void Remover(TEntidade entidade);

    /// <summary>Grava no banco todas as alteracoes pendentes.</summary>
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
