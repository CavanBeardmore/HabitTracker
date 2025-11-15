import { ChangeEvent, useEffect, useState } from "react";
import { AuthedWrapper } from "../components/authed-wrapper";
import { useHabitService } from "../hooks/useHabitService";
import { useHabitLogService } from "../hooks/useHabitLogService";
import { HabitInformation } from "../components/habit-information";
import { useLocation } from "react-router-dom";
import { Autorenew, Delete, Edit } from "@mui/icons-material";
import { HabitLogs } from "../components/habit-logs";
import { useToast } from "../hooks/useToast";

export const SelectedHabit = () => {
    const {habit, updateHabit, deleteHabit, registerHabitEvents, removeHabitEvents, getHabitById} = useHabitService();
    const {addHabitLog, mostRecentHabitLogForHabit, getHabitLogsByHabitId, habitLogsForHabit, getMostRecentHabitLog, registerHabitLogEvents, removeHabitLogEvents, hasMoreLogs} = useHabitLogService();

    const location = useLocation();
    const {showToast} = useToast();

    const habitId: number | undefined = location.state.habitId;
    const selectedHabit = habitId ? habit(habitId) : null
    const moreLogs = selectedHabit?.Id ? hasMoreLogs(selectedHabit?.Id) : false;

    const [habitName, setHabitName] = useState<string>();
    const [isEditing, setIsEditing] = useState<boolean>(false);

    const onDeleteHabit = () => {
        if (selectedHabit?.Id) {
            deleteHabit(selectedHabit?.Id);
        }
    }

    const onDeleteClicked = () => {
        showToast(
            "Are you sure you want to delete this habit?",
            [
                {
                    text: "Yes",
                    onClick: onDeleteHabit
                },
            ]
        )
    };
    const onEditClicked = () => setIsEditing((prev) => !prev);

    const onHabitNameChange = (e: ChangeEvent<HTMLInputElement>) => {
        setHabitName(e.target.value.toUpperCase());
    }

    const onHabitNameSubmit = () => {
        if (selectedHabit && habitName) {
            updateHabit(selectedHabit?.Id, habitName, selectedHabit.StreakCount);
            setIsEditing(false);
        }
    }

    useEffect(() => {
        setHabitName(selectedHabit?.Name.toUpperCase())
    }, [selectedHabit])

    useEffect(() => {
        registerHabitEvents();
        registerHabitLogEvents();

        if (habitId) {
            getHabitById(habitId);
            getMostRecentHabitLog(habitId);
            getHabitLogsByHabitId(habitId);
        }

        return () => {
            removeHabitLogEvents();
            removeHabitEvents();
        }
    }, [habitId])
    
    return (
        <AuthedWrapper>
            <div className="flex flex-col space-y-8 text-stone-300 w-[80%] items-center">
                {
                    selectedHabit 
                    ?   (
                            <>
                                <div className="flex flex-row ml-auto space-x-16 mr-4">
                                    <button className="hover:animate-pulse" onClick={onEditClicked}><Edit fontSize="large"/></button>
                                    <button className="hover:animate-pulse" onClick={onDeleteClicked}><Delete fontSize="large" /></button>
                                </div>
                                {
                                    isEditing 
                                    ? (
                                        <div 
                                            className="flex flex-col items-center justify-items-center bg-blue-900 text-stone-300 border-1 border-stone-300 rounded-2xl p-4 sm:p-6 md:p-8 lg:p-10 mr-4 w-full space-y-6"
                                        >
                                            <input value={habitName} onChange={onHabitNameChange} className="bg-purple-900 p-4 rounded-md border-1 border-stone-300 text-lg font-semibold text-white" />
                                            <button onClick={onHabitNameSubmit} className="bg-purple-900 p-2 rounded-lg font-bold shadow-lg text-stone-300 text-lg border-1 border-stone-300">Submit</button>
                                        </div>
                                    )
                                    : (
                                        <HabitInformation 
                                            habit={selectedHabit} 
                                            onLogHabit={addHabitLog}
                                            mostRecentLog={mostRecentHabitLogForHabit}
                                        />
                                    )
                                }
                                <HabitLogs 
                                    habitId={selectedHabit.Id} 
                                    logs={habitLogsForHabit}
                                    onGetHabitLogs={getHabitLogsByHabitId}
                                    hasMoreLogs={moreLogs}
                                />
                            </>
                        )
                    : <Autorenew className="animate-spin"/>
                }
            </div>
        </AuthedWrapper>
    )
}