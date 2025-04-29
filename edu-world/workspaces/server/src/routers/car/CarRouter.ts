import { FastifyPluginAsync, FastifyRequest } from 'fastify';

import { Car } from '../../entities/Car';
import { CreateCarSchema, FindIdSchema, UpdateCarSchema } from '../../middleware/Validator/CarValidator';
import { authMiddleware, validateRequestData } from '../../middleware/authMiddleware';
import { initializeRepositories } from '../../repository';
import { CreateCarRequest, FindIdRequest, UpdateCarRequest } from '../../request/Car';
import { successResponse } from '../../response/BaseReponse';
import { createCar, deleteCar, updateCar } from '../../services/CarService';
import { SimpleRouter } from '../base';

export class CarRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }

  routes(): FastifyPluginAsync {
    return async (app, opts): Promise<void> => {
      initializeRepositories(app);
      app.post(
        '',
        {
          schema: {
            tags: ['Car'],
            body: {
              type: 'object',
              required: ['model_name', 'speed', 'handling', 'nitro', 'rarity', 'acceleration', 'car_code'],
              properties: {
                car_code: { type: 'string' },
                model_name: { type: 'string'},
                color: { type: 'string'},
                speed: { type: 'integer' },
                handling: { type: 'integer' },
                nitro: { type: 'integer' },
                price: { type: 'integer' },
                rarity: { type: 'string' },
                acceleration: { type: 'integer' },
              },
            },
          },
          preHandler: [
            authMiddleware,
            validateRequestData(CreateCarSchema)
          ],
        },
        async (request: FastifyRequest<{ Body: CreateCarRequest }>, reply) => {
          try {
            const newCar: Car = await createCar(request.body);
            reply.send(successResponse<Car>(newCar));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.patch(
        '',
        {
          schema: {
            tags: ['Car'],
            querystring: {
              type: 'object',
              required: ['car_id'],
              properties: {
                car_id: { type: 'string' },
              },
            },
            body: {
              type: 'object',
              required: ['model_name', 'speed', 'handling', 'nitro', 'rarity', 'acceleration', 'car_code'],
              properties: {
                car_code: { type: 'string' },
                model_name: { type: 'string'},
                color: { type: 'string'},
                speed: { type: 'integer' },
                handling: { type: 'integer' },
                nitro: { type: 'integer' },
                price: { type: 'integer' },
                rarity: { type: 'string' },
                acceleration: { type: 'integer' },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(UpdateCarSchema)]
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest; Body: UpdateCarRequest }>, reply) => {
          try {
            
            const {car_id} = request.query;
            const payload = {
              ...request.body,
              car_id,
            };
            
            const data: Car = await updateCar(payload);
            reply.send(successResponse<Car>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.delete(
        '',
        {
          schema: {
            tags: ['Car'],
            querystring: {
              type: 'object',
              required: ['car_id'],
              properties: {
                car_id: { type: 'string' },
              },
            },
          },
          preHandler: [
            authMiddleware,
            validateRequestData(FindIdSchema)
          ]
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest }>, reply) => {
          try {
            const data: Car = await deleteCar(request.query.car_id);
            reply.send(successResponse<Car>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
    };
  }
}
