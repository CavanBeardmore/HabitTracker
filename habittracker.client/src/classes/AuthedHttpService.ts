import { httpOptions, HttpServiceRes, IHttpService } from "./IHttpService";

const AUTH_TOKEN = "AUTH_TOKEN";

export class AuthedHttpService implements IHttpService {

    constructor(
        private readonly _httpService: IHttpService
    ){}
    
    public async Request<T>(url: string, options: httpOptions): Promise<HttpServiceRes<T>> {
        const jwt = this.RetrieveJwtFromStorage()
        return await this._httpService.Request(
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
    }

    private RetrieveJwtFromStorage(): string {
        const token = localStorage.getItem(AUTH_TOKEN);

        if (token === null) {
            throw new Error("No token found");
        }

        return token;
    }
}