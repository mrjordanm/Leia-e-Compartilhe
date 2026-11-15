# Leia & Compartilhe

Plataforma web de inclusão literária e sustentabilidade, desenvolvida como projeto da Atividade Extensionista III. O sistema busca incentivar a leitura, facilitar o compartilhamento de livros e aproximar instituições e educadores da comunidade escolar.

## Tecnologias utilizadas

### Backend
- C#
- ASP.NET Core
- Entity Framework Core
- MySQL
- JWT para autenticação e autorização

### Frontend
- React
- JavaScript
- Vite
- CSS

## Funcionalidades

- Cadastro e autenticação de usuários.
- Controle de acesso por perfil: Administrador, Educador e Aluno.
- Consulta pública ao catálogo de livros.
- Solicitação e gerenciamento de empréstimos.
- Cadastro e avaliação de doações de livros.
- Cadastro e consulta de parcerias institucionais.
- Cadastro de educadores pelo administrador.
- Consulta e gerenciamento de usuários.
- Consulta de perfil do usuário.
- Registro de auditoria das ações administrativas.

## Estrutura do projeto

- `LeiaCompartilhe.API`: API, autenticação e regras de negócio.
- `LeiaCompartilhe.Web`: interface web da plataforma.

## Requisitos para execução

- .NET SDK compatível com o projeto.
- Node.js e npm.
- MySQL.
- Visual Studio ou editor compatível.

## Como executar

### 1. Configuração do banco de dados

Configure a conexão com o MySQL e as chaves de autenticação utilizando User Secrets ou variáveis de ambiente.

Aplique as migrações existentes do Entity Framework Core para preparar o banco de dados.

### 2. Backend

Abra a solução da API no Visual Studio e execute o projeto `LeiaCompartilhe.API`.

Após iniciar a API, a documentação Swagger estará disponível em:

`https://localhost:7298/swagger/index.html`

### 3. Frontend

Abra um terminal na pasta `LeiaCompartilhe.Web` e execute:

```bash
npm install
npm run dev
```

Acesse o endereço local informado pelo Vite, normalmente:

`http://localhost:5173`

## Projeto acadêmico

Desenvolvido para a Atividade Extensionista III, com foco em inclusão literária, incentivo à leitura e sustentabilidade por meio do compartilhamento de livros.

## Acesso para avaliação

Caso queira testar, o projeto disponibiliza três contas de demonstração, cada uma com um perfil e permissões específicas:

- **Administrador:** acesso às funcionalidades administrativas do sistema.
- **Educador:** acesso ao gerenciamento de empréstimos e às funcionalidades destinadas aos educadores.
- **Aluno:** acesso ao catálogo de livros e à solicitação de empréstimos.

**Observações sobre o cadastro:**

- Por padrão, novos usuários cadastrados recebem o perfil de Aluno.
- O cadastro de Educadores é realizado por um Administrador.
- Para facilitar a avaliação, já está disponível uma conta de demonstração para o perfil Educador.

**[Clique aqui para consultar as credenciais de demonstração](CREDENCIAIS_DEMO.md).**

As contas foram criadas exclusivamente para testes e avaliação acadêmica do protótipo, utilizando dados fictícios.
