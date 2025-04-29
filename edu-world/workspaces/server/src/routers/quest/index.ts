import { BaseRouter, BaseRouterGroup } from '../base';

import { QuestRouter } from './QuestRouter';

export class QuestRouterGroup extends BaseRouterGroup {
  prefix(): string {
    return '/quests';
  }

  subRouters(): BaseRouter[] {
    const app = this.getApp();
    return [new QuestRouter(app)];
  }
}
