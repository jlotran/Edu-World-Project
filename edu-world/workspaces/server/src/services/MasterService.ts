import { Car } from "../entities/Car";
import { Clothes } from "../entities/Clothes";
import { Quest } from "../entities/Quest";
import ValidationError from "../error/ValidationError";
import { getAllCars, getAllClothes, getAllQuests } from "../repository/master.repository";

export const getMaster = async (): Promise<{cars:Car[], clothes: Clothes[], quest: Quest[]}> => {
  try {
    const [cars, clothes, quest] = await Promise.all([
      getAllCars(),
      getAllClothes(),
      getAllQuests()
    ]);
  
    return { cars, clothes, quest };
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};