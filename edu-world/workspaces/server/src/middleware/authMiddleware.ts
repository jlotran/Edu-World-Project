/* eslint-disable @typescript-eslint/no-explicit-any */
import { FastifyRequest, FastifyReply } from 'fastify';
import jwt, { JwtPayload } from 'jsonwebtoken';

import { appEnv } from '../core/env';
import NotAuthorized from '../error/NotAuthoried';
import ValidationError from '../error/ValidationError';
import { validateRequest } from '../utils/validateRequest';
// Mở rộng kiểu dữ liệu để thêm user vào request
declare module 'fastify' {
  interface FastifyRequest {
    user?: JwtPayload;
  }
}

export const authMiddleware = async (request: FastifyRequest, reply: FastifyReply): Promise<void> => {
  try {
    const authHeader = request.headers.authorization;

    if (!authHeader || !authHeader.startsWith('Bearer ')) {
      throw new NotAuthorized('Unauthorized: Token is missing or invalid');
    }
    
    const token = authHeader.split(' ')[1];

    try {
      const decoded = jwt.verify(token, appEnv.JWT_SECRET) as JwtPayload;

      if (!decoded) {
        throw new NotAuthorized('Unauthorized: Invalid token');
      }

      // Gán thông tin user vào request
      request.user = decoded;
    } catch (err) {
      throw new NotAuthorized('Unauthorized: Invalid or expired token');
    }
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'Internal Server Error';
    reply.status(error instanceof NotAuthorized ? 401 : 500).send({
      error: errorMessage,
    });
  }
};

export const validateRequestData = (rules: Record<string, any>) => {
  return async (req: FastifyRequest): Promise<void> => {
    try {
      const data = {
        ...(typeof req.body === 'object' && req.body !== null ? req.body : {}),
        ...(typeof req.query === 'object' && req.query !== null ? req.query : {}),
        ...(typeof req.params === 'object' && req.params !== null ? req.params : {}),
      };
      validateRequest(data as Record<string, any>, rules);
    } catch (err) {
      // Re-throw để Fastify error handler bắt
      if (err instanceof Error) {
        throw new ValidationError(err.message);
      }
      throw new ValidationError('An unknown error occurred');
    }
  };
};
