export interface CreateQuestRequest {
  name: string;
  description: string;
  reward_exp: number;
  currency: number;
  item?: string;
  time_finish: string;
}

export interface UpdateQuestRequest extends CreateQuestRequest{
  quest_id: string;
}

export interface FindIdRequest {
  quest_id: string;
}