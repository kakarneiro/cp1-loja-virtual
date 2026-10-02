namespace Projeto.Domain.Entities;

/// <summary>
/// Classe base da hierarquia de produtos. E abstrata: todo produto vendido e
/// fisico (<see cref="ProdutoFisico"/>) ou digital (<see cref="ProdutoDigital"/>).
/// </summary>
public abstract class Produto
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public required string CodigoBarras { get; set; }
    public decimal Preco { get; set; }
    public int EstoqueAtual { get; set; }
    public bool Ativo { get; set; } = true;

    public Guid CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}
