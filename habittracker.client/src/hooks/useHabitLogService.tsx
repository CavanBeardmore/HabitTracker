import { useState } from "react";
import { Resolve } from "@here-mobility/micro-di";
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { EventType } from "../classes/ServerEventHandler";
import { HabitLogService } from "../classes/HabitLogService";
import { HabitLog } from "../data/HabitLog";

interface UseHabitLogServiceReturn {
    mostRecentHabitLogForHabit(habitId: number): HabitLog | null;
    habitLogsForHabit(habitId: number, pageNumber: number): HabitLog[]
    getMostRecentHabitLog(habitId: number): Promise<HabitLog | null>;
    getHabitLogsByHabitId: (habitId: number, pageNumber?: number) => Promise<HabitLog[]>;
    addHabitLog(habitId: number): Promise<void>;
    registerHabitLogEvents(): void;
    removeHabitLogEvents(): void;
    hasMoreLogs: (habitId: number) => boolean;
    logLoading: (habitId: number) => boolean
}
export const useHabitLogService = (): UseHabitLogServiceReturn => {

    const habitLogService = Resolve<HabitLogService>(HabitLogService);
    const globalEventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);

    const [mostRecentHabitLog, setMostRecentHabitLog] = useState<Map<number, HabitLog>>(new Map<number, HabitLog>())
    const [habitLogs, setHabitLogs] = useState<Map<number, Map<number, HabitLog[]>>>(new Map<number, Map<number, HabitLog[]>>());
    const [hasMoreHabitLogs, setHasMoreHabitLogs] = useState<Map<number, boolean>>(new Map<number, boolean>());
    const [logsLoading, setLogsLoading] = useState<Map<number, boolean>>(new Map<number, boolean>());

    //@ts-ignore
    window.recentLogs = mostRecentHabitLog;
    //@ts-ignore
    window.logs = habitLogs;
    
    const logLoading = (habitId: number): boolean => logsLoading.get(habitId) || false;
    const hasMoreLogs = (habitId: number): boolean => hasMoreHabitLogs.get(habitId) || false;

    const mostRecentHabitLogForHabit = (habitId: number): HabitLog | null => mostRecentHabitLog.get(habitId) || null; 
    const habitLogsForHabit = (habitId: number, pageNumber: number): HabitLog[] => habitLogs.get(habitId)?.get(pageNumber) || [];

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

        setMostRecentHabitLog((prev) => {
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

    const getHabitLogsByHabitId = async (habitId: number, pageNumber: number = 1): Promise<HabitLog[]> => {
        setLogsLoading((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitId, true);
            return newMap;
        });

        const {success, status, data, errorMessage} = await habitLogService.GetHabitLogs(habitId, pageNumber);
        console.log("data", data)
        setLogsLoading((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitId, false);
            return newMap;
        })

        if (success && data) {
            setHabitLogs((prev) => {
                const newMap = new Map(prev);
                newMap.set(habitId, new Map([
                    [pageNumber, data.habitLogs]
                ]));
                return newMap;
            })

            setHasMoreHabitLogs((prev) => {
                const newMap = new Map(prev);
                newMap.set(habitId, data.hasMore);
                return newMap;
            })
            return data.habitLogs;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise("UNAUTHED", errorMessage);
            return [];
        }

        if (success === false) globalEventObserver.raise(EventType.ERROR);
        return [];
    }

    const addHabitLog = async (habitId: number): Promise<void> => {
        setLogsLoading((prev) => {
            const newMap = new Map(prev);
            newMap.set(habitId, true);
            return newMap;
        })
        await habitLogService.AddHabitLog(habitId);
    }

    return {
        mostRecentHabitLogForHabit,
        habitLogsForHabit,
        getMostRecentHabitLog,
        getHabitLogsByHabitId,
        addHabitLog,
        registerHabitLogEvents,
        removeHabitLogEvents,
        logLoading,
        hasMoreLogs
    }
} 