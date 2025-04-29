import { FastifyPluginAsync, FastifyRequest } from 'fastify';
import httpStatus from 'http-status';

import { ProfileUserDTO } from '../../dto/user/UserDTO';
import { UpdateProfileSchema } from '../../middleware/Validator/UserValidator';
import { authMiddleware, validateRequestData } from '../../middleware/authMiddleware';
import { initializeRepositories } from '../../repository';
import { UpdateProfileUser } from '../../request/User';
import { successResponse } from '../../response/BaseReponse';
import { getDetailUser, updateProfileUser } from '../../services/UserService';
import { SimpleRouter } from '../base';

export class UsersRouter extends SimpleRouter {
  prefix(): string {
    return '';
  }

  routes(): FastifyPluginAsync {
    return async (app) => {
      initializeRepositories(app); // Khởi tạo repository khi ứng dụng chạy
      app.get('/mine', { schema: { tags: ['User'] }, preHandler: authMiddleware }, async (request, reply) => {
        try {
          const user = request.user;

          if (!user) {
            return reply.status(httpStatus.UNAUTHORIZED).send({ error: 'User not authenticated' });
          }

          const data: ProfileUserDTO = await getDetailUser(user.id);

          reply.send(successResponse<ProfileUserDTO>(data));
        } catch (error) {
          reply.status(httpStatus.INTERNAL_SERVER_ERROR).send(error);
        }
      });

      app.post(
        '/update-profile',
        { schema: { tags: ['User'] }, preHandler: [authMiddleware, validateRequestData(UpdateProfileSchema)] },
        async (request: FastifyRequest<{ Body: UpdateProfileUser }>, reply) => {
          try {
            const user = request.user;

            if (!user) {
              return reply.status(httpStatus.UNAUTHORIZED).send({ error: 'User not authenticated' });
            }
            const payload = {
              ...request.body,
              id: user.id,
            };
            const data: ProfileUserDTO = await updateProfileUser(payload);
            reply.send(successResponse<ProfileUserDTO>(data));
          } catch (error) {
            reply.status(httpStatus.INTERNAL_SERVER_ERROR).send(error);
          }
        }
      );
    };
  }
}
