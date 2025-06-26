import { ReactNode } from "react"


export const Wrapper = ({children}: {children: ReactNode}) => {
    return (
        <div className="flex flex-col min-h-screen w-full bg-gradient-to-br from-purple-900 via-indigo-900 to-blue-900 pt-[80px]">
            <div className="flex justify-center items-start w-full px-4 py-8">
                {children}
            </div>
        </div>
    )
}
