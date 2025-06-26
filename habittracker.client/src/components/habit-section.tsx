import { AddCircle, Autorenew, Cancel, Check, Whatshot } from "@mui/icons-material";
import { getToday, getYesterday } from "../utils/getToday";
import { Habit } from "../data/Habit";
import { HabitLog } from "../data/HabitLog";

interface HabitSectionProps {
    habitCollection: Habit[]
    habitLog:(habitId: number) => HabitLog | null;
    onAddHabitClicked: () => void;
    onLogHabit: (habitId: number) => Promise<void>;
    logLoading: (habitId: number) => boolean;
}

export const HabitSection = ({habitCollection, habitLog, onAddHabitClicked, onLogHabit, logLoading}: HabitSectionProps) => {

    const habitLoggedToday = (date: string): boolean => date.split("T")[0] === getToday();
    const isStreakBroken = (date: string): boolean => {
        const loggedToday = habitLoggedToday(date);
        const loggedYesterday = date.split("T")[0] === getYesterday();

        return !(loggedToday || loggedYesterday);
    }

    const handleLog = (habitId: number) => {
        onLogHabit(habitId);
    }

    return (
        <div className="flex flex-col items-center justify-items-center space-y-4">
            <button onClick={onAddHabitClicked} className="sm:pt-2 md:pt-4 lg:pt-6">
                <AddCircle className="text-stone-300" fontSize="large"/>
            </button>
            <div className="flex flex-wrap items-center space-y-4">
                {habitCollection.length > 0 && habitCollection.map(habit => {

                    const log = habitLog(habit.Id);
                    console.log("LOG", log)
                    return (
                        <div key={habit.Id} className="flex flex-col items-center justify-items-center bg-blue-900 text-stone-300 border-1 border-stone-300 rounded-2xl p-4 sm:p-6 md:p-8 lg:p-10 mr-4 w-full space-y-6">
                            <p className="text-lg font-semibold text-white">{habit.Name.toUpperCase()}</p>
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
                                    {
                                        (habit.StreakCount > 0  && log && isStreakBroken(log.Start_date) === false)  
                                        ?   <div className="flex flex-col items-center">
                                                <p className="text-sm text-slate-300 mt-2">You're on a <span className="font-medium text-white">{habit.StreakCount} </span>day streak!</p>
                                                <div className="mt-3 text-red-500 text-2xl"><Whatshot className="text-red-500" /></div>
                                            </div>
                                        : ((log === null || habitLoggedToday(log.Start_date) === false) && <button onClick={() => handleLog(habit.Id)} className="underline">Log your habit today to start a streak!</button>)
                                    }
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