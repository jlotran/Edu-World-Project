import { parseProfileUserDTO, ProfileUserDTO } from '../dto/user/UserDTO';
import ValidationError from '../error/ValidationError';
import { findUserById, findUserByUsername, saveUser } from '../repository/user.repository';
import { UpdateProfileUser } from '../request/User';
import { validateUsername } from '../utils/validator';

export const getDetailUser = async (id: string): Promise<ProfileUserDTO> => {
  try {
    const user = await findUserById(id);
    if (!user) throw new ValidationError('User not found, please try again');
    return parseProfileUserDTO(user);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const updateProfileUser = async (request: UpdateProfileUser): Promise<ProfileUserDTO> => {
  try {
    validateUsername(request.username);

    const user = await findUserById(request.id);
    if (!user) throw new ValidationError('User not found, please try again');

    const existingUser = await findUserByUsername(request.username);
    if (existingUser && existingUser.id !== request.id) {
      throw new ValidationError('Username already exists');
    }

    user.username = request.username;
    user.avatar = request.avatar;

    const updated = await saveUser(user);
    return parseProfileUserDTO(updated);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};
