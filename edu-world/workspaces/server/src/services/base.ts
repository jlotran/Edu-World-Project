import { FastifyInstance } from 'fastify';

export class BaseService {
  protected app: FastifyInstance;
  private static instance: BaseService;

  constructor(app: FastifyInstance) {
    if (BaseService.instance) {
      return BaseService.instance;
    }
    this.app = app;
    BaseService.instance = this;
  }
}
