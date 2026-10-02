using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

/// <summary>
/// Raiz da hierarquia TPH (Table-per-Hierarchy): Produto, ProdutoFisico e ProdutoDigital
/// ficam todos na tabela Produtos, distinguidos pela coluna discriminadora TipoProduto.
/// As propriedades dos subtipos estao em ProdutoFisicoConfiguration e ProdutoDigitalConfiguration.
/// </summary>
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public const string ColunaDiscriminadora = "TipoProduto";
    public const string TipoFisico = "Fisico";
    public const string TipoDigital = "Digital";

    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos", t =>
        {
            t.HasCheckConstraint("CK_Produtos_Preco", "Preco >= 0");
            t.HasCheckConstraint("CK_Produtos_EstoqueAtual", "EstoqueAtual >= 0");

            t.HasCheckConstraint("CK_Produtos_TipoProduto",
                $"{ColunaDiscriminadora} IN ('{TipoFisico}', '{TipoDigital}')");

            // No TPH as colunas de um subtipo sao NULL para as linhas do outro subtipo, entao o
            // banco nao pode declara-las NOT NULL. Estes CHECKs devolvem a obrigatoriedade
            // por subtipo: produto fisico exige peso e medidas; digital exige link e tamanho.
            t.HasCheckConstraint("CK_Produtos_Fisico",
                $"{ColunaDiscriminadora} <> '{TipoFisico}' OR (PesoGramas IS NOT NULL AND AlturaCm IS NOT NULL " +
                "AND LarguraCm IS NOT NULL AND ComprimentoCm IS NOT NULL)");
            t.HasCheckConstraint("CK_Produtos_Digital",
                $"{ColunaDiscriminadora} <> '{TipoDigital}' OR (UrlDownload IS NOT NULL AND TamanhoArquivoMb IS NOT NULL)");
        });

        builder.HasKey(p => p.Id);

        builder.HasDiscriminator<string>(ColunaDiscriminadora)
            .HasValue<ProdutoFisico>(TipoFisico)
            .HasValue<ProdutoDigital>(TipoDigital);

        builder.Property<string>(ColunaDiscriminadora)
            .IsRequired()
            .HasMaxLength(20);

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
