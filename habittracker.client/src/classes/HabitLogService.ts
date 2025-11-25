import { HabitLog } from "../data/HabitLog";
import { getToday } from "../utils/getToday";
import { HttpServiceRes, IHttpService, RequestMethod } from "./IHttpService";

export class HabitLogService {
    constructor(
        private readonly _backendUrl: string,
        private readonly _httpService: IHttpService
    ) {}

    private getUtcDate(): string {
        return getToday();
    }

    public async AddHabitLog(habitId: number): Promise<void> {
        await this._httpService.Request(
            `${this._backendUrl}/HabitLog`,
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

    public async GetHabitLogs(habitId: number, pageNumber: number): Promise<HttpServiceRes<{habitLogs: HabitLog[], hasMore: boolean}>> {
        return await this._httpService.Request<{habitLogs: HabitLog[], hasMore: boolean}>(
            `${this._backendUrl}/HabitLog/habit/${habitId}`,
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
        return await this._httpService.Request<HabitLog>(
            `${this._backendUrl}/HabitLog/habit/recent/${habitId}`,
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
        return await this._httpService.Request<HabitLog>(
            `${this._backendUrl}/HabitLog/${habitLogId}`,
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