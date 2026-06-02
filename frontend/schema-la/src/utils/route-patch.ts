import { Rule, Tree } from '@angular-devkit/schematics';
import { getNameVariants } from './names';
import { appRoutesPath } from './paths';

const ROUTE_MARKER = '      // --- Backward compatibility redirects';

export interface FeatureRouteOptions {
  readonly name: string;
  readonly route: string;
}

export function addFeatureRoute(options: FeatureRouteOptions): Rule {
  return (tree: Tree) => {
    const path = appRoutesPath();
    if (!tree.exists(path)) {
      throw new Error(`Could not find ${path}. Register the route manually.`);
    }

    const routePath = options.route.replace(/^\/+/, '');
    const { dasherized, camelized } = getNameVariants(options.name);
    const buffer = tree.read(path);
    if (!buffer) {
      return tree;
    }

    const content = buffer.toString('utf-8');
    if (content.includes(`path: '${routePath}'`)) {
      return tree;
    }

    const markerIndex = content.indexOf(ROUTE_MARKER);
    if (markerIndex === -1) {
      throw new Error(
        `Could not find route insertion marker in app.routes.ts. Add a lazy route for "${routePath}" manually.`
      );
    }

    const block = `      {
        path: '${routePath}',
        loadChildren: () =>
          import('./features/${dasherized}/${dasherized}.routes').then((m) => m.${camelized}Routes),
      },

`;

    const recorder = tree.beginUpdate(path);
    recorder.insertLeft(markerIndex, block);
    tree.commitUpdate(recorder);
    return tree;
  };
}
