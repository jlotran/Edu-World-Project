/* eslint-disable @typescript-eslint/no-explicit-any */
import { FastifyPluginAsync } from 'fastify';

import { initializeRepositories } from '../../repository';
import { successResponse } from '../../response/BaseReponse';
import { getMaster } from '../../services/MasterService';
import { SimpleRouter } from '../base';

export class MasterRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }
  routes(): FastifyPluginAsync {
    return async (app, opts) => {
      initializeRepositories(app);
      app.get('', { schema: { tags: ['Master'] } }, async (request, reply) => {
        try {
          const data = await getMaster();
          reply.send(successResponse<any>(data));
        } catch (error) {
          reply.send(error)
        }
      });
    };
  }
}
