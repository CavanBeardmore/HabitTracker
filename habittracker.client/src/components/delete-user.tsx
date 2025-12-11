import { ChangeEvent, useEffect, useState } from "react";
import { Credentials } from "../classes/AuthenticationService";

export const DeleteUser = ({onDeleteClicked}: {onDeleteClicked: (creds: Credentials) => void}) => {
    const [credentials, setCredentials] = useState<Credentials>({
        username: "",
        password: ""
    });
    
    const [disabled, setDisabled] = useState<boolean>(true);

    const onChange = (e: ChangeEvent<HTMLInputElement>) => {
        const {name, value} = e.target;

        setCredentials((prev) => {
            return {
                ...(prev),
                [name]: value
            }
        });
    }

    useEffect(() => {
        const hasValues: boolean = credentials?.username !== "" && credentials?.password !== "";
            
        setDisabled(!hasValues);
    }, [credentials]);

    return (
        <div className="flex flex-col space-y-6 w-full items-center">
            <input placeholder="Username" className="border-1 border-s-stone-300 rounded-md p-2" value={credentials?.username} name="username" onChange={onChange}/>
            <input placeholder="Password" className="border-1 border-s-stone-300 rounded-md p-2" value={credentials?.password} name="password" onChange={onChange}/>
            <button 
                className="text-lg font-semibold border-1 border-s-stone-300 rounded-md p-4 text-stone-300 disabled:opacity-90 w-[50%]"
                disabled={disabled}
                onClick={() => onDeleteClicked(credentials)}
            >
                Delete
            </button>
        </div>
    )
}