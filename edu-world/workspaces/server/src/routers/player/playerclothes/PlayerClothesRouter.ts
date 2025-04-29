import { FastifyPluginAsync, FastifyRequest } from 'fastify';

import { PlayerClothes } from '../../../entities/PlayerClothes';
import ValidationError from '../../../error/ValidationError';
import { CreatePlayerClothesSchema, FindIdSchema, UpdatePlayerClothesSchema } from '../../../middleware/Validator/player/PlayClothesValidator';
import { authMiddleware, validateRequestData } from '../../../middleware/authMiddleware';
import { initializeRepositories } from '../../../repository';
import { CreatePlayerClothesRequest, FindIdRequest, UpdatePlayerClothesRequest } from '../../../request/PlayerClothes';
import { successResponse } from '../../../response/BaseReponse';
import { createPlayerClothes, deletePlayerClothes, getPlayerClothes, getPlayerClothesIsDelete, restorePlayerClothes, updatePlayerClothes } from '../../../services/PlayerClothesService';
import { SimpleRouter } from '../../base';



export class PlayerClothesRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }

  routes(): FastifyPluginAsync {
    return async (app) => {
      initializeRepositories(app);
      app.get(
        '',
        {
          schema: {
            tags: ['Player Clothes'],
          },
          preHandler: authMiddleware,
        },
        async (request, reply) => {
          try {
            const data: PlayerClothes[] = await getPlayerClothes();
            reply.send(successResponse<PlayerClothes[]>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.get(
        '/deleted',
        {
          schema: {
            tags: ['Player Clothes'],
          },
          preHandler: authMiddleware,
        },
        async (request, reply) => {
          try {
            const data: PlayerClothes[] = await getPlayerClothesIsDelete();
            reply.send(successResponse<PlayerClothes[]>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.post(
        '',
        {
          schema: {
            tags: ['Player Clothes'],
            body: {
              type: 'object',
              required: ['clothes_id'],
              properties: {
                car_id: { type: 'string' },
                custom_color: {type: 'string'},
                equipped: {
                  type: 'boolean',
                  enum: [true, false],
                },
              },
            }
          },
          preHandler: [authMiddleware, validateRequestData(CreatePlayerClothesSchema)]
        },
        async (request: FastifyRequest<{ Body: CreatePlayerClothesRequest }>, reply) => {
          try {
            const user = request.user;
            const payload = {
              ...request.body,
              player_id: user?.id,
            };
            const data: PlayerClothes = await createPlayerClothes(payload);
            reply.send(successResponse<PlayerClothes>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.patch(
        '',
        {
          schema: {
            tags: ['Player Clothes'],
            querystring: {
              type: 'object',
              required: ['clothes_id'],
              properties: {
                clothes_id: { type: 'string' },
              },
            },
            body: {
              type: 'object',
              properties: {
                custom_color: {type: 'string'},
                equipped: {
                  type: 'boolean',
                  enum: [true, false],
                },
              },
            }
          },
          preHandler: [authMiddleware, validateRequestData(UpdatePlayerClothesSchema)],
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest, Body: UpdatePlayerClothesRequest }>, reply) => {
          try {
            const { clothes_id } = request.query;

            const user = request.user;
            const payload = {
              ...request.body,
              clothes_id: clothes_id,
              player_id: user?.id,
            };
            
            const data: PlayerClothes = await updatePlayerClothes(payload);
            reply.send(successResponse<PlayerClothes>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.delete(
        '',
        {
          schema: {
            tags: ['Player Clothes'],
            querystring: {
              type: 'object',
              required: ['clothes_id'],
              properties: {
                clothes_id: { type: 'string'},
              },
            }
          },
          preHandler: [authMiddleware, validateRequestData(FindIdSchema)],
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest }>, reply) => {
          try {
            const { clothes_id } = request.query;
            const user = request.user;
            
            const payload = {
              player_id: user?.id,
              clothes_id: clothes_id,
            }

            const data: PlayerClothes = await deletePlayerClothes(payload);
            reply.send(successResponse<PlayerClothes>(data));
            return payload;
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.post(
        '/:clothes_id/restore',
        {
          schema: {
            tags: ['Player Clothes'],
            params: {
              type: 'object',
              required: ['clothes_id'],
              properties: {
                clothes_id: { type: 'string'},
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(FindIdSchema)],
        },
        async (request: FastifyRequest<{ Params: FindIdRequest}>, reply) => {
          try {
            const { clothes_id } = request.params;
            const user = request.user;

            const payload = {
              player_id: user?.id,
              clothes_id: clothes_id,
            }

            const data: PlayerClothes = await restorePlayerClothes(payload);
            reply.send(successResponse<PlayerClothes>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
    };
  }
}
