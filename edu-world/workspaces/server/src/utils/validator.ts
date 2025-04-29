export const checkLength = (value: string, min: number, max: number): boolean => {
    return !!value && value.length >= min && value.length <= max;
};

export  const validateUUID = (uuid: string): boolean => {
    const regex = new RegExp("/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/");
    return regex.test(uuid);
}

export const validateHexColor = (color: string): boolean => {
    const regex = /^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$/;
    return regex.test(color);
};

export const validateUsername = (username: string): boolean => {
    const regex = new RegExp("/^[a-zA-Z0-9_-]+$/");

    return regex.test(username);
};

export const validateDateYMD = (dateStr: string): boolean => {
    const regex = /^\d{4}-\d{2}-\d{2}$/;
  
    if (!regex.test(dateStr)) return false;
  
    const [year, month, day] = dateStr.split('-').map(Number);
    const date = new Date(dateStr); // ISO date string is valid here
  
    return (
      date.getFullYear() === year &&
      date.getMonth() + 1 === month &&
      date.getDate() === day
    );
};

export const parseDateYMD = (dateStr: string): Date | null => {
    return validateDateYMD(dateStr) ? new Date(dateStr) : null;
};
  