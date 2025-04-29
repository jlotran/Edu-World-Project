
import { BaseRouter, BaseRouterGroup } from '../../base';

import { PlayerInventoryRouter } from './PlayerInventoryRouter';


export class PlayerInventoryRouterGroup extends BaseRouterGroup {
  prefix(): string {
    return '/player-inventory';
  }

  subRouters(): BaseRouter[] {
    const app = this.getApp();
    return [new PlayerInventoryRouter(app)];
  }
}
