import { ReactNode } from "react"

export const Wrapper = ({children}: {children: ReactNode}) => {
    return (
        <div className="flex relative min-h-screen bg-gradient-to-br from-purple-900 via-indigo-900 to-blue-900 overflow-hidden h-full w-full justify-center items-center m-auto">
            {children}
        </div>
    )
}