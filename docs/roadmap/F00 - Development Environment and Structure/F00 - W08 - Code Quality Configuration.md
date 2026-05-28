# F00 - W08 - Configuración de Calidad de Código (Linting, Formatting, EditorConfig)

> **Feature:** F00 - Entorno y Estructura de Desarrollo
> **Release:** 0.0 | **Sprint:** S00
> **Tipo:** devops | **Prioridad:** Media
> **Estimación:** 3 story points
> **Asignable a:** Dev Frontend (linting Angular) + Dev Backend (analyzers .NET)

---

## Descripción

Configurar todas las herramientas de calidad de código para garantizar consistencia entre los dos desarrolladores: EditorConfig, ESLint, Prettier, Roslyn Analyzers, husky pre-commit hooks y templates de PR/Issues.

---

## Tareas

### Compartido
- [ ] Crear `.editorconfig` raíz con reglas para C#, TypeScript, JSON, YAML, Markdown
- [ ] Configurar husky + lint-staged para pre-commit hooks
- [ ] Crear `.github/PULL_REQUEST_TEMPLATE.md`
- [ ] Crear `.github/ISSUE_TEMPLATE/bug_report.md`
- [ ] Crear `.github/ISSUE_TEMPLATE/work_item.md`
- [ ] Crear `.github/CODEOWNERS`

### Backend (.NET)
- [ ] Agregar `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` en `Directory.Build.props`
- [ ] Agregar `<Nullable>enable</Nullable>` en `Directory.Build.props`
- [ ] Agregar `<ImplicitUsings>enable</ImplicitUsings>`
- [ ] Configurar Roslyn analyzers: `Microsoft.CodeAnalysis.NetAnalyzers`
- [ ] Agregar `.globalconfig` con reglas de análisis (naming, async, null checks)
- [ ] Configurar XML documentation warnings

### Frontend (Angular)
- [ ] Configurar ESLint con `angular-eslint` y reglas custom
- [ ] Configurar Prettier con `.prettierrc` y `.prettierignore`
- [ ] Configurar `lint-staged` para ejecutar lint + format en pre-commit
- [ ] Agregar regla de import ordering (eslint-plugin-import)
- [ ] Configurar strict TypeScript (`strict: true` en tsconfig)

---

## .editorconfig

```ini
# .editorconfig
root = true

[*]
charset = utf-8
end_of_line = lf
indent_style = space
indent_size = 2
insert_final_newline = true
trim_trailing_whitespace = true

[*.cs]
indent_size = 4
dotnet_sort_system_directives_first = true
csharp_new_line_before_open_brace = all
csharp_style_var_for_built_in_types = false:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_expression_bodied_methods = when_on_single_line:suggestion

[*.{ts,js,html}]
indent_size = 2

[*.{json,yml,yaml}]
indent_size = 2

[*.md]
trim_trailing_whitespace = false
```

---

## .prettierrc

```json
{
  "singleQuote": true,
  "trailingComma": "all",
  "printWidth": 120,
  "tabWidth": 2,
  "semi": true,
  "bracketSpacing": true,
  "arrowParens": "always",
  "endOfLine": "lf"
}
```

---

## ESLint Config (angular-eslint)

```json
// frontend/.eslintrc.json
{
  "root": true,
  "overrides": [
    {
      "files": ["*.ts"],
      "extends": [
        "eslint:recommended",
        "plugin:@typescript-eslint/recommended",
        "plugin:@angular-eslint/recommended",
        "plugin:@angular-eslint/template/process-inline-templates"
      ],
      "rules": {
        "@angular-eslint/directive-selector": ["error", { "type": "attribute", "prefix": "app", "style": "camelCase" }],
        "@angular-eslint/component-selector": ["error", { "type": "element", "prefix": "app", "style": "kebab-case" }],
        "@typescript-eslint/no-unused-vars": ["error", { "argsIgnorePattern": "^_" }],
        "@typescript-eslint/explicit-function-return-type": "warn",
        "no-console": ["warn", { "allow": ["warn", "error"] }]
      }
    },
    {
      "files": ["*.html"],
      "extends": ["plugin:@angular-eslint/template/recommended", "plugin:@angular-eslint/template/accessibility"]
    }
  ]
}
```

---

## PR Template

```markdown
<!-- .github/PULL_REQUEST_TEMPLATE.md -->
## Descripción
<!-- Qué cambia este PR y por qué -->

## Work Item
<!-- Ej: F01-W02 -->
Closes #

## Tipo de cambio
- [ ] feat: nueva funcionalidad
- [ ] fix: corrección de bug
- [ ] refactor: refactorización
- [ ] test: tests nuevos o actualizados
- [ ] docs: documentación
- [ ] chore: mantenimiento

## Checklist
- [ ] El código compila sin warnings
- [ ] Los tests pasan localmente
- [ ] He agregado/actualizado tests para mis cambios
- [ ] La documentación está actualizada (si aplica)
- [ ] He revisado mi propio código
- [ ] No hay console.log() ni código de debug
```

---

## CODEOWNERS

```
# .github/CODEOWNERS
# Default: Tech Lead reviews all
* @tech-lead

# Backend
/backend/ @dev-backend @tech-lead

# Frontend
/frontend/ @dev-frontend @tech-lead

# Infrastructure
/infra/ @tech-lead
/.github/ @tech-lead
```

---

## Husky + Lint-Staged

```json
// frontend/package.json (agregar)
{
  "husky": {
    "hooks": {
      "pre-commit": "lint-staged",
      "commit-msg": "npx commitlint --edit $1"
    }
  },
  "lint-staged": {
    "*.ts": ["eslint --fix", "prettier --write"],
    "*.html": ["prettier --write"],
    "*.scss": ["prettier --write"]
  }
}
```

---

## Criterios de Aceptación

- [ ] `.editorconfig` aplica reglas al formatear en VS Code y Visual Studio
- [ ] ESLint detecta errores de estilo en TypeScript y templates Angular
- [ ] Prettier formatea automáticamente al guardar (con config de VS Code)
- [ ] Pre-commit hook ejecuta lint y format antes de cada commit
- [ ] Los commits siguen Conventional Commits (validado por commitlint)
- [ ] El PR template se muestra al crear un PR en GitHub
- [ ] CODEOWNERS asigna reviewers automáticamente
- [ ] El backend compila con `TreatWarningsAsErrors` sin warnings

---

## Dependencias

- **Depende de:** F00-W02 (repo existente), F00-W03 (frontend existente)
- **Bloquea:** Ninguno (pero mejora la experiencia de desarrollo desde el día 1)

---

*F00 - W08 - Configuración de Calidad de Código — Legal Ai Ar*
