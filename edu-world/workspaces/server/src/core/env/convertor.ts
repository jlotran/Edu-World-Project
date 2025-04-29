export type Convertor<T> = (value: string) => T;

export const StringConvertor: Convertor<string> = (value: string) => value;

export const NumberConvertor: Convertor<number> = (value: string) => Number(value);

export const BooleanConvertor: Convertor<boolean> = (value: string) => value === 'true';

export const StringOrNumberConvertor: Convertor<string | number> = (value: string) => {
  const num = Number(value);
  return Number.isNaN(num) ? value : num;
};

export const ArrayConvertor =
  <T extends string | number>(itemConvertor: Convertor<T>): Convertor<T[]> =>
  (value: string) => {
    const cleanValue = value.trim();
    const items =
      cleanValue.startsWith('[') && cleanValue.endsWith(']')
        ? cleanValue.slice(1, -1).split(',')
        : cleanValue.split(',');

    return items.map((item) => itemConvertor(item.trim()));
  };
