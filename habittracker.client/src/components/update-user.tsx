import { ChangeEvent, useEffect, useState } from "react"
import { UpdateUserArgs } from "../classes/UserService"
import { User } from "../data/User";
import { Autorenew } from "@mui/icons-material";

export const UpdateUser = ({userDetails, onUpdateUser}: {userDetails: User, onUpdateUser: (data: UpdateUserArgs) => void}) => {
    const [updateUserData, setUpdateUserData] = useState<UpdateUserArgs>({Email: userDetails.Email, NewUsername: userDetails.Username});
    const [disabled, setDisabled] = useState<boolean>(true);

    const onChange = (e: ChangeEvent<HTMLInputElement>) => {
        const {name, value} = e.target;

        setUpdateUserData((prev) => {
            return {
                ...prev,
                [name]: value
            }
        });
    }

    useEffect(() => {
        const hasValues: boolean = 
            updateUserData.Email !== "" &&
            updateUserData.Email !== undefined &&
            updateUserData.NewUsername !== "" &&
            updateUserData.NewUsername !== undefined;
            updateUserData.OldPassword !== "" &&
            updateUserData.OldPassword !== undefined;
        
        const valuesArentTheSame: boolean =             
            userDetails.Email !== updateUserData.Email;

        setDisabled(!hasValues && !valuesArentTheSame);
    }, [updateUserData]);

    return (
        <div className="flex flex-col space-y-6 w-full items-center">
            <input placeholder="Email" className="border-1 border-s-stone-300 rounded-md p-2" value={updateUserData?.Email} name="Email" onChange={onChange}/>
            <input placeholder="Username" className="border-1 border-s-stone-300 rounded-md p-2" value={updateUserData?.NewUsername} name="NewUsername" onChange={onChange}/>
            <input placeholder="New Password" className="border-1 border-s-stone-300 rounded-md p-2" value={updateUserData?.NewPassword} name="NewPassword" onChange={onChange}/>
            <input placeholder="Current Password" className="border-1 border-s-stone-300 rounded-md p-2" value={updateUserData?.OldPassword} name="OldPassword" onChange={onChange}/>
            <button 
                className="text-lg font-semibold border-1 border-s-stone-300 rounded-md p-4 text-stone-300 disabled:opacity-90 w-[50%]"
                disabled={disabled}
                onClick={() => onUpdateUser(updateUserData)}
            >
                Update
            </button>
        </div>
    )
}