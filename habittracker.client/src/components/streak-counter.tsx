import { Whatshot } from "@mui/icons-material";
import { Habit } from "../data/Habit"
import { HabitLog } from "../data/HabitLog"

interface StreakCounterProps {
    habit: Habit;
    log: HabitLog | null;
    isStreakBroken: boolean;
    habitLoggedToday: boolean;
    handleLog: (habitId: number) => void
}

export const StreakCounter = ({habit, log, isStreakBroken, habitLoggedToday, handleLog}: StreakCounterProps) => {

    return (
        <>
            {
                (habit.StreakCount > 0  && log && isStreakBroken === false)  
                ?   <div className="flex flex-col items-center space-y-2">
                        <p className="text-sm text-slate-300 mt-2">You're on a <span className="font-medium text-white">{habit.StreakCount} </span>day streak!</p>
                        <div className="mt-3 text-red-500 text-2xl"><Whatshot className="text-red-500" /></div>
                        {habitLoggedToday === false && (
                            <button onClick={() => handleLog(habit.Id)} className="underline">Log your habit to extend your streak!</button>
                        )}
                    </div>
                : ((log === null || habitLoggedToday === false) && <button onClick={() => handleLog(habit.Id)} className="underline">Log your habit today to start a streak!</button>)
            }
        </>
    )
}