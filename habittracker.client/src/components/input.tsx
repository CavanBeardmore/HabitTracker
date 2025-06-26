import { ChangeEvent, useState } from "react";
import {Visibility, VisibilityOff} from "@mui/icons-material"

export enum InputType {
    TEXT = "text",
    EMAIL = "email",
    PASSWORD = "password"
}

interface InputProps {
    onChange: (e: ChangeEvent<HTMLInputElement>) => void;
    placeholder: string;
    value: string;
    name: string;
    type: InputType;
    required?: boolean;
}

export const Input = ({
    onChange,
    placeholder,
    value,
    name,
    type,
    required = false
}: InputProps) => {

    const [isVisible, setIsVisible] = useState<boolean>(false);

    return (
        <div className="relative flex flex-row bg-gray-500 p-2 rounded-md text-stone-300 placeholder:text-stone-300 border-stone-300 border-1 w-full">
            <input 
                required={required}
                className="flex-grow placeholder:text-stone-300 focus:outline-none"
                type={isVisible ? InputType.TEXT : type}
                onChange={onChange} 
                value={value} 
                name={name} 
                placeholder={placeholder}
            />
            { type === InputType.PASSWORD && (
                <button
                    className="absolute right-2 top-1/2 transform -translate-y-1/2"
                    onClick={() => setIsVisible((prev) => {
                        return !prev;
                    })}
                >
                    {
                        isVisible 
                        ? <Visibility />
                        : <VisibilityOff />
                    }
                </button>
            )}
        </div>
    )
}