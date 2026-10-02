using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

/// <summary>
/// Subtipo da hierarquia TPH de Produto: so declara as colunas proprias do produto fisico.
/// </summary>
public class ProdutoFisicoConfiguration : IEntityTypeConfiguration<ProdutoFisico>
{
    public void Configure(EntityTypeBuilder<ProdutoFisico> builder)
    {
        builder.HasBaseType<Produto>();

        builder.Property(p => p.PesoGramas).IsRequired();
        builder.Property(p => p.AlturaCm).IsRequired();
        builder.Property(p => p.LarguraCm).IsRequired();
        builder.Property(p => p.ComprimentoCm).IsRequired();

        // Dado inicial para o endpoint de verificacao (Ids fixos para a migration ser deterministica).
        builder.HasData(new ProdutoFisico
        {
            Id = DadosIniciais.ProdutoMouseId,
            Nome = "Mouse sem fio",
            Descricao = "Mouse óptico sem fio, 1600 DPI",
            CodigoBarras = "7891000000017",
            Preco = 89.90m,
            EstoqueAtual = 25,
            Ativo = true,
            CategoriaId = DadosIniciais.CategoriaEletronicosId,
            PesoGramas = 95,
            AlturaCm = 4,
            LarguraCm = 7,
            ComprimentoCm = 11
        });
    }
}
