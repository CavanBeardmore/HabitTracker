export const getToday = (): string => {
    const now = new Date();
    const formatDate = (date: Date): string => {
        return date.toISOString().split('T')[0];
    }

    return formatDate(now);
};

export const getYesterday = (): string => {
  const today = new Date();
  const yesterday = new Date(today);
  yesterday.setDate(today.getDate() - 1);
  
  return formatDate(yesterday);
}


const formatDate = (date: Date): string => {
    return date.toISOString().split('T')[0];
}
