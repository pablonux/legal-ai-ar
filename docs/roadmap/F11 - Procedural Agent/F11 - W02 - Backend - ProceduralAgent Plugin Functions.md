# F11 - W02 - Backend - ProceduralAgent Plugin Functions

> **Feature:** F11 - Procedural Agent
> **Release:** 2.0 | **Sprint:** S07
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Implement the Semantic Kernel plugin for Procedural Agent. Includes native functions, semantic prompts, and connector configuration.

---

## Tasks

- [ ] Create the plugin class in `Application/Agents/Plugins/`
- [ ] Implement native functions with the `[KernelFunction]` attribute
- [ ] Create semantic prompts in `Application/Agents/Prompts/`
- [ ] Registrar plugin en el Kernel builder
- [ ] Configure connectors (AI Search, SQL, OpenAI)
- [ ] Write unit tests con mock del Kernel
- [ ] Validar respuestas con el set de consultas tipificadas

---

## Acceptance Criteria

- [ ] The implemented functionality meets the W01 acceptance criteria
- [ ] Tests pass
- [ ] The code is reviewed by at least 1 peer

---

## Technical Notes

- Framework: .NET 10 LTS with ASP.NET Core Minimal API
- ORM: Entity Framework Core 10
- Validation: FluentValidation 12.x
- Logging: Serilog with an Application Insights sink
- Refer to the comprehensive documentation (F11-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Application/Agents/Plugins/{Agente}Plugin.cs
src/Application/Agents/Prompts/{agente}_system.txt
src/Application/Agents/Prompts/{agente}_user.txt
tests/Application.Tests/Agents/{Agente}PluginTests.cs
```

---

## Dependencies

- Depends on: F11-W01 (Comprehensive Documentation)

---

*F11 - W02 - Backend - ProceduralAgent Plugin Functions — Legal Ai Ar*
