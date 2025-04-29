import { DatabaseMetadata, Resource } from '@adminjs/sql';

import { getGroupResource } from './Group.resource.js';
import { getUserResource } from './User.resource.js';
export const getResources = (postgres: DatabaseMetadata) => {
  return [getUserResource(postgres), getGroupResource(postgres)];
};
