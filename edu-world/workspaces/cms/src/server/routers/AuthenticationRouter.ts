import AdminJSFastify from '@adminjs/fastify';
import FastifySession from '@fastify/session';
import AdminJS from 'adminjs';
import Connect from 'connect-pg-simple';
import { FastifyInstance } from 'fastify';

import { appConfig } from '../../core/index.js';

// @ts-ignore
const ConnectSession = Connect(FastifySession);
const sessionStore = new ConnectSession({
  conObject: {
    connectionString: appConfig.POSTGRES_CONNECTION_STRING,
    ssl: process.env.NODE_ENV === 'production',
  },
  tableName: 'session',
  createTableIfMissing: true,
});

/**
 * Default admin credentials
 */
const DEFAULT_ADMIN = {
  email: appConfig.ADMINJS_DEFAULT_ADMIN_EMAIL,
  password: appConfig.ADMINJS_DEFAULT_ADMIN_PASSWORD,
};

/**
 * Authenticate function
 */
const authenticate = async (email: string, password: string): Promise<unknown | null> => {
  if (email === DEFAULT_ADMIN.email && password === DEFAULT_ADMIN.password) {
    return Promise.resolve(DEFAULT_ADMIN); // Todo verify password
  }
  return null;
};

export const generateLoginPage = async (app: FastifyInstance, adminjs: AdminJS): Promise<void> => {
  await AdminJSFastify.buildAuthenticatedRouter(
    adminjs,
    {
      authenticate,
      cookiePassword: appConfig.ADMINJS_COOKIE_SECRET,
      cookieName: 'adminjs',
    },
    app,
    {
      store: sessionStore,
      saveUninitialized: true,
      secret: appConfig.ADMINJS_COOKIE_SECRET,
      cookie: {
        httpOnly: process.env.NODE_ENV === 'production',
        secure: process.env.NODE_ENV === 'production',
      },
    }
  );
};
