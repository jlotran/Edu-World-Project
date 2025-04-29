import { JobLevel } from "../../entities/JobLevel";
import { User } from "../../entities/User";

interface ProfileUserDTO {
    id: string;
    username: string;
    currency: number;
    level: JobLevel;
    experience: number;
    matches_played: number;
    win_rate: number;
    avatar:string;
}

const parseProfileUserDTO = (user: User): ProfileUserDTO => {
    return {
        id: user.id,
        username: user.username,
        currency: user.currency,
        level: user.level,
        experience: user.experience,
        matches_played: user.matches_played,
        win_rate: user.win_rate,
        avatar: user.avatar,
    };
}

export {
    parseProfileUserDTO,
    type ProfileUserDTO
}