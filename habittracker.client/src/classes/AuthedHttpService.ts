import { httpOptions, HttpService, HttpServiceRes } from "./HttpService";

const AUTH_TOKEN = "AUTH_TOKEN";

export abstract class AuthedHttpService extends HttpService {
    
    public async AuthedRequest<T>(url: string, options: httpOptions): Promise<HttpServiceRes<T>> {
        const jwt = this.RetrieveJwtFromStorage()
        return await this.Request(
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