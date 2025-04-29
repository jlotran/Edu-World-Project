module.exports = {
  root: true,
  // Cấu hình cơ bản chung cho toàn bộ project
  extends: ['eslint:recommended', 'plugin:@typescript-eslint/recommended'],
  parser: '@typescript-eslint/parser',
  plugins: ['@typescript-eslint'],
  ignorePatterns: ['dist', 'build', 'node_modules', '.next'],
};
