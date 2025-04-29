import { DatabaseMetadata } from '@adminjs/sql';

export const getUserResource = (postgres: DatabaseMetadata) => {
  return {
    resource: postgres.table('users'),
    options: {
      id: 'users',
      href: ({ h, resource }) => {
        return h.resourceActionUrl({
          resourceId: resource.decorate().id(),
          actionName: 'list',
          params: {
            'filters.status': 'active',
          },
        });
      },
    },
  };
};
