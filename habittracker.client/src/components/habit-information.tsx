import { Habit } from "../data/Habit"
import { HabitLog } from "../data/HabitLog"
import { habitLoggedToday } from "../utils/habit-logged-today";
import { isStreakBroken } from "../utils/is-streak-broken";
import { StreakCounter } from "./streak-counter"

interface HabitInformationProps {
    habit: Habit
    mostRecentLog: (habitId: number) => HabitLog | null;
    onLogHabit: (habitId: number) => Promise<void>;
}

export const HabitInformation = ({habit, mostRecentLog, onLogHabit}: HabitInformationProps) => {
    const mostRecentHabitLog = mostRecentLog(habit.Id);
    return (
        <div 
            className="flex flex-col items-center justify-items-center bg-blue-900 text-stone-300 border-1 border-stone-300 rounded-2xl p-4 sm:p-6 md:p-8 lg:p-10 mr-4 w-full space-y-6"
        >
            <p className="text-lg font-semibold text-white">{habit.Name.toUpperCase()}</p>
            <StreakCounter 
                habit={habit} 
                log={mostRecentHabitLog} 
                isStreakBroken={
                    mostRecentHabitLog 
                        ? isStreakBroken(mostRecentHabitLog.Start_date) 
                        : false
                } 
                habitLoggedToday={
                    mostRecentHabitLog 
                    ? habitLoggedToday(mostRecentHabitLog.Start_date) 
                    : false
                } 
                handleLog={() => onLogHabit(habit.Id)}
            />
        </div>
    )
}