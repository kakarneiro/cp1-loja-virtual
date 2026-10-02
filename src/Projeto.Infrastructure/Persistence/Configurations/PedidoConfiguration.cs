using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos", t =>
        {
            t.HasCheckConstraint("CK_Pedidos_ValorTotal", "ValorTotal >= 0");
            t.HasCheckConstraint("CK_Pedidos_Desconto", "Desconto IS NULL OR Desconto >= 0");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.DataPedido)
            .IsRequired();

        // Enums gravados como int, como no MER.
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.FormaPagamento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.ValorTotal)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(p => p.Desconto)
            .HasPrecision(10, 2);

        builder.Property(p => p.DataEntrega);

        builder.Property(p => p.Observacao)
            .HasMaxLength(300);

        builder.HasIndex(p => p.Numero).IsUnique();

        // 1:N Pedido -> ItemPedido. Cascade: os itens nao existem sem o pedido.
        // O "minimo 1 item" do MER nao e expressavel em DDL; e regra de dominio/aplicacao.
        builder.HasMany(p => p.Itens)
            .WithOne(i => i.Pedido)
            .HasForeignKey(i => i.PedidoId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // A FK ClienteId e configurada em ClienteConfiguration.
    }
}
