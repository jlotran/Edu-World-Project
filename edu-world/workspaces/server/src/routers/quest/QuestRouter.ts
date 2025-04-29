import { FastifyPluginAsync, FastifyRequest } from 'fastify';

import { Quest } from '../../entities/Quest';
import { CreateQuestSchema, FindIdSchema, UpdateQuestSchema } from '../../middleware/Validator/QuestValidator';
import { authMiddleware, validateRequestData } from '../../middleware/authMiddleware';
import { initializeRepositories } from '../../repository';
import { CreateQuestRequest, FindIdRequest, UpdateQuestRequest } from '../../request/Quest';
import { successResponse } from '../../response/BaseReponse';
import { createQuest, deleteQuest, updateQuest } from '../../services/QuestService';
import { SimpleRouter } from '../base';

export class QuestRouter extends SimpleRouter {
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
            tags: ['Quest'],
            body: {
              type: 'object',
              required: ['name', 'description', 'reward_exp', 'currency','time_finish'],
              properties: {
                name: { type: 'string' },
                description: { type: 'string' },
                reward_exp: { type: 'integer' },
                currency: { type: 'number' },
                item: { type: 'string' },
                time_finish: { type: 'string' },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(CreateQuestSchema)]
        },
        async (request: FastifyRequest<{ Body: CreateQuestRequest }>, reply) => {
          try {
            const newQuest: Quest = await createQuest(request.body);
            reply.send(successResponse<Quest>(newQuest));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.patch(
        '',
        {
          schema: {
            tags: ['Quest'],
            querystring: {
              type: 'object',
              required: ['quest_id'],
              properties: {
                quest_id: { type: 'string' },
              },
            },
            body: {
              type: 'object',
              required: ['name', 'description', 'reward_exp', 'currency','time_finish'],
              properties: {
                name: { type: 'string' },
                description: { type: 'string' },
                reward_exp: { type: 'integer' },
                currency: { type: 'number' },
                item: { type: 'string' },
                time_finish: { type: 'string' },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(UpdateQuestSchema)],
        },
        async (request: FastifyRequest<{ Querystring:FindIdRequest, Body: UpdateQuestRequest }>, reply) => {
          try {
            const payload = {
              ...request.body,
              quest_id: request.query.quest_id
            }
            const data: Quest = await updateQuest(payload);
            reply.send(successResponse<Quest>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
      app.delete(
        '',
        {
          schema: {
            tags: ['Quest'],
            querystring: {
              type: 'object',
              required: ['quest_id'],
              properties: {
                quest_id: { type: 'string' },
              },
            },
          },
          preHandler: [authMiddleware, validateRequestData(FindIdSchema)],
        },
        async (request: FastifyRequest<{ Querystring: FindIdRequest }>, reply) => {
          try {
            const data: Quest = await deleteQuest(request.query.quest_id);
            reply.send(successResponse<Quest>(data));
          } catch (error) {
            reply.send(error);
          }
        }
      );
    };
  }
}
