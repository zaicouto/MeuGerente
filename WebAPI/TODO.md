# ✅ Checklist de Implementação — Core API (.NET 8)

## 📌 1. Estrutura de Projeto
- [ ] Organizar solução em camadas:
  - [x] **API Layer** (Controllers)
  - [ ] **Application Layer** (Services, Use Cases)
  - [x] **Domain Layer** (Entidades, Regras)
  - [x] **Infrastructure Layer** (MongoDB, RabbitMQ, External Services)
- [x] Configurar **Swagger** para documentação.
- [ ] Configurar **Logging** centralizado (Serilog, Elastic ou Sentry).
- [x] Criar **Dockerfile** e **docker-compose.yml**.

---

## 📌 2. Multi-Tenant
- [ ] Toda entidade principal (`Order`, `Product`, `User`, `Inventory`) deve ter `TenantId`.
- [ ] Middleware global para **resolver `TenantId`** do JWT.
- [ ] Filtros automáticos para queries (`Query Filters`).
- [ ] Estratégia para **Seed de Tenant/Admin**.

---

## 📌 3. Autenticação e Autorização
- [ ] Implementar **JWT Auth** com `TenantId` + `Roles` embutidos.
- [ ] Endpoint para **login** e **refresh token**.
- [ ] Roles: `Admin`, `Gerente`, `Funcionário`, `Caixa`, etc.
- [ ] Policy-based authorization para proteger endpoints sensíveis.

---

## 📌 4. CRUD Básico
- [ ] CRUD **Orders**
  - [x] Criar pedido
  - [ ] Atualizar status
  - [ ] Cancelar pedido
  - [ ] Consultar por status, período
- [ ] CRUD **Products**
  - [ ] Adicionar produto
  - [ ] Atualizar preço, estoque
  - [ ] Ativar/Inativar
- [ ] CRUD **Users**
  - [ ] Criar funcionário
  - [ ] Reset de senha
  - [ ] Ativar/Inativar
- [ ] CRUD **Inventory**
  - [ ] Entradas e saídas de estoque
  - [ ] Ajustes manuais

---

## 📌 5. Controle de Planos
- [ ] CRUD **Plans** (`Free`, `Premium`)
- [ ] Associar `Tenant` ao plano.
- [ ] Middleware para **validar limites** de acordo com o plano.
- [ ] Endpoint para **upgrade/downgrade** de plano.
- [ ] Lógica de ads habilitado/desabilitado conforme plano.

---

## 📌 6. Sincronização Offline
- [ ] Endpoint **/sync**:
  - [ ] Receber pedidos salvos offline.
  - [ ] Marcar como `synced`.
- [ ] Log de tentativas de sincronização.
- [ ] Endpoint para **consultar histórico** de pedidos sincronizados.

---

## 📌 7. Fila de Eventos
- [ ] Configurar **RabbitMQ** ou Redis Streams.
- [ ] Implementar **Publisher** para eventos:
  - [ ] `ORDER_STATUS_UPDATED`
  - [ ] `ORDER_CREATED` (opcional)
  - [ ] `PLAN_CHANGED` (opcional)
- [ ] Serialização clara do payload.
- [ ] Retry em caso de falha no publish.

---

## 📌 8. Relatórios
- [ ] Endpoint para relatório **resumido diário**.
- [ ] Endpoint para relatório **faturamento período**.
- [ ] Endpoint para **métricas de vendas por produto**.
- [ ] Usar **Aggregation Pipeline** do MongoDB.

---

## 📌 9. Monitoramento
- [ ] Healthcheck de API (`/health`).
- [ ] Healthcheck de conexão MongoDB.
- [ ] Healthcheck de conexão RabbitMQ.
- [ ] Logs de auditoria para:
  - [ ] Login/logout
  - [ ] Criação/alteração de pedido
  - [ ] Upgrade/downgrade de plano

---

## 📌 10. Segurança
- [ ] Validação de payloads com **FluentValidation**.
- [ ] Rate limit para endpoints sensíveis.
- [ ] CORS configurado.
- [ ] Backup automático do banco.

---

## ✅ Opcional: Ferramentas Extras
- [ ] Scripts de **migrations MongoDB** (se precisar).
- [ ] Scripts de **seed** (Planos + Admin).
- [ ] Testes unitários para Application Layer.
- [ ] Testes de integração para Publisher RabbitMQ.

---

**Pronto!**  
Este checklist cobre o **mínimo viável** para um Core robusto, multi-tenant, escalável e pronto para operar **online + offline**.

---
