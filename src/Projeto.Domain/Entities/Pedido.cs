using Projeto.Domain.Enums;

namespace Projeto.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public required string Numero { get; set; }
    public DateTime DataPedido { get; set; }
    public StatusPedido Status { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? Desconto { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string? Observacao { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}
