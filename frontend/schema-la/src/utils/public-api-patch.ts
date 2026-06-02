import { Rule, Tree } from '@angular-devkit/schematics';
import { getNameVariants } from './names';
import { uiPublicApiPath } from './paths';

export function appendUiPublicApiExport(componentName: string): Rule {
  return (tree: Tree) => {
    const path = uiPublicApiPath();
    if (!tree.exists(path)) {
      throw new Error(`Could not find ${path}`);
    }

    const { classified, dasherized } = getNameVariants(componentName);
    const exportLine = `export { ${classified}Component } from './lib/${dasherized}/${dasherized}.component';\n`;
    const content = tree.read(path)!.toString('utf-8');

    if (content.includes(exportLine.trim())) {
      return tree;
    }

    const recorder = tree.beginUpdate(path);
    recorder.insertRight(content.length, exportLine);
    tree.commitUpdate(recorder);
    return tree;
  };
}
