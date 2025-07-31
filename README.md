# FastTech Foods - Sistema de Pedidos

Sistema de pedidos para rede de fast food desenvolvido em C# com arquitetura de microsserviços para o Hackathon .NET.

## 🏗️ Arquitetura

O sistema é composto por 5 microsserviços independentes:

- **AuthService** (porta 5001) - Autenticação e gerenciamento de usuários
- **MenuService** (porta 5002) - Gerenciamento do cardápio
- **OrderService** (porta 5003) - Gerenciamento de pedidos
- **KitchenService** (porta 5004) - Gerenciamento da cozinha
- **ConsumerService** - Consome mensagens das filas do RabbitMQ

## 🚀 Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - APIs REST
- **Entity Framework Core** - ORM para acesso a dados (SqlServer)
- **JWT Bearer Authentication** - Autenticação e autorização
- **BCrypt** - Hash de senhas
- **Swagger/OpenAPI** - Documentação das APIs
- **RabbitMQ** - Mensageria por filas
- **Prometheus** - Monitoramento
- **Grafana** - Dashboards com dados do monitoramento


## 📋 Requisitos Funcionais Implementados

### ✅ Funcionários
- Autenticação com e-mail corporativo e senha
- Cadastro e edição de itens do cardápio
- Visualização e gerenciamento de pedidos
- Aceite ou rejeição de pedidos na cozinha

### ✅ Clientes
- Autenticação via CPF ou e-mail e senha
- Busca de produtos com filtros por categoria
- Realização de pedidos com escolha de entrega
- Cancelamento de pedidos com justificativa

## 🛠️ Como Executar

### Pré-requisitos
- .NET 6 SDK instalado
- Git (para clonar o repositório)

### Executando os Microsserviços

1. **Clone o repositório:**
```bash
git clone <url-do-repositorio>
cd FastTechFoods
```

2. **Execute cada microsserviço em terminais separados:**

```bash
# Terminal 1 - AuthService
cd src/AuthService
dotnet run

# Terminal 2 - MenuService  
cd src/MenuService
dotnet run

# Terminal 3 - OrderService
cd src/OrderService
dotnet run

# Terminal 4 - KitchenService
cd src/KitchenService
dotnet run

# Terminal 5 - ConsumerService
cd src/ConsumerService
dotnet run
```

### URLs dos Serviços
- AuthService: http://localhost:5001
- MenuService: http://localhost:5002
- OrderService: http://localhost:5003
- KitchenService: http://localhost:5004

### Documentação Swagger
Cada serviço possui documentação Swagger disponível em:
- http://localhost:5001/swagger
- http://localhost:5002/swagger
- http://localhost:5003/swagger
- http://localhost:5004/swagger

## 👥 Usuários Padrão

O sistema já vem com usuários pré-cadastrados:

### Funcionários
- **Gerente:** gerente@fasttechfoods.com | senha: 123456
- **Cozinha:** cozinha@fasttechfoods.com | senha: 123456

### Clientes
- **João Carlos** cliente@gmail.com | senha: 123456

## 🍔 Cardápio Inicial

O sistema já vem com 6 itens pré-cadastrados:
- Big Burger (R$ 25,90)
- Chicken Deluxe (R$ 22,50)
- Batata Frita Grande (R$ 12,90)
- Coca-Cola 350ml (R$ 5,50)
- Suco de Laranja Natural (R$ 8,90)
- Sorvete de Chocolate (R$ 9,90)

## 🔄 Fluxo de Pedidos

1. **Cliente** faz login e cria pedido
2. **Pedido** fica com status "Pending"
3. **Cozinha** visualiza pedidos pendentes
4. **Cozinha** aceita ou rejeita o pedido
5. Se aceito, **Cozinha** pode iniciar preparo ("Preparing")
6. **Cozinha** finaliza pedido ("Ready")
7. **Cliente** pode cancelar pedido antes do preparo


## 🔒 Segurança

- Autenticação JWT com chave secreta
- Autorização baseada em roles (Employee/Customer)
- Hash de senhas com BCrypt
- CORS configurado para desenvolvimento

## 📝 Licença

Este projeto foi desenvolvido para o Hackathon .NET e é destinado apenas para fins educacionais.

