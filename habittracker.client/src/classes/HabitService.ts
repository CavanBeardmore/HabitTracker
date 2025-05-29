import { Habit } from "../data/Habit";
import { AuthedHttpService } from "./AuthedHttpService";
import { HttpServiceRes, RequestMethod } from "./HttpService";

export class HabitService extends AuthedHttpService {
    constructor(private readonly _backendUrl: string) {
        super();
    }

    public async CreateHabit(name: string): Promise<void> {
        await this.AuthedRequest(
            `${this._backendUrl}habits/Habit`,
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
        return await this.AuthedRequest<Habit[]>(
            `${this._backendUrl}habits/Habit`,
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

    public async GetHabitById(habitId: string): Promise<HttpServiceRes<Habit>> {
        return await this.AuthedRequest<Habit>(
            `${this._backendUrl}habits/Habit`,
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
                        key: "habitId",
                        value: habitId
                    }
                ]
            }
        )
    }

    public async UpdateHabit(id: string, name: string): Promise<void> {
        await this.AuthedRequest(
            `${this._backendUrl}`,
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
                    name
                })
            }
        )
    }

    public async DeleteHabit(id: string): Promise<void> {
        await this.AuthedRequest(
            `${this._backendUrl}`,
            {
                method: RequestMethod.DELETE,
                params: [
                    {
                        key: "habitId",
                        value: id
                    }
                ]
            }
        )
    }
}