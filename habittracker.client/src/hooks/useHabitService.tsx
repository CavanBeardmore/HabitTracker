import { useEffect, useMemo, useState } from "react";
import { Resolve } from "@here-mobility/micro-di";
import { HabitService } from "../classes/HabitService";
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { Habit } from "../data/Habit";
import { EventType } from "../classes/ServerEventHandler";

interface UseHabitServiceReturn {
    habit(id: number): Habit | null;
    habitsCollection: Habit[];
    getHabits(): Promise<Map<number, Habit> | null>;
    getHabitById(id: string): Promise<void>;
    addHabit(name: string): Promise<void>;
    updateHabit(id: string, name: string): Promise<void>;
    deleteHabit(id: string): Promise<void>;
    removeHabitEvents(): void;
    registerHabitEvents(): void;
}
export const useHabitService = (): UseHabitServiceReturn => {

    const habitService = Resolve<HabitService>(HabitService);
    const globalEventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);

    const [habits, setHabits] = useState<Map<number, Habit>>(new Map<number, Habit>());
    //@ts-ignore
    window.habits = habits;

    const habit = (id: number) => habits.get(id) || null;

    const habitsCollection = useMemo(() => Array.from(habits.values()), [habits]);

    const registerHabitEvents = () => {
        globalEventObserver.add(
            EventType.ERROR, 
            `HABIT_ERROR_ID`,
            () => {}
        );

        globalEventObserver.add(
            EventType.HABIT_ADDED, 
            `${EventType.HABIT_ADDED}_ID`,
            (data: Habit) => addSingleHabitToState(data)
        );

        globalEventObserver.add(
            EventType.HABIT_UPDATED, 
            `${EventType.HABIT_UPDATED}_ID`,
            (data: Habit) => addSingleHabitToState(data)
        );

        globalEventObserver.add(
            EventType.HABIT_DELETED, 
            `${EventType.HABIT_DELETED}_ID`,
            (id: number) => deleteHabitFromState(id)
        );
    }

    const removeHabitEvents = () => {
        globalEventObserver.remove(
            EventType.ERROR, 
            `HABIT_ERROR_ID`
        );

        globalEventObserver.remove(
            EventType.HABIT_ADDED, 
            `${EventType.HABIT_ADDED}_ID`
        );

        globalEventObserver.remove(
            EventType.HABIT_UPDATED, 
            `${EventType.HABIT_UPDATED}_ID`,
        );

        globalEventObserver.remove(
            EventType.HABIT_DELETED, 
            `${EventType.HABIT_DELETED}_ID`,
        );
    }

    const addSingleHabitToState = (habit: Habit) => {
        setHabits((prev) => {
            const newMap = new Map(prev);
            newMap.set(habit.Id, habit);
            return newMap;
        })
    }

    const deleteHabitFromState = (id: number) => {
        setHabits((prev) => {
            const newMap = new Map(prev);
            newMap.delete(id);
            return newMap;
        })
    }

    const getHabits = async (): Promise<Map<number, Habit> | null> => {
        const {success, status, data, errorMessage} = await habitService.GetHabits();

        if (success && data) {
            const mapOfData = new Map<number, Habit>();
            data.map((v) => {
                mapOfData.set(v.Id, v)
            });
            setHabits(mapOfData);
            return mapOfData;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise("UNAUTHED", errorMessage);
            return null;
        }

        if (success === false) globalEventObserver.raise(EventType.ERROR);
        return null;
    };

    const getHabitById = async (id: string): Promise<void> => {
        const {success, status, data, errorMessage} = await habitService.GetHabitById(id);

        if (success && data) {
            addSingleHabitToState(data);
            return;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise("UNAUTHED", errorMessage);
            return;
        }

        if (success === false) globalEventObserver.raise(EventType.ERROR);
    }

    const addHabit = async (name: string): Promise<void> => await habitService.CreateHabit(name);

    const updateHabit = async (id: string, name: string): Promise<void> => await habitService.UpdateHabit(id, name);

    const deleteHabit = async (id: string): Promise<void> => await habitService.DeleteHabit(id);

    return {
        habit,
        habitsCollection,
        getHabits,
        getHabitById,
        addHabit,
        updateHabit,
        deleteHabit,
        registerHabitEvents,
        removeHabitEvents
    }
} 