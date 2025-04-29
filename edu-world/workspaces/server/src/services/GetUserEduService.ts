import ValidationError from '../error/ValidationError';
import { AuthLoginRequest } from '../request/AuthPayload';
export class GetUserEduService {
  async oauth(req: AuthLoginRequest): Promise<{ username: string }> {
    try {
      return {
        username: 'phungvanmanh',
      };
    } catch (error) {
      throw new ValidationError(error instanceof Error ? error.message : String(error));
    }
  }
}
