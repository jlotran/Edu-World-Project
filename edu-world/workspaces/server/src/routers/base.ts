import { FastifyPluginAsync } from 'fastify';
import { FastifyInstance } from 'fastify/types/instance';

export interface BaseRouter {
  routes: () => FastifyPluginAsync;
  prefix: () => string;
  getApp: () => FastifyInstance;
}

export abstract class BaseRouterGroup implements BaseRouter {
  protected app: FastifyInstance;

  constructor(app: FastifyInstance) {
    this.app = app;
  }

  abstract subRouters(): BaseRouter[];

  getApp(): FastifyInstance {
    return this.app;
  }

  prefix(): string {
    return '';
  }

  routes(): FastifyPluginAsync {
    const router: FastifyPluginAsync = async (app, config) => {
      this.subRouters().forEach((subRouter) => {
        app.register(subRouter.routes(), { prefix: subRouter.prefix() });
      });
    };
    return router;
  }
}

export abstract class SimpleRouter implements BaseRouter {
  protected app: FastifyInstance; // Chỉnh sửa từ `private` thành `protected`
  
  constructor(app: FastifyInstance) {
    this.app = app;
  }

  abstract routes(): FastifyPluginAsync;
  abstract prefix(): string;

  getApp(): FastifyInstance {
    return this.app;
  }
}
