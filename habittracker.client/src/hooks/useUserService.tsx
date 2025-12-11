import { Resolve } from "@here-mobility/micro-di";
import { UpdateUserArgs, UserService } from "../classes/UserService";
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { EventType } from "../classes/ServerEventHandler";
import { createRandomUUID } from "../utils/createRandomUUID";
import { AuthenticationService, Credentials } from "../classes/AuthenticationService";
import { useState } from "react";
import { User } from "../data/User";
import { useToast } from "./useToast";

interface UseUserServiceReturn {
    registerUserEvents(): void;
    removeUserEvents(): void;
    updateUser: (args: UpdateUserArgs) => Promise<void>;
    deleteUser: (creds: Credentials) => Promise<void>;
    getUser: () => Promise<void>;
    userDetails: User | null;
    updating: boolean;
    deleting: boolean;
}

export const useUserService = (): UseUserServiceReturn => {

    const userService: UserService = Resolve<UserService>(UserService);
    const globalEventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);
    const authService = Resolve<AuthenticationService>(AuthenticationService);

    const { showToast } = useToast();

    const [userDetails, setUserDetails] = useState<User | null>(null);
    const [updating, setUpdating] = useState<boolean>(false);
    const [deleting, setDeleting] = useState<boolean>(false);

    const registerUserEvents = (): void => {
        globalEventObserver.add(
            EventType.ERROR,
            `${EventType.ERROR}_USE_USER_SERVICE`,
            () => {
                setUpdating(false);
                setDeleting(false);
            }
        )

        globalEventObserver.add(
            EventType.USER_UPDATED, 
            `${EventType.USER_UPDATED}_USE_USER_SERVICE`,
            (data: {jwt: string, user: User}) => {
                console.log("UPDATED USER DATA", data)
                setUpdating(false);
                setUserDetails(data.user);
                authService.ReplaceAuthToken(data.jwt);
                showToast("Successfully updated user information.", []);
            }
        );

        globalEventObserver.add(
            EventType.USER_DELETED, 
            `${EventType.USER_DELETED}_USE_USER_SERVICE`,
            () => {
                setDeleting(false);
                setUserDetails(null);
                authService.RemoveAuthToken();
                showToast("Successfully deleted account.", []);
            }
        );
    };

    const removeUserEvents = (): void => {
        globalEventObserver.remove(
            EventType.USER_UPDATED, 
            `${EventType.USER_UPDATED}_USE_USER_SERVICE`,
        );

        globalEventObserver.remove(
            EventType.USER_DELETED, 
            `${EventType.USER_DELETED}_USE_USER_SERVICE`,
        );

        globalEventObserver.remove(
            EventType.ERROR, 
            `${EventType.ERROR}_USE_USER_SERVICE`,
        );
    };

    const getUser = async (): Promise<void> => {
        const {success, status, data, errorMessage}= await userService.GetUser();

        if (success && data) {
            setUserDetails(data);
            return;
        }

        if (status === 401 || status == 403) {
            globalEventObserver.raise(EventType.UNAUTHED, errorMessage);
            return;
        }
    }

    const updateUser = async (args: UpdateUserArgs): Promise<void> => {
        setUpdating(true);
        await userService.UpdateUser(args);
    }

    const deleteUser = async (creds: Credentials): Promise<void> => await userService.DeleteUser(creds);

    return {
        registerUserEvents,
        removeUserEvents,
        updateUser,
        deleteUser,
        getUser,
        userDetails,
        updating,
        deleting
    };
}