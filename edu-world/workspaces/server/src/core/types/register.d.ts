import { FastifyInstance } from 'fastify';

export type AppRegister = (app: FastifyInstance) => void;
