---
name: ia-tools
description: "Standard AI/RAG tools for Legal Ai Ar: YAML prompt templates for Semantic Kernel, evaluation configuration with golden sets and LLM-as-judge, and Semantic Kernel plugin scaffolding with [KernelFunction]. Use when asked to create a prompt, an agent plugin, configure AI evaluation, or any task related to Semantic Kernel, agents, or RAG. Also applies with 'prompt', 'plugin', 'agent', 'evaluation', 'golden set', 'Semantic Kernel', 'tool calling'."
---

# IA Tools — Legal Ai Ar

Templates and conventions for AI agent development with Semantic Kernel.

> **Language note**: per the project rule, the **agent prompt content stays in Spanish** (it is the end-user contact layer — users and legal sources are in Spanish). Everything else (file names, YAML keys, code, JSON keys, documentation) is in English.

---

## 1. Prompt Templates (YAML)

Agent prompts are versioned as YAML files in the `LegalAiAr.Agents` project.

### Location

```
backend/src/shared/LegalAiAr.Agents/Prompts/
├── system/
│   ├── normativo.yaml          # Regulatory agent system prompt
│   ├── jurisprudencial.yaml    # Case law agent system prompt
│   └── procesal.yaml           # Procedural agent system prompt
├── router/
│   └── classifier.yaml         # Agent router prompt
└── tools/
    └── query-expansion.yaml    # Query expansion prompt
```

### YAML template

The prompt body (`template:`) stays in Spanish; the YAML structure and metadata are in English.

```yaml
# {prompt-name}.yaml
# Agent: {normativo|jurisprudencial|procesal|router}
# Version: {N.M}
# Date: {YYYY-MM-DD}

name: {PromptName}
description: "{Short description of what this prompt does}"
template_format: semantic-kernel
input_variables:
  - name: query
    description: "User query"
    is_required: true
  - name: context
    description: "Context retrieved by RAG"
    is_required: false
    default_value: ""
template: |
  # Rol
  Sos un {rol} especializado en {dominio} del ordenamiento jurídico argentino.

  # Instrucciones
  {Instrucciones específicas del prompt}

  # Contexto
  {{$context}}

  # Consulta del usuario
  {{$query}}

  # Formato de respuesta
  {Descripción del formato esperado}
execution_settings:
  default:
    model_id: gpt-5o
    temperature: 0.1
    max_tokens: 4096
    top_p: 0.95
```

### Prompt conventions

- Prompt body language: Spanish (users and sources are in Spanish)
- Variables with Semantic Kernel syntax: `{{$variable}}`
- Low temperature (0.0-0.2) for factual answers
- Medium temperature (0.3-0.5) for summaries and analysis
- Always include an instruction to cite sources with a specific format
- Versioning in the YAML header — increment when modifying
- One file per prompt — do not mix multiple prompts in one file

### Standard citation instructions

Include in all agent system prompts (kept in Spanish as agent prompt content):

```yaml
template: |
  # Citación
  Toda afirmación legal DEBE estar respaldada por una fuente.
  Formato de cita: [Tipo Número/Año, Art. X] o [Tribunal, Fecha, Carátula]
  Si no encontrás fuentes para respaldar una afirmación, indicalo explícitamente.
  NUNCA inventar citas o números de norma.
```

---

## 2. AI Evaluation — golden sets and LLM-as-judge

### Location

```
backend/tests/LegalAiAr.AgentEvals/
├── GoldenSets/
│   ├── normativo/
│   │   ├── golden-set.json       # Test cases
│   │   └── rubric.yaml           # Evaluation criteria
│   ├── jurisprudencial/
│   └── procesal/
├── Judges/
│   └── evaluator.yaml            # LLM-as-judge prompt
└── Config/
    └── eval-config.json           # Evaluation configuration
```

### Golden set template

JSON keys in English; legal-domain values (questions, expected answers) may be in Spanish as they represent end-user queries and Argentine legal content.

```json
{
  "version": "1.0",
  "agent": "normativo",
  "description": "Regulatory agent evaluation cases",
  "cases": [
    {
      "id": "NORM-001",
      "category": "validity",
      "question": "¿Está vigente la ley 19.550 de sociedades comerciales?",
      "difficulty": "easy",
      "expectedAnswer": {
        "summary": "La ley 19.550 fue reemplazada por la Ley General de Sociedades 19.550 (t.o. 1984) y sigue vigente con modificaciones",
        "mustMention": ["19.550", "vigente", "modificaciones"],
        "mustNotMention": ["derogada"],
        "requiredCitations": ["Ley 19.550"],
        "expectedTools": ["search_norm", "check_validity"]
      },
      "minMetrics": {
        "relevance": 0.8,
        "citationPrecision": 0.9,
        "completeness": 0.7
      }
    }
  ]
}
```

### Rubric template

```yaml
# rubric.yaml — Evaluation criteria for the {name} agent

name: rubric-{agent}
version: "1.0"
criteria:
  - name: relevance
    weight: 0.3
    description: "The answer directly addresses the user query"
    scale:
      1: "Does not answer the question"
      3: "Partially answers"
      5: "Fully answers the question"

  - name: citation_precision
    weight: 0.3
    description: "Legal citations are correct and verifiable"
    scale:
      1: "Incorrect or fabricated citations"
      3: "Some correct citations, others missing"
      5: "All citations are correct and relevant"

  - name: completeness
    weight: 0.2
    description: "Covers all relevant aspects of the query"
    scale:
      1: "Omits fundamental aspects"
      3: "Covers the main aspects"
      5: "Exhaustive coverage"

  - name: tool_usage
    weight: 0.2
    description: "Uses the appropriate tools for the query"
    scale:
      1: "Does not use tools or uses the wrong ones"
      3: "Uses some correct tools"
      5: "Optimal tool usage"
```

### LLM-as-judge template

The evaluator prompt body stays in Spanish (it grades Spanish legal answers); YAML metadata and the output JSON keys are in English.

```yaml
# evaluator.yaml — Prompt for automatic evaluation

name: LegalAnswerEvaluator
template: |
  Evaluá la siguiente respuesta de un agente legal contra los criterios dados.

  ## Pregunta del usuario
  {{$question}}

  ## Respuesta del agente
  {{$answer}}

  ## Criterios de evaluación
  {{$rubric}}

  ## Respuesta esperada (referencia)
  {{$expectedAnswer}}

  Evaluá cada criterio con una puntuación de 1 a 5 y una justificación breve.

  Formato de respuesta (JSON):
  {
    "scores": {
      "{criterion}": { "score": N, "rationale": "..." }
    },
    "totalScore": N.N,
    "passed": true/false,
    "notes": "..."
  }
execution_settings:
  default:
    model_id: gpt-5o
    temperature: 0.0
    max_tokens: 2048
```

---

## 3. Semantic Kernel Plugin

Template to create plugins (tools) that agents can invoke.

### Location

```
backend/src/shared/LegalAiAr.Agents/
├── Plugins/
│   ├── Normativo/
│   │   ├── SearchLegalNormPlugin.cs
│   │   ├── CheckValidityPlugin.cs
│   │   └── RepealChainPlugin.cs
│   ├── Jurisprudencial/
│   └── Procesal/
├── Prompts/              # (see section 1)
└── Orchestration/
    ├── AgentRouter.cs
    └── ReActOrchestrator.cs
```

### Plugin template

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace LegalAiAr.Agents.Plugins.{Agent};

/// <summary>
/// {Plugin description: what it does and when the agent uses it.}
/// </summary>
public class {Name}Plugin(
    {IDependency} dependency,
    ILogger<{Name}Plugin> logger)
{
    [KernelFunction("{function_name}")]
    [Description("{Description in English of what this function does. " +
                 "The agent reads this description to decide when to invoke it.}")]
    public async Task<string> {MethodName}Async(
        [Description("{Parameter description}")] string parameter,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Executing {Function} with parameter: {Parameter}",
            nameof({MethodName}Async), parameter);

        // Implementation using the injected dependencies
        var result = await dependency.ExecuteAsync(parameter, cancellationToken);

        // Return as a formatted string for the LLM
        return FormatResult(result);
    }

    private static string FormatResult(object result)
    {
        // Format so the LLM can interpret it
        // Prefer structured text over raw JSON
        return $"""
            ## Search result
            {result}
            """;
    }
}
```

### Plugin registration in DI

```csharp
// In LegalAiAr.Api/Extensions/SemanticKernelExtensions.cs

public static IServiceCollection AddSemanticKernel(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddKernel()
        .AddAzureOpenAIChatCompletion(
            deploymentName: configuration["AzureOpenAI:DeploymentName"]!,
            endpoint: configuration["AzureOpenAI:Endpoint"]!,
            apiKey: configuration["AzureOpenAI:ApiKey"]!)
        .Plugins
            .AddFromType<SearchLegalNormPlugin>()
            .AddFromType<CheckValidityPlugin>();

    return services;
}
```

### Plugin conventions

- One plugin per capability (search, verify, calculate, etc.)
- `[Description]` in English and descriptive — the agent reads it to decide when to use it
- Parameters also carry `[Description]`
- Return a formatted `string` for the LLM, not complex objects
- Structured logging at the start of each function
- Inject dependencies via constructor (the kernel resolves DI)
- Handle errors with try/catch and return a descriptive message to the agent
