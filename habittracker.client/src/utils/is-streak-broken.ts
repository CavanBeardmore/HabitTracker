import { getYesterday } from "./getToday";
import { habitLoggedToday } from "./habit-logged-today";

export const isStreakBroken = (date: string): boolean => {
    const loggedToday = habitLoggedToday(date);
    const loggedYesterday = date.split("T")[0] === getYesterday();

    return !(loggedToday || loggedYesterday);
}