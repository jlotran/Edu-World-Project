export const CreatePlayerCarSchema = {
  car_id: {
    required: true,
    minLength: 1,
    maxLength: 255,
  },
  equipped: {
    type: 'boolean',
    enum: [true, false],
  },
};

export const UpdatePlayerCarSchema = {
  ...CreatePlayerCarSchema
}

export const FindIdSchema = {
  car_id: { required: true, minLength: 1 },
};