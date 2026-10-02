CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `Categorias` (
    `Id` char(36) NOT NULL,
    `Nome` varchar(80) NOT NULL,
    `Descricao` varchar(250) NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Enderecos` (
    `Id` char(36) NOT NULL,
    `Logradouro` varchar(150) NOT NULL,
    `Numero` varchar(10) NOT NULL,
    `Complemento` varchar(60) NULL,
    `Bairro` varchar(80) NOT NULL,
    `Cidade` varchar(80) NOT NULL,
    `Uf` char(2) NOT NULL,
    `Cep` char(8) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Produtos` (
    `Id` char(36) NOT NULL,
    `Nome` varchar(120) NOT NULL,
    `Descricao` varchar(500) NULL,
    `CodigoBarras` varchar(13) NOT NULL,
    `Preco` decimal(10,2) NOT NULL,
    `EstoqueAtual` int NOT NULL DEFAULT 0,
    `Ativo` tinyint(1) NOT NULL DEFAULT TRUE,
    `CategoriaId` char(36) NOT NULL,
    `TipoProduto` varchar(20) NOT NULL,
    `UrlDownload` varchar(300) NULL,
    `TamanhoArquivoMb` decimal(10,2) NULL,
    `LimiteDownloads` int NULL,
    `PesoGramas` int NULL,
    `AlturaCm` int NULL,
    `LarguraCm` int NULL,
    `ComprimentoCm` int NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Produtos_Digital` CHECK (TipoProduto <> 'Digital' OR (UrlDownload IS NOT NULL AND TamanhoArquivoMb IS NOT NULL)),
    CONSTRAINT `CK_Produtos_EstoqueAtual` CHECK (EstoqueAtual >= 0),
    CONSTRAINT `CK_Produtos_Fisico` CHECK (TipoProduto <> 'Fisico' OR (PesoGramas IS NOT NULL AND AlturaCm IS NOT NULL AND LarguraCm IS NOT NULL AND ComprimentoCm IS NOT NULL)),
    CONSTRAINT `CK_Produtos_Preco` CHECK (Preco >= 0),
    CONSTRAINT `CK_Produtos_TipoProduto` CHECK (TipoProduto IN ('Fisico', 'Digital')),
    CONSTRAINT `FK_Produtos_Categorias_CategoriaId` FOREIGN KEY (`CategoriaId`) REFERENCES `Categorias` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Clientes` (
    `Id` char(36) NOT NULL,
    `NomeCompleto` varchar(120) NOT NULL,
    `Cpf` char(11) NOT NULL,
    `Email` varchar(150) NOT NULL,
    `Telefone` varchar(20) NULL,
    `DataNascimento` date NULL,
    `DataCadastro` datetime(6) NOT NULL,
    `Ativo` tinyint(1) NOT NULL DEFAULT TRUE,
    `EnderecoId` char(36) NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Clientes_Enderecos_EnderecoId` FOREIGN KEY (`EnderecoId`) REFERENCES `Enderecos` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Pedidos` (
    `Id` char(36) NOT NULL,
    `Numero` varchar(20) NOT NULL,
    `DataPedido` datetime(6) NOT NULL,
    `Status` int NOT NULL,
    `FormaPagamento` int NOT NULL,
    `ValorTotal` decimal(10,2) NOT NULL,
    `Desconto` decimal(10,2) NULL,
    `DataEntrega` datetime(6) NULL,
    `Observacao` varchar(300) NULL,
    `ClienteId` char(36) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Pedidos_Desconto` CHECK (Desconto IS NULL OR Desconto >= 0),
    CONSTRAINT `CK_Pedidos_ValorTotal` CHECK (ValorTotal >= 0),
    CONSTRAINT `FK_Pedidos_Clientes_ClienteId` FOREIGN KEY (`ClienteId`) REFERENCES `Clientes` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ItensPedido` (
    `Id` char(36) NOT NULL,
    `PedidoId` char(36) NOT NULL,
    `ProdutoId` char(36) NOT NULL,
    `Quantidade` int NOT NULL,
    `PrecoUnitario` decimal(10,2) NOT NULL,
    `Subtotal` decimal(10,2) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `CK_ItensPedido_PrecoUnitario` CHECK (PrecoUnitario >= 0),
    CONSTRAINT `CK_ItensPedido_Quantidade` CHECK (Quantidade > 0),
    CONSTRAINT `CK_ItensPedido_Subtotal` CHECK (Subtotal >= 0),
    CONSTRAINT `FK_ItensPedido_Pedidos_PedidoId` FOREIGN KEY (`PedidoId`) REFERENCES `Pedidos` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ItensPedido_Produtos_ProdutoId` FOREIGN KEY (`ProdutoId`) REFERENCES `Produtos` (`Id`) ON DELETE RESTRICT
);

INSERT INTO `Categorias` (`Id`, `Descricao`, `Nome`)
VALUES ('0b1d6c2e-5a1f-4c3a-9f10-000000000001', 'Periféricos e acessórios de informática', 'Eletrônicos');
SELECT ROW_COUNT();

INSERT INTO `Categorias` (`Id`, `Descricao`, `Nome`)
VALUES ('0b1d6c2e-5a1f-4c3a-9f10-000000000002', 'Livros impressos e digitais', 'Livros e E-books');
SELECT ROW_COUNT();


INSERT INTO `Produtos` (`Id`, `AlturaCm`, `Ativo`, `CategoriaId`, `CodigoBarras`, `ComprimentoCm`, `Descricao`, `EstoqueAtual`, `LarguraCm`, `Nome`, `PesoGramas`, `Preco`, `TipoProduto`)
VALUES ('5e7a9d4c-2b3f-4e8a-8c20-000000000001', 4, TRUE, '0b1d6c2e-5a1f-4c3a-9f10-000000000001', '7891000000017', 11, 'Mouse óptico sem fio, 1600 DPI', 25, 7, 'Mouse sem fio', 95, 89.9, 'Fisico');
SELECT ROW_COUNT();


INSERT INTO `Produtos` (`Id`, `Ativo`, `CategoriaId`, `CodigoBarras`, `Descricao`, `LimiteDownloads`, `Nome`, `Preco`, `TamanhoArquivoMb`, `TipoProduto`, `UrlDownload`)
VALUES ('5e7a9d4c-2b3f-4e8a-8c20-000000000002', TRUE, '0b1d6c2e-5a1f-4c3a-9f10-000000000002', '9786500000011', 'E-book em PDF e EPUB', 3, 'E-book Clean Architecture na prática', 39.9, 12.5, 'Digital', 'https://loja.exemplo.com/downloads/ebook-clean-architecture');
SELECT ROW_COUNT();


CREATE UNIQUE INDEX `IX_Categorias_Nome` ON `Categorias` (`Nome`);

CREATE UNIQUE INDEX `IX_Clientes_Cpf` ON `Clientes` (`Cpf`);

CREATE UNIQUE INDEX `IX_Clientes_Email` ON `Clientes` (`Email`);

CREATE UNIQUE INDEX `IX_Clientes_EnderecoId` ON `Clientes` (`EnderecoId`);

CREATE UNIQUE INDEX `IX_ItensPedido_PedidoId_ProdutoId` ON `ItensPedido` (`PedidoId`, `ProdutoId`);

CREATE INDEX `IX_ItensPedido_ProdutoId` ON `ItensPedido` (`ProdutoId`);

CREATE INDEX `IX_Pedidos_ClienteId` ON `Pedidos` (`ClienteId`);

CREATE UNIQUE INDEX `IX_Pedidos_Numero` ON `Pedidos` (`Numero`);

CREATE INDEX `IX_Produtos_CategoriaId` ON `Produtos` (`CategoriaId`);

CREATE UNIQUE INDEX `IX_Produtos_CodigoBarras` ON `Produtos` (`CodigoBarras`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261002122105_CriacaoInicial', '9.0.17');

COMMIT;

