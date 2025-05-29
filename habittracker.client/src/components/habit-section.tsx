// import { Cancel, Check } from "@mui/icons-material";
import { AddCircle, Whatshot } from "@mui/icons-material";
import { Habit } from "../data/Habit";

interface HabitSectionProps {
    habits: Habit[]
    onAddHabitClicked: () => void;
}

export const HabitSection = ({habits, onAddHabitClicked}: HabitSectionProps) => {

    const streakNumber: number = 7;

    return (
        <div className="flex flex-col items-center justify-items-center space-y-4">
            <div className="flex flex-row items-center space-x-8">
                {habits.length > 0 && habits.map(habit => {
                    return (
                        <div className="bg-blue-900 text-stone-300 border-1 border-stone-300 rounded-lg p-20 mr-4 w-full">
                            <p className="text-lg font-semibold text-white tracking-wide">{habit.name.toUpperCase()}</p>
                            {/* {
                                habit.logged ?
                                <Check /> :
                                <Cancel />
                            } */}
                            {
                                streakNumber !== 0 
                                ?   <div>
                                        <p className="text-sm text-slate-300 mt-2">You're on a <span className="font-medium text-white">{streakNumber} </span>day streak!</p>
                                        <div className="mt-3 text-red-500 text-2xl"><Whatshot className="text-red-500" /></div>
                                    </div>
                                : <p>Log your habit today to start a streak!</p>
                            }

                        </div>
                    )
                })}
                {habits.length === 0 && (
                    <p className="text-stone-300 font-light text-lg">Get tracking and create a habit!</p>
                )}
                <button onClick={onAddHabitClicked} className="animate-bounce">
                    <AddCircle className="text-stone-300" fontSize="large"/>
                </button>
            </div>
        </div>
    )
}