export const CreateCarSchema = {
    car_code: { required: true, minLength: 3 },
    model_name: { required: true },
    speed: { required: true, min: 1 },
    handling: { required: true, min: 1 },
    nitro: { required: true, min: 0 },
    acceleration: { required: true, min: 0 },
    price: { min: 0 },
    rarity: { required: true },
    color: {minLength: 7, maxLength: 7}, // optional
};

export const UpdateCarSchema = {
  car_id: { required: true, minLength: 1 },
  ...CreateCarSchema, // dùng lại schema của Create
};

export const FindIdSchema = {
  car_id: { required: true, minLength: 1 },
};
