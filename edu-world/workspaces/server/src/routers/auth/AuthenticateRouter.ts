import { FastifyPluginAsync, FastifyRequest } from 'fastify';

import { initializeRepositories } from '../../repository';
import { AuthLoginRequest } from '../../request/AuthPayload';
import { successResponse } from '../../response/BaseReponse';
import { loginUser } from '../../services/AuthService';
import { SimpleRouter } from '../base';

export class AuthenticateRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }

  routes(): FastifyPluginAsync {
    return async (app, opts) => {
      initializeRepositories(app);

      // Login handler
      app.post(
        '/login',
        {
          schema: {
            tags: ['Auth'],
            body: {
              type: 'object',
              required: ['token'],
              properties: {
                token: { type: 'string' },
              },
            },
          },
        },
        async (request: FastifyRequest<{ Body: AuthLoginRequest }>, reply) => {
          try {
            const data: { accessToken: string } = await loginUser(request.body);
            reply.send(successResponse<{ accessToken: string }>(data));
          } catch (error) {
            reply.status(500).send({ error: (error as Error).message });
          }
        }
      );
      // Logout handler
      app.get(
        '/logout',
        {
          schema: {
            tags: ['Auth'],
            params: {
              type: 'object',
              properties: {
                token: { type: 'string' },
              },
            },
          },
        },
        async (request, reply) => {
          return {
            
          };
        }
      );
    };
  }
}
