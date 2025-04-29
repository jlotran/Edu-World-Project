export interface CreateCarRequest {
    model_name: string;
    color: string;
    speed: number;
    handling: number;
    nitro: number;
    price: number;
    rarity: string;
    acceleration: number;
    car_code: string;
}

export interface UpdateCarRequest extends CreateCarRequest {
    car_id: string;
}

export interface FindIdRequest {
    car_id:string
}