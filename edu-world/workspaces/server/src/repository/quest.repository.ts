import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { Quest } from '../entities/Quest';

let questRepo: Repository<Quest>;

export const initQuestRepo = (app: FastifyInstance): void => {
  questRepo = app.orm['typeorm'].getRepository(Quest);
};

export const findQuestById = async (id: string): Promise<Quest | null> => {
  return await questRepo.findOne({ where: { id } });
};

export const saveQuest = async (quest: Quest): Promise<Quest> => {
  return await questRepo.save(quest);
};

export const createQuestEntity = (data: Partial<Quest>): Quest => {
  return questRepo.create(data);
};

export const deleteQuestEntity = async (quest: Quest): Promise<Quest> => {
  return await questRepo.remove(quest);
};
