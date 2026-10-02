using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Projeto.Application.Repositories;
using Projeto.Infrastructure.Persistence;

namespace Projeto.Infrastructure.Repositories;

/// <summary>
/// Implementacao EF Core do IRepositorio. Apenas acesso a dados: nenhuma regra de negocio.
/// </summary>
public class Repositorio<TEntidade> : IRepositorio<TEntidade> where TEntidade : class
{
    private readonly LojaVirtualContext _context;
    private readonly DbSet<TEntidade> _dbSet;

    public Repositorio(LojaVirtualContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntidade>();
    }

    public async Task<TEntidade?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<TEntidade>> ListarAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntidade>> BuscarAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(filtro).ToListAsync(cancellationToken);

    public Task<bool> ExisteAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(filtro, cancellationToken);

    public async Task AdicionarAsync(TEntidade entidade, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entidade, cancellationToken);

    public void Atualizar(TEntidade entidade) => _dbSet.Update(entidade);

    public void Remover(TEntidade entidade) => _dbSet.Remove(entidade);

    public Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
