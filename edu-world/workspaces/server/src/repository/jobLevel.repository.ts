import { FastifyInstance } from 'fastify';
import { Repository } from 'typeorm';

import { JobLevel } from '../entities/JobLevel';

let levelRepo: Repository<JobLevel>;

export const initJobLevelRepo = (app: FastifyInstance): void => {
  levelRepo = app.orm['typeorm'].getRepository(JobLevel);
};

export const createEmptyJobLevel = async (): Promise<JobLevel> => {
  return await levelRepo.save({});
};
