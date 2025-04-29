import { Clothes } from "../entities/Clothes";
import ValidationError from "../error/ValidationError";
import { createClothesEntity, deleteClothesEntity, findClothesById, saveClothes } from "../repository/clothes.repository";
import { CreateClothesRequest, UpdateClothesRequest } from "../request/Clothes";
import { validateHexColor } from "../utils/validator";

export const createClothes = async (request: CreateClothesRequest): Promise<Clothes> => {
  try {
    if(request.base_color && !validateHexColor(request.base_color)) {
      throw new ValidationError('Color not in correct format!');
    }
  
    const clothes = createClothesEntity(request);
    return await saveClothes(clothes);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const updateClothes = async (request: UpdateClothesRequest): Promise<Clothes> => {
  try {
    const clothes = await findClothesById(request.clothes_id);
    if (!clothes) throw new ValidationError('Clothes not found, please try again!');

    if(request.base_color && !validateHexColor(request.base_color)) {
      throw new ValidationError('Color not in correct format!');
    }
    
    Object.assign(clothes, request);
    return await saveClothes(clothes);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const deleteClothes = async (clothes_id: string): Promise<Clothes> => {
  try {
    const clothes = await findClothesById(clothes_id);
    if (!clothes) throw new ValidationError('Clothes not found, please try again!');

    return await deleteClothesEntity(clothes);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};
