using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos", t =>
        {
            t.HasCheckConstraint("CK_Produtos_Preco", "Preco >= 0");
            t.HasCheckConstraint("CK_Produtos_EstoqueAtual", "EstoqueAtual >= 0");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        builder.Property(p => p.CodigoBarras)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(p => p.Preco)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(p => p.EstoqueAtual)
            .IsRequired()
            .HasDefaultValue(0);

        // Mesmo tratamento de Cliente.Ativo: so omite no INSERT quando o valor e true.
        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true)
            .HasSentinel(true);

        builder.HasIndex(p => p.CodigoBarras).IsUnique();

        // A FK CategoriaId e configurada em CategoriaConfiguration.
    }
}
