import { ToastContainer as Container } from 'react-toastify';

export const ToastContainer = () => {
    return (
        <Container
            position='bottom-center'
            autoClose={false}
            newestOnTop={true}
            stacked={false}
            closeOnClick={true}
            limit={1}
            toastClassName={() =>
                "relative flex flex-col-reverse items-center bg-blue-900 text-stone-300 text-xl px-4 py-2 rounded-lg shadow-lg mb-4 border-stone-300 border w-fit py-8"}
        />
    )
}