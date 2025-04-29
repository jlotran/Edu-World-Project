import { Convertor, NumberConvertor, StringConvertor } from './convertor';

/**
 * Declare the environment variables
 */
export type EnvironmentType = {
  PORT: number;
  POSTGRES_CONNECTION_STRING: string;
  POSTGRES_HOST: string;
  POSTGRES_PORT: number;
  POSTGRES_USERNAME: string;
  POSTGRES_PASSWORD: string;
  POSTGRES_DATABASE: string;
  JWT_SECRET: string;
};

/**
 * Config environment variables is required, and datatype convertor
 */
const EnvironmentRequired: EnvironmentRequiredType = {
  POSTGRES_CONNECTION_STRING: [StringConvertor, true],
  PORT: [NumberConvertor, true],
  POSTGRES_HOST: [StringConvertor, true],
  POSTGRES_PORT: [NumberConvertor, true],
  POSTGRES_USERNAME: [StringConvertor, true],
  POSTGRES_PASSWORD: [StringConvertor, true],
  POSTGRES_DATABASE: [StringConvertor, true],
  JWT_SECRET: [StringConvertor, true]
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
