import { BaseRouter, BaseRouterGroup } from '../base';

import { MasterRouter } from './MasterRouter';

export class MasterRouterGroup extends BaseRouterGroup {
  prefix(): string {
    return '/master';
  }

  subRouters(): BaseRouter[] {
    const app = this.getApp();
    return [new MasterRouter(app)];
  }
}
