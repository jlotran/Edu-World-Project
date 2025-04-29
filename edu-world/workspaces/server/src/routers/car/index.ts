import { BaseRouter, BaseRouterGroup } from '../base';

import { CarRouter } from './CarRouter';

export class CarRouterGroup extends BaseRouterGroup {
  prefix(): string {
    return '/cars';
  }
  subRouters(): BaseRouter[] {
    return [new CarRouter(this.app)];
  }
}
