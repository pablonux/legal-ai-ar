import { join, normalize, type Path } from '@angular-devkit/core';

const p = (segment: string): Path => segment as Path;

export function featureRoot(featureName: string): Path {
  return normalize(join(p('src'), p('app'), p('features'), p(featureName)));
}

export function uiLibRoot(componentName: string): Path {
  return normalize(join(p('projects'), p('ui'), p('src'), p('lib'), p(componentName)));
}

export function appRoutesPath(): Path {
  return normalize(p('src/app/app.routes.ts'));
}

export function uiPublicApiPath(): Path {
  return normalize(p('projects/ui/src/public-api.ts'));
}
