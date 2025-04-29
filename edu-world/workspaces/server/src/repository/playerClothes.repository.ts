import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { Clothes } from '../entities/Clothes';
import { PlayerClothes } from '../entities/PlayerClothes';
import { User } from '../entities/User';

let playerClothesRepo: Repository<PlayerClothes>;
let clothesRepo: Repository<Clothes>;
let playerRepo: Repository<User>;
export const initPlayerClothesRepo = (app: FastifyInstance): void => {
  const orm = app.orm;
  playerClothesRepo = orm['typeorm'].getRepository(PlayerClothes);
  clothesRepo = orm['typeorm'].getRepository(Clothes);
  playerRepo = orm['typeorm'].getRepository(User);
};

export const getAllPlayerClothes = async (deleted = false): Promise<PlayerClothes[]> => {
  return await playerClothesRepo.find({
    where: { is_deleted: deleted ? 1 : 0 },
    select: { id: true, equipped: true, is_deleted: true, custom_color: true },
    relations: ['clothes_id', 'player_id'],
  });
};

export const findPlayerClothes = async (player_id: string, clothes_id: string): Promise<PlayerClothes | null> => {
  return await playerClothesRepo
    .createQueryBuilder('player_clothes')
    .where('player_clothes.player_id = :playerId', { playerId: player_id })
    .andWhere('player_clothes.clothes_id = :clothesId', { clothesId: clothes_id })
    .andWhere('player_clothes.is_deleted = false')
    .getOne();
};

export const findPlayerClothesIsDel = async (player_id: string, clothes_id: string): Promise<PlayerClothes | null> => {
  return await playerClothesRepo
    .createQueryBuilder('player_clothes')
    .where('player_clothes.player_id = :playerId', { playerId: player_id })
    .andWhere('player_clothes.clothes_id = :clothesId', { clothesId: clothes_id })
    .andWhere('player_clothes.is_deleted = true')
    .getOne();
};

export const savePlayerClothes = async (data: Partial<PlayerClothes>): Promise<Partial<PlayerClothes> & PlayerClothes> => {
  return await playerClothesRepo.save(data);
};

export const clothesExists = async (clothes_id: string): Promise<Clothes | null> => {
  return await clothesRepo.findOne({ where: { clothes_id } });
};

export const findUserById = async (id: string): Promise<User | null> => {
  return await playerRepo.findOne({ where: { id } });
};
