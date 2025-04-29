
import { BaseRouter, BaseRouterGroup } from '../../base';

import { PlayerCarRouter } from './PlayerCarRouter';

export class PlayerCarRouterGroup extends BaseRouterGroup {

  prefix(): string {
    return '/player-car';
  }

  subRouters(): BaseRouter[] {
    return [new PlayerCarRouter(this.app)];
  }
}
