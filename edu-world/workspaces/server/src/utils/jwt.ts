import jwt from 'jsonwebtoken';

import { appEnv } from '../core/env';

export const generateAccessToken = (payload: object, expiresIn = '365d'): string => {
  return jwt.sign(payload, appEnv.JWT_SECRET as jwt.Secret, { expiresIn: expiresIn as jwt.SignOptions['expiresIn'] });
};
