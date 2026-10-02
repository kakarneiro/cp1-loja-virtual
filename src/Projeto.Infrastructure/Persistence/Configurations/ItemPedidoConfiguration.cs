using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entidade associativa que decompoe o N:N Pedido x Produto em dois 1:N.
/// </summary>
public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido", t =>
        {
            t.HasCheckConstraint("CK_ItensPedido_Quantidade", "Quantidade > 0");
            t.HasCheckConstraint("CK_ItensPedido_PrecoUnitario", "PrecoUnitario >= 0");
            t.HasCheckConstraint("CK_ItensPedido_Subtotal", "Subtotal >= 0");
        });

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantidade)
            .IsRequired();

        builder.Property(i => i.PrecoUnitario)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(i => i.Subtotal)
            .IsRequired()
            .HasPrecision(10, 2);

        // Um mesmo produto aparece uma unica vez dentro de um pedido (a quantidade vai em
        // Quantidade). O indice composto tambem atende a FK PedidoId.
        builder.HasIndex(i => new { i.PedidoId, i.ProdutoId }).IsUnique();

        // 1:N Produto -> ItemPedido. Restrict: produto ja vendido nao pode ser apagado,
        // senao o historico dos pedidos se perde (use Produto.Ativo = false).
        builder.HasOne(i => i.Produto)
            .WithMany(p => p.Itens)
            .HasForeignKey(i => i.ProdutoId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // A FK PedidoId e configurada em PedidoConfiguration.
    }
}
