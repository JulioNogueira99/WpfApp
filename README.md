# WpfApp

Aplicação desktop WPF para gerenciamento de Pessoas, Produtos e Pedidos.

## Requisitos

- Windows 10 ou superior  
- .NET Framework 4.6.1 (ou versão especificada no projeto)  
- Visual Studio 2019 ou superior (com workload para desenvolvimento WPF instalado)  

## Como executar

1. Clone este repositório:  
   ```bash
   git clone https://github.com/JulioNogueira99/WpfApp.git
Abra o arquivo WpfApp.sln no Visual Studio.

Restaure os pacotes NuGet (Visual Studio geralmente faz isso automaticamente).

Compile o projeto (Build > Build Solution).

Execute a aplicação (Debug > Start Debugging ou Ctrl+F5).

Funcionalidades principais
Cadastro e gerenciamento de Pessoas, Produtos e Pedidos.

Filtros e validações nos cadastros.

Atualização de status de pedidos com persistência local em arquivos JSON.

Dependências
Newtonsoft.Json para serialização JSON.

Observações
Os dados são armazenados localmente em arquivos JSON na pasta Data ao lado do executável.

Ao adicionar ou alterar dados, os arquivos JSON são atualizados automaticamente.

Contato
Desenvolvedor: Julio Nogueira

Email: julio.nogueira561@gmail.com