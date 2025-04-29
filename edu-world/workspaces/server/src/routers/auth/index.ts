import { BaseRouter, BaseRouterGroup } from '../base';

import { AuthenticateRouter } from './AuthenticateRouter';

export class AuthRouterRouterGroup extends BaseRouterGroup {
  prefix(): string {
    return '/auth';
  }
  subRouters(): BaseRouter[] {
    return [new AuthenticateRouter(this.app)];
  }
}
