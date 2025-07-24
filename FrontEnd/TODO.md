# ✅ Checklist de Implementação — Frontend (Next.js + Electron)

## 📌 1. Estrutura do Projeto
- [ ] Organizar monorepo (Next + Electron).
- [ ] Pasta `/app` (Next.js Pages/Routes).
- [ ] Pasta `/electron` (Main process).
- [ ] Pasta `/shared` (tipos, configs).

---

## 📌 2. Autenticação
- [ ] Tela de login (JWT).
- [ ] Salvar token no localStorage ou IPC.
- [ ] Middleware de rota protegida (Next.js).

---

## 📌 3. Dashboard Restaurante
- [ ] Visualizar pedidos abertos.
- [ ] CRUD produtos.
- [ ] Controle de estoque.
- [ ] Módulo de usuários/funcionários.
- [ ] Relatórios básicos.

---

## 📌 4. Pedido + Impressão Local
- [ ] Componente de Pedido:
  - [ ] Montar items.
  - [ ] Enviar para API Core (.NET).
- [ ] Receber resposta e disparar:
  - [ ] Impressão local via **Node ESC/POS**.
- [ ] Gerenciar status `printed`.

---

## 📌 5. Offline First
- [ ] Salvar pedido local (IndexedDB, SQLite).
- [ ] Sincronizar `/sync` quando voltar internet.
- [ ] Gerenciar fila de pedidos locais.

---

## 📌 6. Electron IPC + Impressora
- [ ] Criar canal IPC para:
  - [ ] Gerar cupom.
  - [ ] Abrir gaveta (opcional).
- [ ] Configurar porta USB/Rede.
- [ ] Tratar erros de impressão.

---

## 📌 7. Ads (Freemium)
- [ ] Se plano é Freemium, exibir banner/ads.
- [ ] Consultar config do tenant.
- [ ] Acompanhar impressões/clicks.

---

## 📌 8. Plano + Upgrade
- [ ] Tela para exibir plano atual.
- [ ] Link para upgrade.
- [ ] Mostrar features bloqueadas do Premium.

---

## 📌 9. Monitoramento UX
- [ ] Loader para sync offline.
- [ ] Toasts de sucesso/erro.
- [ ] Tela de logs locais.

---

## 📌 10. Build & Deploy
- [ ] Configurar build Electron (.exe / .dmg).
- [ ] Empacotar Next.js dentro do app.
- [ ] Script para auto-update (opcional).
- [ ] Assinatura do instalador (Windows/macOS).

---

## ✅ Extras
- [ ] PWA fallback (opcional).
- [ ] Scripts de mock API para testes.
- [ ] Testes E2E (Playwright, Cypress).

	