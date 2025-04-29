import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { Car } from '../entities/Car';
import { Clothes } from '../entities/Clothes';
import { Quest } from '../entities/Quest';

let clothesRepo: Repository<Clothes>;
let carRepo: Repository<Car>;
let questRepo: Repository<Quest>;

export const initMasterRepo = (app: FastifyInstance): void => {
  const orm = app.orm;
  clothesRepo = orm['typeorm'].getRepository(Clothes);
  carRepo = orm['typeorm'].getRepository(Car);
  questRepo = orm['typeorm'].getRepository(Quest);
};

export const getAllCars = async (): Promise<Car[]> => carRepo.find();
export const getAllClothes = async (): Promise<Clothes[]> => clothesRepo.find();
export const getAllQuests = async (): Promise<Quest[]> => questRepo.find();
