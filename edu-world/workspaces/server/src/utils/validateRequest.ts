/* eslint-disable @typescript-eslint/no-explicit-any */
import ValidationError from '../error/ValidationError';

type ValidationRule = {
  type?: 'string' | 'number' | 'boolean';
  required?: boolean;
  min?: number;
  max?: number;
  minLength?: number;
  maxLength?: number;
  pattern?: RegExp;
  validate?: (value: any) => boolean | string; // custom validator
};

export type ValidationRules = {
  [key: string]: ValidationRule;
};

// Hàm validate logic gốc
export const validateRequest = (data: Record<string, any>, rules: ValidationRules): void => {
  for (const field in rules) {
    const value = data[field];
    const rule = rules[field];

    if (rule.required && (value === undefined || value === null || value === '')) {
      throw new ValidationError(`${field} is required`);
    }

    if (value !== undefined && value !== null) {
      if (rule.type && typeof value !== rule.type) {
        throw new ValidationError(`${field} must be of type ${rule.type}`);
      }

      if (rule.min !== undefined && typeof value === 'number' && value < rule.min) {
        throw new ValidationError(`${field} must be at least ${rule.min}`);
      }

      if (rule.max !== undefined && typeof value === 'number' && value > rule.max) {
        throw new ValidationError(`${field} must be at most ${rule.max}`);
      }

      if (rule.minLength !== undefined && typeof value === 'string' && value.length < rule.minLength) {
        throw new ValidationError(`${field} must have at least ${rule.minLength} characters`);
      }

      if (rule.maxLength !== undefined && typeof value === 'string' && value.length > rule.maxLength) {
        throw new ValidationError(`${field} must have at most ${rule.maxLength} characters`);
      }

      if (rule.pattern && typeof value === 'string' && !rule.pattern.test(value)) {
        throw new ValidationError(`${field} is not in correct format`);
      }

      if (rule.validate) {
        const result = rule.validate(value);
        if (result !== true) {
          throw new ValidationError(typeof result === 'string' ? result : `${field} is invalid`);
        }
      }
    }
  }
};