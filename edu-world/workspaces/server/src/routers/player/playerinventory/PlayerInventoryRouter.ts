/* eslint-disable @typescript-eslint/no-explicit-any */
import { FastifyPluginAsync } from 'fastify';

import { authMiddleware } from '../../../middleware/authMiddleware';
import { initializeRepositories } from '../../../repository';
import { successResponse } from '../../../response/BaseReponse';
import { getPlayerInventory } from '../../../services/PlayerInventoryService';
import { SimpleRouter } from '../../base';


export class PlayerInventoryRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }
  routes(): FastifyPluginAsync {
    return async (app, opts) => {
      initializeRepositories(app);
      app.get('', { schema: { tags: ['Player Inventory'] }, preHandler: authMiddleware }, async (request, reply) => {
        try {
          const data = await getPlayerInventory();
          reply.send(successResponse<any>(data));
        } catch (error) {
          reply.send(error)
        }
      });
    };
  }
}
