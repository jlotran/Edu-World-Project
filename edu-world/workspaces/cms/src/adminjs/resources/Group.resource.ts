import { DatabaseMetadata } from '@adminjs/sql';

export const getGroupResource = (postgres: DatabaseMetadata) => {
  return {
    resource: postgres.table('groups'),
  };
};
