namespace Projeto.Domain.Entities;

/// <summary>
/// Produto entregue por transportadora: peso e dimensoes da embalagem sao usados no frete.
/// </summary>
public class ProdutoFisico : Produto
{
    public int PesoGramas { get; set; }
    public int AlturaCm { get; set; }
    public int LarguraCm { get; set; }
    public int ComprimentoCm { get; set; }
}
