import { Repository } from 'typeorm';

import { PlayerCar } from '../entities/PlayerCar';
import { PlayerClothes } from '../entities/PlayerClothes';
import ValidationError from '../error/ValidationError';
import { findAllPlayerCars, findAllPlayerClothes } from '../repository/playerInventory.repository';

export const getPlayerInventory = async (): Promise<{ playerClothes: PlayerClothes[]; playerCar: PlayerCar[] }> => {
  try {
    const playerClothes = await findAllPlayerClothes();
    const playerCar = await findAllPlayerCars();
  
    return {
      playerClothes,
      playerCar,
    };
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};
