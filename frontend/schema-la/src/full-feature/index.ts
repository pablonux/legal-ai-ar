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
import { featureRoot } from '../utils/paths';
import { addFeatureRoute } from '../utils/route-patch';

export interface FullFeatureOptions {
  name: string;
  route: string;
  skipRoutes?: boolean;
}

export default function fullFeature(options: FullFeatureOptions): Rule {
  return (tree: Tree, context: SchematicContext) => {
    const { dasherized } = getNameVariants(options.name);
    const target = featureRoot(dasherized);

    if (tree.exists(`${target}/${dasherized}.routes.ts`)) {
      throw new Error(`Feature "${dasherized}" already exists at ${target}`);
    }

    const templateSource = apply(url('./files'), [
      applyTemplates(
        getTemplateContext(options.name, {
          titleLabel: 'Sección',
          descriptionLabel: 'Contenido en construcción.'
        })
      ),
      move(target)
    ]);

    const rules: Rule[] = [mergeWith(templateSource)];

    if (!options.skipRoutes) {
      rules.push(addFeatureRoute({ name: options.name, route: options.route }));
    }

    context.logger.info(`Created full feature "${dasherized}" at ${target}`);
    return chain(rules)(tree, context);
  };
}
