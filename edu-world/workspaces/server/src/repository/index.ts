import { FastifyInstance } from 'fastify';

import { initCarRepo } from './car.repository';
import { initClothesRepo } from './clothes.repository';
import { initJobLevelRepo } from './jobLevel.repository';
import { initMasterRepo } from './master.repository';
import { initPlayerCarRepo } from './playerCar.repository';
import { initPlayerClothesRepo } from './playerClothes.repository';
import { initPlayerInventoryRepo } from './playerInventory.repository';
import { initQuestRepo } from './quest.repository';
import { initUserRepo } from './user.repository';


export const initializeRepositories = (app: FastifyInstance): void => {
    initUserRepo(app);
    initJobLevelRepo(app);
    initCarRepo(app);
    initClothesRepo(app);
    initQuestRepo(app);
    initMasterRepo(app);
    initPlayerCarRepo(app);
    initPlayerClothesRepo(app);
    initPlayerInventoryRepo(app);
};
