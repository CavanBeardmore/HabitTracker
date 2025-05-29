import { useEffect, useState } from "react";
import { AuthedWrapper } from "../components/authed-wrapper";
import { HabitSection } from "../components/habit-section";
import { useHabitService } from "../hooks/useHabitService";

enum Hours {
    EARLY_MORNING = 0,
    MORNING = 6,
    AFTERNOON = 12,
    EVENING = 18,
    NIGHT = 21
}

export const Home = () => {

    const {habits, getHabits} = useHabitService();
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

    useEffect(() => {
        determineWelcomeMessage();
        if(!habits) {
            getHabits();
        }
    }, [])
    
    return (
        <AuthedWrapper>
            <div className="flex flex-col space-y-8 text-stone-300 w-[80%]">
                <p className="font-semibold text-4xl">{welcomeMessage}</p>
                <HabitSection habits={habits.size > 0 ? Array.from(habits.values()).filter((_,i) => i <= 2) : []} onAddHabitClicked={() => console.log("clicked")} />
            </div>
        </AuthedWrapper>
    )
}