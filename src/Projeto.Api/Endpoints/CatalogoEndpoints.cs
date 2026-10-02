using Projeto.Application.Repositories;
using Projeto.Domain.Entities;

namespace Projeto.Api.Endpoints;

/// <summary>
/// Endpoints somente leitura para verificar a persistencia de ponta a ponta:
/// API -> IRepositorio (Application) -> Repositorio (Infrastructure) -> MySQL.
/// Usam apenas o contrato do repositorio, nunca o DbContext.
/// </summary>
public static class CatalogoEndpoints
{
    public static IEndpointRouteBuilder MapCatalogoEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapGet("/categorias", async (IRepositorio<Categoria> repositorio, CancellationToken ct) =>
        {
            var categorias = await repositorio.ListarAsync(ct);
            return Results.Ok(categorias.OrderBy(c => c.Nome).Select(c => new CategoriaResposta(c.Id, c.Nome, c.Descricao)));
        });

        // Le a tabela inteira: o EF materializa cada linha no subtipo certo pelo discriminador.
        api.MapGet("/produtos", async (IRepositorio<Produto> repositorio, CancellationToken ct) =>
        {
            var produtos = await repositorio.ListarAsync(ct);
            return Results.Ok(produtos.OrderBy(p => p.Nome).Select(ProdutoResposta.De));
        });

        api.MapGet("/produtos/{id:guid}", async (Guid id, IRepositorio<Produto> repositorio, CancellationToken ct) =>
        {
            var produto = await repositorio.ObterPorIdAsync(id, ct);
            return produto is null ? Results.NotFound() : Results.Ok(ProdutoResposta.De(produto));
        });

        // O repositorio de um subtipo filtra pelo discriminador (WHERE TipoProduto = '...').
        api.MapGet("/produtos/fisicos", async (IRepositorio<ProdutoFisico> repositorio, CancellationToken ct) =>
        {
            var produtos = await repositorio.ListarAsync(ct);
            return Results.Ok(produtos.OrderBy(p => p.Nome).Select(ProdutoResposta.De));
        });

        api.MapGet("/produtos/digitais", async (IRepositorio<ProdutoDigital> repositorio, CancellationToken ct) =>
        {
            var produtos = await repositorio.ListarAsync(ct);
            return Results.Ok(produtos.OrderBy(p => p.Nome).Select(ProdutoResposta.De));
        });

        return app;
    }
}

public record CategoriaResposta(Guid Id, string Nome, string? Descricao);

public record ProdutoResposta(
    Guid Id,
    string Tipo,
    string Nome,
    string? Descricao,
    string CodigoBarras,
    decimal Preco,
    int EstoqueAtual,
    bool Ativo,
    Guid CategoriaId,
    object Detalhes)
{
    public static ProdutoResposta De(Produto produto) => produto switch
    {
        ProdutoFisico f => Criar(f, "Fisico", new
        {
            f.PesoGramas,
            f.AlturaCm,
            f.LarguraCm,
            f.ComprimentoCm
        }),
        ProdutoDigital d => Criar(d, "Digital", new
        {
            d.UrlDownload,
            d.TamanhoArquivoMb,
            d.LimiteDownloads
        }),
        _ => throw new NotSupportedException($"Tipo de produto não mapeado: {produto.GetType().Name}")
    };

    private static ProdutoResposta Criar(Produto p, string tipo, object detalhes) => new(
        p.Id, tipo, p.Nome, p.Descricao, p.CodigoBarras, p.Preco, p.EstoqueAtual, p.Ativo, p.CategoriaId, detalhes);
}
