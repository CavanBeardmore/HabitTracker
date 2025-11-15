import { useEffect } from "react";
import { AuthedWrapper } from "../components/authed-wrapper"
import { useUserService } from "../hooks/useUserService"

export const Account = () => {

    const {
        registerUserEvents,
        removeUserEvents,
        updateUser,
        deleteUser
    } = useUserService();

    useEffect(() => {
        registerUserEvents();

        return () => {
            removeUserEvents();
        }
    }, []);

    return (
        <AuthedWrapper>
            <div className="flex flex-col space-y-8 text-stone-300 w-[80%] items-center">
                <p className="font-semibold text-4xl">Your Account</p>
            </div>
        </AuthedWrapper>
    )
}