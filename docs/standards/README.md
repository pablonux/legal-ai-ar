# PwC Internal Application Standards

> Cross-cutting standards for building and operating internal applications on the PwC corporate platform.

---

## Documents

| Document                                                                      | Scope                                                                                                                                     |
| ----------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| [**PwC Internal Application Architecture**](pwc-internal-app-architecture.md) | **Single reference guide** — backend, frontend, auth, API, domain, testing, delivery, and quality gates for any internal PwC app on GCaaS |

---

## How to use this folder

1. **Before scaffolding or a major feature**, read the architecture guide end-to-end.
2. **When implementing a work item**, verify the [Architecture compliance checklist](pwc-internal-app-architecture.md#16-architecture-compliance-checklist) in the guide.
3. **When closing a work item**, include the standards doc in the [Definition of Done](../roadmap/DEFINITION-OF-DONE.md) documentation round-trip if architecture, auth, frontend structure, or delivery patterns changed.
4. **Product-specific docs** in `docs/technical/` describe _this_ application's implementation; they must stay aligned with the standards guide, not replace it.

### Roadmap alignment (F0.0)

| Work item    | Standard sections                   |
| ------------ | ----------------------------------- |
| F0.0-W03, W11 | §6 Frontend                         |
| F0.0-W08      | §9 Testing                          |
| FT.5-W01–W03 | §10 Delivery                        |
| F0.0-W09      | §4.4 API                            |
| F0.0-W10      | §4 Contracts                        |
| F0.0-W12      | §4.5 Outbox                         |
| F1.1 (R1)    | §5 Auth production                  |
| F2.1 (R2)    | §4.2 Rich aggregates + outbox usage |
| F4.5 (R4)    | §7 Observability                    |

---

_PwC Internal Application Standards_
