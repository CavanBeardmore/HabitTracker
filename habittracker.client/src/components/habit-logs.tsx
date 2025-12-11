import { Autorenew, Beenhere } from "@mui/icons-material"
import { HabitLog } from "../data/HabitLog"
import { useState } from "react";
import { Paginator } from "./paginator";

interface HabitLogsProps {
    habitId: number;
    logs:(habitId: number, pageNumber: number) => HabitLog[]
    onGetHabitLogs: (habitId: number, pageNumber?: number) => Promise<HabitLog[]>
    hasMoreLogs: boolean;
}

export const HabitLogs = ({habitId, logs, onGetHabitLogs, hasMoreLogs}: HabitLogsProps) => {

    const [pageNumber, setPageNumber] = useState<number>(1);

    const habitLogs = logs(habitId, pageNumber);

    const handleGettingHabitLogs = async (pageNumber: number) => {
        await onGetHabitLogs(habitId, pageNumber);
    }

    return (
        <div 
            className="flex flex-col items-center justify-items-center bg-blue-900 text-stone-300 border-1 border-stone-300 rounded-2xl p-4 sm:p-6 md:p-8 lg:p-10 mr-4 w-full space-y-6"
        >
            <p className="font-semibold">Habit Logs</p>
            <div className="grid sm:grid-cols-3 md:grid-cols-5 lg:grid-cols-8 gap-2">
                {
                    habitLogs.length > 0 
                    ? habitLogs.map((v) => {
                        const date = v.Start_date.split("T")[0];
                        return (
                            <div
                                key={v.Id}
                                className="flex flex-col items-center justify-items-center bg-purple-900 text-stone-300 border-1 border-stone-300 py-4 px-8"
                            >
                                {v.Habit_logged && (
                                    <Beenhere fontSize="large" color="success" />
                                )}
                                <p>{date}</p>
                            </div>
                        )
                    }) 
                    : <Autorenew className="animate-spin" />
                }
            </div>
            <Paginator 
                onBack={{
                    onClick: async (pageNumber: number) => await handleGettingHabitLogs(pageNumber)
                }}
                onForward={{
                    onClick: async (pageNumber: number) => await handleGettingHabitLogs(pageNumber),
                    isDisabled: !hasMoreLogs
                }}
                pageNumber={pageNumber}
                setPageNumber={setPageNumber}
            />
        </div>
    )
}