export const pathToTitle = (path: string): string => {
  path = path.replace(/\//g, '');
  path = path.charAt(0).toUpperCase() + path.slice(1);
  return path;
};
