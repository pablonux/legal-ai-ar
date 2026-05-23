/**
 * Professional demo recorder for Legal AI AR.
 * Records a ~5.5 min pitch video walking through all major features
 * including search, ruling detail, two chat scenarios, catalogs, and tesauro.
 *
 * Prerequisites:
 *   - Frontend running on http://localhost:4200  (ng serve)
 *   - Backend  running on http://localhost:5088  (dotnet run)
 *
 * Usage:
 *   npm install && npm run install-browser
 *   npm run record          # full recording
 *   npm run preview         # preview without recording
 *
 * Output: videos/ directory (webm files)
 */
import { chromium, type Page } from "playwright";
import path from "path";

const BASE = "http://localhost:4200";
const CREDENTIALS = { email: "demo@pwc.com", password: "demo1234" };

const BEAT = 1_200;
const PAUSE = 2_500;
const HOLD = 4_000;
const READ = 6_000;
const CHAT_WAIT = 15_000;

const wait = (ms: number) => new Promise((r) => setTimeout(r, ms));

async function humanType(page: Page, text: string) {
  for (const char of text) {
    await page.keyboard.type(char, { delay: 0 });
    await wait(45 + Math.random() * 55);
  }
}

async function smoothScroll(page: Page, distance = 600, step = 60) {
  const steps = Math.ceil(distance / step);
  for (let i = 0; i < steps; i++) {
    await page.mouse.wheel(0, step);
    await wait(50);
  }
}

async function scrollToBottom(page: Page) {
  await page.evaluate(() => {
    const main = document.querySelector(".shell-main") || document.scrollingElement || document.body;
    main.scrollTo({ top: main.scrollHeight, behavior: "smooth" });
  });
  await wait(800);
}

async function scrollToTop(page: Page) {
  await page.evaluate(() => {
    const main = document.querySelector(".shell-main") || document.scrollingElement || document.body;
    main.scrollTo({ top: 0, behavior: "smooth" });
  });
  await wait(500);
}

async function annotate(page: Page, text: string) {
  await page.evaluate((t) => {
    let banner = document.getElementById("demo-banner");
    if (!banner) {
      banner = document.createElement("div");
      banner.id = "demo-banner";
      Object.assign(banner.style, {
        position: "fixed", top: "18px", left: "50%", transform: "translateX(-50%)",
        zIndex: "99999", padding: "6px 22px", borderRadius: "24px",
        background: "rgba(12, 12, 12, 0.82)", backdropFilter: "blur(14px)",
        WebkitBackdropFilter: "blur(14px)", border: "1px solid rgba(255,255,255,0.10)",
        color: "rgba(255, 255, 255, 0.95)", fontSize: "14px",
        fontFamily: "'Segoe UI', Arial, sans-serif", fontWeight: "500",
        letterSpacing: "0.3px", lineHeight: "1.5",
        boxShadow: "0 6px 24px rgba(0,0,0,0.32)",
        transition: "opacity 0.5s ease, transform 0.5s ease",
        maxWidth: "80vw", textAlign: "center",
      });
      document.body.appendChild(banner);
    }
    banner.textContent = t;
    banner.style.opacity = "1";
    banner.style.transform = "translateX(-50%) translateY(0)";
  }, text);
  await wait(300);
}

async function clearBanner(page: Page) {
  await page.evaluate(() => {
    const b = document.getElementById("demo-banner");
    if (b) {
      b.style.opacity = "0";
      b.style.transform = "translateX(-50%) translateY(-8px)";
    }
  });
  await wait(500);
}

async function visible(page: Page, selector: string, ms = 3000): Promise<boolean> {
  return page.locator(selector).first().isVisible({ timeout: ms }).catch(() => false);
}

async function main() {
  const noRecord = process.argv.includes("--no-record");
  const videosDir = path.join(__dirname, "videos");

  console.log(noRecord
    ? "Preview mode (no recording)..."
    : "Recording professional demo...");

  const browser = await chromium.launch({
    headless: false,
    args: ["--start-maximized"],
  });
  const context = await browser.newContext({
    viewport: null,
    ...(noRecord ? {} : {
      recordVideo: { dir: videosDir, size: { width: 1920, height: 1080 } },
    }),
    locale: "es-AR",
  });
  const page = await context.newPage();

  try {
    // ═══════════════════════════════════════════════════════════
    // ESCENA 1 — Bienvenida (dev API auto-authenticates via /me)
    // ═══════════════════════════════════════════════════════════
    console.log("1/12  Bienvenida");
    await page.goto(`${BASE}/bienvenida`);
    await page.waitForURL("**/bienvenida", { timeout: 15_000 });
    await annotate(page, "Legal AI AR — Plataforma de Inteligencia Artificial Legal");
    await clearBanner(page);
    await wait(PAUSE);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 2 — Bienvenida (tour)                        (~10s)
    // ═══════════════════════════════════════════════════════════
    console.log("2/12  Bienvenida");
    await annotate(page, "Todas las capacidades de la plataforma en un solo lugar");
    await wait(HOLD);

    for (const href of ["/buscar", "/chat", "/dashboard"]) {
      const card = page.locator(`a.feature-card[href="${href}"]`);
      if (await card.isVisible().catch(() => false)) {
        await card.hover();
        await wait(800);
      }
    }
    await wait(BEAT);
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 3 — Búsqueda con filtros                     (~35s)
    // ═══════════════════════════════════════════════════════════
    console.log("3/12  Búsqueda con filtros");
    await page.click('a.feature-card[href="/buscar"]');
    await page.waitForLoadState("domcontentloaded");
    await annotate(page, "Búsqueda semántica en lenguaje natural");
    await wait(PAUSE);

    // Type a short query for visual effect
    const searchInput = page.locator("app-search-home input.search-input");
    await searchInput.click();
    await wait(300);
    await annotate(page, "Escribimos una consulta en lenguaje natural");
    await humanType(page, "repetición de impuestos AFIP");
    await wait(BEAT);

    // Set materia filter (guaranteed 345 results, fast response)
    await annotate(page, "Filtros avanzados: jurisdicción, fecha, materia, tribunal");
    await wait(BEAT);

    const materiaSelect = page.locator("select#i-subjectArea");
    if (await materiaSelect.isVisible().catch(() => false)) {
      const options = await materiaSelect.locator("option").allTextContents();
      const tributario = options.find(o => o.toLowerCase().includes("tribut"));
      if (tributario) {
        await materiaSelect.selectOption({ label: tributario });
        await wait(800);
      }
    }

    const jurisdictionSelect = page.locator("select#i-jurisdictionArea");
    if (await jurisdictionSelect.isVisible().catch(() => false)) {
      const options = await jurisdictionSelect.locator("option").allTextContents();
      const apelacion = options.find(o => o.toLowerCase().includes("apelacion extraordinaria"));
      if (apelacion) {
        await jurisdictionSelect.selectOption({ label: apelacion });
        await wait(600);
      }
    }

    await wait(BEAT);
    const searchBtn = page.locator("button.search-btn");
    await searchBtn.click();

    // Wait generously for results (semantic search + filters can take 10-30s)
    await annotate(page, "Buscando en más de 8.000 fallos indexados...");
    try {
      await page.locator("a.ruling-card").first().waitFor({ timeout: 45_000 });
    } catch {
      await wait(10_000);
    }
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 4 — Resultados de búsqueda                   (~30s)
    // ═══════════════════════════════════════════════════════════
    console.log("4/12  Resultados");
    await scrollToTop(page);
    await annotate(page, "Resultados rankeados por relevancia semántica");
    await wait(HOLD);
    await smoothScroll(page, 500);
    await wait(PAUSE);
    await annotate(page, "Cada resultado incluye relevancia, resumen y metadata enriquecida");
    await wait(PAUSE);
    await smoothScroll(page, 300);
    await wait(PAUSE);
    await clearBanner(page);

    // Click BANCO DE LA CIUDAD ruling specifically, or fallback to direct nav
    const BANCO_CIUDAD_ID = "dc2ad842-8a2f-43c4-8634-d7a8380e37c8";
    const bancoRuling = page.locator(`a.ruling-card:has-text("BANCO DE LA CIUDAD")`);
    if (await bancoRuling.isVisible({ timeout: 5000 }).catch(() => false)) {
      await bancoRuling.hover();
      await wait(800);
      await annotate(page, "Abriendo detalle del fallo...");
      await wait(BEAT);
      await bancoRuling.click();
      await page.waitForLoadState("domcontentloaded");
    } else {
      const firstRuling = page.locator("a.ruling-card").first();
      if (await firstRuling.isVisible({ timeout: 3000 }).catch(() => false)) {
        await firstRuling.hover();
        await wait(800);
        await firstRuling.click();
        await page.waitForLoadState("domcontentloaded");
      } else {
        console.log("  (no visible results, navigating to BANCO CIUDAD directly)");
        await page.goto(`${BASE}/fallos/${BANCO_CIUDAD_ID}`);
        await page.waitForLoadState("domcontentloaded");
      }
    }

    // ═══════════════════════════════════════════════════════════
    // ESCENA 5 — Detalle del fallo (EXTENDIDA)            (~60s)
    // ═══════════════════════════════════════════════════════════
    if (page.url().includes("/fallos/")) {
      console.log("5/12  Detalle del fallo (extendida)");

      // Header: title, tribunal, badges, date
      await annotate(page, "Análisis completo del fallo con documento original");
      await wait(READ);

      // PDF panel (left side) — give it time to load
      await wait(PAUSE);
      await smoothScroll(page, 300);
      await wait(HOLD);

      // Summary & Considerando cards (right side)
      await annotate(page, "Resumen y considerandos generados por IA");
      await smoothScroll(page, 400);
      await wait(READ);

      // Tab Información
      const tabInfo = page.locator('button.tab-btn:has-text("Información")');
      if (await tabInfo.isVisible().catch(() => false)) {
        await annotate(page, "Magistrados, palabras clave y metadata estructurada");
        await tabInfo.click();
        await wait(READ);
        await smoothScroll(page, 400);
        await wait(HOLD);
      }

      // Tab Relaciones
      const tabRel = page.locator('button.tab-btn:has-text("Relaciones")');
      if (await tabRel.isVisible().catch(() => false)) {
        await annotate(page, "Normas citadas y relaciones jurisprudenciales");
        await tabRel.click();
        await wait(READ);
        await smoothScroll(page, 400);
        await wait(HOLD);
      }

      // Related rulings section at the bottom
      await clearBanner(page);
      await scrollToBottom(page);
      await wait(BEAT);
      const relatedCard = page.locator("app-ruling-card a.ruling-card").first();
      if (await relatedCard.isVisible().catch(() => false)) {
        await annotate(page, "Fallos relacionados por similitud semántica");
        await wait(HOLD);
      }

      // CTA → Asistente
      const ctaAssistant = page.locator("div.assistant-cta");
      if (await ctaAssistant.isVisible().catch(() => false)) {
        await ctaAssistant.hover();
        await wait(800);
        await annotate(page, "Consultemos al Asistente sobre este fallo");
        await wait(PAUSE);
        await ctaAssistant.click();
        await page.waitForURL("**/chat**", { timeout: 15_000 }).catch(() => {});
        await wait(PAUSE);
      } else {
        await page.goto(`${BASE}/chat`);
        await wait(PAUSE);
      }
      await clearBanner(page);
    } else {
      // Fallback: navigate to BANCO CIUDAD ruling
      console.log("5/12  (fallback to known ruling)");
      await page.goto(`${BASE}/fallos/${BANCO_CIUDAD_ID}`);
      await page.waitForLoadState("domcontentloaded");
      await annotate(page, "Análisis completo del fallo con documento original");
      await wait(READ);
      await smoothScroll(page, 600);
      await wait(HOLD);

      const tabInfoFb = page.locator('button.tab-btn:has-text("Información")');
      if (await tabInfoFb.isVisible().catch(() => false)) {
        await annotate(page, "Magistrados, palabras clave y metadata estructurada");
        await tabInfoFb.click();
        await wait(READ);
      }

      const tabRelFb = page.locator('button.tab-btn:has-text("Relaciones")');
      if (await tabRelFb.isVisible().catch(() => false)) {
        await annotate(page, "Normas citadas y relaciones jurisprudenciales");
        await tabRelFb.click();
        await wait(READ);
      }

      await clearBanner(page);
      await page.goto(`${BASE}/chat`);
      await page.waitForLoadState("domcontentloaded");
    }

    // ═══════════════════════════════════════════════════════════
    // ESCENA 6 — Chat: ficha de jurisprudencia            (~50s)
    // ═══════════════════════════════════════════════════════════
    console.log("6/12  Chat — Ficha de jurisprudencia");
    await annotate(page, "Asistente jurídico con inteligencia artificial");
    await wait(PAUSE);

    const chatInput = page.locator("textarea.chat-textarea");
    if (await chatInput.isVisible({ timeout: 5000 }).catch(() => false)) {
      await chatInput.click();
      await wait(400);

      const fichaQuery =
        "Armá una ficha de jurisprudencia de este fallo con un resumen del mismo";
      await annotate(page, "Le pedimos al asistente una ficha de jurisprudencia del fallo");
      await humanType(page, fichaQuery);
      await wait(BEAT);

      await page.keyboard.press("Enter");
      await wait(BEAT);

      await annotate(page, "El asistente analiza el fallo y construye la ficha...");
      await page.locator("span.typing-cursor").waitFor({ timeout: 20_000 }).catch(() => {});
      await wait(HOLD);

      await annotate(page, "Generando ficha con datos estructurados y resumen...");
      try {
        await page.locator("span.typing-cursor").waitFor({ state: "hidden", timeout: 60_000 });
      } catch {
        await wait(CHAT_WAIT);
      }
      await wait(PAUSE);

      await annotate(page, "Ficha de jurisprudencia generada automáticamente");
      await smoothScroll(page, 800);
      await wait(READ);
    } else {
      const suggestion = page.locator("button.suggestion-card").first();
      if (await suggestion.isVisible().catch(() => false)) {
        await suggestion.click();
        await wait(CHAT_WAIT);
        await smoothScroll(page, 600);
        await wait(READ);
      }
    }
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 7 — Chat: fallos sobre despido               (~60s)
    // ═══════════════════════════════════════════════════════════
    console.log("7/12  Chat — Fallos sobre despido");

    // Reload /chat to start a fresh conversation
    await page.goto(`${BASE}/chat`);
    await wait(PAUSE);

    const chatInput2 = page.locator("textarea.chat-textarea");
    if (await chatInput2.isVisible({ timeout: 5000 }).catch(() => false)) {
      await chatInput2.click();
      await wait(400);

      const despidoQuery =
        "Busca los fallos más relevantes sobre despido";
      await annotate(page, "Nueva consulta: fallos relevantes sobre despido");
      await humanType(page, despidoQuery);
      await wait(BEAT);

      await page.keyboard.press("Enter");
      await wait(BEAT);

      await annotate(page, "El asistente busca en la base de conocimiento...");
      await page.locator("span.typing-cursor").waitFor({ timeout: 20_000 }).catch(() => {});
      await wait(HOLD);

      await annotate(page, "Construyendo lista de fallos relevantes...");
      try {
        await page.locator("span.typing-cursor").waitFor({ state: "hidden", timeout: 60_000 });
      } catch {
        await wait(CHAT_WAIT);
      }
      await wait(PAUSE);

      await annotate(page, "El asistente devuelve fallos citados y verificados");
      await smoothScroll(page, 600);
      await wait(READ);

      // Click a ruling link to open the side panel
      const rulingLink = page.locator("a.ruling-link").first();
      if (await rulingLink.isVisible().catch(() => false)) {
        await annotate(page, "Abrimos el detalle del fallo desde el chat");
        await rulingLink.click();
        await wait(HOLD);

        // The side panel should open — read it
        const sidePanel = page.locator("aside.side-panel");
        if (await sidePanel.isVisible({ timeout: 5000 }).catch(() => false)) {
          await annotate(page, "Panel de detalle del documento integrado en el chat");
          await wait(READ);

          // Click "Ver fallo completo" to navigate to full ruling
          const verFallo = page.locator("button.doc-action-link");
          if (await verFallo.isVisible().catch(() => false)) {
            await annotate(page, "Vamos al fallo completo");
            await wait(BEAT);
            await verFallo.click();
            await page.waitForLoadState("domcontentloaded");
            await wait(READ);
            await smoothScroll(page, 400);
            await wait(HOLD);
          }
        }
      }
    }
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 8 — Dashboard de la KB                       (~20s)
    // ═══════════════════════════════════════════════════════════
    console.log("8/12  Dashboard KB");
    await page.goto(`${BASE}/dashboard`);
    await page.waitForLoadState("domcontentloaded");
    await annotate(page, "8.000+ fallos indexados y creciendo");
    await wait(HOLD);
    await smoothScroll(page, 500);
    await wait(PAUSE);
    await annotate(page, "Métricas de calidad y cobertura en tiempo real");
    await smoothScroll(page, 500);
    await wait(HOLD);
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 9 — Catálogos: Tribunales y Jueces           (~40s)
    // ═══════════════════════════════════════════════════════════
    console.log("9/12  Catálogos — Tribunales y Jueces");
    await page.goto(`${BASE}/catalogos/tribunales`);
    await page.waitForLoadState("domcontentloaded");
    await annotate(page, "Catálogos: tribunales, jueces y tesauro jurídico");
    await wait(PAUSE);

    const courtSearch = page.locator('input[placeholder*="Buscar tribunal"]');
    if (await courtSearch.isVisible().catch(() => false)) {
      await courtSearch.click();
      await humanType(page, "Corte Suprema");
      await wait(PAUSE);
    }

    const courtLink = page.locator('a[href*="/catalogos/tribunales/"]').first();
    if (await courtLink.isVisible({ timeout: 3000 }).catch(() => false)) {
      await courtLink.click();
      await page.waitForLoadState("domcontentloaded");
      await wait(HOLD);
      await smoothScroll(page, 300);
      await wait(BEAT);

      // Click a judge from the court detail
      await annotate(page, "Perfiles de jueces con historial de participación");
      const judgeCard = page.locator('a[href*="/catalogos/jueces/"]').first();
      if (await judgeCard.isVisible().catch(() => false)) {
        await judgeCard.click();
        await page.waitForLoadState("domcontentloaded");
        await wait(HOLD);
        await smoothScroll(page, 300);
        await wait(PAUSE);

        // Click a ruling from the judge's profile (ROJAS or any available)
        const rojasRuling = page.locator('a.ruling-row:has-text("ROJAS")');
        const anyRuling = page.locator("a.ruling-row").first();
        const targetRuling = await rojasRuling.isVisible().catch(() => false) ? rojasRuling : anyRuling;
        if (await targetRuling.isVisible().catch(() => false)) {
          await annotate(page, "Desde el perfil del juez accedemos al detalle de un fallo");
          await targetRuling.hover();
          await wait(BEAT);
          await targetRuling.click();
          await page.waitForLoadState("domcontentloaded");
          await wait(READ);
          await smoothScroll(page, 400);
          await wait(HOLD);

          // Go back to continue with tesauro
          await page.goBack();
          await page.waitForLoadState("domcontentloaded");
          await wait(BEAT);
        }
      }
    }
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 10 — Tesauro jurídico                        (~35s)
    // ═══════════════════════════════════════════════════════════
    console.log("10/12 Tesauro jurídico");
    await page.goto(`${BASE}/catalogos/tesauro`);
    await page.waitForLoadState("domcontentloaded");
    await annotate(page, "Tesauro jurídico: taxonomía de conceptos legales");
    await wait(PAUSE);

    // Search for "IMPUESTO A LAS GANANCIAS"
    const thesaurusSearch = page.locator('input[placeholder*="Buscar descriptor"]');
    if (await thesaurusSearch.isVisible().catch(() => false)) {
      await thesaurusSearch.click();
      await humanType(page, "impuesto a la las ganancias");
      await wait(PAUSE);
    }

    // Click the specific descriptor "IMPUESTO A LAS GANANCIAS"
    const gananciasLink = page.locator('a[href*="/catalogos/tesauro/"]:has-text("IMPUESTO A LA LAS GANANCIAS")');
    const anyTermLink = page.locator('a[href*="/catalogos/tesauro/"]').first();
    const termLink = await gananciasLink.isVisible({ timeout: 5000 }).catch(() => false) ? gananciasLink : anyTermLink;
    if (await termLink.isVisible({ timeout: 3000 }).catch(() => false)) {
      await termLink.click();
      await page.waitForLoadState("domcontentloaded");
      await wait(HOLD);
      await smoothScroll(page, 200);
      await wait(BEAT);

      // Click "Buscar fallos con este descriptor" → navigates to search results
      const searchByTerm = page.locator('a.cta-link:has-text("Buscar fallos con este descriptor")');
      if (await searchByTerm.isVisible().catch(() => false)) {
        await annotate(page, "Buscamos fallos asociados a este descriptor del tesauro");
        await searchByTerm.hover();
        await wait(BEAT);
        await searchByTerm.click();
        await page.waitForLoadState("domcontentloaded");

        // Wait for results to appear
        try {
          await page.locator("a.ruling-card").first().waitFor({ timeout: 15_000 });
        } catch {
          await wait(5_000);
        }
        await wait(PAUSE);
        await annotate(page, "Fallos vinculados al descriptor del tesauro");
        await smoothScroll(page, 400);
        await wait(PAUSE);

        // Click a ruling from the results to see its detail
        const tesauroRuling = page.locator("a.ruling-card").first();
        if (await tesauroRuling.isVisible().catch(() => false)) {
          await tesauroRuling.hover();
          await wait(BEAT);
          await tesauroRuling.click();
          await page.waitForLoadState("domcontentloaded");
          await annotate(page, "Detalle del fallo encontrado desde el tesauro");
          await wait(READ);
          await smoothScroll(page, 400);
          await wait(HOLD);
        }
      }
    }
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 11 — Admin breve                             (~15s)
    // ═══════════════════════════════════════════════════════════
    console.log("11/12 Admin (breve)");
    await page.goto(`${BASE}/admin`);
    await page.waitForLoadState("domcontentloaded");
    await annotate(page, "Panel de administración del pipeline de ingesta");
    await wait(HOLD);

    await page.goto(`${BASE}/admin/crawlers`);
    await page.waitForLoadState("domcontentloaded");
    await wait(PAUSE);
    await clearBanner(page);

    // ═══════════════════════════════════════════════════════════
    // ESCENA 12 — Cierre                                  (~15s)
    // ═══════════════════════════════════════════════════════════
    console.log("12/12 Cierre");
    await page.goto(`${BASE}/bienvenida`);
    await page.waitForLoadState("domcontentloaded");
    await annotate(page, "Legal AI AR — Jurisprudencia argentina al alcance de una pregunta");
    await wait(HOLD);
    await annotate(page, "pwc  ·  Inteligencia artificial al servicio de la práctica legal");
    await wait(HOLD);
    await clearBanner(page);
    await wait(BEAT);

  } finally {
    await context.close();
    await browser.close();
  }

  if (!noRecord) {
    const video = page.video();
    if (video) {
      const videoPath = await video.path();
      console.log(`\nVideo saved: ${videoPath}`);
    }
  }
  console.log("Done!");
}

main().catch((err) => {
  console.error("Recording failed:", err);
  process.exit(1);
});
