import { Habit } from "../data/Habit";
import { HttpServiceRes, IHttpService, RequestMethod } from "./IHttpService";

export class HabitService {
    constructor(
        private readonly _backendUrl: string,
        private readonly _httpService: IHttpService
    ) {}

    public async CreateHabit(name: string): Promise<void> {
        await this._httpService.Request(
            `${this._backendUrl}/Habit`,
            {
                method: RequestMethod.POST,
                headers: [
                    {
                        key: "Content-Type", 
                        value: "application/json"
                    }
                ],
                body: JSON.stringify({name})
            }
        )
    }

    public async GetHabits(): Promise<HttpServiceRes<Habit[]>> {
        return await this._httpService.Request<Habit[]>(
            `${this._backendUrl}/Habit`,
            {
                method: RequestMethod.GET,
                headers: [
                    {
                        key: "Content-Type", 
                        value: "application/json"
                    }
                ],
            }
        );
    }

    public async GetHabitById(habitId: number): Promise<HttpServiceRes<Habit>> {
        return await this._httpService.Request<Habit>(
            `${this._backendUrl}/Habit/${habitId}`,
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

    public async UpdateHabit(id: number, name: string, streakCount: number): Promise<void> {
        await this._httpService.Request(
            `${this._backendUrl}/Habit/update`,
            {
                method: RequestMethod.PATCH,
                headers: [
                    {
                        key: "Content-Type",
                        value: "application/json"
                    }
                ],
                body: JSON.stringify({
                    id,
                    name,
                    streakCount
                })
            }
        )
    }

    public async DeleteHabit(id: number): Promise<void> {
        await this._httpService.Request(
            `${this._backendUrl}/Habit/delete/${id}`,
            {
                method: RequestMethod.DELETE,
            }
        )
    }
}