import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { Car } from '../entities/Car';

let carRepo: Repository<Car>;

export const initCarRepo = (app: FastifyInstance): void => {
  carRepo = app.orm['typeorm'].getRepository(Car);
};

export const findCarById = async (car_id: string): Promise<Car | null> => {
  return await carRepo.findOne({ where: { car_id: car_id } });
};

export const saveCar = async (car: Car): Promise<Car> => {
  return await carRepo.save(car);
};

export const createCarEntity = (data: Partial<Car>): Car => {
  return carRepo.create(data);
};

export const deleteCarEntity = async (car: Car): Promise<Car> => {
  return await carRepo.remove(car);
};
