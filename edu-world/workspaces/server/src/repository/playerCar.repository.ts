import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { Car } from '../entities/Car';
import { PlayerCar } from '../entities/PlayerCar';
import { User } from '../entities/User';

let playerCarRepo: Repository<PlayerCar>;
let carRepo: Repository<Car>;
let playerRepo: Repository<User>;

export const initPlayerCarRepo = (app: FastifyInstance): void => {
  const orm = app.orm;
  playerCarRepo = orm['typeorm'].getRepository(PlayerCar);
  carRepo = orm['typeorm'].getRepository(Car);
  playerRepo = orm['typeorm'].getRepository(User);
};

export const getAllPlayerCars = async (deleted = false): Promise<PlayerCar[]> => {
  return await playerCarRepo.find({
    where: { is_deleted: deleted ? 1 : 0 },
    select: { id: true, equipped: true, is_deleted: true },
    relations: ['car_id', 'player_id'],
  });
};

export const findPlayerCar = async (player_id: string, car_id: string): Promise<PlayerCar | null> => {
  return await playerCarRepo
    .createQueryBuilder('player_car')
    .where('player_car.player_id = :playerId', { playerId: player_id })
    .andWhere('player_car.car_id = :carId', { carId: car_id })
    .andWhere('player_car.is_deleted = false')
    .getOne();
};

export const findPlayerCarIsDel = async (player_id: string, car_id: string): Promise<PlayerCar | null> => {
  return await playerCarRepo
    .createQueryBuilder('player_car')
    .where('player_car.player_id = :playerId', { playerId: player_id })
    .andWhere('player_car.car_id = :carId', { carId: car_id })
    .andWhere('player_car.is_deleted = true')
    .getOne();
};

export const savePlayerCar = async (data: Partial<PlayerCar>): Promise<Partial<PlayerCar> & PlayerCar> => {
  return await playerCarRepo.save(data);
};

export const carExists = async (car_id: string): Promise<Car | null> => {
  return await carRepo.findOne({ where: { car_id } });
};

export const findUserById = async (id: string): Promise<User | null> => {
  return await playerRepo.findOne({ where: { id } });
};
