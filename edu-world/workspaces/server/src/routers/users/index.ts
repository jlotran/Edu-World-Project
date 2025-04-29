import { BaseRouter, BaseRouterGroup } from '../base';

import { UsersRouter } from './UserRouter';

export class UserRouterGroup extends BaseRouterGroup {

  prefix(): string {
    return '/users';
  }

  subRouters(): BaseRouter[] {
    return [new UsersRouter(this.app)];
  }
}
