export const UpdateProfileSchema = {
    username: { type: 'string', minLength: 1, maxLength: 255 },
    avatar: { type: 'string', minLength: 1, maxLength: 255 },
}