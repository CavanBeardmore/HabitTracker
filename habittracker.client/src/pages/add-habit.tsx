import { ChangeEvent, useEffect, useState } from "react";
import { AuthedWrapper } from "../components/authed-wrapper";
import { Input, InputType } from "../components/input";
import { useToast } from "../hooks/useToast";
import { Autorenew } from "@mui/icons-material";
import { Resolve } from "@here-mobility/micro-di";
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { EventType } from "../classes/ServerEventHandler";
import { Habit } from "../data/Habit";
import { useHabitService } from "../hooks/useHabitService";

export const AddHabit = () => {

    const globalEventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);
    const nameRegex = new RegExp(/^[A-Za-z\s]+$/);

    const {showToast} = useToast();
    const {addHabit} = useHabitService();

    const [habitName, setHabitName] = useState<string>("");
    const [loading, setLoading] = useState<boolean>(false);

    const checkIfNameIsValid = (name: string): boolean => {
        return nameRegex.test(name);
    }

    const onLoginCredsChange = (e: ChangeEvent<HTMLInputElement>) => {
        setHabitName(e.target.value);
    }

    const handleSubmit = async () => {
        setLoading(true);
        const trimmedName = habitName.trim();
        const isNameValid = checkIfNameIsValid(trimmedName)
        if(!isNameValid) {
            setLoading(false);
            showToast("Habit name should contain only letters and spaces.");
            return;
        }
        await addHabit(trimmedName);
        setLoading(false);
    }

    useEffect(() => {
        globalEventObserver.add(EventType.HABIT_ADDED, `${EventType.HABIT_ADDED}_ID`, (habit: Habit) => {
            showToast("Successfully added your new habit")
        })

        return () => {
            globalEventObserver.remove(EventType.HABIT_ADDED, `${EventType.HABIT_ADDED}_ID`);
        }
    }, [])

    return (
        <AuthedWrapper>
            <div className="flex flex-col items-center w-full space-y-8">
                <p className="font-semibold text-2xl text-stone-300">Add your new habit below.</p>
                <div className="flex flex-col items-center space-y-8 text-stone-300 w-[40%] sm:h-20 md:h-36 lg:h-40 bg-blue-900 p-4 rounded-xl">
                    <Input
                        onChange={onLoginCredsChange}
                        value={habitName}
                        placeholder={"Habit Name"}
                        name={"name"}
                        type={InputType.TEXT}
                        required
                    />
                    <button
                        onClick={handleSubmit}
                        className="flex-grow font-bold bg-purple-900 text-stone-300 text-sm p-4 rounded-lg border-1 border-stone-300"
                    >
                        {
                            loading
                            ? <Autorenew className="animate-spin"/>
                            : "Submit"
                        }
                    </button>
                </div>
            </div>
        </AuthedWrapper>
    )
}