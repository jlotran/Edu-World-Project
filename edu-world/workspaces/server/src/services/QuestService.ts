import { Quest } from '../entities/Quest';
import ValidationError from '../error/ValidationError';
import { createQuestEntity, deleteQuestEntity, findQuestById, saveQuest } from '../repository/quest.repository';
import { CreateQuestRequest, UpdateQuestRequest } from '../request/Quest';
import { parseDateYMD, validateDateYMD } from '../utils/validator';

export const createQuest = async (request: CreateQuestRequest): Promise<Quest> => {
  try {
    if (request.time_finish && !validateDateYMD(request.time_finish)) {
      throw new ValidationError('time_finish must be in format YYYY-MM-DD');
    }
  
    const quest = createQuestEntity({
      ...request,
      time_finish: parseDateYMD(request.time_finish) ?? undefined,
    });
    return await saveQuest(quest);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const updateQuest = async (request: UpdateQuestRequest): Promise<Quest> => {
  try {
    const quest = await findQuestById(request.quest_id);
    if (!quest) throw new ValidationError('Quest not found, please try again');
    if (request.time_finish && !validateDateYMD(request.time_finish)) {
      throw new ValidationError('time_finish must be in format YYYY-MM-DD');
    }
    Object.assign(quest, request);
    return await saveQuest(quest);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};

export const deleteQuest = async (quest_id: string): Promise<Quest> => {
  try {
    const quest = await findQuestById(quest_id);
    if (!quest) throw new ValidationError('Quest not found, please try again');

    return await deleteQuestEntity(quest);
  } catch (error) {
    throw new ValidationError(error instanceof Error ? error.message : String(error));
  }
};
