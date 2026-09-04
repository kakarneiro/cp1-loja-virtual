namespace Projeto.Domain.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public required string NomeCompleto { get; set; }
    public required string Cpf { get; set; }
    public required string Email { get; set; }
    public string? Telefone { get; set; }
    public DateOnly? DataNascimento { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; } = true;

    public Guid? EnderecoId { get; set; }
    public Endereco? Endereco { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
