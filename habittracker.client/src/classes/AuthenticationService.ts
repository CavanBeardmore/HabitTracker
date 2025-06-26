import { HttpService, RequestMethod } from "./HttpService";
import { jwtDecode } from "jwt-decode";

const AUTH_TOKEN = "AUTH_TOKEN";

export interface Credentials {
    username: string;
    password: string;
}

export class AuthenticationService extends HttpService {

    private _authToken: string | null = null;

    constructor(private readonly _backendUrl: string) {
        super();
    }

    public async Login(credentials: Credentials): Promise<[boolean, string]> {
        console.log("AuthenticationService - Login - attempting to log in");
        const {success, data} = await this.Request<{token?: string, StatusCode?: number, Message?: string}>(
            `${this._backendUrl}auth/Auth/login`,
            {
                method: RequestMethod.GET,
                params: [
                    {
                        key: "username",
                        value: credentials.username
                    },
                    {
                        key: "password",
                        value: credentials.password
                    }
                ]
            }
        );

        if (success && data !== null) {
            console.log("AuthenticationService - Login - log in successful");
            if (data.token) {
                this._authToken = data.token;
                localStorage.setItem(AUTH_TOKEN, this._authToken);
            }
            return [true, ""];
        }

        console.log("AuthenticationService - Login - error", data?.Message);

        return [false, data?.Message || "An error has occurred."];
    }

    public IsUserAuthed(): boolean {
        console.log("AuthenticationService - IsUserAuthed - getting token from storage");
        const token = localStorage.getItem(AUTH_TOKEN);

        console.log("AuthenticationService - IsUserAuthed - token", token);

        if (token === null) return false;

        this._authToken = token;
        
        const expiry = this.GetExpiryFromJwt();

        if (expiry === undefined) {
            console.log("AuthenticationService - IsUserAuthed - has no expiry");
            this.RemoveAuthToken();
            return false;
        }

        const dateNow = Math.floor(Date.now() / 1000);
        console.log(`AuthenticationService - IsUserAuthed - checking if ${dateNow} > ${expiry} `);
        
        if (dateNow > expiry) {
            console.log("AuthenticationService - IsUserAuthed - is out of date");
            this.RemoveAuthToken();
            return false;
        }

        return true;
    }

    private GetExpiryFromJwt(): number | undefined {
        try {
            if (this._authToken === null) return;

            const decoded = jwtDecode(this._authToken, {});
            return decoded.exp;
        } catch {
            return;
        }
    }

    public RemoveAuthToken(): void {
        localStorage.removeItem(AUTH_TOKEN);
        this._authToken = null;
    }
}