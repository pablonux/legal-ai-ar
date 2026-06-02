import { strings } from '@angular-devkit/core';

export interface NameVariants {
  readonly name: string;
  readonly dasherized: string;
  readonly classified: string;
  readonly camelized: string;
  readonly propertyName: string;
}

export function getNameVariants(name: string): NameVariants {
  const dasherized = strings.dasherize(name);
  const classified = strings.classify(dasherized);
  const camelized = strings.camelize(dasherized);
  return {
    name,
    dasherized,
    classified,
    camelized,
    propertyName: camelized
  };
}

export function getTemplateContext(
  name: string,
  extra: Record<string, unknown> = {}
): Record<string, unknown> {
  const variants = getNameVariants(name);
  return {
    ...strings,
    ...variants,
    ...extra
  };
}
