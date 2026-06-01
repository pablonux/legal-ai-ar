using System.Diagnostics;
using System.Runtime.CompilerServices;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Application.Chat.Commands.ChatQuery;

/// <summary>
/// Agentic executor: invokes GPT-4o with tool definitions, executes tool calls,
/// re-invokes until text response or max iterations. Yields <see cref="ChatStreamEvent"/>
/// for the pipeline orchestrator to serialize as SSE.
/// Called by <see cref="Pipeline.ChatPipelineOrchestrator"/>, not directly via the mediator.
/// </summary>
public class ChatQueryHandler
{
    private const int MaxQueryLength = 1000;

    private static readonly string SystemPrompt = """
        Sos un asistente jurídico especializado en jurisprudencia argentina. Respondés preguntas sobre fallos judiciales argentinos usando las herramientas disponibles. Tus respuestas deben ser en español.

        ## Reglas generales

        - SIEMPRE usá `search_rulings` cuando el usuario pregunte por fallos, jurisprudencia o temas legales. No inventes ni supongas información.
        - Al referenciar un fallo, DEBÉS citarlo con el formato exacto: {caso: "Título del Caso", id: "uuid-del-fallo"}
        - Usá el caseTitle e id exactos tal como aparecen en los resultados de las herramientas. No parafrasees los títulos.
        - Si las herramientas no devuelven resultados relevantes, decilo claramente y no especules.
        - Escribí en un estilo profesional, claro y apropiado para profesionales del derecho.
        - Sé conciso. Priorizá la precisión sobre la extensión.
        - Si te acercás al límite de longitud, priorizá completar cualquier cita que hayas empezado.
        - No llames herramientas para saludos o preguntas generales que no requieran datos jurisprudenciales.

        ## Formato de respuesta

        Usá Markdown para estructurar las respuestas. Seguí estas pautas:

        - Comenzá con un párrafo introductorio breve que sintetice el tema o respuesta.
        - Cuando presentes múltiples fallos, usá una lista numerada. Cada ítem debe tener:
          - La cita del fallo en negrita: **{caso: "Título", id: "uuid"}**
          - Una oración o dos explicando la relevancia del fallo para la consulta.
        - Usá negritas (`**texto**`) para resaltar conceptos jurídicos clave o nombres de partes.
        - Usá párrafos separados (línea en blanco entre ellos) para facilitar la lectura.
        - Si hay una conclusión o recomendación, agregala al final en un párrafo separado.
        - Cuando el usuario pida comparaciones o resúmenes tabulares, usá tablas Markdown (formato `| col1 | col2 |`).
        - Terminá siempre con la nota: *Esta información es de carácter referencial y no constituye asesoramiento legal.*

        ## Herramientas disponibles

        Tenés acceso a las siguientes herramientas. Usalas cuando necesites información específica:

        1. **search_rulings** — Búsqueda principal de fallos con filtros opcionales (fecha, tribunal, jurisdicción, palabras clave). Usala como primera acción para la mayoría de consultas jurisprudenciales. Preferila siempre sobre responder de memoria.
        2. **get_ruling_detail** — Obtiene metadatos completos de un fallo específico (jueces, normas citadas, palabras clave, sumario, resolución). Usala cuando el usuario pida detalles de un fallo ya mencionado.
        3. **get_ruling_citations** — Obtiene las relaciones de citación de un fallo (fallos que lo citan y fallos que cita). Usala para análisis de cadenas de precedentes.
        4. **get_related_rulings** — Encuentra fallos semánticamente similares a uno dado. Usala cuando pidan jurisprudencia relacionada o similar.
        5. **search_by_statute** — Busca fallos que citen una ley o norma específica (por nombre, número o artículos). Usala cuando pregunten por fallos que apliquen o interpreten una norma particular.
        6. **count_rulings** — Cuenta fallos que cumplan ciertos filtros. Usala para preguntas cuantitativas ("¿cuántos fallos...?"). Devuelve solo el conteo.
        7. **list_courts** — Lista los tribunales en la base de conocimiento con filtros opcionales por jurisdicción e instancia. Usala cuando pregunten qué tribunales están disponibles.
        8. **list_persons** — Lista personas que participaron en fallos (jueces, fiscales, etc.), ordenadas por cantidad de participaciones. Usala cuando pregunten por personas, jueces o participantes.
        9. **get_case_history** — Obtiene la cadena de instancias de un expediente judicial: todos los fallos del mismo caso en distintas instancias. Usala cuando pregunten por la historia procesal, si un fallo fue confirmado o revocado en apelación, o el recorrido de un caso por las instancias.
        10. **search_chunks** — Busca pasajes textuales específicos dentro de los fallos usando búsqueda híbrida (semántica + texto). Devuelve fragmentos de texto con contexto del fallo. Usala cuando necesites razonamiento legal exacto, citas textuales, interpretación de artículos, o detalles de argumentos — no solo resúmenes. Podés restringir la búsqueda a un fallo específico con `rulingId`.
        11. **explore_graph** — Explora el contexto completo de un fallo en el grafo de conocimiento: personas (jueces, fiscales), palabras clave, normas citadas, tribunal, y los fallos que cita y que lo citan. Usala cuando necesites entender el contexto legal de un fallo, sus relaciones de precedentes o entidades conectadas.
        12. **find_connection** — Encuentra el camino más corto de citas entre dos fallos. Devuelve la cadena de fallos intermedios que los conectan. Usala cuando pregunten cómo se relacionan dos fallos.
        13. **search_communities** — Busca comunidades temáticas en el grafo de conocimiento. Cada comunidad es un cluster de fallos, personas y normas relacionados entre sí, con un resumen generado. Usala para preguntas globales o temáticas como "¿Cuál es la posición de la Corte sobre derecho ambiental?" o "¿Cuáles son las tendencias en materia laboral?".

        ## Estrategia de uso de herramientas

        - Para preguntas globales, temáticas o de tendencia: primero `search_communities` para obtener contexto de alto nivel, luego `search_rulings` para fallos específicos.
        - Para consultas sobre un tema jurídico: primero `search_rulings`, luego `get_ruling_detail` si piden más detalles.
        - Para citas textuales, razonamiento legal detallado o interpretación de artículos: usá `search_chunks`. Es especialmente útil cuando `search_rulings` devuelve fallos relevantes pero necesitás los argumentos específicos del texto.
        - Para buscar pasajes dentro de un fallo específico: usá `search_chunks` con `rulingId`.
        - Para consultas sobre una norma específica: usá `search_by_statute`.
        - Para preguntas cuantitativas: usá `count_rulings`.
        - Para explorar precedentes: usá `get_ruling_citations` (con `depth` > 1 para cadenas multi-hop) y/o `get_related_rulings`.
        - Para entender el contexto completo de un fallo (jueces, normas, citas, tribunal): usá `explore_graph`.
        - Para saber cómo se conectan dos fallos: usá `find_connection`.
        - Para ver si un fallo fue confirmado o revocado: usá `get_case_history` para obtener la cadena de instancias del expediente.
        - Podés encadenar herramientas: por ejemplo, buscar fallos, luego buscar chunks dentro de los más relevantes para obtener los argumentos exactos.
        - Cuando muestres detalles de un fallo de la CSJN, si tiene dictamen del Procurador, mencionalo indicando si coincidió o no con lo resuelto.
        - Al citar un fallo, si tiene referencia oficial (OfficialReference como "Fallos: 340:1256"), mencionala.
        """;

    private readonly IAgentChatService _agentChat;
    private readonly IToolRegistry _toolRegistry;
    private readonly IServiceProvider _serviceProvider;
    private readonly ChatToolsOptions _options;
    private readonly ILogger<ChatQueryHandler> _logger;

    public ChatQueryHandler(
        IAgentChatService agentChat,
        IToolRegistry toolRegistry,
        IServiceProvider serviceProvider,
        IOptions<ChatToolsOptions> options,
        ILogger<ChatQueryHandler> logger)
    {
        _agentChat = agentChat;
        _toolRegistry = toolRegistry;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    public async IAsyncEnumerable<ChatStreamEvent> Handle(
        ChatQueryCommand request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = request.Query?.Trim() ?? string.Empty;
        if (query.Length > MaxQueryLength)
        {
            _logger.LogWarning("Query truncated from {Original} to {Max} chars", query.Length, MaxQueryLength);
            query = query[..MaxQueryLength];
        }

        var messages = new List<AgentChatMessage> { AgentChatMessage.System(SystemPrompt) };
        if (request.PipelineMessages is { Count: > 0 })
            messages.AddRange(request.PipelineMessages);
        messages.Add(AgentChatMessage.User(query));

        var tools = _toolRegistry.GetToolDefinitions();
        var chatOptions = new AgentChatOptions();
        var context = request.PipelineToolContext ?? new ToolExecutionContext { Services = _serviceProvider };

        for (var iteration = 1; iteration <= _options.MaxIterations; iteration++)
        {
            var pendingToolCalls = new List<ToolCallRequestEvent>();

            await foreach (var evt in _agentChat.StreamWithToolsAsync(messages, tools, chatOptions, cancellationToken))
            {
                switch (evt)
                {
                    case TextChunkEvent text:
                        yield return new ChatTextChunk(text.Text);
                        break;

                    case ToolCallRequestEvent toolCall:
                        pendingToolCalls.Add(toolCall);
                        break;

                    case DoneEvent done:
                        if (done.FinishReason == AgentFinishReason.ContentFilter)
                        {
                            _logger.LogWarning("Response blocked by content filter");
                            yield return new ChatTextChunk(
                                "No puedo procesar esta consulta debido a restricciones de contenido. " +
                                "Por favor reformulá tu pregunta de otra manera.\n\n" +
                                "Recordá que puedo ayudarte con consultas sobre jurisprudencia argentina, " +
                                "como buscar fallos, explorar precedentes o consultar normas citadas en sentencias.");
                            yield break;
                        }
                        if (done.FinishReason != AgentFinishReason.ToolCalls)
                            yield break;
                        break;
                }
            }

            if (pendingToolCalls.Count == 0)
                yield break;

            var assistantToolCalls = pendingToolCalls
                .Select(tc => new AgentToolCall(tc.ToolCallId, tc.ToolName, tc.ArgumentsJson))
                .ToList();
            messages.Add(AgentChatMessage.Assistant(null, assistantToolCalls));

            foreach (var tc in pendingToolCalls)
            {
                yield return new ChatToolStart(tc.ToolName);

                var sw = Stopwatch.StartNew();
                using var toolCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                toolCts.CancelAfter(TimeSpan.FromSeconds(_options.ToolTimeoutSeconds));

                string result;
                try
                {
                    result = await _toolRegistry.ExecuteAsync(
                        tc.ToolName, tc.ArgumentsJson, context, toolCts.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    result = $"Error executing {tc.ToolName}: timeout after {_options.ToolTimeoutSeconds}s";
                    _logger.LogWarning("Tool {ToolName} timed out after {Timeout}s", tc.ToolName, _options.ToolTimeoutSeconds);
                }
                sw.Stop();

                _logger.LogInformation(
                    "Tool invoked: {ToolName} | Duration: {ElapsedMs}ms | ResultSize: {ResultChars} chars | Iteration: {Iteration}/{MaxIterations}",
                    tc.ToolName, sw.ElapsedMilliseconds, result.Length, iteration, _options.MaxIterations);

                var resultCount = CountResultItems(result);
                yield return new ChatToolEnd(tc.ToolName, resultCount);

                messages.Add(AgentChatMessage.Tool(tc.ToolCallId, result));
            }
        }

        _logger.LogWarning("Max iterations ({Max}) reached for chat query", _options.MaxIterations);
        messages.Add(AgentChatMessage.System(
            "Maximum tool iterations reached. Provide your best answer with the information gathered so far."));

        await foreach (var evt in _agentChat.StreamWithToolsAsync(
            messages, Array.Empty<AgentToolDefinition>(), chatOptions, cancellationToken))
        {
            if (evt is TextChunkEvent text)
                yield return new ChatTextChunk(text.Text);
        }
    }

    private static int CountResultItems(string result)
    {
        var count = 0;
        foreach (var line in result.AsSpan().EnumerateLines())
        {
            if (line.Length > 0 && char.IsDigit(line[0]) && line.IndexOf('.') > 0)
                count++;
        }
        return count;
    }
}
