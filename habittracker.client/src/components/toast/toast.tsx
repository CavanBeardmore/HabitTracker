interface ToastProps {
    message: string;
    buttons: {
        text: string;
        onClick: () => void;
    }[]
}

export const Toast = ({message, buttons}: ToastProps) => {
    return (
        <div className="flex flex-col space-y-4">
            <p className="font-thin text-md">{message}</p>
            <div className="flex flex-row items-center justify-center space-x-2">
                {buttons.map(({onClick, text}, index) => {
                    return (
                        <button key={index} className="underline px-2" onClick={onClick}>
                            {text}
                        </button>
                    )
                })}
            </div>
        </div>
    )
} 