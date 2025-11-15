import { useEffect, useState } from "react";
import { AuthedWrapper } from "../components/authed-wrapper";
import { HabitSection } from "../components/habit-section";
import { useHabitService } from "../hooks/useHabitService";
import { useHabitLogService } from "../hooks/useHabitLogService";
import { useNavigate } from "react-router-dom";
import { Paths } from "../App";

enum Hours {
    EARLY_MORNING = 0,
    MORNING = 6,
    AFTERNOON = 12,
    EVENING = 18,
    NIGHT = 21
}

export const Home = () => {

    const {habitsCollection, getHabits, registerHabitEvents, removeHabitEvents} = useHabitService();
    const {mostRecentHabitLogForHabit, getMostRecentHabitLog, logLoading, addHabitLog, registerHabitLogEvents, removeHabitLogEvents} = useHabitLogService();
    const navigate = useNavigate();

    const [welcomeMessage, setWelcomeMessage] = useState<string>();

    const determineWelcomeMessage = (): void => {
        const hour = new Date().getHours();

        if (hour >= Hours.NIGHT) {
            setWelcomeMessage("Welcome Night Owl! Log those habits before midnight to preserve your streak!")
        }

        if (hour >= Hours.EARLY_MORNING && hour < Hours.MORNING) {
            setWelcomeMessage("Welcome Early Bird! Start your day off right and log those habits!")
        }

        if (hour >= Hours.MORNING && hour < Hours.AFTERNOON) {
            setWelcomeMessage("Welcome! Smash those morning goals and log your habits now!")
        }

        if (hour >= Hours.AFTERNOON && hour < Hours.NIGHT) {
            setWelcomeMessage("Welcome! Get to logging those habits!")
        }
    }

    const getHabitLogForEachHabit = async () => {
        for (const habit of habitsCollection) {
            if (habit.StreakCount > 0) {
                await getMostRecentHabitLog(habit.Id);
            }
        }
    }

    const getThreeHabitsFromCollection = () => {
        return habitsCollection.length > 0 
            ? habitsCollection.filter((_, i) => i < 3)
            : []
    }

    useEffect(() => {
        if (habitsCollection.length > 0) {
            getHabitLogForEachHabit();
        }
    }, [habitsCollection])

    useEffect(() => {
        registerHabitEvents();
        registerHabitLogEvents();
        determineWelcomeMessage();

        if (habitsCollection.length === 0) {
            getHabits();
        }

        return () => {
            removeHabitLogEvents();
            removeHabitEvents();
        }
    }, [])
    
    
    return (
        <AuthedWrapper>
            <div className="flex flex-col space-y-8 text-stone-300 w-[80%]">
                <p className="font-semibold text-4xl">{welcomeMessage}</p>
                <HabitSection 
                    onHabitClicked={(habitId: number) => navigate(Paths.HABIT, {state: {habitId}})}
                    habitCollection={getThreeHabitsFromCollection()} 
                    habitLog={mostRecentHabitLogForHabit} 
                    onAddHabitClicked={() => navigate(Paths.HABITS_ADD)} 
                    logLoading={logLoading}
                    onLogHabit={addHabitLog}
                />
            </div>
        </AuthedWrapper>
    )
}