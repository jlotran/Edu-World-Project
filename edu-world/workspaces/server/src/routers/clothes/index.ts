import { BaseRouter, BaseRouterGroup } from '../base';

import { ClothesRouter } from './ClothesRouter';

export class ClothesRouterGroup extends BaseRouterGroup {
  prefix(): string {
    return '/clothes';
  }
  subRouters(): BaseRouter[] {
    return [new ClothesRouter(this.app)];
  }
}
