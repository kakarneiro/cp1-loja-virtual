using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    private static readonly ValueConverter<DateOnly, DateTime> DataNascimentoConverter = new(
        data => data.ToDateTime(TimeOnly.MinValue),
        dataHora => DateOnly.FromDateTime(dataHora));

    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NomeCompleto)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(c => c.Cpf)
            .IsRequired()
            .HasMaxLength(11)
            .IsFixedLength();

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        // O provider MySql.EntityFrameworkCore grava DateOnly mas nao consegue le-lo de volta
        // (InvalidCastException DateTime -> DateOnly); o conversor resolve mantendo a coluna "date".
        builder.Property(c => c.DataNascimento)
            .HasConversion(DataNascimentoConverter)
            .HasColumnType("date");

        builder.Property(c => c.DataCadastro)
            .IsRequired();

        // Sentinel = true: o EF so omite a coluna no INSERT quando o valor e true,
        // deixando o default do banco agir; um false explicito e sempre gravado.
        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasDefaultValue(true)
            .HasSentinel(true);

        builder.HasIndex(c => c.Cpf).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();

        // 1:1 opcional Cliente -> Endereco. A FK fica em Cliente e e nullable (cliente pode
        // nao ter endereco); o indice UNIQUE na FK impede dois clientes no mesmo endereco,
        // o que transformaria o 1:1 em 1:N.
        builder.HasOne(c => c.Endereco)
            .WithOne(e => e.Cliente)
            .HasForeignKey<Cliente>(c => c.EnderecoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.EnderecoId).IsUnique();

        // 1:N Cliente -> Pedido. Restrict: cliente com historico de pedidos nao pode ser apagado.
        builder.HasMany(c => c.Pedidos)
            .WithOne(p => p.Cliente)
            .HasForeignKey(p => p.ClienteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
