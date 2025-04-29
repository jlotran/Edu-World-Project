import { Database, Resource, Adapter } from '@adminjs/sql';
import AdminJS from 'adminjs';
import Fastify from 'fastify';

import { getAdminJsConfig } from './adminjs/config.js';
import { appConfig } from './core/index.js';
import { generateLoginPage } from './server/routers/AuthenticationRouter.js';

AdminJS.registerAdapter({
  Database,
  Resource,
});

const start = async (): Promise<void> => {
  // Create Fastify Instance
  const app = Fastify();

  // Create Postgresql Database instance
  const postgres = await new Adapter('postgresql', {
    connectionString: appConfig.POSTGRES_CONNECTION_STRING,
    database: appConfig.POSTGRES_DATABASE,
  }).init();

  // Create AdminJS Instance
  const admin = new AdminJS(getAdminJsConfig(app, postgres));

  // Auto init login page
  await generateLoginPage(app, admin);

  const port = appConfig.ADMINJS_PORT;
  app.listen({ port }, (err, addr) => {
    if (err) {
      console.error(err);
    } else {
      console.warn(`AdminJS started on http://localhost:${port}${admin.options.rootPath}`);
    }
  });
};

start();
