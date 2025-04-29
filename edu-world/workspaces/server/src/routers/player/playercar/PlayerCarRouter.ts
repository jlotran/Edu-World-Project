import { FastifyPluginAsync, FastifyRequest } from 'fastify';


import { PlayerCar } from '../../../entities/PlayerCar';
import ValidationError from '../../../error/ValidationError';
import { CreatePlayerCarSchema, FindIdSchema, UpdatePlayerCarSchema } from '../../../middleware/Validator/player/PlayCarValidator';
import { authMiddleware, validateRequestData } from '../../../middleware/authMiddleware';
import { initializeRepositories } from '../../../repository';
import { CreatePlayerCarRequest, FindIdRequest, UpdatePlayerCarRequest } from '../../../request/PlayerCar';
import { successResponse } from '../../../response/BaseReponse';
import { createPlayerCar, deletePlayerCar, getPlayerCar, getPlayerCarIsDelete, restorePlayerCar, updatePlayerCar } from '../../../services/PlayerCarService';
import { SimpleRouter } from '../../base';
export class PlayerCarRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }

  routes(): FastifyPluginAsync {
    return async (app) => {
      initializeRepositories(app);
      app.get('', { schema: { tags: ['Player Car'] }, preHandler: authMiddleware }, async (request, reply) => {
        try {
          const data: PlayerCar[] = await getPlayerCar();
          reply.send(successResponse<PlayerCar[]>(data));
        } catch (error) {
          reply.send(error);
        }
      });
      app.get('/deleted', { schema: { tags: ['Player Car'] }, preHandler: authMiddleware }, async (request, reply) => {
        try {
          const data: PlayerCar[] = await getPlayerCarIsDelete();
          reply.send(successResponse<PlayerCar[]>(data));
        } catch (error) {
          reply.send(error);
        }
      });
      app.post(
        '',
        {
          schema: {
            tags: ['Player Car'],
            body: {
              type: 'object',
              required: ['car_id'],
              properties: {
                car_id: { type: 'string' },
                equipped: {
                  type: 'boolean',
                  enum: [true, false],
                },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(CreatePlayerCarSchema)],
        },
        async (request: FastifyRequest<{ Body: CreatePlayerCarRequest }>, reply) => {
          try {
            const user = request.user;
            request.body.player_id = user?.id;
            const data = await createPlayerCar(request.body);
            reply.send(successResponse<PlayerCar>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.patch(
        '',
        {
          schema: {
            tags: ['Player Car'],
            querystring: {
              type: 'object',
              required: ['car_id'],
              properties: {
                car_id: { type: 'string', description: 'ID của xe cần cập nhật'},
              },
            },
            body: {
              type: 'object',
              properties: {
                equipped: {
                  type: 'boolean',
                  enum: [true, false],
                },
              },
            }
          },
          preHandler: [authMiddleware, validateRequestData(UpdatePlayerCarSchema)],
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest, Body: UpdatePlayerCarRequest }>, reply) => {
          try {
            const { car_id } = request.query;
            const user = request.user;
            const payload = {
              ...request.body,
              car_id: car_id,
              player_id: user?.id,
            };
            
            const data: PlayerCar = await updatePlayerCar(payload);
            reply.send(successResponse<PlayerCar>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.delete(
        '',
        {
          schema: {
            tags: ['Player Car'],
            querystring: {
              type: 'object',
              required: ['car_id'],
              properties: {
                car_id: { type: 'string', description: 'ID của xe cần xóa' },
              },
            }
          },
          preHandler: [authMiddleware, validateRequestData(FindIdSchema)]
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest }>, reply) => {
          try {
            const { car_id } = request.query;
            const user = request.user;

            const payload = {
              player_id: user?.id,
              car_id: car_id,
            }

            const data: PlayerCar = await deletePlayerCar(payload);
            reply.send(successResponse<PlayerCar>(data));
            return payload;
          } catch (error) {
            reply.send(error);
          }
        }

      );
      app.post(
        '/:car_id/restore',
        {
          schema: {
            tags: ['Player Car'],
            params: {
              type: 'object',
              required: ['car_id'],
              properties: {
                car_id: { type: 'string', description: 'ID của xe cần khôi phục' },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(FindIdSchema)],
        },
        async (request: FastifyRequest<{ Params: FindIdRequest}>, reply) => {
          try {
            const { car_id } = request.params;
            const user = request.user;

            const payload = {
              player_id: user?.id,
              car_id: car_id,
            }

            const data: PlayerCar = await restorePlayerCar(payload);
            reply.send(successResponse<PlayerCar>(data));
          } catch (error) {
            reply.send(error);
          }

        }
      );
    };
  }
}
