import { ToastContainer as Container } from 'react-toastify';

export const ToastContainer = () => {
    return (
        <Container
            position='bottom-center'
            autoClose={false}
            closeButton={<ToastButton message={"Close"}/>}
            newestOnTop={false}
            closeOnClick={true}
            toastClassName={() =>
                "relative flex flex-col-reverse items-center bg-blue-900 text-stone-300 text-xl px-4 py-2 rounded-lg shadow-lg mb-4 border-stone-300 border w-fit py-8"}
        />
    )
}

export const ToastButton = ({message}: {message: string}) => {
    return (
        <button
            className="absolute top-2 left-2 font-bold text-stone-300 underline text-sm"
        >
            {message}
        </button>
    )
}