import { ReactNode } from "react"

export const Wrapper = ({children}: {children: ReactNode}) => {
    return (
        <div className="flex bg-gray-800 h-full w-full justify-center items-center m-auto">
            {children}
        </div>
    )
}