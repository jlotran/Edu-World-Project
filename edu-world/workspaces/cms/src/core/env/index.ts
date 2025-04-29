import { BooleanConvertor, Convertor, NumberConvertor, StringConvertor } from './convertor.js';

/**
 * Declare the environment variables
 */
export type EnvironmentType = {
  POSTGRES_CONNECTION_STRING: string;
  POSTGRES_DATABASE: string;
  ADMINJS_COOKIE_SECRET: string;
  ADMINJS_PORT: number;
  ADMINJS_DEFAULT_ADMIN_EMAIL: string;
  ADMINJS_DEFAULT_ADMIN_PASSWORD: string;
};

/**
 * Config environment variables is required, and datatype convertor
 */
const EnvironmentRequired: EnvironmentRequiredType = {
  POSTGRES_CONNECTION_STRING: [StringConvertor, true],
  POSTGRES_DATABASE: [StringConvertor, true],
  ADMINJS_COOKIE_SECRET: [StringConvertor, true],
  ADMINJS_PORT: [NumberConvertor, true],
  ADMINJS_DEFAULT_ADMIN_EMAIL: [StringConvertor, true],
  ADMINJS_DEFAULT_ADMIN_PASSWORD: [StringConvertor, true],
};

export type EnvironmentRequiredType = {
  [K in keyof EnvironmentType]: [Convertor<EnvironmentType[K]>, boolean];
};

const initAppEnv = (): EnvironmentType => {
  const appEnv = {} as EnvironmentType;
  const missingKeysRequired: string[] = [];
  const missingKeysOptional: string[] = [];
  for (let key in EnvironmentRequired) {
    const [convertor, required] = EnvironmentRequired[key as keyof EnvironmentRequiredType];
    const value = process.env[key];

    if (!value) {
      required ? missingKeysRequired.push(key) : missingKeysOptional.push(key);
    } else {
      appEnv[key as keyof EnvironmentRequiredType] = convertor(value) as never;
    }
  }
  if (missingKeysOptional.length > 0) {
    console.warn(`AppConfig: ${missingKeysOptional.join(', ')} is optional`);
  }
  if (missingKeysRequired.length > 0) {
    throw new Error(`AppConfig: ${missingKeysRequired.join(', ')} is required`);
  }

  return appEnv;
};

export const appEnv: EnvironmentType = initAppEnv();
