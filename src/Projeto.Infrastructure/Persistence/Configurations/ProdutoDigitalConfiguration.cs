using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

/// <summary>
/// Subtipo da hierarquia TPH de Produto: so declara as colunas proprias do produto digital.
/// </summary>
public class ProdutoDigitalConfiguration : IEntityTypeConfiguration<ProdutoDigital>
{
    public void Configure(EntityTypeBuilder<ProdutoDigital> builder)
    {
        builder.HasBaseType<Produto>();

        builder.Property(p => p.UrlDownload)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.TamanhoArquivoMb)
            .IsRequired()
            .HasPrecision(10, 2);

        // Opcional de verdade: NULL significa downloads ilimitados.
        builder.Property(p => p.LimiteDownloads);

        // Dado inicial para o endpoint de verificacao (Ids fixos para a migration ser deterministica).
        builder.HasData(new ProdutoDigital
        {
            Id = DadosIniciais.ProdutoEbookId,
            Nome = "E-book Clean Architecture na prática",
            Descricao = "E-book em PDF e EPUB",
            CodigoBarras = "9786500000011",
            Preco = 39.90m,
            EstoqueAtual = 0,
            Ativo = true,
            CategoriaId = DadosIniciais.CategoriaLivrosId,
            UrlDownload = "https://loja.exemplo.com/downloads/ebook-clean-architecture",
            TamanhoArquivoMb = 12.50m,
            LimiteDownloads = 3
        });
    }
}
