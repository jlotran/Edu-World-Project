import { appConstance } from './contance/index.js';
import { appEnv } from './env/index.js';

export const appConfig = {
  ...appEnv,
  ...appConstance,
};
