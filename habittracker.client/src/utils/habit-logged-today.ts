import { getToday } from "./getToday";

export const habitLoggedToday = (date: string): boolean => date.split("T")[0] === getToday();