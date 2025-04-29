

import { PlayerCar } from '../entities/PlayerCar';
import ValidationError from '../error/ValidationError';
import { carExists, findPlayerCar, findPlayerCarIsDel, findUserById, getAllPlayerCars, savePlayerCar } from '../repository/playerCar.repository';
import {
  CreatePlayerCarRequest,
  UpdatePlayerCarRequest,
  BackupPlayCarRequest
} from '../request/PlayerCar';

export const getPlayerCar = (): Promise<PlayerCar[]> => getAllPlayerCars(false);
export const getPlayerCarIsDelete = (): Promise<PlayerCar[]> => getAllPlayerCars(true);

export const createPlayerCar = async (req: CreatePlayerCarRequest): Promise<PlayerCar> => {
  try {
    const car = await carExists(req.car_id);
    if (!car) throw new ValidationError('Car does not exist');

    const player = await findUserById(req.player_id);
    if (!player) throw new ValidationError('Player not found');

    const existing = await findPlayerCar(req.player_id, req.car_id);
    
    if (existing) {
      throw new ValidationError('Player already owns this car');
    } 

    const playerCar = new PlayerCar();
    playerCar.player_id = player;
    playerCar.car_id = car;
    playerCar.equipped = req.equipped;
    playerCar.is_deleted = 0;

    return await savePlayerCar(playerCar);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const updatePlayerCar = async (req: UpdatePlayerCarRequest): Promise<PlayerCar> => {
  try {
    const existing = await findPlayerCar(req.player_id, req.car_id);
    if (!existing || existing.is_deleted === 1) {
      throw new ValidationError('PlayerCar does not exist');
    }

    existing.equipped = req.equipped;
    return await savePlayerCar(existing);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const deletePlayerCar = async (req: BackupPlayCarRequest): Promise<PlayerCar> => {
  try {
    const existing = await findPlayerCar(req.player_id, req.car_id);
    if (!existing) throw new ValidationError('PlayerCar not found');

    existing.is_deleted = 1;
    return await savePlayerCar(existing);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const restorePlayerCar = async (req: BackupPlayCarRequest): Promise<PlayerCar> => {
  try {
    const existing = await findPlayerCarIsDel(req.player_id, req.car_id);
    if (!existing) throw new ValidationError('PlayerCar not found');
  
    existing.is_deleted = 0;
    return await savePlayerCar(existing);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};
