# ✅ Checklist de Implementação — API WhatsApp (NestJS)

## 📌 1. Estrutura do Projeto
- [ ] Estruturar projeto em módulos:
  - [ ] **Orders Module** (se consultar Mongo)
  - [ ] **Messages Module**
  - [ ] **RabbitMQ Listener Module**
  - [ ] **Auth Module** (se precisar restrições)
- [ ] Usar **Config Module** para secrets/API keys.
- [ ] Conectar com MongoDB via **Mongoose**.

---

## 📌 2. RabbitMQ Subscriber
- [ ] Configurar conexão com RabbitMQ.
- [ ] Declarar fila `ORDER_STATUS_UPDATED`.
- [ ] Consumir mensagem:
  - [ ] Validar payload.
  - [ ] Buscar dados extras no MongoDB (opcional).
  - [ ] Montar mensagem final.
  - [ ] Chamar provedor WhatsApp.
- [ ] Retry e dead-letter para falhas.

---

## 📌 3. Integração com Provedor WhatsApp
- [ ] Escolher API (Zenvia, Gupshup, 360dialog, não oficial).
- [ ] Gerenciar tokens/credentials no `.env`.
- [ ] Criar service para envio:
  - [ ] Enviar texto.
  - [ ] Lidar com template messages (se for API oficial).
  - [ ] Tratar erros.
- [ ] Testar com número real ou sandbox.

---

## 📌 4. Logs
- [ ] Criar `whatsapp_logs` collection:
  - [ ] Salvar tenantId, orderId, phone, payload, status.
- [ ] Logar status de envio:
  - [ ] sent
  - [ ] failed
  - [ ] queued
- [ ] Endpoint para consultar logs (opcional, restrito).

---

## 📌 5. Monitoramento
- [ ] Healthcheck RabbitMQ.
- [ ] Healthcheck MongoDB.
- [ ] Healthcheck Provedor WhatsApp (opcional ping).
- [ ] Endpoint `/health`.

---

## 📌 6. Segurança
- [ ] Validar todos payloads recebidos.
- [ ] Proteger rotas sensíveis (se expuser hooks).
- [ ] Logar erros críticos.

---

## 📌 7. DevOps
- [ ] Dockerfile.
- [ ] docker-compose com RabbitMQ + MongoDB.
- [ ] Variáveis via `.env` e `ConfigService`.

---

## 📌 Extras
- [ ] Testes unitários para subscriber.
- [ ] Teste manual end-to-end.
- [ ] Script de seed para simular evento RabbitMQ.
