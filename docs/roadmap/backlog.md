# Legal Ai Ar — Work Item Backlog

> Generated: 2026-04-02
> Total features: 27
> Stack: Angular 19 + .NET 10 + Azure

---

## Folder Structure

```
docs/roadmap/
├── features.md              # Features roadmap (existing)
├── backlog.md               # This file (backlog index)
├── F01 - Authentication and Authorization/
│   ├── F01 - W01 - Comprehensive Documentation.md
│   ├── F01 - W02 - Backend - Entra ID and JWT Configuration.md
│   ├── F01 - W03 - Backend - Role-Based Authorization Middleware.md
│   ├── F01 - W04 - Frontend - MSAL Angular Setup and AuthService.md
│   ├── F01 - W05 - Frontend - AuthGuard and RoleGuard.md
│   ├── F01 - W06 - Frontend - AuthInterceptor and ErrorInterceptor.md
│   └── F01 - W07 - Testing - E2E Authentication Tests.md
├── F02 - Main Dashboard/
│   ├── F02 - W01 - Comprehensive Documentation.md
│   ├── F02 - W02 - Backend - Dashboard Aggregator Endpoint.md
│   ├── F02 - W03 - Frontend - Shell Layout Sidebar and Navbar.md
│   ├── F02 - W04 - Frontend - Dashboard Component and Widgets.md
│   ├── F02 - W05 - Frontend - Upcoming Deadlines Widget.md
│   ├── F02 - W06 - Frontend - Regulatory Updates Widget.md
│   └── F02 - W07 - Testing - Dashboard Tests.md
├── F03 - Legal Norm Search/
│   ├── F03 - W01 - Comprehensive Documentation.md
│   ├── F03 - W02 - Backend - AI Search Index for Legal Norms.md
│   ├── F03 - W03 - Backend - Hybrid BM25 and Vector Scoring Profile.md
│   ├── F03 - W04 - Backend - POST Search Legal Norms Endpoint.md
│   ├── F03 - W05 - Frontend - SearchBar with Autocomplete.md
│   ├── F03 - W06 - Frontend - Sidebar Filters with Facets.md
│   ├── F03 - W07 - Frontend - Results List with Highlight.md
│   └── F03 - W08 - Testing - Legal Norm Search Tests.md
├── F04 - Case Law Search/
│   ├── F04 - W01 - Comprehensive Documentation.md
│   ├── F04 - W02 - Backend - AI Search Index for Case Law.md
│   ├── F04 - W03 - Backend - POST Search Case Law Endpoint.md
│   ├── F04 - W04 - Frontend - Case Law Search Page.md
│   ├── F04 - W05 - Frontend - Ruling Result Card.md
│   └── F04 - W06 - Testing - Case Law Search Tests.md
├── F05 - Legal Norm Detail/
│   ├── F05 - W01 - Comprehensive Documentation.md
│   ├── F05 - W02 - Backend - GET Legal Norm Detail Endpoint.md
│   ├── F05 - W03 - Backend - GET Legal Norm Graph SQL Graph Endpoint.md
│   ├── F05 - W04 - Backend - GET Legal Norm Articles Paged Endpoint.md
│   ├── F05 - W05 - Frontend - Legal Norm Detail Page with Tabs.md
│   ├── F05 - W06 - Frontend - Relationship Graph Visualization.md
│   ├── F05 - W07 - Frontend - Amendments Timeline.md
│   └── F05 - W08 - Testing - Legal Norm Detail Tests.md
├── F06 - Article Detail/
│   ├── F06 - W01 - Comprehensive Documentation.md
│   ├── F06 - W02 - Backend - GET Article and Related Case Law Endpoint.md
│   ├── F06 - W03 - Frontend - Article Detail Page.md
│   ├── F06 - W04 - Frontend - Related Case Law Panel.md
│   └── F06 - W05 - Testing - Article Detail Tests.md
├── F07 - Regulatory Updates/
│   ├── F07 - W01 - Comprehensive Documentation.md
│   ├── F07 - W02 - Backend - Azure Function Timer Trigger Official Gazette.md
│   ├── F07 - W03 - Backend - GET Updates and SignalR Push Endpoint.md
│   ├── F07 - W04 - Frontend - Updates Feed Page.md
│   ├── F07 - W05 - Frontend - Alert Subscription by Law Branch.md
│   └── F07 - W06 - Testing - Updates Tests.md
├── F08 - AI Agent Chat/
│   ├── F08 - W01 - Comprehensive Documentation.md
│   ├── F08 - W02 - Backend - Semantic Kernel Setup and Orchestrator.md
│   ├── F08 - W03 - Backend - POST Chat with SignalR Streaming Endpoint.md
│   ├── F08 - W04 - Backend - Conversation Persistence.md
│   ├── F08 - W05 - Frontend - Chat UI with Markdown Rendering.md
│   ├── F08 - W06 - Frontend - SignalR Client for Streaming.md
│   ├── F08 - W07 - Frontend - Cited Sources Panel.md
│   ├── F08 - W08 - Frontend - Conversation History.md
│   └── F08 - W09 - Testing - E2E Chat Tests.md
├── F09 - Regulatory Agent/
│   ├── F09 - W01 - Comprehensive Documentation.md
│   ├── F09 - W02 - Backend - RegulatoryAgent Plugin Functions.md
│   ├── F09 - W03 - Backend - Agent Semantic Prompts.md
│   ├── F09 - W04 - Backend - Integration with AI Search and SQL Graph.md
│   └── F09 - W05 - Testing - Evaluation with 15 Typified Queries.md
├── F10 - Case Law Agent/
│   ├── F10 - W01 - Comprehensive Documentation.md
│   ├── F10 - W02 - Backend - CaseLawAgent Plugin Functions.md
│   ├── F10 - W03 - Backend - RAG Pipeline over Case Law.md
│   ├── F10 - W04 - Backend - Graph Traversal Ruling to Article.md
│   └── F10 - W05 - Testing - Evaluation with 15 Typified Queries.md
├── F11 - Procedural Agent/
│   ├── F11 - W01 - Comprehensive Documentation.md
│   ├── F11 - W02 - Backend - ProceduralAgent Plugin Functions.md
│   ├── F11 - W03 - Backend - Business Days Calculation Engine.md
│   ├── F11 - W04 - Backend - National and Court Holidays Calendar.md
│   └── F11 - W05 - Testing - Evaluation with 10 Typified Queries.md
├── F12 - Case File Management/
│   ├── F12 - W01 - Comprehensive Documentation.md
│   ├── F12 - W02 - Backend - EF Core Case File Model and Migrations.md
│   ├── F12 - W03 - Backend - Case File CRUD Endpoints.md
│   ├── F12 - W04 - Backend - Movements and Documents Subresources.md
│   ├── F12 - W05 - Backend - Upload Documents to Blob Storage.md
│   ├── F12 - W06 - Frontend - Case File List with DataTable.md
│   ├── F12 - W07 - Frontend - Case File Create and Edit Form.md
│   ├── F12 - W08 - Frontend - Case File Detail with Tabs.md
│   ├── F12 - W09 - Frontend - Movements Timeline.md
│   ├── F12 - W10 - Frontend - Attached Documents Management.md
│   └── F12 - W11 - Testing - Case File CRUD Tests.md
├── F13 - Deadline Management/
│   ├── F13 - W01 - Comprehensive Documentation.md
│   ├── F13 - W02 - Backend - EF Core Deadline Model and Migrations.md
│   ├── F13 - W03 - Backend - Deadline CRUD Endpoints.md
│   ├── F13 - W04 - Backend - Azure Function Daily Deadline Evaluation.md
│   ├── F13 - W05 - Backend - Alert Generation via Storage Queue.md
│   ├── F13 - W06 - Frontend - Deadline List with Visual Traffic Light.md
│   ├── F13 - W07 - Frontend - Deadline Create Form with Business Days Calculation.md
│   └── F13 - W08 - Testing - Deadline and Business Days Calculation Tests.md
├── F14 - Legal Calendar/
│   ├── F14 - W01 - Comprehensive Documentation.md
│   ├── F14 - W02 - Backend - GET Aggregated Calendar Endpoint.md
│   ├── F14 - W03 - Frontend - FullCalendar Angular Integration.md
│   ├── F14 - W04 - Frontend - Calendar Filters and Navigation.md
│   └── F14 - W05 - Testing - Calendar Tests.md
├── F15 - Legal Risk Analysis/
│   ├── F15 - W01 - Comprehensive Documentation.md
│   ├── F15 - W02 - Backend - RiskAgent Semantic Kernel Plugin.md
│   ├── F15 - W03 - Backend - Risk Taxonomy Model.md
│   ├── F15 - W04 - Backend - POST Analyze Risk Endpoint.md
│   ├── F15 - W05 - Backend - Analysis Persistence in SQL.md
│   ├── F15 - W06 - Frontend - Case Input Form.md
│   ├── F15 - W07 - Frontend - Result View with Visual Score.md
│   └── F15 - W08 - Testing - Risk Analysis Evaluation.md
├── F16 - Risk Analysis History/
│   ├── F16 - W01 - Comprehensive Documentation.md
│   ├── F16 - W02 - Backend - GET History and Re-analysis Endpoint.md
│   ├── F16 - W03 - Frontend - History List with DataTable.md
│   ├── F16 - W04 - Frontend - Visual Diff Between Analyses.md
│   └── F16 - W05 - Testing - History Tests.md
├── F17 - Legal Report Generation/
│   ├── F17 - W01 - Comprehensive Documentation.md
│   ├── F17 - W02 - Backend - OpenXml Template Engine.md
│   ├── F17 - W03 - Backend - Report Templates in Blob Storage.md
│   ├── F17 - W04 - Backend - POST Generate Report Endpoint.md
│   ├── F17 - W05 - Frontend - Report Type Selector.md
│   ├── F17 - W06 - Frontend - Report Preview and Download.md
│   └── F17 - W07 - Testing - Report Generation Tests.md
├── F18 - Operational Reports/
│   ├── F18 - W01 - Comprehensive Documentation.md
│   ├── F18 - W02 - Backend - Aggregated Reports Endpoints.md
│   ├── F18 - W03 - Frontend - Charts with ngx-charts.md
│   ├── F18 - W04 - Frontend - Export to PDF and Excel.md
│   └── F18 - W05 - Testing - Reports Tests.md
├── F19 - User Administration/
│   ├── F19 - W01 - Comprehensive Documentation.md
│   ├── F19 - W02 - Backend - User CRUD and Custom Permissions.md
│   ├── F19 - W03 - Backend - Audit Log in Azure SQL.md
│   ├── F19 - W04 - Frontend - User Admin Page.md
│   ├── F19 - W05 - Frontend - Audit Log.md
│   └── F19 - W06 - Testing - Admin Tests.md
├── F20 - Advanced Alert Configuration/
│   ├── F20 - W01 - Comprehensive Documentation.md
│   ├── F20 - W02 - Backend - Alert Configuration CRUD.md
│   ├── F20 - W03 - Backend - Azure Function Condition Evaluation.md
│   ├── F20 - W04 - Backend - Email Sending with SendGrid or SMTP.md
│   ├── F20 - W05 - Frontend - Alert Configuration Wizard.md
│   ├── F20 - W06 - Frontend - Alert Center Inbox.md
│   └── F20 - W07 - Testing - Alert Tests.md
├── F21 - Legal Graph Explorer/
│   ├── F21 - W01 - Comprehensive Documentation.md
│   ├── F21 - W02 - Backend - GET Explore Graph with Depth Endpoint.md
│   ├── F21 - W03 - Frontend - Interactive Graph Component D3 or Cytoscape.md
│   ├── F21 - W04 - Frontend - Selected Node Detail Panel.md
│   ├── F21 - W05 - Frontend - Filters by Relationship Type.md
│   └── F21 - W06 - Testing - Graph Explorer Tests.md
├── F22 - Agent Feedback and Improvement/
│   ├── F22 - W01 - Comprehensive Documentation.md
│   ├── F22 - W02 - Backend - POST Feedback Endpoint and SQL Model.md
│   ├── F22 - W03 - Backend - Azure Function Weekly Feedback Analysis.md
│   ├── F22 - W04 - Frontend - Feedback Buttons in Chat.md
│   ├── F22 - W05 - Frontend - Satisfaction Metrics Dashboard.md
│   └── F22 - W06 - Testing - Feedback Tests.md
├── F23 - PWA Offline Mode/
│   ├── F23 - W01 - Comprehensive Documentation.md
│   ├── F23 - W02 - Backend - ETags and Caching Headers.md
│   ├── F23 - W03 - Frontend - Angular Service Worker Setup.md
│   ├── F23 - W04 - Frontend - IndexedDB for Offline Cache.md
│   ├── F23 - W05 - Frontend - Sync on Reconnect.md
│   └── F23 - W06 - Testing - Offline Tests.md
├── FT01 - Real-Time Notifications/
│   ├── FT01 - W01 - Comprehensive Documentation.md
│   ├── FT01 - W02 - Backend - SignalR NotificationHub.md
│   ├── FT01 - W03 - Backend - Worker Storage Queue to SignalR.md
│   ├── FT01 - W04 - Frontend - NotificationService SignalR Client.md
│   ├── FT01 - W05 - Frontend - Badge and Toast Components.md
│   ├── FT01 - W06 - Frontend - Notification Center.md
│   └── FT01 - W07 - Testing - Notification Tests.md
├── FT02 - Global Omnisearch/
│   ├── FT02 - W01 - Comprehensive Documentation.md
│   ├── FT02 - W02 - Backend - GET Global Search Multi-Index Endpoint.md
│   ├── FT02 - W03 - Frontend - Omnisearch Modal with Keyboard Nav.md
│   ├── FT02 - W04 - Frontend - Results Grouped by Type.md
│   └── FT02 - W05 - Testing - Omnisearch Tests.md
├── FT03 - Theme and Accessibility/
│   ├── FT03 - W01 - Comprehensive Documentation.md
│   ├── FT03 - W02 - Frontend - Angular Material Light and Dark Theming.md
│   ├── FT03 - W03 - Frontend - Tailwind Config and Design Tokens.md
│   ├── FT03 - W04 - Frontend - WCAG 2.1 AA Accessibility.md
│   └── FT03 - W05 - Testing - Accessibility Audit.md
├── FT04 - Audit and Logging/
│   ├── FT04 - W01 - Comprehensive Documentation.md
│   ├── FT04 - W02 - Backend - Audit Middleware .NET 10.md
│   ├── FT04 - W03 - Backend - AuditLog Table and Retention Policy.md
│   ├── FT04 - W04 - Backend - Application Insights Integration.md
│   └── FT04 - W05 - Testing - Audit Tests.md
└── FT05 - Delivery and Hosting/
    ├── FT05 - W01 - Comprehensive Documentation.md
    ├── FT05 - W02 - GitHub - CI and CD to Azure Staging.md
    ├── FT05 - W03 - GCaaS - Helm Chart and Knative Deployment.md
    ├── FT05 - W04 - GCaaS - Platform Authentication id_token Cookie.md
    ├── FT05 - W05 - GCaaS - Vault Secrets and ConfigMap Wiring.md
    └── FT05 - W06 - GCaaS - Post-Deploy Verification and Rollback Runbook.md
```

---

## Release 1.0 (7 features, 47 work items)

| Feature | Name | Sprint | Work Items |
|---------|------|--------|------------|
| F01 | [Authentication and Authorization](F01%20-%20Authentication%20and%20Authorization/) | S01 | 7 |
| F02 | [Main Dashboard](F02%20-%20Main%20Dashboard/) | S02 | 7 |
| F03 | [Legal Norm Search](F03%20-%20Legal%20Norm%20Search/) | S02-S03 | 8 |
| F04 | [Case Law Search](F04%20-%20Case%20Law%20Search/) | S03 | 6 |
| F05 | [Legal Norm Detail](F05%20-%20Legal%20Norm%20Detail/) | S03-S04 | 8 |
| F06 | [Article Detail](F06%20-%20Article%20Detail/) | S04 | 5 |
| F07 | [Regulatory Updates](F07%20-%20Regulatory%20Updates/) | S04 | 6 |

### Work Item Detail — Release 1.0

| Feature | Work Item | Type | Estimate |
|---------|-----------|------|----------|
| F01 | F01-W01: Comprehensive Documentation | doc | 3 SP |
| F01 | F01-W02: Backend - Entra ID and JWT Configuration | backend | 5 SP |
| F01 | F01-W03: Backend - Role-Based Authorization Middleware | backend | 5 SP |
| F01 | F01-W04: Frontend - MSAL Angular Setup and AuthService | frontend | 3 SP |
| F01 | F01-W05: Frontend - AuthGuard and RoleGuard | frontend | 3 SP |
| F01 | F01-W06: Frontend - AuthInterceptor and ErrorInterceptor | frontend | 3 SP |
| F01 | F01-W07: Testing - E2E Authentication Tests | testing | 3 SP |
| F02 | F02-W01: Comprehensive Documentation | doc | 3 SP |
| F02 | F02-W02: Backend - Dashboard Aggregator Endpoint | backend | 5 SP |
| F02 | F02-W03: Frontend - Shell Layout Sidebar and Navbar | frontend | 5 SP |
| F02 | F02-W04: Frontend - Dashboard Component and Widgets | frontend | 3 SP |
| F02 | F02-W05: Frontend - Upcoming Deadlines Widget | frontend | 3 SP |
| F02 | F02-W06: Frontend - Regulatory Updates Widget | frontend | 3 SP |
| F02 | F02-W07: Testing - Dashboard Tests | testing | 3 SP |
| F03 | F03-W01: Comprehensive Documentation | doc | 3 SP |
| F03 | F03-W02: Backend - AI Search Index for Legal Norms | backend | 5 SP |
| F03 | F03-W03: Backend - Hybrid BM25 and Vector Scoring Profile | backend | 5 SP |
| F03 | F03-W04: Backend - POST Search Legal Norms Endpoint | backend | 3 SP |
| F03 | F03-W05: Frontend - SearchBar with Autocomplete | frontend | 3 SP |
| F03 | F03-W06: Frontend - Sidebar Filters with Facets | frontend | 3 SP |
| F03 | F03-W07: Frontend - Results List with Highlight | frontend | 3 SP |
| F03 | F03-W08: Testing - Legal Norm Search Tests | testing | 3 SP |
| F04 | F04-W01: Comprehensive Documentation | doc | 3 SP |
| F04 | F04-W02: Backend - AI Search Index for Case Law | backend | 5 SP |
| F04 | F04-W03: Backend - POST Search Case Law Endpoint | backend | 5 SP |
| F04 | F04-W04: Frontend - Case Law Search Page | frontend | 3 SP |
| F04 | F04-W05: Frontend - Ruling Result Card | frontend | 3 SP |
| F04 | F04-W06: Testing - Case Law Search Tests | testing | 3 SP |
| F05 | F05-W01: Comprehensive Documentation | doc | 3 SP |
| F05 | F05-W02: Backend - GET Legal Norm Detail Endpoint | backend | 5 SP |
| F05 | F05-W03: Backend - GET Legal Norm Graph SQL Graph Endpoint | backend | 5 SP |
| F05 | F05-W04: Backend - GET Legal Norm Articles Paged Endpoint | backend | 3 SP |
| F05 | F05-W05: Frontend - Legal Norm Detail Page with Tabs | frontend | 3 SP |
| F05 | F05-W06: Frontend - Relationship Graph Visualization | frontend | 3 SP |
| F05 | F05-W07: Frontend - Amendments Timeline | frontend | 3 SP |
| F05 | F05-W08: Testing - Legal Norm Detail Tests | testing | 3 SP |
| F06 | F06-W01: Comprehensive Documentation | doc | 3 SP |
| F06 | F06-W02: Backend - GET Article and Related Case Law Endpoint | backend | 5 SP |
| F06 | F06-W03: Frontend - Article Detail Page | frontend | 5 SP |
| F06 | F06-W04: Frontend - Related Case Law Panel | frontend | 3 SP |
| F06 | F06-W05: Testing - Article Detail Tests | testing | 3 SP |
| F07 | F07-W01: Comprehensive Documentation | doc | 3 SP |
| F07 | F07-W02: Backend - Azure Function Timer Trigger Official Gazette | backend | 5 SP |
| F07 | F07-W03: Backend - GET Updates and SignalR Push Endpoint | backend | 5 SP |
| F07 | F07-W04: Frontend - Updates Feed Page | frontend | 3 SP |
| F07 | F07-W05: Frontend - Alert Subscription by Law Branch | frontend | 3 SP |
| F07 | F07-W06: Testing - Updates Tests | testing | 3 SP |

---

## Release 2.0 (7 features, 48 work items)

| Feature | Name | Sprint | Work Items |
|---------|------|--------|------------|
| F08 | [AI Agent Chat](F08%20-%20AI%20Agent%20Chat/) | S05-S06 | 9 |
| F09 | [Regulatory Agent](F09%20-%20Regulatory%20Agent/) | S06 | 5 |
| F10 | [Case Law Agent](F10%20-%20Case%20Law%20Agent/) | S06-S07 | 5 |
| F11 | [Procedural Agent](F11%20-%20Procedural%20Agent/) | S07 | 5 |
| F12 | [Case File Management](F12%20-%20Case%20File%20Management/) | S05-S06 | 11 |
| F13 | [Deadline Management](F13%20-%20Deadline%20Management/) | S06-S07 | 8 |
| F14 | [Legal Calendar](F14%20-%20Legal%20Calendar/) | S07 | 5 |

### Work Item Detail — Release 2.0

| Feature | Work Item | Type | Estimate |
|---------|-----------|------|----------|
| F08 | F08-W01: Comprehensive Documentation | doc | 3 SP |
| F08 | F08-W02: Backend - Semantic Kernel Setup and Orchestrator | backend | 5 SP |
| F08 | F08-W03: Backend - POST Chat with SignalR Streaming Endpoint | backend | 5 SP |
| F08 | F08-W04: Backend - Conversation Persistence | backend | 3 SP |
| F08 | F08-W05: Frontend - Chat UI with Markdown Rendering | frontend | 3 SP |
| F08 | F08-W06: Frontend - SignalR Client for Streaming | frontend | 3 SP |
| F08 | F08-W07: Frontend - Cited Sources Panel | frontend | 3 SP |
| F08 | F08-W08: Frontend - Conversation History | frontend | 3 SP |
| F08 | F08-W09: Testing - E2E Chat Tests | testing | 3 SP |
| F09 | F09-W01: Comprehensive Documentation | doc | 3 SP |
| F09 | F09-W02: Backend - RegulatoryAgent Plugin Functions | backend | 5 SP |
| F09 | F09-W03: Backend - Agent Semantic Prompts | backend | 5 SP |
| F09 | F09-W04: Backend - Integration with AI Search and SQL Graph | backend | 3 SP |
| F09 | F09-W05: Testing - Evaluation with 15 Typified Queries | testing | 3 SP |
| F10 | F10-W01: Comprehensive Documentation | doc | 3 SP |
| F10 | F10-W02: Backend - CaseLawAgent Plugin Functions | backend | 5 SP |
| F10 | F10-W03: Backend - RAG Pipeline over Case Law | backend | 5 SP |
| F10 | F10-W04: Backend - Graph Traversal Ruling to Article | backend | 3 SP |
| F10 | F10-W05: Testing - Evaluation with 15 Typified Queries | testing | 3 SP |
| F11 | F11-W01: Comprehensive Documentation | doc | 3 SP |
| F11 | F11-W02: Backend - ProceduralAgent Plugin Functions | backend | 5 SP |
| F11 | F11-W03: Backend - Business Days Calculation Engine | backend | 5 SP |
| F11 | F11-W04: Backend - National and Court Holidays Calendar | backend | 3 SP |
| F11 | F11-W05: Testing - Evaluation with 10 Typified Queries | testing | 3 SP |
| F12 | F12-W01: Comprehensive Documentation | doc | 3 SP |
| F12 | F12-W02: Backend - EF Core Case File Model and Migrations | backend | 5 SP |
| F12 | F12-W03: Backend - Case File CRUD Endpoints | backend | 5 SP |
| F12 | F12-W04: Backend - Movements and Documents Subresources | backend | 3 SP |
| F12 | F12-W05: Backend - Upload Documents to Blob Storage | backend | 3 SP |
| F12 | F12-W06: Frontend - Case File List with DataTable | frontend | 3 SP |
| F12 | F12-W07: Frontend - Case File Create and Edit Form | frontend | 3 SP |
| F12 | F12-W08: Frontend - Case File Detail with Tabs | frontend | 3 SP |
| F12 | F12-W09: Frontend - Movements Timeline | frontend | 3 SP |
| F12 | F12-W10: Frontend - Attached Documents Management | frontend | 3 SP |
| F12 | F12-W11: Testing - Case File CRUD Tests | testing | 3 SP |
| F13 | F13-W01: Comprehensive Documentation | doc | 3 SP |
| F13 | F13-W02: Backend - EF Core Deadline Model and Migrations | backend | 5 SP |
| F13 | F13-W03: Backend - Deadline CRUD Endpoints | backend | 5 SP |
| F13 | F13-W04: Backend - Azure Function Daily Deadline Evaluation | backend | 3 SP |
| F13 | F13-W05: Backend - Alert Generation via Storage Queue | backend | 3 SP |
| F13 | F13-W06: Frontend - Deadline List with Visual Traffic Light | frontend | 3 SP |
| F13 | F13-W07: Frontend - Deadline Create Form with Business Days Calculation | frontend | 3 SP |
| F13 | F13-W08: Testing - Deadline and Business Days Calculation Tests | testing | 3 SP |
| F14 | F14-W01: Comprehensive Documentation | doc | 3 SP |
| F14 | F14-W02: Backend - GET Aggregated Calendar Endpoint | backend | 5 SP |
| F14 | F14-W03: Frontend - FullCalendar Angular Integration | frontend | 5 SP |
| F14 | F14-W04: Frontend - Calendar Filters and Navigation | frontend | 3 SP |
| F14 | F14-W05: Testing - Calendar Tests | testing | 3 SP |

---

## Release 3.0 (4 features, 25 work items)

| Feature | Name | Sprint | Work Items |
|---------|------|--------|------------|
| F15 | [Legal Risk Analysis](F15%20-%20Legal%20Risk%20Analysis/) | S08-S09 | 8 |
| F16 | [Risk Analysis History](F16%20-%20Risk%20Analysis%20History/) | S09 | 5 |
| F17 | [Legal Report Generation](F17%20-%20Legal%20Report%20Generation/) | S08-S09 | 7 |
| F18 | [Operational Reports](F18%20-%20Operational%20Reports/) | S09 | 5 |

### Work Item Detail — Release 3.0

| Feature | Work Item | Type | Estimate |
|---------|-----------|------|----------|
| F15 | F15-W01: Comprehensive Documentation | doc | 3 SP |
| F15 | F15-W02: Backend - RiskAgent Semantic Kernel Plugin | backend | 5 SP |
| F15 | F15-W03: Backend - Risk Taxonomy Model | backend | 5 SP |
| F15 | F15-W04: Backend - POST Analyze Risk Endpoint | backend | 3 SP |
| F15 | F15-W05: Backend - Analysis Persistence in SQL | backend | 3 SP |
| F15 | F15-W06: Frontend - Case Input Form | frontend | 3 SP |
| F15 | F15-W07: Frontend - Result View with Visual Score | frontend | 3 SP |
| F15 | F15-W08: Testing - Risk Analysis Evaluation | testing | 3 SP |
| F16 | F16-W01: Comprehensive Documentation | doc | 3 SP |
| F16 | F16-W02: Backend - GET History and Re-analysis Endpoint | backend | 5 SP |
| F16 | F16-W03: Frontend - History List with DataTable | frontend | 5 SP |
| F16 | F16-W04: Frontend - Visual Diff Between Analyses | frontend | 3 SP |
| F16 | F16-W05: Testing - History Tests | testing | 3 SP |
| F17 | F17-W01: Comprehensive Documentation | doc | 3 SP |
| F17 | F17-W02: Backend - OpenXml Template Engine | backend | 5 SP |
| F17 | F17-W03: Backend - Report Templates in Blob Storage | backend | 5 SP |
| F17 | F17-W04: Backend - POST Generate Report Endpoint | backend | 3 SP |
| F17 | F17-W05: Frontend - Report Type Selector | frontend | 3 SP |
| F17 | F17-W06: Frontend - Report Preview and Download | frontend | 3 SP |
| F17 | F17-W07: Testing - Report Generation Tests | testing | 3 SP |
| F18 | F18-W01: Comprehensive Documentation | doc | 3 SP |
| F18 | F18-W02: Backend - Aggregated Reports Endpoints | backend | 5 SP |
| F18 | F18-W03: Frontend - Charts with ngx-charts | frontend | 5 SP |
| F18 | F18-W04: Frontend - Export to PDF and Excel | frontend | 3 SP |
| F18 | F18-W05: Testing - Reports Tests | testing | 3 SP |

---

## Release 4.0 (5 features, 31 work items)

| Feature | Name | Sprint | Work Items |
|---------|------|--------|------------|
| F19 | [User Administration](F19%20-%20User%20Administration/) | S10 | 6 |
| F20 | [Advanced Alert Configuration](F20%20-%20Advanced%20Alert%20Configuration/) | S10 | 7 |
| F21 | [Legal Graph Explorer](F21%20-%20Legal%20Graph%20Explorer/) | S11 | 6 |
| F22 | [Agent Feedback and Improvement](F22%20-%20Agent%20Feedback%20and%20Improvement/) | S11 | 6 |
| F23 | [PWA Offline Mode](F23%20-%20PWA%20Offline%20Mode/) | S11 | 6 |

### Work Item Detail — Release 4.0

| Feature | Work Item | Type | Estimate |
|---------|-----------|------|----------|
| F19 | F19-W01: Comprehensive Documentation | doc | 3 SP |
| F19 | F19-W02: Backend - User CRUD and Custom Permissions | backend | 5 SP |
| F19 | F19-W03: Backend - Audit Log in Azure SQL | backend | 5 SP |
| F19 | F19-W04: Frontend - User Admin Page | frontend | 3 SP |
| F19 | F19-W05: Frontend - Audit Log | frontend | 3 SP |
| F19 | F19-W06: Testing - Admin Tests | testing | 3 SP |
| F20 | F20-W01: Comprehensive Documentation | doc | 3 SP |
| F20 | F20-W02: Backend - Alert Configuration CRUD | backend | 5 SP |
| F20 | F20-W03: Backend - Azure Function Condition Evaluation | backend | 5 SP |
| F20 | F20-W04: Backend - Email Sending with SendGrid or SMTP | backend | 3 SP |
| F20 | F20-W05: Frontend - Alert Configuration Wizard | frontend | 3 SP |
| F20 | F20-W06: Frontend - Alert Center Inbox | frontend | 3 SP |
| F20 | F20-W07: Testing - Alert Tests | testing | 3 SP |
| F21 | F21-W01: Comprehensive Documentation | doc | 3 SP |
| F21 | F21-W02: Backend - GET Explore Graph with Depth Endpoint | backend | 5 SP |
| F21 | F21-W03: Frontend - Interactive Graph Component D3 or Cytoscape | frontend | 5 SP |
| F21 | F21-W04: Frontend - Selected Node Detail Panel | frontend | 3 SP |
| F21 | F21-W05: Frontend - Filters by Relationship Type | frontend | 3 SP |
| F21 | F21-W06: Testing - Graph Explorer Tests | testing | 3 SP |
| F22 | F22-W01: Comprehensive Documentation | doc | 3 SP |
| F22 | F22-W02: Backend - POST Feedback Endpoint and SQL Model | backend | 5 SP |
| F22 | F22-W03: Backend - Azure Function Weekly Feedback Analysis | backend | 5 SP |
| F22 | F22-W04: Frontend - Feedback Buttons in Chat | frontend | 3 SP |
| F22 | F22-W05: Frontend - Satisfaction Metrics Dashboard | frontend | 3 SP |
| F22 | F22-W06: Testing - Feedback Tests | testing | 3 SP |
| F23 | F23-W01: Comprehensive Documentation | doc | 3 SP |
| F23 | F23-W02: Backend - ETags and Caching Headers | backend | 5 SP |
| F23 | F23-W03: Frontend - Angular Service Worker Setup | frontend | 5 SP |
| F23 | F23-W04: Frontend - IndexedDB for Offline Cache | frontend | 3 SP |
| F23 | F23-W05: Frontend - Sync on Reconnect | frontend | 3 SP |
| F23 | F23-W06: Testing - Offline Tests | testing | 3 SP |

---

## Cross-Cutting Release (4 features, 22 work items)

| Feature | Name | Sprint | Work Items |
|---------|------|--------|------------|
| FT01 | [Real-Time Notifications](FT01%20-%20Real-Time%20Notifications/) | S03-S04 | 7 |
| FT02 | [Global Omnisearch](FT02%20-%20Global%20Omnisearch/) | S04 | 5 |
| FT03 | [Theme and Accessibility](FT03%20-%20Theme%20and%20Accessibility/) | S02 | 5 |
| FT04 | [Audit and Logging](FT04%20-%20Audit%20and%20Logging/) | S03 | 5 |
| FT05 | [Delivery and Hosting](FT05%20-%20Delivery%20and%20Hosting/) | S00-S02 | 6 |

### Work Item Detail — Cross-Cutting Release

| Feature | Work Item | Type | Estimate |
|---------|-----------|------|----------|
| FT01 | FT01-W01: Comprehensive Documentation | doc | 3 SP |
| FT01 | FT01-W02: Backend - SignalR NotificationHub | backend | 5 SP |
| FT01 | FT01-W03: Backend - Worker Storage Queue to SignalR | backend | 5 SP |
| FT01 | FT01-W04: Frontend - NotificationService SignalR Client | frontend | 3 SP |
| FT01 | FT01-W05: Frontend - Badge and Toast Components | frontend | 3 SP |
| FT01 | FT01-W06: Frontend - Notification Center | frontend | 3 SP |
| FT01 | FT01-W07: Testing - Notification Tests | testing | 3 SP |
| FT02 | FT02-W01: Comprehensive Documentation | doc | 3 SP |
| FT02 | FT02-W02: Backend - GET Global Search Multi-Index Endpoint | backend | 5 SP |
| FT02 | FT02-W03: Frontend - Omnisearch Modal with Keyboard Nav | frontend | 5 SP |
| FT02 | FT02-W04: Frontend - Results Grouped by Type | frontend | 3 SP |
| FT02 | FT02-W05: Testing - Omnisearch Tests | testing | 3 SP |
| FT03 | FT03-W01: Comprehensive Documentation | doc | 3 SP |
| FT03 | FT03-W02: Frontend - Angular Material Light and Dark Theming | frontend | 5 SP |
| FT03 | FT03-W03: Frontend - Tailwind Config and Design Tokens | frontend | 5 SP |
| FT03 | FT03-W04: Frontend - WCAG 2.1 AA Accessibility | frontend | 3 SP |
| FT03 | FT03-W05: Testing - Accessibility Audit | testing | 3 SP |
| FT04 | FT04-W01: Comprehensive Documentation | doc | 3 SP |
| FT04 | FT04-W02: Backend - Audit Middleware .NET 10 | backend | 5 SP |
| FT04 | FT04-W03: Backend - AuditLog Table and Retention Policy | backend | 5 SP |
| FT04 | FT04-W04: Backend - Application Insights Integration | backend | 3 SP |
| FT04 | FT04-W05: Testing - Audit Tests | testing | 3 SP |
| FT05 | FT05-W01: Comprehensive Documentation | docs | 3 SP |
| FT05 | FT05-W02: GitHub - CI and CD to Azure Staging | devops | 5 SP |
| FT05 | FT05-W03: GCaaS - Helm Chart and Knative Deployment | devops | 5 SP |
| FT05 | FT05-W04: GCaaS - Platform Authentication id_token Cookie | backend/frontend | 5 SP |
| FT05 | FT05-W05: GCaaS - Vault Secrets and ConfigMap Wiring | devops | 3 SP |
| FT05 | FT05-W06: GCaaS - Post-Deploy Verification and Rollback Runbook | devops/docs | 3 SP |

---

## Summary

| Metric | Value |
|--------|-------|
| Total features | 28 |
| Total work items | 179 |
| Total story points | 651 SP |
| Estimated sprints (2 weeks each) | 11 |
| Expected velocity (2-3 devs) | ~25-35 SP/sprint |

> Note: FT05 (Delivery and Hosting) is largely implemented in the MVP (~70%); its work items mainly document and reconcile the existing GitHub + GCaaS setup.

---

*Legal Ai Ar — Work Item Backlog — 2026-04-02*
