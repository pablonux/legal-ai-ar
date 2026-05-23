#!/usr/bin/env python3
"""
Genera exportación para Microsoft Visio de la ontología Legal AI AR v2.1.

Salidas:
  - legal-ai-ar-ontology-v2.1.visio.vdx  (abrir directamente en Visio)
  - legal-ai-ar-ontology-v2.1.svg          (importar en Visio: Archivo > Abrir)

Uso:
  python docs/ontology/diagrams/generate_visio_export.py
"""

from __future__ import annotations

import math
import xml.etree.ElementTree as ET
from collections import defaultdict
from dataclasses import dataclass
from pathlib import Path

NS = "http://schemas.microsoft.com/visio/2003/core"
ET.register_namespace("", NS)

OUTPUT_DIR = Path(__file__).parent

# Página apaisada para jerarquía (pulgadas)
PAGE_LANDSCAPE_W = 17.0
PAGE_LANDSCAPE_H = 11.0
PAGE_PORTRAIT_W = 11.0
PAGE_PORTRAIT_H = 8.5

ROW_H = 0.62
BOX_W = 1.85
BOX_H = 0.5
COL_GAP = 2.05


@dataclass(frozen=True)
class Node:
    id: str
    label: str
    parent: str | None = None
    style: str = "subclass"
    kb_entity: str | None = None


@dataclass(frozen=True)
class Edge:
    source: str
    target: str
    label: str = ""
    edge_type: str = "is-a"


HIERARCHY: list[Node] = [
    Node("Thing", "owl:Thing", None, "core"),
    Node("NormaJuridica", "NormaJurídica", "Thing", "core", "Statute"),
    Node("Constitucion", "Constitución", "NormaJuridica"),
    Node("Tratado", "Tratado", "NormaJuridica"),
    Node("Ley", "Ley", "NormaJuridica"),
    Node("Decreto", "Decreto", "NormaJuridica"),
    Node("Resolucion", "Resolución", "NormaJuridica"),
    Node("Ordenanza", "Ordenanza", "NormaJuridica"),
    Node("Acordada", "Acordada", "NormaJuridica"),
    Node("SujetoDeDerecho", "Sujeto de Derecho", "Thing", "core"),
    Node("PersonaHumana", "Persona Humana", "SujetoDeDerecho", "kb", "Person"),
    Node("PersonaJuridica", "Persona Jurídica", "SujetoDeDerecho"),
    Node("PJPublica", "PJ Pública", "PersonaJuridica"),
    Node("PJPrivada", "PJ Privada", "PersonaJuridica"),
    Node("Juez", "Juez (rol)", "PersonaHumana", "role"),
    Node("Fiscal", "Fiscal (rol)", "PersonaHumana", "role"),
    Node("Defensor", "Defensor (rol)", "PersonaHumana", "role"),
    Node("Abogado", "Abogado (rol)", "PersonaHumana", "role"),
    Node("OrganoEstatal", "Órgano Estatal", "Thing", "core"),
    Node("OrganoJudicial", "Órgano Judicial", "OrganoEstatal"),
    Node("OrganoLegislativo", "Órgano Legislativo", "OrganoEstatal"),
    Node("OrganoEjecutivo", "Órgano Ejecutivo", "OrganoEstatal"),
    Node("OrganoDescentralizado", "Órgano Descentralizado", "OrganoEstatal"),
    Node("Tribunal", "Tribunal", "OrganoJudicial", "kb", "Court"),
    Node("MinisterioPublico", "Ministerio Público", "OrganoJudicial"),
    Node("HechoJuridico", "Hecho Jurídico", "Thing", "conceptual"),
    Node("HechoInvoluntario", "Hecho Involuntario", "HechoJuridico"),
    Node("HechoVoluntario", "Hecho Voluntario", "HechoJuridico"),
    Node("ActoLicito", "Acto Lícito", "HechoVoluntario"),
    Node("ActoIlicito", "Acto Ilícito", "HechoVoluntario"),
    Node("ActoJuridico", "Acto Jurídico", "ActoLicito"),
    Node("Delito", "Delito", "ActoIlicito"),
    Node("Cuasidelito", "Cuasidelito", "ActoIlicito"),
    Node("Contrato", "Contrato", "ActoJuridico"),
    Node("ProcesoJudicial", "Proceso Judicial", "Thing", "kb", "JudicialProceeding"),
    Node("ProcesoCivil", "Proceso Civil", "ProcesoJudicial"),
    Node("ProcesoPenal", "Proceso Penal", "ProcesoJudicial"),
    Node("ProcesoLaboral", "Proceso Laboral", "ProcesoJudicial"),
    Node("FuenteDelDerecho", "Fuente del Derecho", "Thing", "conceptual"),
    Node("FuenteFormal", "Fuente Formal", "FuenteDelDerecho"),
    Node("FuenteMaterial", "Fuente Material", "FuenteDelDerecho"),
    Node("Jurisprudencia", "Jurisprudencia", "FuenteMaterial"),
    Node("Doctrina", "Doctrina", "FuenteMaterial"),
    Node("Jurisdiccion", "Jurisdicción", "Thing", "core"),
    Node("Federal", "Federal", "Jurisdiccion"),
    Node("Provincial", "Provincial", "Jurisdiccion"),
    Node("CABA", "CABA", "Jurisdiccion"),
    Node("Sentencia", "Sentencia", None, "kb", "Ruling"),
    Node("ThesaurusTerm", "ThesaurusTerm (skos)", None, "kb", "ThesaurusTerm"),
    Node("PalabraClave", "Palabra Clave", None, "kb", "Keyword"),
    Node("Fuente", "Fuente de Datos", None, "kb", "Source"),
]

RELATIONSHIPS: list[Edge] = [
    Edge("NormaJuridica", "NormaJuridica", "deroga / modifica"),
    Edge("NormaJuridica", "SujetoDeDerecho", "regula"),
    Edge("NormaJuridica", "OrganoEstatal", "faculta"),
    Edge("NormaJuridica", "ActoJuridico", "valida"),
    Edge("SujetoDeDerecho", "ActoJuridico", "celebra"),
    Edge("SujetoDeDerecho", "HechoJuridico", "produce"),
    Edge("SujetoDeDerecho", "ProcesoJudicial", "participaEn"),
    Edge("OrganoEstatal", "NormaJuridica", "emite"),
    Edge("OrganoEstatal", "OrganoEstatal", "superiorJerárquico"),
    Edge("HechoJuridico", "ProcesoJudicial", "activa"),
    Edge("ProcesoJudicial", "Sentencia", "conduceA"),
    Edge("ProcesoJudicial", "NormaJuridica", "aplicaNorma"),
    Edge("Sentencia", "Sentencia", "citaFallo"),
    Edge("Sentencia", "NormaJuridica", "citaNorma"),
    Edge("Sentencia", "Tribunal", "emitidoPor"),
    Edge("Sentencia", "PersonaHumana", "firmadoPor"),
    Edge("Sentencia", "ThesaurusTerm", "tieneDescriptor"),
    Edge("Sentencia", "PalabraClave", "tienePalabraClave"),
    Edge("Sentencia", "Fuente", "provenienteDe"),
    Edge("PalabraClave", "ThesaurusTerm", "normalizadoPor"),
]

STYLE_COLORS = {
    "core": "#1E3A5F",
    "kb": "#D04A02",
    "subclass": "#4A7CB5",
    "role": "#EB8C00",
    "conceptual": "#9E9E9E",
}

THING_CHILDREN = [
    "NormaJuridica",
    "SujetoDeDerecho",
    "OrganoEstatal",
    "HechoJuridico",
    "ProcesoJudicial",
    "FuenteDelDerecho",
    "Jurisdiccion",
]

KB_ORPHANS = ["Sentencia", "ThesaurusTerm", "PalabraClave", "Fuente"]


def build_children_map(nodes: list[Node]) -> dict[str, list[str]]:
    children: dict[str, list[str]] = defaultdict(list)
    for n in nodes:
        if n.parent:
            children[n.parent].append(n.id)
    return children


def layout_hierarchy_columns(nodes: list[Node]) -> dict[str, tuple[float, float]]:
    """
    Layout por columnas: cada hijo directo de Thing es una columna;
    descendientes apilados verticalmente. Evita compresión vertical.
    """
    lookup = {n.id: n for n in nodes}
    children = build_children_map(nodes)
    positions: dict[str, tuple[float, float]] = {}

    # Thing centrado arriba
    positions["Thing"] = (PAGE_LANDSCAPE_W / 2, PAGE_LANDSCAPE_H - 0.55)

    start_x = 0.9
    for i, root_child in enumerate(THING_CHILDREN):
        if root_child not in lookup:
            continue
        col_x = start_x + i * COL_GAP

        def stack_subtree(node_id: str, y: float) -> float:
            positions[node_id] = (col_x, y)
            y_next = y - ROW_H
            for child_id in children.get(node_id, []):
                if child_id in lookup:
                    y_next = stack_subtree(child_id, y_next)
            return y_next

        stack_subtree(root_child, PAGE_LANDSCAPE_H - 1.35)

    # Entidades KB sin padre — fila inferior
    orphan_y = 0.85
    ox = 1.0
    for oid in KB_ORPHANS:
        if oid in lookup:
            positions[oid] = (ox, orphan_y)
            ox += COL_GAP

    return positions


def layout_relationships(
    edges: list[Edge], node_ids: set[str]
) -> dict[str, tuple[float, float]]:
    ids = sorted(node_ids)
    n = len(ids)
    cx, cy, r = PAGE_PORTRAIT_W / 2, PAGE_PORTRAIT_H / 2, min(PAGE_PORTRAIT_W, PAGE_PORTRAIT_H) / 2 - 1.2
    return {
        node_id: (
            cx + r * math.cos(2 * math.pi * i / n - math.pi / 2),
            cy + r * math.sin(2 * math.pi * i / n - math.pi / 2),
        )
        for i, node_id in enumerate(ids)
    }


def el(parent: ET.Element, tag: str, text: str | None = None, **attrs: str) -> ET.Element:
    elem = ET.SubElement(parent, tag, attrs)
    if text is not None:
        elem.text = text
    return elem


def add_rectangle_geometry(shape: ET.Element) -> None:
    """Geometría de rectángulo relativa a Width/Height (requerida para que Visio dibuje la forma)."""
    geom = el(shape, "Geom", IX="0")
    segments = [
        ("MoveTo", "1", "0", "0"),
        ("LineTo", "2", "Width", "0"),
        ("LineTo", "3", "Width", "Height"),
        ("LineTo", "4", "0", "Height"),
        ("LineTo", "5", "0", "0"),
    ]
    for tag, ix, x_val, y_val in segments:
        node = el(geom, tag, IX=ix)
        x_attrs = {"F": "Inh"} if x_val == "Width" else {}
        el(node, "X", x_val if x_val == "0" else "Width", **x_attrs)
        y_attrs = {"F": "Inh"} if y_val == "Height" else {}
        el(node, "Y", y_val if y_val == "0" else "Height", **y_attrs)


def add_shape(
    shapes: ET.Element,
    shape_id: int,
    label: str,
    pin_x: float,
    pin_y: float,
    width: float = BOX_W,
    height: float = BOX_H,
    fill: str = "#4A7CB5",
    dashed: bool = False,
) -> int:
    shape = ET.SubElement(
        shapes,
        "Shape",
        {"ID": str(shape_id), "NameU": f"Shape.{shape_id}", "Type": "Shape"},
    )
    xform = el(shape, "XForm")
    el(xform, "PinX", f"{pin_x:.4f}")
    el(xform, "PinY", f"{pin_y:.4f}")
    el(xform, "Width", f"{width:.4f}")
    el(xform, "Height", f"{height:.4f}")
    el(xform, "LocPinX", f"{width / 2:.4f}")
    el(xform, "LocPinY", f"{height / 2:.4f}")

    fill_el = el(shape, "Fill")
    el(fill_el, "FillForegnd", fill)
    el(fill_el, "FillBkgnd", fill)
    if dashed:
        line = el(shape, "Line")
        el(line, "LinePattern", "2")

    add_rectangle_geometry(shape)

    txt = el(shape, "Text")
    txt.text = label
    # Formato texto centrado
    chars = el(shape, "Chars")
    el(chars, "Size", "0.1111111")  # ~8pt
    return shape_id


def add_connector(
    shapes: ET.Element,
    shape_id: int,
    x1: float,
    y1: float,
    x2: float,
    y2: float,
    label: str = "",
    dashed: bool = False,
) -> None:
    shape = ET.SubElement(
        shapes,
        "Shape",
        {"ID": str(shape_id), "NameU": f"Connector.{shape_id}", "Type": "Shape"},
    )
    el(shape, "OneD", "1")
    xform = el(shape, "XForm")
    el(xform, "BeginX", f"{x1:.4f}")
    el(xform, "BeginY", f"{y1:.4f}")
    el(xform, "EndX", f"{x2:.4f}")
    el(xform, "EndY", f"{y2:.4f}")
    if dashed:
        line = el(shape, "Line")
        el(line, "LinePattern", "2")
    if label:
        el(shape, "Text").text = label


def connector_points(
    parent: tuple[float, float],
    child: tuple[float, float],
    parent_above: bool = True,
) -> tuple[tuple[float, float], tuple[float, float]]:
    """Puntos de conexión en borde inferior del padre y superior del hijo."""
    px, py = parent
    cx, cy = child
    half_h = BOX_H / 2
    if parent_above:
        return (px, py - half_h), (cx, cy + half_h)
    return (px, py + half_h), (cx, cy - half_h)


def build_page(name: str, page_id: int, width: float, height: float) -> ET.Element:
    page = ET.Element("Page", {"ID": str(page_id), "NameU": name})
    page_sheet = el(page, "PageSheet")
    page_props = el(page_sheet, "PageProps")
    el(page_props, "PageWidth", f"{width:.4f}")
    el(page_props, "PageHeight", f"{height:.4f}")
    el(page, "Shapes")
    return page


def generate_vdx() -> ET.ElementTree:
    root = ET.Element("VisioDocument", {"xmlns": NS, "xml:space": "preserve"})
    doc_props = el(root, "DocumentProperties")
    el(doc_props, "Title", "Legal AI AR — Ontología v2.1")
    el(doc_props, "Creator", "generate_visio_export.py")

    pages = el(root, "Pages")
    node_lookup = {n.id: n for n in HIERARCHY}

    # --- Página 1: Jerarquía (apaisada) ---
    page1 = build_page("1-Jerarquia-Clases", 0, PAGE_LANDSCAPE_W, PAGE_LANDSCAPE_H)
    shapes1 = page1.find("Shapes")
    assert shapes1 is not None
    positions1 = layout_hierarchy_columns(HIERARCHY)
    sid = 1

    for node in HIERARCHY:
        if node.id not in positions1:
            continue
        x, y = positions1[node.id]
        label = f"legar:{node.id}"
        if node.kb_entity:
            label += f"\n[{node.kb_entity}]"
        fill = STYLE_COLORS.get(node.style, STYLE_COLORS["subclass"])
        add_shape(shapes1, sid, label, x, y, fill=fill, dashed=node.style in ("role", "conceptual"))
        sid += 1

    for node in HIERARCHY:
        if node.parent and node.parent in positions1 and node.id in positions1:
            p1, p2 = connector_points(positions1[node.parent], positions1[node.id])
            add_connector(
                shapes1,
                sid,
                p1[0],
                p1[1],
                p2[0],
                p2[1],
                "rol" if node.style == "role" else "is-a",
                dashed=node.style == "role",
            )
            sid += 1

    pages.append(page1)

    # --- Página 2: Relaciones ---
    page2 = build_page("2-Relaciones-Dominio", 1, PAGE_PORTRAIT_W, PAGE_PORTRAIT_H)
    shapes2 = page2.find("Shapes")
    assert shapes2 is not None
    rel_nodes = {e.source for e in RELATIONSHIPS} | {e.target for e in RELATIONSHIPS}
    positions2 = layout_relationships(RELATIONSHIPS, rel_nodes)
    sid = 1
    for node_id in sorted(rel_nodes):
        n = node_lookup.get(node_id)
        label = f"legar:{node_id}" if n else node_id
        x, y = positions2[node_id]
        style = n.style if n else "subclass"
        add_shape(shapes2, sid, label, x, y, width=1.5, height=0.42, fill=STYLE_COLORS.get(style, "#4A7CB5"))
        sid += 1
    for edge in RELATIONSHIPS:
        if edge.source in positions2 and edge.target in positions2:
            p1, p2 = positions2[edge.source], positions2[edge.target]
            add_connector(shapes2, sid, p1[0], p1[1], p2[0], p2[1], edge.label)
            sid += 1
    pages.append(page2)

    # --- Página 3: Implementación ---
    page3 = build_page("3-Estado-Implementacion", 2, PAGE_PORTRAIT_W, PAGE_PORTRAIT_H)
    shapes3 = page3.find("Shapes")
    assert shapes3 is not None
    sid = 1
    for text, color, x, y in [
        ("Implementada (KB + UI)", "#2E7D32", 1.0, 7.6),
        ("Parcial / en curso", "#F9A825", 1.0, 7.0),
        ("Conceptual / planificada", "#BDBDBD", 1.0, 6.4),
    ]:
        add_shape(shapes3, sid, text, x, y, width=2.6, height=0.38, fill=color)
        sid += 1
    items = [
        ("legar:Sentencia → Ruling", "kb", "/jurisprudencia", 7.3),
        ("legar:Tribunal → Court", "kb", "/organismos", 6.5),
        ("legar:SujetoDeDerecho → Person", "kb", "/sujetos", 5.7),
        ("legar:NormaJuridica → Statute", "subclass", "/ordenamiento (F3)", 4.9),
        ("legar:ProcesoJudicial → JudicialProceeding", "kb", "/procesos (F3)", 4.1),
        ("legar:ThesaurusTerm", "kb", "/vocabulario", 3.3),
        ("legar:HechoJuridico", "conceptual", "Futuro", 2.5),
        ("legar:Recurso", "conceptual", "F4", 1.7),
    ]
    for label, style, route, y in items:
        add_shape(
            shapes3,
            sid,
            f"{label}\n{route}",
            4.5,
            y,
            width=5.5,
            height=0.55,
            fill=STYLE_COLORS.get(style, "#4A7CB5"),
        )
        sid += 1
    pages.append(page3)

    return ET.ElementTree(root)


def generate_svg() -> str:
    positions = layout_hierarchy_columns(HIERARCHY)
    lookup = {n.id: n for n in HIERARCHY}
    w, h = int(PAGE_LANDSCAPE_W * 72), int(PAGE_LANDSCAPE_H * 72)

    def sx(x: float) -> float:
        return x * 72

    def sy(y: float) -> float:
        return (PAGE_LANDSCAPE_H - y) * 72

    lines = [
        '<?xml version="1.0" encoding="UTF-8"?>',
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{w}" height="{h}" viewBox="0 0 {w} {h}">',
        '<style>text{font-family:Segoe UI,Arial,sans-serif;font-size:10px;fill:#fff}</style>',
        '<rect width="100%" height="100%" fill="#f5f5f5"/>',
        '<text x="24" y="28" font-size="16" font-weight="bold" fill="#333">'
        "Legal AI AR — Ontología v2.1 — Jerarquía de clases</text>",
    ]
    for node in HIERARCHY:
        if node.parent and node.parent in positions and node.id in positions:
            p1, p2 = positions[node.parent], positions[node.id]
            dash = ' stroke-dasharray="5,4"' if node.style == "role" else ""
            lines.append(
                f'<line x1="{sx(p1[0]):.0f}" y1="{sy(p1[1]):.0f}" '
                f'x2="{sx(p2[0]):.0f}" y2="{sy(p2[1]):.0f}" stroke="#555" stroke-width="1.5"{dash}/>'
            )
    for node in HIERARCHY:
        if node.id not in positions:
            continue
        x, y = positions[node.id]
        fill = STYLE_COLORS.get(node.style, STYLE_COLORS["subclass"])
        label = node.id + (f" [{node.kb_entity}]" if node.kb_entity else "")
        bw, bh = BOX_W * 72, BOX_H * 72
        lines.append(
            f'<rect x="{sx(x)-bw/2:.0f}" y="{sy(y)-bh/2:.0f}" width="{bw:.0f}" height="{bh:.0f}" '
            f'rx="4" fill="{fill}" stroke="#222"/>'
        )
        lines.append(
            f'<text x="{sx(x):.0f}" y="{sy(y)+4:.0f}" text-anchor="middle">{label}</text>'
        )
    lines.append("</svg>")
    return "\n".join(lines)


def main() -> None:
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
    tree = generate_vdx()
    vdx_path = OUTPUT_DIR / "legal-ai-ar-ontology-v2.1.visio.vdx"
    tree.write(str(vdx_path), encoding="utf-8", xml_declaration=True)
    print(f"VDX: {vdx_path}")
    svg_path = OUTPUT_DIR / "legal-ai-ar-ontology-v2.1.svg"
    svg_path.write_text(generate_svg(), encoding="utf-8")
    print(f"SVG: {svg_path}")


if __name__ == "__main__":
    main()
