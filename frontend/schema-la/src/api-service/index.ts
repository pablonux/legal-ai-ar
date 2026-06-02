import {
  Rule,
  SchematicContext,
  Tree,
  apply,
  applyTemplates,
  mergeWith,
  move,
  url
} from '@angular-devkit/schematics';
import { join, normalize, type Path } from '@angular-devkit/core';
import { getNameVariants, getTemplateContext } from '../utils/names';
import { featureRoot } from '../utils/paths';

export interface ApiServiceOptions {
  name: string;
  domain: string;
  feature: string;
  slice?: 'thin' | 'full';
}

export default function apiService(options: ApiServiceOptions): Rule {
  return (tree: Tree, context: SchematicContext) => {
    const slice = options.slice ?? 'full';
    const subfolder = slice === 'thin' ? 'data' : 'api';
    const { dasherized: featureDasherized } = getNameVariants(options.feature);
    const { dasherized: nameDasherized } = getNameVariants(options.name);
    const target = normalize(join(featureRoot(featureDasherized), subfolder as Path));
    const filePath = normalize(join(target, `${nameDasherized}.api.ts` as Path));

    if (tree.exists(filePath)) {
      throw new Error(`API client already exists at ${filePath}`);
    }

    if (!tree.exists(featureRoot(featureDasherized))) {
      throw new Error(
        `Feature "${featureDasherized}" not found. Run thin-feature or full-feature first.`
      );
    }

    const templateSource = apply(url('./files'), [
      applyTemplates(
        getTemplateContext(options.name, {
          domain: options.domain.replace(/^\/+|\/+$/g, ''),
          featureDasherized
        })
      ),
      move(target)
    ]);

    context.logger.info(`Created API client at ${filePath}`);
    return mergeWith(templateSource)(tree, context);
  };
}
