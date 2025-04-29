import { Car } from '../entities/Car';
import ValidationError from '../error/ValidationError';
import { createCarEntity, deleteCarEntity, findCarById, saveCar } from '../repository/car.repository';
import { CreateCarRequest, UpdateCarRequest } from '../request/Car';
import { checkLength, validateHexColor } from '../utils/validator';

export const createCar = async (request: CreateCarRequest): Promise<Car> => {
	try {
		if(request.color && !validateHexColor(request.color)) {
			throw new ValidationError('Color not in correct format!');
		}
	
		const newCar = createCarEntity(request);
		return await saveCar(newCar);
	} catch (error) {
	  throw new ValidationError(error instanceof Error ? error.message : String(error));
	}
};

export const updateCar = async (request: UpdateCarRequest): Promise<Car> => {
	try {
		if(request.color && !validateHexColor(request.color)) {
			throw new ValidationError('Color not in correct format!');
		}
		const car = await findCarById(request.car_id);
		if (!car) throw new ValidationError('Car not found, please try again!');
	
		Object.assign(car, request);
		
		return await saveCar(car);
	} catch (error) {
	  throw new ValidationError(error instanceof Error ? error.message : String(error));
	}
};

export const deleteCar = async (id: string): Promise<Car> => {
	try {
		const car = await findCarById(id);
		if (!car) throw new ValidationError('Car not found, please try again!');
		return await deleteCarEntity(car);
	} catch (error) {
	  throw new ValidationError(error instanceof Error ? error.message : String(error));
	}
};