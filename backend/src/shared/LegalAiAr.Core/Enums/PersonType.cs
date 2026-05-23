namespace LegalAiAr.Core.Enums;

/// <summary>
/// Classification of a person as a legal subject per Argentine Civil and Commercial Code.
/// </summary>
public enum PersonType
{
    /// <summary>Persona humana (Art. 19 CCyC).</summary>
    Physical,
    /// <summary>Persona jurídica de derecho público (Art. 146 CCyC): Estado, provincias, municipios, entes autárquicos.</summary>
    LegalPublic,
    /// <summary>Persona jurídica de derecho privado (Art. 148 CCyC): sociedades, asociaciones, fundaciones.</summary>
    LegalPrivate,
    /// <summary>Entidad estatal no personalizada (ministerios, secretarías, organismos descentralizados).</summary>
    StateEntity,
    /// <summary>Tipo no determinable a partir de los datos disponibles.</summary>
    Indeterminate
}
