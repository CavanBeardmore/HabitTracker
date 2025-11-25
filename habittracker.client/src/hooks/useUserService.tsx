import { Resolve } from "@here-mobility/micro-di";
import { UpdateUserArgs, UserService } from "../classes/UserService";
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { EventType } from "../classes/ServerEventHandler";
import { createRandomUUID } from "../utils/createRandomUUID";
import { Credentials } from "../classes/AuthenticationService";
import { useState } from "react";
import { User } from "../data/User";

interface UseUserServiceReturn {
    registerUserEvents(): void;
    removeUserEvents(): void;
    updateUser: (args: UpdateUserArgs) => Promise<void>;
    deleteUser: (creds: Credentials) => Promise<void>;
    getUser: () => Promise<void>;
    userDetails: User | null;
}

export const useUserService = (): UseUserServiceReturn => {

    const userService: UserService = Resolve<UserService>(UserService);
    const globalEventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);

    const uuid: string = createRandomUUID();

    const [userDetails, setUserDetails] = useState<User | null>(null);

    const registerUserEvents = (): void => {
        globalEventObserver.add(
            EventType.USER_UPDATED, 
            `${EventType.USER_UPDATED}_ID_${uuid}`,
            () => {
                console.log("user has been updated");
            }
        );

        globalEventObserver.add(
            EventType.USER_DELETED, 
            `${EventType.USER_DELETED}_ID_${uuid}`,
            () => {
                console.log("user has been deleted");
            }
        );
    };

    const removeUserEvents = (): void => {
        globalEventObserver.remove(
            EventType.USER_UPDATED, 
            `${EventType.USER_UPDATED}_ID_${uuid}`,
        );

        globalEventObserver.remove(
            EventType.USER_DELETED, 
            `${EventType.USER_DELETED}_ID_${uuid}`,
        );
    };

    const getUser = async (): Promise<void> => {
        const {success, status, data, errorMessage}= await userService.GetUser();

        if (success && data) {
            setUserDetails(data);
            return;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise("UNAUTHED", errorMessage);
            return;
        }

        if (success === false) globalEventObserver.raise(EventType.ERROR);
    }

    const updateUser = async (args: UpdateUserArgs) => await userService.UpdateUser(args);

    const deleteUser = async (creds: Credentials) => await userService.DeleteUser(creds);

    return {
        registerUserEvents,
        removeUserEvents,
        updateUser,
        deleteUser,
        getUser,
        userDetails
    };
}