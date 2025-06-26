import { useEffect, useState } from "react";
import { Resolve } from "@here-mobility/micro-di";
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { EventType } from "../classes/ServerEventHandler";
import { HabitLogService } from "../classes/HabitLogService";
import { HabitLog } from "../data/HabitLog";
import { Habit } from "../data/Habit";

interface UseHabitLogServiceReturn {
    habitLog(habitId: number): HabitLog | null;
    getMostRecentHabitLog(habitId: number): Promise<HabitLog | null>;
    addHabitLog(habitId: number): Promise<void>;
    registerHabitLogEvents(): void;
    removeHabitLogEvents(): void;
    logLoading: (habitId: number) => boolean
}
export const useHabitLogService = (): UseHabitLogServiceReturn => {

    const habitLogService = Resolve<HabitLogService>(HabitLogService);
    const globalEventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);

    const [habitLogs, setHabitLogs] = useState<Map<number, HabitLog>>(new Map<number, HabitLog>());
    const [logsLoading, setLogsLoading] = useState<Map<number, boolean>>(new Map<number, boolean>());

    //@ts-ignore
    window.logs = habitLogs;
    
    const logLoading = (habitId: number) => logsLoading.get(habitId) || false;

    const habitLog = (habitId: number) => habitLogs.get(habitId) || null; 

    const registerHabitLogEvents = () => {
        globalEventObserver.add(
            EventType.ERROR, 
            `HABITLOG_ERROR_ID`,
            () => setLogsLoading(new Map())
        );

        globalEventObserver.add(
            EventType.HABIT_LOG_ADDED, 
            `${EventType.HABIT_LOG_ADDED}_ID`,
            (data: {habitLog: HabitLog}) => addSingleHabitLogToState(data.habitLog)
        );
    }

    const removeHabitLogEvents = () => {
        globalEventObserver.remove(
            EventType.ERROR, 
            `HABITLOG_ERROR_ID`,
        );

        globalEventObserver.remove(
            EventType.HABIT_LOG_ADDED, 
            `${EventType.HABIT_LOG_ADDED}_ID`
        );
    }

    const addSingleHabitLogToState = (habitLog: HabitLog) => {
        setLogsLoading((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitLog.Habit_id, false);
            return newMap;
        })

        setHabitLogs((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitLog.Habit_id, habitLog);
            return newMap;
        })
    }

    const getMostRecentHabitLog = async (habitId: number): Promise<HabitLog | null> => {
        setLogsLoading((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitId, true);
            return newMap;
        })

        const {success, status, data, errorMessage} = await habitLogService.GetMostRecentHabitLog(habitId);
        
        setLogsLoading((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitId, false);
            return newMap;
        })

        if (success && data) {
            addSingleHabitLogToState(data);
            return data;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise("UNAUTHED", errorMessage);
            return null;
        }

        if (success === false) globalEventObserver.raise(EventType.ERROR);
        return null;
    };

    const addHabitLog = async (habitId: number): Promise<void> => {
        setLogsLoading((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitId, true);
            return newMap;
        })
        await habitLogService.AddHabitLog(habitId);
    }

    return {
        habitLog,
        getMostRecentHabitLog,
        addHabitLog,
        registerHabitLogEvents,
        removeHabitLogEvents,
        logLoading
    }
} 