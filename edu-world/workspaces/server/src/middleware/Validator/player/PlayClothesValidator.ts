export const CreatePlayerClothesSchema = {
  clothes_id : {required: true, minLength: 1},
  custom_color: {minLength: 7, maxLength: 7},
  equipped: {min: 0, max: 1},
}

export const UpdatePlayerClothesSchema = {
  ...CreatePlayerClothesSchema
}

export const FindIdSchema = {
  clothes_id: { required: true, minLength: 1 },
};