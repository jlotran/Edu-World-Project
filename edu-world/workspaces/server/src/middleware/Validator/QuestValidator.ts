export const CreateQuestSchema = {
  name: { required: true, minLength: 1, maxLength: 255  },
  description: { required: true, minLength: 1, maxLength: 255 },
  reward_exp: { required: true, min: 0 },
  currency: { required: true, min: 0 },
  item: { minLength: 1 },
  time_finish: { required: true, type: 'string' }
}

export const UpdateQuestSchema = {
  quest_id: { required: true, minLength: 1 },
  ...CreateQuestSchema
}

export const FindIdSchema = {
  quest_id: { required: true, minLength: 1 },
}
