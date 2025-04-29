export interface CreatePlayerClothesRequest {
    player_id: string;
    clothes_id: string;
    custom_color?:string;
    equipped: boolean;
}

export interface UpdatePlayerClothesRequest {
    id: string;
    player_id: string;
    clothes_id: string;
    custom_color:string;
    equipped: boolean;
}

export interface BackupPlayClothesRequest {
    player_id: string;
    clothes_id: string;
}

export interface FindIdRequest {
    clothes_id:string;
}