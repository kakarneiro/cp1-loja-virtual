namespace Projeto.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; }

    public Guid PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public Guid ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
