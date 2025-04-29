import { AppRegister } from '../core/types';

import { AuthRouterRouterGroup } from './auth';
import { BaseRouterGroup } from './base';
import { CarRouterGroup } from './car';
import { ClothesRouterGroup } from './clothes';
import { MasterRouterGroup } from './master';
import { PlayerCarRouterGroup } from './player/playercar';
import { PlayerClothesRouterGroup } from './player/playerclothes';
import { PlayerInventoryRouterGroup } from './player/playerinventory';
import { QuestRouterGroup } from './quest';
import { UserRouterGroup } from './users';

export const registerRouter: AppRegister = (app) => {
  const basePrefix = '/api/v1';

  const routerGroup: BaseRouterGroup[] = [
    new UserRouterGroup(app),
    new AuthRouterRouterGroup(app),
    new ClothesRouterGroup(app),
    new CarRouterGroup(app),
    new QuestRouterGroup(app),
    new PlayerCarRouterGroup(app),
    new PlayerInventoryRouterGroup(app),
    new PlayerClothesRouterGroup(app),
    new MasterRouterGroup(app),
  ];

  routerGroup.forEach((group) => {
    app.register(group.routes(), { prefix: basePrefix + group.prefix() });
  });
};
