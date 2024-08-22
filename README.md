# GerenciamentoTarefasWorker

Este projeto é um serviço Worker desenvolvido em .NET 6 que funciona 
em segundo plano para processar mensagens de uma fila RabbitMQ. 
Ele é parte de um sistema de gerenciamento de tarefas, 
onde o Worker lida com operações assíncronas, como a criação, 
atualização e exclusão de tarefas.

## Funcionalidades

- Conexão ao RabbitMQ usando `ConnectionFactory`.
- Escuta contínua de uma fila chamada `task_queue`.
- Processamento de mensagens recebidas da fila.
- Confirmação manual do recebimento de mensagens, 
  garantindo que as mensagens sejam processadas corretamente antes de serem removidas da fila.

## Configuração do Ambiente de Desenvolvimento

### Pré-requisitos

- .NET 6 SDK
- RabbitMQ Server (deve estar em execução em `localhost` na porta padrão `5672`)

### Configuração do RabbitMQ

Certifique-se de que o RabbitMQ esteja instalado e rodando. O Worker está configurado para se conectar ao RabbitMQ usando as credenciais padrão:

```csharp
var factory = new ConnectionFactory
{
    HostName = "localhost", // Endereço do servidor RabbitMQ
    Port = 5672,            // Porta padrão do RabbitMQ
    UserName = "guest",     // Usuário padrão do RabbitMQ
    Password = "guest"      // Senha padrão do RabbitMQ
};
