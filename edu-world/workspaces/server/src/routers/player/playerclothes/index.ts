
import { BaseRouter, BaseRouterGroup } from '../../base';

import { PlayerClothesRouter } from './PlayerClothesRouter';

export class PlayerClothesRouterGroup extends BaseRouterGroup {

  prefix(): string {
    return '/player-clothes';
  }

  subRouters(): BaseRouter[] {
    return [new PlayerClothesRouter(this.app)];
  }
}
