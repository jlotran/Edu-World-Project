import { FastifyPluginAsync, FastifyRequest } from 'fastify';

import { Clothes } from '../../entities/Clothes';
import { CreateClothesSchema, FindIdSchema, UpdateClothesSchema } from '../../middleware/Validator/ClothesValidator';
import { authMiddleware, validateRequestData } from '../../middleware/authMiddleware';
import { initializeRepositories } from '../../repository';
import { CreateClothesRequest, FindIdRequest, UpdateClothesRequest } from '../../request/Clothes';
import { successResponse } from '../../response/BaseReponse';
import { createClothes, deleteClothes, updateClothes } from '../../services/ClothesService';
import { SimpleRouter } from '../base';

export class ClothesRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }

  routes(): FastifyPluginAsync {
    return async (app, opts) => {
      initializeRepositories(app);
      app.post(
        '',
        {
          schema: {
            tags: ['Clothes'],
            body: {
              type: 'object',
              required: ['gender', 'category', 'rarity'],
              properties: {
                gender: { type: 'integer' },
                base_color: { type: 'string' },
                category: { type: 'string' },
                status: { type: 'integer' },
                price: { type: 'integer' },
                rarity: { type: 'string' },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(CreateClothesSchema)]
        },
        async (request: FastifyRequest<{ Body: CreateClothesRequest }>, reply) => {
          try {
            const newClothes: Clothes = await createClothes(request.body);
            reply.send(successResponse<Clothes>(newClothes));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.patch(
        '',
        {
          schema: {
            tags: ['Clothes'],
            querystring: {
              type: 'object',
              required: ['clothes_id'],
              properties: {
                clothes_id: { type: 'string' },
              },
            },
            body: {
              type: 'object',
              required: ['gender', 'base_color', 'category', 'rarity'],
              properties: {
                gender: { type: 'integer' },
                base_color: { type: 'string' },
                category: { type: 'string' },
                status: { type: 'integer' },
                price: { type: 'integer' },
                rarity: { type: 'string' },
              },
            },
          },
          preHandler: [
            authMiddleware,
            validateRequestData(UpdateClothesSchema)
          ],
        },
        async (request: FastifyRequest<{ Querystring:FindIdRequest, Body: UpdateClothesRequest }>, reply) => {
          try {
            const payload = {
              ...request.body,
              clothes_id: request.query.clothes_id
            }
            const data: Clothes = await updateClothes(payload);
            reply.send(successResponse<Clothes>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.delete(
        '',
        {
          schema: {
            tags: ['Clothes'],
            querystring: {
              type: 'object',
              required: ['clothes_id'],
              properties: {
                clothes_id: { type: 'string' },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(FindIdSchema)]
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest }>, reply) => {
          try {
            const data: Clothes = await deleteClothes(request.query.clothes_id);
            reply.send(successResponse<Clothes>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
    };
  }
}
