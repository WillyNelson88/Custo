CREATE TABLE operacao
GO

USE custo

(	
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	data_criacao VARCHAR(10) NOT NULL,
	operacao VARCHAR (100) NOT NULL,
	custo DECIMAL(10,2) NOT NULL,
	observacoes VARCHAR(100)
)
GO

INSERT INTO operacao (data_criacao, operacao, custo, observacoes) VALUES ('11/11/2021', 'Calça Basica -R$0,82 corte', 12.90, 'Base costura');
INSERT INTO operacao (data_criacao, operacao, custo, observacoes) VALUES ('11/11/2021', 'Corte', 0.82, 'corte');
INSERT INTO operacao (data_criacao, operacao, custo, observacoes) VALUES ('11/11/2021', 'Etiqueta Ellus', 0.68, 'Etiqueta de cós interno');

CREATE TABLE cliente	
(	
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	cliente VARCHAR(50) NOT NULL,
	razao VARCHAR (100) NOT NULL,
	endereco VARCHAR(100)
)	
	
CREATE TABLE montar_custo	
(	
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	oc INT NOT NULL ,
	id_operacao INT NOT NULL,
	freq INT NOT NULL,
	fase VARCHAR(20) NOT NULL,
	total DECIMAL (10,2) NOT NULL
)	
	
GO

CREATE TABLE modelo	
(	
	oc INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	data_criacao VARCHAR(10) NOT NULL,
	referencia VARCHAR(50) NOT NULL,
	colecao VARCHAR(5) NOT NULL,
	id_cliente INT NOT NULL,
	descricao VARCHAR(50) NOT NULL,
	preco DECIMAL(10,2) NOT NULL

)	
	
GO

CREATE TABLE cobranca	
(	
	talao INT NOT NULL PRIMARY KEY,
	oc INT NOT NULL ,
	dataCriacao VARCHAR(10) NOT NULL,
	tipo VARCHAR(20) NOT NULL,	
	preco DECIMAL (10,2) NOT NULL
)
GO

CREATE TABLE login(
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	usuario VARCHAR(50) NOT NULL UNIQUE,
	senha VARCHAR (255) NOT NULL,
	situacao VARCHAR(50) NOT NULL,
)

INSERT INTO login (usuario, senha, situacao) VALUES ('Willy', '$2a$12$C9pTXOXNLeP26OBluHJqlO.pVtOp5V5SzFeSSULU55Bt5sevmAELW', 'ADMIN');
GO	