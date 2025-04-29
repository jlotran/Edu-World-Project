export interface CreateClothesRequest {
    gender:number;
    base_color:string;
    category:string;
    status: number;
    price:number;
    rarity: string;
}

export interface UpdateClothesRequest extends CreateClothesRequest {
    clothes_id: string;
}

export interface FindIdRequest {
    clothes_id:string;
}