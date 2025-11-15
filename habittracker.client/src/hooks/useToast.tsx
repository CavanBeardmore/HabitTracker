import { toast,  } from "react-toastify";
import { Toast } from "../components/toast/toast";

interface UseToastReturn {
    showToast: (message: string, buttons: {text: string, onClick: () => void}[]) => void;
}

export const useToast = (): UseToastReturn => {

      const showToast = (message: string, buttons: {text: string, onClick: () => void}[]) => {
        toast(({closeToast}) => Toast({message, buttons: [...buttons, {text: "Close", onClick: closeToast}]}));
    }

    return {
        showToast
    }
}