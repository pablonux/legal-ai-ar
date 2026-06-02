/** @type {import('jest').Config} */
module.exports = {
  preset: 'jest-preset-angular',
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testPathIgnorePatterns: ['<rootDir>/node_modules/', '<rootDir>/dist/', '<rootDir>/e2e/'],
  testMatch: ['**/*.spec.ts'],
  moduleNameMapper: {
    '^@legal-ai-ar/core$': '<rootDir>/projects/core/src/public-api.ts',
    '^@legal-ai-ar/shell$': '<rootDir>/projects/shell/src/public-api.ts',
    '^@legal-ai-ar/ui$': '<rootDir>/projects/ui/src/public-api.ts',
    '^@legal-ai-ar/shared-common$': '<rootDir>/projects/shared-common/src/public-api.ts',
    '^@legal-ai-ar/shared-common/(.*)$': '<rootDir>/projects/shared-common/src/lib/shared/$1',
    '^@legal-ai-ar/app/(.*)$': '<rootDir>/src/app/$1'
  }
};
