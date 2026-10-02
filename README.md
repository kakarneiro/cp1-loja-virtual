# CP2 — 2TDSPB 2026.2 · Persistência com EF Core, Mapeamento e Infraestrutura

Continuação do CP1: o MER da **Loja Virtual** agora é persistido em **MySQL** (Docker) com
**Entity Framework Core 9**, mapeamento via **Fluent API**, **migration** versionada e
**repositório genérico** registrado por injeção de dependência, tudo dentro da
**Clean Architecture** montada no CP1.

## Integrantes

| Nome | RM |
|---|---|
| Murilo Marques Cabral | 568224 |
| Paulo Henrique Da Silva Kian | 563343 |
| Matheus Carneiro Maciel | 567753 |

## Domínio: Loja Virtual (resumo do CP1)

Uma loja que vende produtos pela internet. O **cliente** se cadastra e pode informar um
**endereço** de entrega; os **produtos** são organizados em **categorias**; o cliente fecha um
**pedido**, cujos **itens** registram quais produtos foram comprados, em que quantidade e por
qual preço.

| Relacionamento | Tipo | Opcionalidade |
|---|---|---|
| Cliente ⟷ Endereco | **1:1** | Cliente tem 0 ou 1 endereço (`EnderecoId` nullable + UNIQUE) |
| Cliente → Pedido | **1:N** | Pedido tem exatamente 1 cliente |
| Categoria → Produto | **1:N** | Produto tem exatamente 1 categoria |
| Pedido → ItemPedido | **1:N** | Pedido tem 1 ou muitos itens |
| Produto → ItemPedido | **1:N** | Produto aparece em 0 ou muitos itens |
| Pedido × Produto | **N:N** | Resolvido pela entidade associativa `ItemPedido` |

O MER do CP1 está em [`docs/mer.png`](docs/mer.png) (fonte: [`docs/mer.mmd`](docs/mer.mmd)).

## SGBD

**MySQL** (imagem `mysql:latest`) em Docker, no padrão da aula, com o provider oficial
**`MySql.EntityFrameworkCore` 9.0.17** (`options.UseMySQL(...)`).

| Pacote | Projeto | Versão |
|---|---|---|
| `MySql.EntityFrameworkCore` (traz o EF Core 9) | Projeto.Infrastructure | 9.0.17 |
| `Microsoft.EntityFrameworkCore.Design` | Projeto.Api (startup) | 9.0.17 |
| `dotnet-ef` (ferramenta local, `.config/dotnet-tools.json`) | — | 9.0.17 |

## Como executar

Requisitos: **.NET SDK 9.0** e **Docker**.

### 1. Subir o MySQL

```bash
docker run --name TDSPB -e MYSQL_ROOT_PASSWORD=TDSPB123 -p 3306:3306 -d mysql:latest
```

Se o container já existir:

```bash
docker start TDSPB
```

> Se a porta 3306 já estiver em uso por outro MySQL, pare-o antes (`docker stop <nome>`).

### 2. Restaurar pacotes e a ferramenta `dotnet-ef`

```bash
dotnet restore
dotnet tool restore
```

### 3. Aplicar a migration (cria o banco `LojaVirtual` e as tabelas)

```bash
dotnet ef database update --project src/Projeto.Infrastructure --startup-project src/Projeto.Api
```

### 4. Rodar a API

```bash
dotnet run --project src/Projeto.Api
```

A API sobe em `http://localhost:5000` com o `DbContext` e os repositórios registrados. Esta
entrega não expõe endpoints; o foco do CP2 é a camada de persistência.

### Gerar uma nova migration (se o modelo mudar)

```bash
dotnet ef migrations add NomeDaMigration --project src/Projeto.Infrastructure --startup-project src/Projeto.Api --output-dir Persistence/Migrations
```

## Configuração da connection string

A connection string fica em
[`src/Projeto.Api/appsettings.Development.json`](src/Projeto.Api/appsettings.Development.json),
com a chave `ConnectionStrings:MySql`:

```json
"ConnectionStrings": {
  "MySql": "Server=127.0.0.1;Port=3306;Database=LojaVirtual;User=root;Password=TDSPB123;"
}
```

- A senha `TDSPB123` é a **senha de desenvolvimento** do container da aula, não um segredo real.
- O `appsettings.json` (usado fora do ambiente Development) **não** contém connection string. Em
  outro ambiente ela deve vir de variável de ambiente (`ConnectionStrings__MySql`) ou de um
  cofre de segredos; sem ela, a API não sobe e informa o motivo.
- O `launchSettings.json` define `ASPNETCORE_ENVIRONMENT=Development` para o `dotnet run`, e o
  `dotnet ef` usa Development por padrão.

## Arquitetura

```
Projeto.Api -> Projeto.Infrastructure -> Projeto.Application -> Projeto.Domain
```

| Camada | O que tem no CP2 |
|---|---|
| **Domain** | Entidades e enums do CP1, **sem alteração** e sem dependência do EF Core |
| **Application** | Contrato `IRepositorio<TEntidade>` — sem dependência do EF Core |
| **Infrastructure** | `LojaVirtualContext`, 6 classes `IEntityTypeConfiguration<T>`, `Repositorio<TEntidade>` e a pasta `Migrations` |
| **Api** | Registro de DI no `Program.cs` e a connection string |

```
src/
├─ Projeto.Domain/
│  ├─ Entities/                     # Cliente, Endereco, Categoria, Produto, Pedido, ItemPedido
│  └─ Enums/                        # StatusPedido, FormaPagamento
├─ Projeto.Application/
│  └─ IRepositorio.cs               # contrato do repositório genérico
├─ Projeto.Infrastructure/
│  ├─ Persistence/
│  │  ├─ LojaVirtualContext.cs      # DbContext
│  │  ├─ Configurations/            # Fluent API, uma classe por entidade
│  │  └─ Migrations/                # migration inicial + snapshot
│  └─ Repositories/
│     └─ Repositorio.cs             # implementação EF Core do IRepositorio<T>
└─ Projeto.Api/
   ├─ Program.cs                    # AddDbContext + AddScoped dos repositórios
   └─ appsettings.Development.json  # connection string de desenvolvimento
```

### Injeção de dependência ([`Program.cs`](src/Projeto.Api/Program.cs))

```csharp
builder.Services.AddDbContext<LojaVirtualContext>(options =>
{
    options.UseMySQL(connectionString);
}, ServiceLifetime.Scoped);

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));
```

`DbContext` e repositórios são **Scoped**: uma instância por requisição, compartilhada entre
os repositórios da mesma requisição, de modo que um único `SalvarAlteracoesAsync` grava tudo
numa transação.

## Repositórios: estratégia escolhida

Usamos **um repositório genérico**, `IRepositorio<TEntidade>`, aplicado a **todas** as
entidades; não há repositórios específicos por agregado. O modelo tem 6 entidades com
operações de acesso a dados idênticas, então um único contrato evita seis interfaces
repetidas. Consultas com filtro usam `BuscarAsync(Expression<...>)`, sem expor `IQueryable` nem
tipos do EF Core para a Application.

| Método | Descrição |
|---|---|
| `ObterPorIdAsync(Guid id)` | Busca pela PK |
| `ListarAsync()` | Lista todos (sem tracking) |
| `BuscarAsync(filtro)` | Lista os que atendem ao filtro (sem tracking) |
| `ExisteAsync(filtro)` | Verifica se existe algum |
| `AdicionarAsync` / `Atualizar` / `Remover` | Marcam a alteração no contexto |
| `SalvarAlteracoesAsync()` | Grava as alterações pendentes |

## Mapeamento (Fluent API)

Cada entidade tem sua própria classe em
[`Persistence/Configurations/`](src/Projeto.Infrastructure/Persistence/Configurations/),
aplicadas com `ApplyConfigurationsFromAssembly`. Todo o esquema do MER do CP1 é configurado
explicitamente: nome da tabela, PK, `IsRequired`, `HasMaxLength`, `IsFixedLength` (`char`),
`HasPrecision(10,2)`, defaults, índices e relacionamentos.

| Regra do MER | Como foi mapeada |
|---|---|
| **1:1 opcional** Cliente ⟷ Endereco | `HasOne(Endereco).WithOne(Cliente).HasForeignKey<Cliente>(EnderecoId).IsRequired(false)` + **índice único** em `EnderecoId` — sem o UNIQUE o 1:1 viraria 1:N |
| **N:N** Pedido × Produto | Entidade associativa `ItemPedido` com dois 1:N obrigatórios + **índice único composto** `(PedidoId, ProdutoId)` |
| **1:N obrigatórios** | FKs `NOT NULL` com `IsRequired()` |
| UK: `Cpf`, `Email`, `Categoria.Nome`, `CodigoBarras`, `Pedido.Numero` | `HasIndex(...).IsUnique()` |
| Enums `StatusPedido` e `FormaPagamento` | `HasConversion<int>()` → coluna `int` |
| `Ativo` default `true`, `EstoqueAtual` default `0` | `HasDefaultValue(...)`; nos booleanos, `HasSentinel(true)` garante que um `false` explícito seja gravado |
| PK `Guid` | `char(36)` no MySQL |

### Regras de exclusão

| FK | `OnDelete` | Motivo |
|---|---|---|
| `Clientes.EnderecoId` | `SET NULL` | Apagar o endereço não apaga o cliente; ele só fica sem endereço |
| `Pedidos.ClienteId` | `RESTRICT` | Cliente com histórico de pedidos não pode ser apagado |
| `Produtos.CategoriaId` | `RESTRICT` | Categoria com produtos não pode ser apagada |
| `ItensPedido.PedidoId` | `CASCADE` | Item não existe sem o pedido |
| `ItensPedido.ProdutoId` | `RESTRICT` | Produto já vendido não pode sumir do histórico (desative com `Ativo = false`) |

## Migrations

Há **uma única migration**, `CriacaoInicial`, em
[`src/Projeto.Infrastructure/Persistence/Migrations/`](src/Projeto.Infrastructure/Persistence/Migrations/),
que cria o esquema completo: 6 tabelas, 5 FKs, 10 índices e 7 CHECKs.

O SQL equivalente, gerado com `dotnet ef migrations script`, está em
[`docs/esquema-fisico.sql`](docs/esquema-fisico.sql).

## Esquema físico

[![Esquema físico — LojaVirtual](docs/esquema-fisico.png)](docs/esquema-fisico.png)

Fonte do diagrama: [`docs/esquema-fisico.mmd`](docs/esquema-fisico.mmd).

### O que mudou em relação ao MER do CP1

As entidades, atributos, tipos, tamanhos e relacionamentos são **os mesmos do CP1**: o Domain
não foi alterado. O esquema físico só **acrescenta restrições de integridade** que o MER não
detalhava:

- **Índice único `(PedidoId, ProdutoId)` em `ItensPedido`**: um mesmo produto aparece uma vez
  por pedido, e a quantidade fica em `Quantidade`. É a forma física da entidade associativa
  que resolve o N:N.
- **CHECKs**: `Quantidade > 0`; `Preco`, `EstoqueAtual`, `ValorTotal`, `Desconto`,
  `PrecoUnitario` e `Subtotal` não negativos.

### Limitações conhecidas do modelo físico

- O "pedido tem **no mínimo 1** item" do MER não é expressável em DDL; é uma regra de
  domínio/aplicação.
- Como a FK do 1:1 fica em `Clientes`, o banco não impede um endereço sem cliente (por exemplo,
  depois de apagar o cliente). Essa limpeza também é responsabilidade da aplicação.
- O provider `MySql.EntityFrameworkCore` grava `DateOnly`, mas falha ao lê-lo de volta. Por
  isso, `Cliente.DataNascimento` tem um conversor `DateOnly ⇄ DateTime` e continua sendo uma
  coluna `date`.

## Relação com o enunciado

O enunciado do CP2 usa como exemplo o domínio da aula (Recommenda). Como o CP1 do grupo foi a
**Loja Virtual**, aplicamos os mesmos requisitos ao nosso MER:

| Exigência do enunciado (exemplo Recommenda) | Equivalente na Loja Virtual |
|---|---|
| N:N `Content`/`Genre` | N:N `Pedido`/`Produto` via `ItemPedido` |
| 1:1 opcional `User`/`UserConfiguration` | 1:1 opcional `Cliente`/`Endereco` |
| Opcionalidade `Season`/`Episode`, `Rating` | 1:N obrigatórios e campos `NULL` do MER |
| E-mail do usuário único | `Clientes.Email` único (e também `Cpf`) |
| Número do episódio único **dentro** da temporada | Produto único **dentro** do pedido: `(PedidoId, ProdutoId)` |
| `RecommendaContext` | `LojaVirtualContext` |
