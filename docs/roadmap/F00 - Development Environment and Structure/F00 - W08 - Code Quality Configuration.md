# F00 - W08 - Code Quality Configuration (Linting, Formatting, EditorConfig)

> **Feature:** F00 - Development Environment and Structure
> **Release:** 0.0 | **Sprint:** S00
> **Type:** devops | **Priority:** Medium
> **Estimate:** 3 story points
> **Assignable to:** Frontend Dev (Angular linting) + Backend Dev (.NET analyzers)

---

## Description

Configure all code-quality tools to ensure consistency between the two developers: EditorConfig, ESLint, Prettier, Roslyn Analyzers, husky pre-commit hooks, and PR/Issue templates.

---

## Tasks

### Shared
- [ ] Create a root `.editorconfig` with rules for C#, TypeScript, JSON, YAML, Markdown
- [ ] Configure husky + lint-staged for pre-commit hooks
- [ ] Create `.github/PULL_REQUEST_TEMPLATE.md`
- [ ] Create `.github/ISSUE_TEMPLATE/bug_report.md`
- [ ] Create `.github/ISSUE_TEMPLATE/work_item.md`
- [ ] Create `.github/CODEOWNERS`

### Backend (.NET)
- [ ] Add `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `Directory.Build.props`
- [ ] Add `<Nullable>enable</Nullable>` in `Directory.Build.props`
- [ ] Add `<ImplicitUsings>enable</ImplicitUsings>`
- [ ] Configure Roslyn analyzers: `Microsoft.CodeAnalysis.NetAnalyzers`
- [ ] Add a `.globalconfig` with analysis rules (naming, async, null checks)
- [ ] Configure XML documentation warnings

### Frontend (Angular)
- [ ] Configure ESLint with `angular-eslint` and custom rules
- [ ] Configure Prettier with `.prettierrc` and `.prettierignore`
- [ ] Configure `lint-staged` to run lint + format on pre-commit
- [ ] Add an import-ordering rule (eslint-plugin-import)
- [ ] Configure strict TypeScript (`strict: true` in tsconfig)

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
## Description
<!-- What this PR changes and why -->

## Work Item
<!-- E.g.: F01-W02 -->
Closes #

## Type of change
- [ ] feat: new functionality
- [ ] fix: bug fix
- [ ] refactor: refactoring
- [ ] test: new or updated tests
- [ ] docs: documentation
- [ ] chore: maintenance

## Checklist
- [ ] The code compiles with no warnings
- [ ] Tests pass locally
- [ ] I have added/updated tests for my changes
- [ ] The documentation is updated (if applicable)
- [ ] I have reviewed my own code
- [ ] There are no console.log() or debug code
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
// frontend/package.json (add)
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

## Acceptance Criteria

- [ ] `.editorconfig` applies rules when formatting in VS Code and Visual Studio
- [ ] ESLint detects style errors in TypeScript and Angular templates
- [ ] Prettier formats automatically on save (with the VS Code config)
- [ ] The pre-commit hook runs lint and format before each commit
- [ ] Commits follow Conventional Commits (validated by commitlint)
- [ ] The PR template is shown when creating a PR on GitHub
- [ ] CODEOWNERS assigns reviewers automatically
- [ ] The backend compiles with `TreatWarningsAsErrors` and no warnings

---

## Dependencies

- **Depends on:** F00-W02 (existing repo), F00-W03 (existing frontend)
- **Blocks:** None (but it improves the development experience from day 1)

---

*F00 - W08 - Code Quality Configuration — Legal Ai Ar*
