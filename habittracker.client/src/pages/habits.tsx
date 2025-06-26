import { useEffect } from "react";
import { AuthedWrapper } from "../components/authed-wrapper";
import { HabitSection } from "../components/habit-section";
import { useHabitService } from "../hooks/useHabitService";
import { useHabitLogService } from "../hooks/useHabitLogService";
import { useNavigate } from "react-router-dom";
import { Paths } from "../App";


export const Habits = () => {

    const {habitsCollection, getHabits, registerHabitEvents, removeHabitEvents, addHabit} = useHabitService();
    const {habitLog, getMostRecentHabitLog, addHabitLog, logLoading, registerHabitLogEvents, removeHabitLogEvents} = useHabitLogService();
    const navigate = useNavigate();

    const getHabitLogForEachHabit = async () => {
        for (const habit of habitsCollection) {
            await getMostRecentHabitLog(habit.Id);
        }
    }

    useEffect(() => {
        if (habitsCollection.length > 0) {
            getHabitLogForEachHabit();
        }
    }, [habitsCollection])

    useEffect(() => {
        registerHabitEvents();
        registerHabitLogEvents()
        getHabits();

        return () => {
            removeHabitLogEvents();
            removeHabitEvents();
        }
    }, [])
    
    return (
        <AuthedWrapper>
            <div className="flex flex-col space-y-8 text-stone-300 w-[80%]">
                <HabitSection 
                    habitCollection={habitsCollection} 
                    habitLog={habitLog} 
                    onAddHabitClicked={() => navigate(Paths.HABITS_ADD)}
                    onLogHabit={addHabitLog}
                    logLoading={logLoading}
                />
            </div>
        </AuthedWrapper>
    )
}