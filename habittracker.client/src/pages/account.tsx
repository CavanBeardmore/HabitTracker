import { useEffect, useState } from "react";
import { AuthedWrapper } from "../components/authed-wrapper"
import { useUserService } from "../hooks/useUserService"
import { UpdateUser } from "../components/update-user";

export const Account = () => {
    const {
        registerUserEvents,
        removeUserEvents,
        updateUser,
        deleteUser,
        getUser,
        userDetails
    } = useUserService();

    const [showUpdate, setShowUpdate] = useState<boolean>(false);
    const [showDelete, setShowDelete] = useState<boolean>(false);

    useEffect(() => {
        if (!userDetails) {
            getUser();
        }

        registerUserEvents();

        return () => {
            removeUserEvents();
        }
    }, []);

    return (
        <AuthedWrapper>
            <div className="flex flex-col space-y-8 text-stone-300 w-[80%] items-center">
                <p className="font-semibold text-4xl">Your Account</p>
                {(!showDelete && !showUpdate) && (
                    <>
                        <p>{userDetails?.Username}</p>
                        <p>{userDetails?.Email}</p>
                    </>
                )}
                {(showUpdate && userDetails) && (
                    <UpdateUser userDetails={userDetails} onUpdateUser={updateUser} />
                )}
                <button 
                    className="text-lg font-semibold text-white hover:underline hover:text-xl"
                    onClick={() => setShowUpdate((prev) => !prev)}
                >
                    {showUpdate ? "Update" : "Cancel Update"}
                </button>
                <button 
                    className="text-lg font-semibold text-white hover:underline hover:text-xl"
                    onClick={() => setShowDelete(true)}
                >
                    {showDelete ? "Delete" : "Cancel Delete"}
                </button>
            </div>
        </AuthedWrapper>
    )
}