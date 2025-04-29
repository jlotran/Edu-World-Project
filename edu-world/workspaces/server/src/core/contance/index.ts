import { defaultConstance, DefaultConstanceType } from './DefaultContance';

export type AppConstanceType = DefaultConstanceType;

export const appConstance: AppConstanceType = {
  ...defaultConstance,
};
