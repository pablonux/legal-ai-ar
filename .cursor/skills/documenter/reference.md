# Documenter — Quality Standards Reference

## .md Documents

- **Header**: Deliverable name, ID, feature, creation date
- **Purpose**: Paragraph for whom and what
- **Body**: Specific content, field mapping, strategy, specification
- **Decisions**: Each with justification
- **Assumptions**: List at end if made without user input
- **References**: Links to architecture or ADRs

## .mermaid Diagrams

**Type by content**:
- Process flows → `flowchart`
- Call sequences → `sequenceDiagram`
- States → `stateDiagram-v2`
- Data relationships → `erDiagram`
- Class relationships → `classDiagram`
- Timelines → `gantt`

**Rules**:
- File contains only Mermaid block — no extra text
- Filename matches roadmap exactly
- Validate syntax before presenting (no special chars in labels that break parser)
