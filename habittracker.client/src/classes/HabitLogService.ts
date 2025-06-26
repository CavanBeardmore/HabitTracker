import { HabitLog } from "../data/HabitLog";
import { getToday } from "../utils/getToday";
import { AuthedHttpService } from "./AuthedHttpService";
import { HttpServiceRes, RequestMethod } from "./HttpService";

export class HabitLogService extends AuthedHttpService {
    constructor(private readonly _backendUrl: string) {
        super();
    }

    private getUtcDate(): string {
        return getToday();
    }

    public async AddHabitLog(habitId: number): Promise<void> {
        await this.AuthedRequest(
            `${this._backendUrl}habitLogs/HabitLog`,
            {
                method: RequestMethod.POST,
                headers: [
                    {
                        key: "Content-Type", 
                        value: "application/json"
                    }
                ],
                body: JSON.stringify({
                    habit_id: habitId,
                    start_date: this.getUtcDate(),
                    habit_logged: true,
                    length_in_days: 1
                })
            }
        )
    }

    public async GetHabitLogs(habitId: number, pageNumber: number): Promise<HttpServiceRes<HabitLog[]>> {
        return await this.AuthedRequest<HabitLog[]>(
            `${this._backendUrl}habitLogs/HabitLog/habit/${habitId}`,
            {
                method: RequestMethod.GET,
                headers: [
                    {
                        key: "Content-Type", 
                        value: "application/json"
                    }
                ],
                params: [
                    {
                        key: "pageNumber",
                        value: pageNumber
                    }
                ]
            }
        );
    }

    public async GetMostRecentHabitLog(habitId: number): Promise<HttpServiceRes<HabitLog>> {
        return await this.AuthedRequest<HabitLog>(
            `${this._backendUrl}habitLogs/HabitLog/habit/recent/${habitId}`,
            {
                method: RequestMethod.GET,
                headers: [
                    {
                        key: "Content-Type", 
                        value: "application/json"
                    }
                ]
            }
        );
    }

    public async GetHabitLogById(habitLogId: number): Promise<HttpServiceRes<HabitLog>> {
        return await this.AuthedRequest<HabitLog>(
            `${this._backendUrl}habitLogs/HabitLog/${habitLogId}`,
            {
                method: RequestMethod.GET,
                headers: [
                    {
                        key: "Content-Type",
                        value: "application/json"
                    }
                ]
            }
        )
    }
}