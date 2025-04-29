import ValidationError from "../error/ValidationError";
import { createEmptyJobLevel } from "../repository/jobLevel.repository";
import { createUser, findUserByUsername } from "../repository/user.repository";
import { AuthLoginRequest } from "../request/AuthPayload";
import { generateAccessToken } from "../utils/jwt";

import { GetUserEduService } from "./GetUserEduService";

export const loginUser = async (request: AuthLoginRequest): Promise<{ accessToken: string }> => {
  try {
    const getUserEduService = await new GetUserEduService().oauth(request);

    let user = await findUserByUsername(getUserEduService.username);

    if (!user) {
      const newLevel = await createEmptyJobLevel();
      user = await createUser(getUserEduService.username, newLevel);
    }

    const accessToken = generateAccessToken({ id: user.id });

    return { accessToken };
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};
