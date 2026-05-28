# F12 - W07 - Frontend - Formulario Alta y Edicion Expediente

> **Feature:** F12 - Gestion de Expedientes
> **Release:** 2.0 | **Sprint:** S05-S06
> **Tipo:** frontend | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (Frontend)

---

## Descripción

Implementar formulario reactivo tipado para Gestion de Expedientes. Incluye validaciones, controles dinámicos y feedback visual.

---

## Tareas

- [ ] Crear componente de formulario como standalone component
- [ ] Implementar Typed Reactive Form con validaciones
- [ ] Agregar controles: inputs, selects, datepickers, textareas según modelo
- [ ] Implementar validación visual (mat-error con mensajes descriptivos)
- [ ] Conectar al service para submit (POST/PUT)
- [ ] Manejar estados: loading, success, error
- [ ] Soportar modo edición (prellenar con datos existentes)
- [ ] Escribir tests unitarios

---

## Criterios de Aceptación

- [ ] El componente renderiza correctamente en desktop (>1200px) y tablet (768-1200px)
- [ ] Los estados de loading, error y empty se manejan correctamente
- [ ] La accesibilidad cumple WCAG 2.1 AA (verificar con axe DevTools)
- [ ] Los tests unitarios cubren > 80% del componente
- [ ] El componente es standalone y reutilizable donde corresponda

---

## Notas Técnicas

- Angular 19 con standalone components (sin NgModules)
- State management: Angular Signals + NgRx Signal Store para estado global
- UI: Angular Material 19 + Tailwind CSS 4
- Formularios: Typed Reactive Forms
- Referir a la documentación integral (F12-W01) para mockups y criterios de aceptación

---

## Archivos a Crear/Modificar

```
src/app/features/{feature}/{feature}-form/{feature}-form.component.ts
src/app/features/{feature}/{feature}-form/{feature}-form.component.html
src/app/features/{feature}/{feature}-form/{feature}-form.component.spec.ts
```

---

## Dependencias

- Depende de: F12-W01 (Documentación integral)

---

*F12 - W07 - Frontend - Formulario Alta y Edicion Expediente — Legal Ai Ar*
