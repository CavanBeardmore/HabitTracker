import { toast } from "react-toastify";

interface UseToastReturn {
    showToast: (message: string) => void;
}

export const useToast = (): UseToastReturn => {
    const showToast = (message: string) => {
        toast(message);
    }

    return {
        showToast
    }
}