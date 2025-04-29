/* eslint-disable @typescript-eslint/no-explicit-any */
import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { User } from '../entities/User';

let userRepo: Repository<User>;

export const initUserRepo = (app: FastifyInstance): void => {
  userRepo = app.orm['typeorm'].getRepository(User);
};

export const findUserByUsername = async (username: string): Promise<User| null> => {
  return await userRepo.findOne({ where: { username } });
};

export const createUser = async (username: string, level: any): Promise<User> => {
  const newUser = userRepo.create({ username, level });
  return await userRepo.save(newUser);
};

export const findUserById = async (id: string): Promise<User| null> => {
  return await userRepo.findOne({ where: { id } });
};

export const saveUser = async (user: User): Promise<User> => {
  return await userRepo.save(user);
};
