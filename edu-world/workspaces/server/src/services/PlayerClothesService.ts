import { PlayerClothes } from '../entities/PlayerClothes';
import ValidationError from '../error/ValidationError';
import { findUserById } from '../repository/playerCar.repository';
import { clothesExists, findPlayerClothes, findPlayerClothesIsDel, getAllPlayerClothes, savePlayerClothes } from '../repository/playerClothes.repository';
import { BackupPlayClothesRequest, CreatePlayerClothesRequest, UpdatePlayerClothesRequest } from '../request/PlayerClothes';
import { validateHexColor, validateUUID } from '../utils/validator';

export const getPlayerClothes = async (): Promise<PlayerClothes[]> => {
  try {
    return getAllPlayerClothes(false);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const getPlayerClothesIsDelete = async (): Promise<PlayerClothes[]> => {
  try {
    return getAllPlayerClothes(true);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const createPlayerClothes = async (request: CreatePlayerClothesRequest): Promise<PlayerClothes> => {

  try {
    const clothes = await clothesExists(request.clothes_id);
    if (!clothes) throw new ValidationError('Clothes does not exist');

    const player = await findUserById(request.player_id);
    if (!player) throw new ValidationError('Player not found');

    const existing = await findPlayerClothes(request.player_id, request.clothes_id);
    
    if (existing) {
      throw new ValidationError('Player already owns this car');
    } 

    if(request.custom_color && !validateHexColor(request.custom_color)) {
      throw new ValidationError('Color not in correct format!');
    }

    const playClothes = new PlayerClothes();
    playClothes.player_id = player;
    playClothes.clothes_id = clothes;
    playClothes.equipped = request.equipped;
    playClothes.custom_color = request.custom_color || "#FFFFFF";
    playClothes.is_deleted = 0;

    return await savePlayerClothes(playClothes);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const updatePlayerClothes = async (request: UpdatePlayerClothesRequest): Promise<{ id: string } & PlayerClothes> => {
  try {
    if(request.custom_color && !validateHexColor(request.custom_color)) {
      throw new ValidationError('Color not in correct format!');
    }
  
    const existing = await findPlayerClothes(request.player_id, request.clothes_id);
    if (!existing || existing.is_deleted === 1) {
      throw new ValidationError('PlayerClothes does not exist!');
    }
  
    existing.custom_color = request.custom_color;
    existing.equipped = request.equipped;
    return await savePlayerClothes(existing);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const deletePlayerClothes = async (request: BackupPlayClothesRequest): Promise<PlayerClothes> => {
  try {
    const existing = await findPlayerClothes(request.player_id, request.clothes_id);
    if (!existing) throw new ValidationError('PlayerClothes not found');

    existing.is_deleted = 1;
    return await savePlayerClothes(existing);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
}

export const restorePlayerClothes = async (request: BackupPlayClothesRequest): Promise<PlayerClothes> => {
  try {
    const existing = await findPlayerClothesIsDel(request.player_id, request.clothes_id);
    if (!existing) throw new ValidationError('PlayerCar not found');

    existing.is_deleted = 0;
    return await savePlayerClothes(existing);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
}
