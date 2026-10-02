using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(c => c.Descricao)
            .HasMaxLength(250);

        builder.HasIndex(c => c.Nome).IsUnique();

        // 1:N Categoria -> Produto. Restrict: nao se apaga categoria que ainda tem produtos.
        builder.HasMany(c => c.Produtos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
