export interface CreatePlayerCarRequest {
    player_id: string;
    car_id: string;
    equipped: boolean;
}

export interface UpdatePlayerCarRequest extends CreatePlayerCarRequest {
    id: string;
}

export interface BackupPlayCarRequest {
    player_id: string;
    car_id: string;
}

export interface FindIdRequest {
    car_id:string;
}