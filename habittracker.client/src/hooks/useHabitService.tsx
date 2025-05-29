import { useState } from "react";
import { Resolve } from "@here-mobility/micro-di";
import { HabitService } from "../classes/HabitService";
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { Habit } from "../data/Habit";

interface UseHabitServiceReturn {
    habits: Map<number, Habit>
    getHabits(): Promise<void>;
    getHabitById(id: string): Promise<void>;
}
export const useHabitService = (): UseHabitServiceReturn => {

    const habitService = Resolve<HabitService>(HabitService);
    const globalEventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);

    const [habits, setHabits] = useState<Map<number, Habit>>(new Map<number, Habit>());

    const registerHabitEvents = () => {
        
    }

    const getHabits = async () => {
        const {success, status, data, errorMessage} = await habitService.GetHabits();

        if (success && data) {
            const mapOfData = new Map<number, Habit>();
            data.map((v) => {
                mapOfData.set(v.id, v)
            });
            setHabits(mapOfData);
            return;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise("UNAUTHED", errorMessage);
            return;
        }

        globalEventObserver.raise("ERROR", errorMessage);
    };

    const getHabitById = async (id: string) => {
        const {success, status, data, errorMessage} = await habitService.GetHabitById(id);

        if (success && data) {
            setHabits((prev) => {
                const newMap = new Map(prev);
                newMap.set(data.id, data);
                return newMap;
            })
            return;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise("UNAUTHED", errorMessage);
            return;
        }

        globalEventObserver.raise("ERROR", errorMessage);
    }

    return {
        habits,
        getHabits,
        getHabitById
    }
} 