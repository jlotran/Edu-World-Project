import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { PlayerCar } from '../entities/PlayerCar';
import { PlayerClothes } from '../entities/PlayerClothes';

let playerClothesRepo: Repository<PlayerClothes>;
let playerCarRepo: Repository<PlayerCar>;

export const initPlayerInventoryRepo = (app: FastifyInstance): void => {
  const orm = app.orm;
  playerClothesRepo = orm['typeorm'].getRepository(PlayerClothes);
  playerCarRepo = orm['typeorm'].getRepository(PlayerCar);
};

export const findAllPlayerClothes = async (): Promise<PlayerClothes[]> => {
  return await playerClothesRepo.find({
    select: { id: true, equipped: true, is_deleted: true, custom_color: true },
    where: { is_deleted: 0 },
    relations: ['clothes_id', 'player_id'],
  });
};

export const findAllPlayerCars = async (): Promise<PlayerCar[]> => {
  return await playerCarRepo.find({
    select: { id: true, equipped: true, is_deleted: true },
    where: { is_deleted: 0 },
    relations: ['car_id', 'player_id'],
  });
};
