export const CreateClothesSchema = {
  gender: { required: true, min: 0, max: 1 },
  base_color: { minLength: 7, maxLength: 7 },
  category: { required: true, minLength: 1 },
  status: { min: 0 },
  price: { min: 0 },
  rarity: { required: true },
}

export const UpdateClothesSchema = {
  clothes_id: { required: true, minLength: 1 },
  ...CreateClothesSchema
}

export const FindIdSchema = {
  clothes_id: { required: true, minLength: 1 },
};