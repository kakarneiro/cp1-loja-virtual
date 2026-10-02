namespace Projeto.Domain.Entities;

/// <summary>
/// Produto entregue por download (e-book, software, curso): nao tem frete nem dimensoes.
/// </summary>
public class ProdutoDigital : Produto
{
    public required string UrlDownload { get; set; }
    public decimal TamanhoArquivoMb { get; set; }
    public int? LimiteDownloads { get; set; }
}
