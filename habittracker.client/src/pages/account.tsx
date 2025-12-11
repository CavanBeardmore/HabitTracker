import { useEffect, useState } from "react";
import { AuthedWrapper } from "../components/authed-wrapper"
import { useUserService } from "../hooks/useUserService"
import { UpdateUser } from "../components/update-user";
import { DeleteUser } from "../components/delete-user";
import { UpdateUserArgs } from "../classes/UserService";
import { Autorenew } from "@mui/icons-material";
import { Credentials } from "../classes/AuthenticationService";

export const Account = ({onLogOut}: {onLogOut: () => void}) => {
    const {
        registerUserEvents,
        removeUserEvents,
        updateUser,
        deleteUser,
        getUser,
        userDetails,
        updating,
        deleting
    } = useUserService();

    const [showUpdate, setShowUpdate] = useState<boolean>(false);
    const [showDelete, setShowDelete] = useState<boolean>(false);

    const handleUpdateUser = async (args: UpdateUserArgs): Promise<void> => {
        await updateUser(args);
        setShowUpdate(false);
    }

    const handleDeleteUser = async (creds: Credentials): Promise<void> => {
        await deleteUser(creds);
        setShowDelete(false);
    }

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
                    <div className="font-semibold text-left space-y-4">
                        <p>Username: {userDetails?.Username}</p>
                        <p>Email: {userDetails?.Email}</p>
                    </div>
                )}
                {(updating || deleting) && (
                    <Autorenew className="animate-spin"/>
                )}
                {(showUpdate && userDetails && !showDelete && !updating) && (
                    <UpdateUser userDetails={userDetails} onUpdateUser={handleUpdateUser} />
                )}
                {(showDelete && !showUpdate && !deleting) && (
                    <DeleteUser onDeleteClicked={handleDeleteUser} />
                )}
                {
                    !showDelete && (
                        <button 
                            className="text-lg font-semibold border-1 border-s-stone-300 rounded-md p-4 text-stone-300 hover:underline hover:text-xl"
                            onClick={() => setShowUpdate((prev) => !prev)}
                        >
                            {!showUpdate ? "Update" : "Cancel Update"}
                        </button>
                    )
                }
                {
                    !showUpdate && (
                        <button 
                            className="text-lg font-semibold border-1 border-s-stone-300 rounded-md p-4 text-stone-300 hover:underline hover:text-xl"
                            onClick={() => setShowDelete((prev) => !prev)}
                        >
                            {!showDelete ? "Delete" : "Cancel Delete"}
                        </button>
                    )
                }
                {
                    (!showDelete && !showUpdate) && (
                    <button 
                        className="text-lg font-semibold border-1 border-s-stone-300 rounded-md p-4 text-stone-300 hover:underline hover:text-xl"
                        onClick={onLogOut}
                    >
                        Logout
                    </button>
                    )
                }
            </div>
        </AuthedWrapper>
    )
}