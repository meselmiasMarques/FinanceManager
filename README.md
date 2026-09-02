# FinanceManager

Aplicação de finanças pessoais no modelo **SaaS**: cada usuário tem suas categorias, transações e dashboards, totalmente isolados dos demais.

![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![Blazor WebAssembly](https://img.shields.io/badge/Blazor-WebAssembly-512BD4)
![MudBlazor](https://img.shields.io/badge/UI-MudBlazor%209-594AE2)
![PostgreSQL](https://img.shields.io/badge/DB-PostgreSQL-336791)
![PWA](https://img.shields.io/badge/PWA-instal%C3%A1vel-5A0FC8)


<img width="3798" height="1984" alt="dash" src="https://github.com/user-attachments/assets/737739e3-cb01-4f76-b16c-ffca7eb3f4c3" />

<img width="3794" height="1988" alt="transaction" src="https://github.com/user-attachments/assets/91893719-60f2-4aa5-9393-a54afb3f7550" />

<img width="3798" height="1980" alt="category" src="https://github.com/user-attachments/assets/a67ab036-1450-4a09-973a-27d1d94a8c3d" />

---

## O que o sistema faz

- **Autenticação e conta** — cadastro, login, renovação de sessão silenciosa, logout e alteração de senha.
- **Isolamento multi-inquilino** — o usuário A nunca acessa categorias, transações ou dashboards do usuário B, nem por listagem, nem por ID direto, nem por referência cruzada.
- **Gestão financeira** — categorias, lançamentos de receitas/despesas e dashboard com saldo, séries mensais e quebra por categoria.
- **PWA** — instalável, responsivo, com aviso de nova versão.

---

## Arquitetura

```
FinanceManager/            API — .NET 10 Minimal API
  ├─ Auth/                 Identity, JWT, refresh token, serviços de auth
  ├─ Data/                 EF Core (Npgsql) + mapeamentos + global query filters
  ├─ Extensions/Endpoints/ Endpoints agrupados (auth, categories, transactions, dashboard)
  ├─ Services / Repositories
  └─ Migrations/           v1..v8

FinanceManager.Web/        Cliente — Blazor WebAssembly + MudBlazor (PWA)
  ├─ Services/Auth/        TokenStore, AuthApiClient, AuthenticationStateProvider,
  │                        DelegatingHandler (Bearer + refresh no 401)
  ├─ Layout/               MainLayout, AuthLayout, guarda de rotas
  ├─ Pages/                Login, Cadastro, Alterar Senha, Dashboard, Transações, Categorias
  └─ wwwroot/              manifest, service worker, ícones

docs/                      Especificação de requisitos + protótipos + script de migração
```

### Segurança e isolamento (defesa em profundidade)

| Camada | Mecanismo |
|---|---|
| Identidade | ASP.NET Core Identity, chave `Guid`, senha só como hash, lockout |
| Sessão | JWT de acesso curto (memória) + refresh token **rotacionado** em cookie `HttpOnly` |
| Detecção de roubo | reuso de refresh token já rotacionado revoga toda a cadeia do usuário |
| Autorização | `RequireAuthorization` + *fallback policy* — todo endpoint de dados exige autenticação |
| Isolamento | **EF Core Global Query Filter** por `UserId` + filtro explícito no repositório + validação de posse em referências cruzadas |
| Origem do inquilino | `UserId` sempre derivado do token, nunca do corpo da requisição |
| Abuso | rate limiting nos endpoints de autenticação |

### Cliente

- `AuthenticationStateProvider` customizado que reconstrói o estado a partir dos *claims* do JWT e faz **renovação silenciosa** no boot (o cookie de refresh sobrevive a recargas).
- `DelegatingHandler` que anexa o `Bearer` a todas as chamadas e, num `401`, renova a sessão e repete a requisição de forma transparente — ou desloga e redireciona para o login preservando o destino (`returnUrl`).
- Guarda de rotas com `AuthorizeRouteView` + `[Authorize]`: nenhuma página protegida renderiza sem sessão.
- Preferências de tema e menu lateral persistidas em `localStorage`.

---

## Stack

**Back-end:** .NET 10 · ASP.NET Core Minimal APIs · Entity Framework Core 10 · Npgsql / PostgreSQL · ASP.NET Core Identity · JWT Bearer · Rate Limiting · OpenAPI/Swagger

**Front-end:** Blazor WebAssembly (.NET 10) · MudBlazor 9 · PWA (service worker + manifest) · cultura pt-BR

---

## Rodando localmente

### Pré-requisitos
- .NET SDK 10
- PostgreSQL

### 1. Configurar a conexão (API)

A string de conexão vem de *user-secrets* ou variável de ambiente — não versionada.

```bash
cd FinanceManager
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=FinanceManager;Username=postgres;Password=SUA_SENHA"
dotnet user-secrets set "Jwt:SigningKey" "uma-chave-aleatoria-com-32-bytes-ou-mais"
```

Em `Development`, se a `Jwt:SigningKey` não for definida, a API usa uma chave de desenvolvimento; em produção ela é obrigatória.

### 2. Aplicar as migrations

```bash
dotnet ef database update --project FinanceManager
```

> A migration `v8` converte o esquema single-tenant existente para multi-tenant: cria as tabelas do Identity e reatribui os dados pré-autenticação a uma **conta legada** (`legacy@financemanager.local`, criada sem senha).

### 3. Subir API e cliente

```bash
# terminal 1 — API  (http://localhost:5046, Swagger em /swagger)
dotnet run --project FinanceManager

# terminal 2 — Web  (http://localhost:5100)
dotnet run --project FinanceManager.Web
```

Acesse `http://localhost:5100`, crie uma conta e comece a usar.

---

