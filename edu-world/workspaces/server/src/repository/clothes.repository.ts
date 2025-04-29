import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { Clothes } from '../entities/Clothes';

let clothesRepo: Repository<Clothes>;

export const initClothesRepo = (app: FastifyInstance): void => {
  clothesRepo = app.orm['typeorm'].getRepository(Clothes);
};

export const findClothesById = async (clothes_id: string): Promise<Clothes | null> => {
  return await clothesRepo.findOne({ where: { clothes_id: clothes_id } });
};

export const saveClothes = async (clothes: Clothes): Promise<Clothes> => {
  return await clothesRepo.save(clothes);
};

export const createClothesEntity = (data: Partial<Clothes>): Clothes => {
  return clothesRepo.create(data);
};

export const deleteClothesEntity = async (clothes: Clothes): Promise<Clothes> => {
  return await clothesRepo.remove(clothes);
};
