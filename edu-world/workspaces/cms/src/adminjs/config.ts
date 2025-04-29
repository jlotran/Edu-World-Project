import { DatabaseMetadata } from '@adminjs/sql';
import { AdminJSOptions } from 'adminjs';
import { FastifyInstance } from 'fastify';

import { getResources } from './resources/index.js';

export const getAdminJsConfig = (app: FastifyInstance, postgres: DatabaseMetadata): AdminJSOptions => {
  return {
    databases: [],
    rootPath: '/admin',
    resources: getResources(postgres),
  };
};
