import { AddCircle, Autorenew, Cancel, Check } from "@mui/icons-material";
import { Habit } from "../data/Habit";
import { HabitLog } from "../data/HabitLog";
import { StreakCounter } from "./streak-counter";
import { habitLoggedToday } from "../utils/habit-logged-today";
import { isStreakBroken } from "../utils/is-streak-broken";

interface HabitSectionProps {
    habitCollection: Habit[]
    habitLog:(habitId: number) => HabitLog | null;
    onAddHabitClicked: () => void;
    onHabitClicked: (habitId: number) => void
    onLogHabit: (habitId: number) => Promise<void>;
    logLoading: (habitId: number) => boolean;
}

export const HabitSection = ({habitCollection, habitLog, onAddHabitClicked, onHabitClicked, onLogHabit, logLoading}: HabitSectionProps) => {

    return (
        <div className="flex flex-col items-center justify-items-center space-y-4">
            <button onClick={onAddHabitClicked} className="sm:pt-2 md:pt-4 lg:pt-6">
                <AddCircle className="text-stone-300" fontSize="large"/>
            </button>
            <div className="flex flex-wrap items-center space-y-4">
                {habitCollection.length > 0 && habitCollection.map(habit => {

                    const log = habitLog(habit.Id);
                    return (
                        <div key={habit.Id} className="flex flex-col items-center justify-items-center bg-blue-900 text-stone-300 border-1 border-stone-300 rounded-2xl p-4 sm:p-6 md:p-8 lg:p-10 mr-4 w-full space-y-6">
                            <button 
                                className="text-lg font-semibold text-white hover:underline hover:text-xl"
                                onClick={() => onHabitClicked(habit.Id)}
                            >
                                {habit.Name.toUpperCase()}
                            </button>
                            {
                                logLoading(habit.Id) 
                                ?
                                <Autorenew className="animate-spin"/>
                                :
                                <>
                                    <p className="flex flex-row items-center">Logged Today: 
                                        <span className="pl-2">          
                                        {
                                            (log && habitLoggedToday(log.Start_date)) 
                                            ? <Check color="success" /> 
                                            : <Cancel color="warning" />
                                        }
                                        </span>
                                    </p>
                                    <StreakCounter
                                        habit={habit} 
                                        log={log} 
                                        isStreakBroken={log ? isStreakBroken(log.Start_date) : false} 
                                        habitLoggedToday={log ? habitLoggedToday(log.Start_date) : false} 
                                        handleLog={() => onLogHabit(habit.Id)}
                                    />
                                </>
                            }

                        </div>
                    )
                })}
                {habitCollection.length === 0 && (
                    <p className="text-stone-300 font-light text-lg">Get tracking and create a habit!</p>
                )}

            </div>
        </div>
    )
}