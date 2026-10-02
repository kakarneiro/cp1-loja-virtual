using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Infrastructure.Persistence.Configurations;

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("Enderecos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Logradouro)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Numero)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Complemento)
            .HasMaxLength(60);

        builder.Property(e => e.Bairro)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(e => e.Cidade)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(e => e.Uf)
            .IsRequired()
            .HasMaxLength(2)
            .IsFixedLength();

        builder.Property(e => e.Cep)
            .IsRequired()
            .HasMaxLength(8)
            .IsFixedLength();

        // O relacionamento 1:1 com Cliente e configurado em ClienteConfiguration,
        // porque e Cliente quem carrega a FK.
    }
}
