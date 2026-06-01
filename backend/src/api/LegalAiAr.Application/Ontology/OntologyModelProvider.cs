using LegalAiAr.Application.Ontology.Models;

namespace LegalAiAr.Application.Ontology;

/// <summary>
/// Builds the legal ontology model in memory from the formal definition
/// in docs/ontology/legal-ai-ar-ontology.md. Registered as singleton.
/// </summary>
public sealed class OntologyModelProvider
{
    private readonly Lazy<IReadOnlyList<OntologyClassDto>> _classes;
    private readonly Lazy<(IReadOnlyList<GraphNodeDto> Nodes, IReadOnlyList<GraphEdgeDto> Edges)> _graph;
    private readonly Lazy<IReadOnlyDictionary<string, TaxonomyDefinition>> _taxonomies;

    public OntologyModelProvider()
    {
        _classes = new Lazy<IReadOnlyList<OntologyClassDto>>(BuildClasses);
        _graph = new Lazy<(IReadOnlyList<GraphNodeDto>, IReadOnlyList<GraphEdgeDto>)>(BuildGraph);
        _taxonomies = new Lazy<IReadOnlyDictionary<string, TaxonomyDefinition>>(BuildTaxonomies);
    }

    public IReadOnlyList<OntologyClassDto> GetClasses() => _classes.Value;

    public (IReadOnlyList<GraphNodeDto> Nodes, IReadOnlyList<GraphEdgeDto> Edges) GetGraph() => _graph.Value;

    public IReadOnlyDictionary<string, TaxonomyDefinition> GetTaxonomies() => _taxonomies.Value;

    private static IReadOnlyList<OntologyClassDto> BuildClasses()
    {
        return
        [
            // Core classes (top-level)
            Cls("NormaJuridica", "Norma Jurídica",
                "Norma general, abstracta y coercitiva que integra el ordenamiento jurídico.",
                null, "core", "Statute", "/buscar/resultados",
                [
                    Prop("identificador", "string", "Número o código oficial"),
                    Prop("denominacion", "string", "Nombre usual"),
                    Prop("tipo", "NormType", "Tipo de norma", "NormType"),
                    Prop("jerarquia", "NormativeLevel", "Nivel en la pirámide normativa", "NormativeLevel"),
                    Prop("ramaDelDerecho", "LegalBranch", "Rama del derecho", "LegalBranch"),
                    Prop("fechaVigenciaDesde", "date", "Inicio de vigencia"),
                    Prop("fechaVigenciaHasta", "date?", "Fin de vigencia"),
                    Prop("organoEmisor", "OrganoEstatal", "Quién la dictó"),
                ],
                ["Constitucion", "Tratado", "Ley", "Decreto", "Resolucion", "Ordenanza", "Acordada"]),

            Cls("SujetoDeDerecho", "Sujeto de Derecho",
                "Toda entidad susceptible de adquirir derechos y contraer obligaciones (art. 22 CCyCN).",
                null, "core", null, null,
                [
                    Prop("nombre", "string", "Nombre completo o razón social"),
                    Prop("tipo", "SubjectType", "Tipo de sujeto"),
                    Prop("domicilio", "string?", "Domicilio legal"),
                ],
                ["PersonaHumana", "PersonaJuridica"]),

            Cls("OrganoEstatal", "Órgano Estatal",
                "Entidad del Estado con competencia asignada por la Constitución o la ley.",
                null, "core", null, null,
                [
                    Prop("nombre", "string", "Nombre oficial"),
                    Prop("tipo", "OrganType", "Tipo de órgano"),
                    Prop("nivel", "GovernmentLevel", "Nivel de gobierno", "GovernmentLevel"),
                ],
                ["OrganoJudicial", "OrganoLegislativo", "OrganoEjecutivo", "OrganoDescentralizado"]),

            Cls("HechoJuridico", "Hecho Jurídico",
                "Acontecimiento que produce efectos jurídicos (arts. 257-258 CCyCN).",
                null, "core", null, null,
                [
                    Prop("descripcion", "string", "Descripción del hecho"),
                    Prop("fecha", "date", "Fecha de ocurrencia"),
                    Prop("esVoluntario", "boolean", "Si interviene la voluntad humana"),
                ],
                ["HechoInvoluntario", "HechoVoluntario"]),

            Cls("ProcesoJudicial", "Proceso Judicial",
                "Secuencia ordenada de actos procesales ante un órgano jurisdiccional. Agrupa sentencias del mismo expediente.",
                null, "kb-entity", "JudicialProceeding", null,
                [
                    Prop("expediente", "string", "Número de expediente"),
                    Prop("jurisdiccion", "string", "Área jurisdiccional"),
                    Prop("cantidadFallos", "int", "Cantidad de fallos vinculados"),
                ],
                ["ProcesoCivil", "ProcesoPenal", "ProcesoLaboral", "ProcesoContAdm", "ProcesoFamilia", "ProcesoConstitucional"]),

            Cls("FuenteDelDerecho", "Fuente del Derecho",
                "Las fuentes del derecho argentino, formalizadas como jerarquía ontológica.",
                null, "core", null, null,
                [],
                ["FuenteFormal", "FuenteMaterial"]),

            Cls("Jurisdiccion", "Jurisdicción",
                "Modelo formal de la estructura jurisdiccional argentina.",
                null, "core", null, null,
                [],
                ["Federal", "Provincial", "CABA", "Municipal"]),

            // Sentencia — KB entity (Ruling)
            Cls("Sentencia", "Sentencia",
                "Resolución judicial que pone fin al proceso en la instancia. Entidad central de la KB.",
                null, "kb-entity", "Ruling", "/buscar",
                [
                    Prop("caratula", "string", "Título del caso"),
                    Prop("fecha", "date", "Fecha de la sentencia"),
                    Prop("ramaDelDerecho", "LegalBranch", "Rama del derecho", "LegalBranch"),
                    Prop("pesoPrecedencial", "PrecedentWeight", "Peso como precedente", "PrecedentWeight"),
                    Prop("esPlenario", "boolean", "Si es fallo plenario"),
                    Prop("esLeadingCase", "boolean", "Si es caso líder"),
                ],
                []),

            // NormaJuridica subclasses
            Cls("Constitucion", "Constitución", "Constitución Nacional y constituciones provinciales.", "NormaJuridica", "subclass", null, null, [], []),
            Cls("Tratado", "Tratado", "Tratados internacionales.", "NormaJuridica", "subclass", null, null, [], []),
            Cls("Ley", "Ley", "Leyes nacionales y provinciales.", "NormaJuridica", "subclass", null, null, [], []),
            Cls("Decreto", "Decreto", "Decretos del Poder Ejecutivo.", "NormaJuridica", "subclass", null, null, [], []),
            Cls("Resolucion", "Resolución", "Resoluciones ministeriales y de organismos.", "NormaJuridica", "subclass", null, null, [], []),
            Cls("Ordenanza", "Ordenanza", "Normas municipales.", "NormaJuridica", "subclass", null, null, [], []),
            Cls("Acordada", "Acordada", "Resoluciones de órganos judiciales sobre su funcionamiento.", "NormaJuridica", "subclass", null, null, [], []),

            // SujetoDeDerecho subclasses
            Cls("PersonaHumana", "Persona Humana", "Persona física (art. 19 CCyCN). Entidad KB que agrupa todas las personas que participan en sentencias.", "SujetoDeDerecho", "kb-entity", "Person", "/catalogos/personas",
                [Prop("totalFallos", "int", "Cantidad de fallos indexados")], ["Juez", "Fiscal", "Defensor", "Abogado"]),
            Cls("PersonaJuridica", "Persona Jurídica", "Entidad con personalidad jurídica (art. 141 CCyCN).", "SujetoDeDerecho", "subclass", null, null, [], ["PJPublica", "PJPrivada"]),
            Cls("Juez", "Juez", "Persona que ejerce la función jurisdiccional.", "PersonaHumana", "subclass", null, null, [], []),
            Cls("Fiscal", "Fiscal", "Miembro del Ministerio Público Fiscal.", "PersonaHumana", "subclass", null, null, [], []),
            Cls("Defensor", "Defensor", "Miembro del Ministerio Público de la Defensa.", "PersonaHumana", "subclass", null, null, [], []),
            Cls("Abogado", "Abogado", "Profesional del derecho matriculado.", "PersonaHumana", "subclass", null, null, [], []),
            Cls("PJPublica", "Persona Jurídica Pública", "Entidades públicas estatales.", "PersonaJuridica", "subclass", null, null, [], []),
            Cls("PJPrivada", "Persona Jurídica Privada", "SA, SRL, SAS, asociaciones, fundaciones.", "PersonaJuridica", "subclass", null, null, [], []),

            // OrganoEstatal subclasses
            Cls("OrganoJudicial", "Órgano Judicial", "Órgano del Poder Judicial.", "OrganoEstatal", "subclass", null, null, [], ["Tribunal", "MinisterioPublico"]),
            Cls("OrganoLegislativo", "Órgano Legislativo", "Órgano del Poder Legislativo.", "OrganoEstatal", "subclass", null, null, [], []),
            Cls("OrganoEjecutivo", "Órgano Ejecutivo", "Órgano del Poder Ejecutivo.", "OrganoEstatal", "subclass", null, null, [], []),
            Cls("OrganoDescentralizado", "Órgano Descentralizado", "Organismos descentralizados y autárquicos.", "OrganoEstatal", "subclass", null, null, [], []),
            Cls("Tribunal", "Tribunal", "Tribunal que dicta sentencias.", "OrganoJudicial", "kb-entity", "Court", "/catalogos/tribunales",
                [
                    Prop("fuero", "Fuero", "Fuero del tribunal", "Fuero"),
                    Prop("tipoTribunal", "CourtType", "Tipo de tribunal", "CourtType"),
                    Prop("instancia", "int", "Nivel de instancia"),
                ], []),
            Cls("MinisterioPublico", "Ministerio Público", "Fiscal y defensa pública.", "OrganoJudicial", "subclass", null, null, [], []),

            // HechoJuridico subclasses
            Cls("HechoInvoluntario", "Hecho Involuntario", "Hecho sin intervención de la voluntad.", "HechoJuridico", "subclass", null, null, [], []),
            Cls("HechoVoluntario", "Hecho Voluntario", "Hecho con intervención de la voluntad.", "HechoJuridico", "subclass", null, null, [], ["ActoLicito", "ActoIlicito"]),
            Cls("ActoLicito", "Acto Lícito", "Hecho voluntario lícito.", "HechoVoluntario", "subclass", null, null, [], ["ActoJuridico"]),
            Cls("ActoIlicito", "Acto Ilícito", "Hecho voluntario ilícito.", "HechoVoluntario", "subclass", null, null, [], ["Delito", "Cuasidelito"]),
            Cls("ActoJuridico", "Acto Jurídico", "Acto voluntario lícito con fin jurídico inmediato (art. 259 CCyCN).", "ActoLicito", "subclass", null, null, [],
                ["Contrato", "ActoAdministrativo", "ActoProcesal", "ActoDeFamilia", "Testamento"]),
            Cls("Delito", "Delito", "Acto ilícito tipificado penalmente.", "ActoIlicito", "subclass", null, null, [], []),
            Cls("Cuasidelito", "Cuasidelito", "Acto ilícito culposo.", "ActoIlicito", "subclass", null, null, [], []),
            Cls("Contrato", "Contrato", "Acuerdo de voluntades que crea obligaciones.", "ActoJuridico", "subclass", null, null, [], []),
            Cls("ActoAdministrativo", "Acto Administrativo", "Declaración unilateral de un órgano estatal.", "ActoJuridico", "subclass", null, null, [], []),
            Cls("ActoProcesal", "Acto Procesal", "Acto producido dentro de un proceso judicial.", "ActoJuridico", "subclass", null, null, [], []),
            Cls("ActoDeFamilia", "Acto de Familia", "Actos jurídicos del derecho de familia.", "ActoJuridico", "subclass", null, null, [], []),
            Cls("Testamento", "Testamento", "Disposición de última voluntad.", "ActoJuridico", "subclass", null, null, [], []),

            // ProcesoJudicial subclasses
            Cls("ProcesoCivil", "Proceso Civil", "Proceso regido por CPCCN.", "ProcesoJudicial", "subclass", null, null, [], []),
            Cls("ProcesoPenal", "Proceso Penal", "Proceso regido por CPPF.", "ProcesoJudicial", "subclass", null, null, [], []),
            Cls("ProcesoLaboral", "Proceso Laboral", "Proceso ante la Justicia del Trabajo.", "ProcesoJudicial", "subclass", null, null, [], []),
            Cls("ProcesoContAdm", "Proceso Contencioso Administrativo", "Proceso contra el Estado.", "ProcesoJudicial", "subclass", null, null, [], []),
            Cls("ProcesoFamilia", "Proceso de Familia", "Procesos de derecho de familia.", "ProcesoJudicial", "subclass", null, null, [], []),
            Cls("ProcesoConstitucional", "Proceso Constitucional", "Amparo, habeas corpus, habeas data.", "ProcesoJudicial", "subclass", null, null, [], []),

            // FuenteDelDerecho subclasses
            Cls("FuenteFormal", "Fuente Formal", "Normas jurídicas positivas.", "FuenteDelDerecho", "subclass", null, null, [], []),
            Cls("FuenteMaterial", "Fuente Material", "Jurisprudencia, doctrina, costumbre.", "FuenteDelDerecho", "subclass", null, null, [], ["Jurisprudencia", "Doctrina", "CostumbreJuridica"]),
            Cls("Jurisprudencia", "Jurisprudencia", "Conjunto de fallos como fuente del derecho.", "FuenteMaterial", "subclass", null, null, [], []),
            Cls("Doctrina", "Doctrina", "Opinión de juristas y académicos.", "FuenteMaterial", "subclass", null, null, [], []),
            Cls("CostumbreJuridica", "Costumbre Jurídica", "Práctica reiterada con convicción de obligatoriedad.", "FuenteMaterial", "subclass", null, null, [], []),

            // Jurisdiccion subclasses
            Cls("Federal", "Federal", "Jurisdicción federal.", "Jurisdiccion", "subclass", null, null, [], []),
            Cls("Provincial", "Provincial", "Jurisdicción provincial.", "Jurisdiccion", "subclass", null, null, [], []),
            Cls("CABA", "CABA", "Ciudad Autónoma de Buenos Aires.", "Jurisdiccion", "subclass", null, null, [], []),
            Cls("Municipal", "Municipal", "Jurisdicción municipal.", "Jurisdiccion", "subclass", null, null, [], []),

            // ThesaurusTerm — KB entity
            Cls("ThesaurusTerm", "Descriptor Temático",
                "Término del tesauro jurídico SAIJ para clasificación temática.",
                null, "kb-entity", "ThesaurusTerm", "/catalogos/tesauro",
                [
                    Prop("rama", "string", "Rama temática de nivel superior"),
                    Prop("profundidad", "int", "Nivel en el árbol jerárquico"),
                ], []),

            // PalabraClave — KB entity (Keyword)
            Cls("PalabraClave", "Palabra Clave",
                "Descriptor temático asignado a sentencias. Puede estar normalizado contra el tesauro SAIJ.",
                null, "kb-entity", "Keyword", "/buscar/resultados",
                [
                    Prop("descripcion", "string", "Texto de la palabra clave"),
                ], []),

            // Fuente — KB entity (Source)
            Cls("Fuente", "Fuente de Datos",
                "Origen de los documentos indexados en la KB (ej. CSJN, SAIJ).",
                null, "kb-entity", "Source", null,
                [
                    Prop("nombre", "string", "Nombre de la fuente"),
                    Prop("estrategia", "string", "Estrategia de crawling"),
                ], []),
        ];
    }

    private static (IReadOnlyList<GraphNodeDto>, IReadOnlyList<GraphEdgeDto>) BuildGraph()
    {
        var classes = BuildClasses();
        var classLookup = classes.ToDictionary(c => c.Id);

        var nodes = classes.Select(c => new GraphNodeDto(c.Id, c.Name, c.Category, 0, c.KbRoute)).ToList();

        var edges = new List<GraphEdgeDto>();
        foreach (var cls in classes)
        {
            if (cls.ParentId is not null)
            {
                edges.Add(new GraphEdgeDto($"e-{cls.ParentId}-{cls.Id}", cls.ParentId, cls.Id, "is-a", "is-a"));
            }
        }

        // Domain relationships from the ontology graph (section 4)
        AddRelationship(edges, "NormaJuridica", "NormaJuridica", "deroga / modifica");
        AddRelationship(edges, "NormaJuridica", "SujetoDeDerecho", "regula");
        AddRelationship(edges, "NormaJuridica", "OrganoEstatal", "faculta");
        AddRelationship(edges, "NormaJuridica", "ActoJuridico", "valida");
        AddRelationship(edges, "SujetoDeDerecho", "ActoJuridico", "celebra");
        AddRelationship(edges, "SujetoDeDerecho", "HechoJuridico", "produce");
        AddRelationship(edges, "SujetoDeDerecho", "ProcesoJudicial", "participaEn");
        AddRelationship(edges, "OrganoEstatal", "NormaJuridica", "emite");
        AddRelationship(edges, "OrganoEstatal", "OrganoEstatal", "superiorJerárquico");
        AddRelationship(edges, "HechoJuridico", "ProcesoJudicial", "activa");
        AddRelationship(edges, "ProcesoJudicial", "Sentencia", "conduceA");
        AddRelationship(edges, "ProcesoJudicial", "NormaJuridica", "aplicaNorma");
        AddRelationship(edges, "Sentencia", "Sentencia", "citaFallo");
        AddRelationship(edges, "Sentencia", "NormaJuridica", "citaNorma");
        AddRelationship(edges, "Sentencia", "Tribunal", "emitidoPor");
        AddRelationship(edges, "Sentencia", "PersonaHumana", "firmadoPor");
        AddRelationship(edges, "Sentencia", "ThesaurusTerm", "tieneDescriptor");
        AddRelationship(edges, "Sentencia", "PalabraClave", "tienePalabraClave");
        AddRelationship(edges, "Sentencia", "Fuente", "provenienteDe");
        AddRelationship(edges, "PalabraClave", "ThesaurusTerm", "normalizadoPor");
        AddRelationship(edges, "Sentencia", "Fiscal", "dictaminadoPor");

        return (nodes, edges);
    }

    private static void AddRelationship(List<GraphEdgeDto> edges, string source, string target, string label)
    {
        edges.Add(new GraphEdgeDto(
            $"r-{source}-{label.Replace(" ", "").Replace("/", "")}-{target}",
            source, target, "relationship", label));
    }

    private static IReadOnlyDictionary<string, TaxonomyDefinition> BuildTaxonomies()
    {
        return new Dictionary<string, TaxonomyDefinition>
        {
            ["LegalBranch"] = new("LegalBranch", "Rama del derecho",
                "Ramas del derecho argentino organizadas en público, privado, social y digital.",
                [
                    TVal("PUB_CONST", "Derecho constitucional", "Derecho público", "CN 1994 — derechos y garantías, organización del Estado"),
                    TVal("PUB_ADMIN", "Derecho administrativo", "Derecho público", "Ley 19.549, Ley 26.944"),
                    TVal("PUB_PENAL", "Derecho penal", "Derecho público", "Código Penal (Ley 11.179)"),
                    TVal("PUB_PROC_CIV", "Derecho procesal civil", "Derecho público", "CPCCN (Ley 17.454)"),
                    TVal("PUB_PROC_PEN", "Derecho procesal penal", "Derecho público", "Cód. Proc. Penal Federal (Ley 27.482)"),
                    TVal("PUB_TRIB", "Derecho tributario", "Derecho público", "Ley 11.683, Ley 20.628, Ley 23.349"),
                    TVal("PUB_INT", "Derecho internacional público", "Derecho público", "Convenciones de Viena, Carta ONU"),
                    TVal("PRIV_CIVIL", "Derecho civil", "Derecho privado", "CCyCN (Ley 26.994)"),
                    TVal("PRIV_COM", "Derecho comercial", "Derecho privado", "CCyCN + Ley 19.550 + Ley 24.522"),
                    TVal("PRIV_LAB", "Derecho laboral individual", "Derecho privado", "Ley 20.744 (LCT)"),
                    TVal("PRIV_LAB_COL", "Derecho colectivo del trabajo", "Derecho privado", "Ley 23.551, Ley 14.250"),
                    TVal("PRIV_SEG", "Derecho de seguros", "Derecho privado", "Ley 17.418"),
                    TVal("PRIV_PI", "Derecho autoral e intelectual", "Derecho privado", "Ley 11.723, Ley 24.481"),
                    TVal("SOC_FAM", "Derecho de familia", "Derecho social", "CCyCN arts. 401-723"),
                    TVal("SOC_PREV", "Derecho previsional", "Derecho social", "Ley 24.241, Ley 26.425"),
                    TVal("SOC_NINEZ", "Derecho de la niñez", "Derecho social", "Ley 26.061, CDN"),
                    TVal("SOC_AMB", "Derecho ambiental", "Derecho social", "Ley 25.675, art. 41 CN"),
                    TVal("SOC_CONS", "Derecho del consumidor", "Derecho social", "Ley 24.240, art. 42 CN"),
                    TVal("DIG_DATOS", "Protección de datos personales", "Derecho digital", "Ley 25.326"),
                    TVal("DIG_CYBER", "Delitos informáticos", "Derecho digital", "Ley 26.388"),
                    TVal("DIG_FIRMA", "Firma digital/electrónica", "Derecho digital", "Ley 25.506"),
                ]),
            ["NormType"] = new("NormType", "Tipo de norma",
                "Tipos de norma jurídica en el sistema legal argentino.",
                [
                    TVal("CONSTITUTION", "Constitución", null, "Constitución Nacional y provinciales"),
                    TVal("TREATY", "Tratado", null, "Tratados internacionales"),
                    TVal("LAW", "Ley", null, "Leyes nacionales y provinciales"),
                    TVal("DECREE", "Decreto", null, "Decretos del Poder Ejecutivo"),
                    TVal("DNU", "Decreto de Necesidad y Urgencia", null, "Decretos art. 99 inc. 3 CN"),
                    TVal("RESOLUTION", "Resolución", null, "Resoluciones ministeriales y de organismos"),
                    TVal("ACORDADA", "Acordada", null, "Resoluciones de órganos judiciales"),
                    TVal("ORDINANCE", "Ordenanza", null, "Normas municipales"),
                ]),
            ["NormativeLevel"] = new("NormativeLevel", "Nivel normativo",
                "Nivel en la pirámide normativa argentina (jerarquía de Kelsen).",
                [
                    TVal("CONSTITUTIONAL", "Constitucional", null, "Constitución + Tratados DDHH art. 75 inc. 22"),
                    TVal("SUPRALEGAL", "Supralegal", null, "Tratados sin jerarquía constitucional"),
                    TVal("LEGAL", "Legal", null, "Leyes nacionales y provinciales"),
                    TVal("REGULATORY", "Reglamentario", null, "Decretos, resoluciones, ordenanzas"),
                    TVal("INDIVIDUAL", "Individual", null, "Sentencias, contratos, actos administrativos"),
                ]),
            ["CourtType"] = new("CourtType", "Tipo de tribunal",
                "Tipos de tribunal en el sistema judicial argentino.",
                [
                    TVal("CSJN", "Corte Suprema de Justicia de la Nación", null, "Tercera instancia, originaria y apelada"),
                    TVal("CAM_NAC", "Cámara Nacional de Apelaciones", null, "Segunda instancia nacional"),
                    TVal("CAM_FED", "Cámara Federal de Apelaciones", null, "Segunda instancia federal"),
                    TVal("CAM_CAS", "Cámara Federal de Casación Penal", null, "Casación penal federal"),
                    TVal("TOF", "Tribunal Oral Federal", null, "Juicio oral penal federal"),
                    TVal("JUZ_NAC", "Juzgado Nacional de 1ª instancia", null, "Primera instancia nacional"),
                    TVal("JUZ_FED", "Juzgado Federal de 1ª instancia", null, "Primera instancia federal"),
                    TVal("SC_PROV", "Suprema Corte / Superior Tribunal Provincial", null, "Máxima instancia provincial"),
                    TVal("CAM_PROV", "Cámara Provincial", null, "Segunda instancia provincial"),
                    TVal("JUZ_PROV", "Juzgado Provincial", null, "Primera instancia provincial"),
                    TVal("TSJ_CABA", "Tribunal Superior de Justicia CABA", null, "Máxima instancia CABA"),
                ]),
            ["PrecedentWeight"] = new("PrecedentWeight", "Peso precedencial",
                "Peso del fallo como precedente para tribunales inferiores.",
                [
                    TVal("BINDING", "Vinculante", null, "Obligatorio para inferiores (plenarios, CSJN consolidada)"),
                    TVal("HIGHLY_PERSUASIVE", "Altamente persuasivo", null, "Fallo CSJN unánime reciente"),
                    TVal("PERSUASIVE", "Persuasivo", null, "Fallo de Cámara en tema sin doctrina CSJN"),
                    TVal("REFERENTIAL", "Referencial", null, "Fallo de 1ª instancia, votos en disidencia"),
                ]),
            ["Fuero"] = new("Fuero", "Fuero",
                "Competencia por materia de los tribunales argentinos.",
                [
                    TVal("CIVIL", "Civil", null, null),
                    TVal("COMMERCIAL", "Comercial", null, null),
                    TVal("CRIMINAL", "Penal", null, null),
                    TVal("LABOR", "Laboral", null, null),
                    TVal("ADMINISTRATIVE", "Contencioso Administrativo", null, null),
                    TVal("FAMILY", "Familia", null, null),
                    TVal("SOCIAL_SECURITY", "Seguridad Social", null, null),
                    TVal("FEDERAL", "Federal", null, null),
                    TVal("ELECTORAL", "Electoral", null, null),
                ]),
            ["GovernmentLevel"] = new("GovernmentLevel", "Nivel de gobierno",
                "Niveles de gobierno en el sistema federal argentino.",
                [
                    TVal("NATIONAL", "Nacional", null, null),
                    TVal("PROVINCIAL", "Provincial", null, null),
                    TVal("MUNICIPAL", "Municipal", null, null),
                    TVal("CABA", "CABA", null, "Ciudad Autónoma de Buenos Aires"),
                ]),
        };
    }

    private static OntologyClassDto Cls(string id, string name, string desc, string? parentId,
        string category, string? kbEntity, string? kbRoute,
        OntologyPropertyDto[] props, string[] children)
        => new(id, name, desc, $"legar:{id}", parentId, category, kbEntity, kbRoute, props, children);

    private static OntologyPropertyDto Prop(string name, string type, string desc, string? taxonomyId = null)
        => new(name, type, desc, taxonomyId);

    private static TaxonomyValueDefinition TVal(string code, string label, string? group, string? desc)
        => new(code, label, group, desc);
}

public record TaxonomyDefinition(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<TaxonomyValueDefinition> Values);

public record TaxonomyValueDefinition(
    string Code,
    string Label,
    string? Group,
    string? Description);
