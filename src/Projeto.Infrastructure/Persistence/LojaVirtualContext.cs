using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence;

public class LojaVirtualContext : DbContext
{
    public LojaVirtualContext(DbContextOptions<LojaVirtualContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as classes IEntityTypeConfiguration<T> da pasta Configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LojaVirtualContext).Assembly);
    }
}
