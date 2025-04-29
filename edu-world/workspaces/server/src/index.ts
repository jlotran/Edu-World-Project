import 'reflect-metadata';
import helmet from '@fastify/helmet';
import rateLimit from '@fastify/rate-limit';
import fastifySwagger from '@fastify/swagger';
import fastifySwaggerUi from '@fastify/swagger-ui';
import Fastify from 'fastify';

import { appConfig } from './core';
import { postgresRegister } from './core/database/postgres';
import { logger } from './core/logger';
import { AppRegister } from './core/types/register';
import { registerRouter } from './routers';

const swaggerOptions = {
  swagger: {
    info: {
      title: 'My Title',
      description: 'My Description.',
      version: '1.0.0',
    },
    schemes: ['http', 'https'],
    consumes: ['application/json'],
    produces: ['application/json'],
    securityDefinitions: {
      BearerAuth: {
        type: 'apiKey' as const,
        name: 'Authorization',
        in: 'header',
        description: 'Enter token with format: Bearer <your_token_here>',
        bearerFormat: 'JWT',
      },
    },
    security: [{ BearerAuth: [] }],
  },
};

const swaggerUiOptions = {
  routePrefix: '/docs',
  exposeRoute: true,
};

async function main(): Promise<void> {
  const app = Fastify({
    logger: {
      transport: {
        target: 'pino-pretty',
        options: {
          colorize: true,
          translateTime: 'yyyy-mm-dd HH:MM:ss.l o',
          ignore: 'pid,hostname',
        },
      },
    },
  });

  // ✅ Register bảo mật helmet
  await app.register(helmet, {
    hidePoweredBy: true,
    frameguard: { action: 'sameorigin' },
    contentSecurityPolicy: false,
  });

  // ✅ Register giới hạn rate-limit
  await app.register(rateLimit, {
    max: 200, // tối đa 200 request
    timeWindow: 3 * 60 * 1000, // 3 phút
    errorResponseBuilder: () => {
      return {
        statusCode: 429,
        error: 'Too Many Requests',
        message: 'Bạn đã gửi quá nhiều yêu cầu. Vui lòng thử lại sau 3 phút.',
      };
    },
  });

  // Swagger docs
  await app.register(fastifySwagger, swaggerOptions);
  await app.register(fastifySwaggerUi, swaggerUiOptions);

  // Database + Router
  await postgresRegister(app);
  await registerRouter(app);

  try {
    await app.listen({ port: appConfig.PORT, host: '0.0.0.0' });
  } catch (error) {
    logger.error(error);
    process.exit(1);
  }
}

main();