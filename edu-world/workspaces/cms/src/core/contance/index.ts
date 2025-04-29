import { defaultConstance, DefaultConstanceType } from './DefaultContance.js';

export type AppConstanceType = DefaultConstanceType;

export const appConstance: AppConstanceType = {
  ...defaultConstance,
};
