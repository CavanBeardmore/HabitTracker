import { HttpService, RequestMethod } from "./HttpService";

export class HabitService extends HttpService {
    constructor(private readonly _backendUrl: string) {
        super();
    }

    public async CreateHabit(name: string): Promise<void> {
        const jwt = this.RetrieveJwtFromStorage()
        this.Request(
            `${this._backendUrl}habits/Habit`,
            {
                method: RequestMethod.POST,
                headers: [
                    {
                        key: "Authorization",
                        value: `Bearer ${jwt}`
                    },
                    {
                        key: "Content-Type", 
                        value: "application/json"
                    }
                ],
                body: JSON.stringify({name})
            }
        )
    }

    private RetrieveJwtFromStorage(): string {
        const token = localStorage.getItem("AUTH_TOKEN");

        if (token === null) {
            throw new Error("No token found");
        }

        return token;
    }
}