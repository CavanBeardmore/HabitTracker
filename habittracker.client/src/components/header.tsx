import { AccountBox, AutoGraph, DateRange, FactCheck, Home } from "@mui/icons-material"
import atomic from "../assets/atom.png"
import { useNavigate } from "react-router-dom"

enum HeaderButtonPath {
    HOME = "/",
    HABITS = "/habits",
    LOGS = "/logs",
    CHARTS = "/charts",
    ACCOUNT = "/account"
}

export const Header = () => {

    const navigate = useNavigate();

    const navigateToPath = (path: HeaderButtonPath) => {
        if (window.location.pathname !== path) {
            navigate(path);
        } 
    }

    return (
        <div className="flex flex-row top-0 absolute bg-gradient-to-br from-blue-900 via-indigo-900 to-purple-900 w-full border-1 border-b-stone-300 z-50 p-4 items-center justify-between">
            <div className="flex flex-row items-center justify-items-center space-x-4 select-none w-full">
                <img 
                    src={atomic} 
                    alt="atomic-icon" 
                    height={50} 
                    width={50} 
                />
            </div>
            <div className="flex flex-row items-center justify-items-center space-x-8">
                <button onClick={() => navigateToPath(HeaderButtonPath.HOME)}><Home className="text-stone-300" fontSize="large"/></button>
                <button onClick={() => navigateToPath(HeaderButtonPath.HABITS)}><FactCheck className="text-stone-300" fontSize="large"/></button>
                <button onClick={() => navigateToPath(HeaderButtonPath.LOGS)}><DateRange className="text-stone-300" fontSize="large"/></button>
                <button onClick={() => navigateToPath(HeaderButtonPath.CHARTS)}><AutoGraph className="text-stone-300" fontSize="large"/></button>
                <button onClick={() => navigateToPath(HeaderButtonPath.ACCOUNT)}><AccountBox className="text-stone-300" fontSize="large"/></button>
            </div>
        </div>
    )
}