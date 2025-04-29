import { appConstance } from './contance';
import { appEnv } from './env';

export const appConfig = {
  ...appEnv,
  ...appConstance,
};
