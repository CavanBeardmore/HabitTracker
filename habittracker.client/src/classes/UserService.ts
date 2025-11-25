import { User } from "../data/User";
import { Credentials } from "./AuthenticationService";
import { HttpServiceRes, IHttpService, RequestMethod } from "./IHttpService";

export interface CredentialsWithEmail extends Credentials {
    email: string;
}

export interface UpdateUserArgs {
    NewUsername?: string;
    Email: string;
    OldPassword?: string;
    NewPassword?: string; 
} 

export class UserService {

    constructor(
        private readonly _backendUrl: string,
        private readonly _httpService: IHttpService,
        private readonly _authedHttpService: IHttpService
    ) {}

    public async GetUser(): Promise<HttpServiceRes<User>> {
        console.log("UserService - CreateUser - attempting to get user");
        return await this._httpService.Request<User>(
            `${this._backendUrl}/User`,
            {
                method: RequestMethod.GET,
            }
        );
    }

    public async CreateUser(credentials: CredentialsWithEmail): Promise<[boolean, string]> {
        console.log("UserService - CreateUser - attempting to create user");
        const {success, data} = await this._httpService.Request<{StatusCode?: number, Message?: string}>(
            `${this._backendUrl}/User`,
            {
                method: RequestMethod.POST,
                headers: [{key: "Content-Type", value: "application/json"}],
                body: JSON.stringify(credentials)
            }
        );

        if (success) {
            return [success, ""];
        }

        return [success, data?.Message || "An error has occurred."];
    }

    public async UpdateUser(args: UpdateUserArgs): Promise<void> {
        console.log("UserService - UpdateUser - attempting to update user");

        await this._authedHttpService.Request(
            `${this._backendUrl}/User/update`,
            {
                method: RequestMethod.PATCH,
                body: JSON.stringify(args)
            }
        );
    }

    public async DeleteUser(creds: Credentials): Promise<void> {
        console.log("UserService - DeleteUser - attempting to delete user");

        await this._authedHttpService.Request(
            `${this._backendUrl}/User/delete`,
            {
                method: RequestMethod.DELETE,
                body: JSON.stringify(creds)
            }
        );
    }
}