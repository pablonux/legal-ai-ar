import {
  Rule,
  SchematicContext,
  Tree,
  apply,
  applyTemplates,
  chain,
  mergeWith,
  move,
  url
} from '@angular-devkit/schematics';
import { getNameVariants, getTemplateContext } from '../utils/names';
import { uiLibRoot } from '../utils/paths';
import { appendUiPublicApiExport } from '../utils/public-api-patch';

export interface UiWrapperOptions {
  name: string;
}

export default function uiWrapper(options: UiWrapperOptions): Rule {
  return (tree: Tree, context: SchematicContext) => {
    const { dasherized } = getNameVariants(options.name);
    const target = uiLibRoot(dasherized);
    const componentPath = `${target}/${dasherized}.component.ts`;

    if (tree.exists(componentPath)) {
      throw new Error(`UI wrapper "${dasherized}" already exists at ${componentPath}`);
    }

    const templateSource = apply(url('./files'), [
      applyTemplates(getTemplateContext(options.name)),
      move(target)
    ]);

    context.logger.info(`Created UI wrapper "${dasherized}" at ${target}`);
    return chain([mergeWith(templateSource), appendUiPublicApiExport(options.name)])(tree, context);
  };
}
