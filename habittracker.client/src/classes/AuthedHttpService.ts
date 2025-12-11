import { GlobalEventObserver } from "./GlobalEventObserver";
import { httpOptions, HttpServiceRes, IHttpService } from "./IHttpService";
import { EventType } from "./ServerEventHandler";

const AUTH_TOKEN = "AUTH_TOKEN";

export class AuthedHttpService implements IHttpService {

    constructor(
        private readonly _httpService: IHttpService,
        private readonly _events: GlobalEventObserver
    ){}
    
    public async Request<T>(url: string, options: httpOptions): Promise<HttpServiceRes<T>> {
        const jwt: string | null = this.RetrieveJwtFromStorage();

        const response: HttpServiceRes<T> = await this._httpService.Request(
            url,
            {
                method: options.method,
                headers: [
                    {
                        key: "Authorization",
                        value: `Bearer ${jwt}`
                    },
                    ...(options.headers || [])
                ],
                body: options.body,
                params: options.params
            }
        )

        if (response.status === 401) {
            this._events.raise(EventType.UNAUTHED);
        }
        
        return response;
    }

    private RetrieveJwtFromStorage(): string | null {
        const token = localStorage.getItem(AUTH_TOKEN);

        if (token === null) {
            this._events.raise(EventType.UNAUTHED)
            return null;
        }

        return token;
    }
}