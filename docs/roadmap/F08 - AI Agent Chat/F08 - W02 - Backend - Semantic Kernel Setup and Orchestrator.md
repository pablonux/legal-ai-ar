# F08 - W02 - Backend - Semantic Kernel Setup y Orquestador

> **Feature:** F08 - Chat con Agentes IA
> **Release:** 2.0 | **Sprint:** S05-S06
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Implementar el plugin de Semantic Kernel para Chat con Agentes IA. Incluye native functions, prompts semánticos y configuración de connectors.

---

## Tareas

- [ ] Crear clase del plugin en `Application/Agents/Plugins/`
- [ ] Implementar native functions con atributo `[KernelFunction]`
- [ ] Crear prompts semánticos en `Application/Agents/Prompts/`
- [ ] Registrar plugin en el Kernel builder
- [ ] Configurar connectors (AI Search, SQL, OpenAI)
- [ ] Escribir tests unitarios con mock del Kernel
- [ ] Validar respuestas con el set de consultas tipificadas

---

## Criterios de Aceptación

- [ ] La funcionalidad implementada cumple con los criterios de aceptación del W01
- [ ] Los tests pasan
- [ ] El código está revisado por al menos 1 peer

---

## Notas Técnicas

- Framework: .NET 10 LTS con ASP.NET Core Minimal API
- ORM: Entity Framework Core 10
- Validación: FluentValidation 12.x
- Logging: Serilog con sink a Application Insights
- Referir a la documentación integral (F08-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Application/Agents/Plugins/{Agente}Plugin.cs
src/Application/Agents/Prompts/{agente}_system.txt
src/Application/Agents/Prompts/{agente}_user.txt
tests/Application.Tests/Agents/{Agente}PluginTests.cs
```

---

## Dependencias

- Depende de: F08-W01 (Documentación integral)

---

*F08 - W02 - Backend - Semantic Kernel Setup y Orquestador — Legal Ai Ar*
