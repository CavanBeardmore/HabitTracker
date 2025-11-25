import { ChangeEvent, useEffect, useState } from "react"
import { UpdateUserArgs } from "../classes/UserService"
import { User } from "../data/User";

export const UpdateUser = ({userDetails, onUpdateUser}: {userDetails: User, onUpdateUser: (data: UpdateUserArgs) => void}) => {
    const [updateUserData, setUpdateUserData] = useState<UpdateUserArgs>({Email: userDetails.Email, OldPassword: userDetails.Password});
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
            (userDetails.Username !== updateUserData.NewUsername || 
            userDetails.Password !== updateUserData.NewPassword);
        
        const valuesArentTheSame: boolean =             
            userDetails.Email !== updateUserData.Email &&
            userDetails.Username !== updateUserData.NewUsername &&
            userDetails.Password !== updateUserData.NewPassword;
            
        setDisabled(hasValues && valuesArentTheSame);
    }, [updateUserData])

    return (
        <div>
            <input value={updateUserData?.Email} name="Email" onChange={onChange}/>
            <input value={updateUserData?.NewUsername} name="NewUsername" onChange={onChange}/>
            <input value={updateUserData?.NewPassword} name="NewPassword" onChange={onChange}/>
            <input value={updateUserData?.OldPassword} name="OldPassword" onChange={onChange}/>
            <button 
                className="text-lg font-semibold text-white hover:underline hover:text-xl disabled:opacity-20"
                disabled={disabled}
                onClick={() => onUpdateUser(updateUserData)}
            >
                Update
            </button>
        </div>
    )
}