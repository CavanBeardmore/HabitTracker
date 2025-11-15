import { Credentials } from "./AuthenticationService";
import { HttpService, RequestMethod } from "./HttpService";

export interface CredentialsWithEmail extends Credentials {
    email: string;
}

export interface UpdateUserArgs {
    NewUsername?: string;
    Email: string;
    OldPassword?: string;
    NewPassword?: string; 
} 

export class UserService extends HttpService {

    constructor(private readonly _backendUrl: string) {super()}

    public async CreateUser(credentials: CredentialsWithEmail): Promise<[boolean, string]> {
        console.log("UserService - CreateUser - attempting to create user");
        const {success, data} = await this.Request<{StatusCode?: number, Message?: string}>(
            `${this._backendUrl}user/User`,
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

        await this.Request(
            `${this._backendUrl}user/User/update`,
            {
                method: RequestMethod.PATCH,
                body: JSON.stringify(args)
            }
        );
    }

    public async DeleteUser(creds: Credentials): Promise<void> {
        console.log("UserService - DeleteUser - attempting to delete user");

        await this.Request(
            `${this._backendUrl}user/User/delete`,
            {
                method: RequestMethod.DELETE,
                body: JSON.stringify(creds)
            }
        );
    }
}