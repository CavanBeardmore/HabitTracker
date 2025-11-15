import { Autorenew, ChevronLeft, ChevronRight } from "@mui/icons-material";
import { useEffect, useState } from "react";

interface PaginatorButton {
    onClick: (pageNumber: number) => Promise<void>;
    isDisabled?: boolean;
}

interface PaginatorProps {
    onBack: PaginatorButton;
    onForward: PaginatorButton;
    pageNumber: number;
    setPageNumber: React.Dispatch<React.SetStateAction<number>>
}

export const Paginator = ({onBack, onForward, pageNumber, setPageNumber}: PaginatorProps) => {
    const [buttonsLoading, setButtonsLoading] = useState<{
        back: boolean;
        forward: boolean;
    }>({
        back: false,
        forward: false
    });

    const handleOnBack = async () => {
        if (buttonsLoading.back) return;

        const page = pageNumber - 1;
        setButtonsLoading((prev) => ({...prev, back: true}));
        await onBack.onClick(page);
        setPageNumber(page);
        setButtonsLoading((prev) => ({...prev, back: false}));
    };

    const handleOnForward = async () => {
        if (buttonsLoading.forward) return;
        
        const page = pageNumber + 1;
        setButtonsLoading((prev) => ({...prev, forward: true}));
        await onForward.onClick(page);
        setPageNumber(page);
        setButtonsLoading((prev) => ({...prev, forward: false}));
    };

    useEffect(() => {
        console.log(buttonsLoading)
    }, [buttonsLoading])

    return (
        <div className="flex flex-row space-x-6 items-center">
            <button
                className="disabled:opacity-50"
                disabled={
                    pageNumber === 1 ||
                    buttonsLoading.back
                } 
                onClick={handleOnBack}
            >
                {
                    buttonsLoading.back
                    ? <Autorenew className="animate-spin" fontSize="large"/>
                    : <ChevronLeft fontSize="large" /> 
                }
            </button>
            <p className="font-semibold text-lg">{pageNumber}</p>
            <button
                className="disabled:opacity-50"
                disabled={
                    onForward.isDisabled ||
                    buttonsLoading.forward
                } 
                onClick={handleOnForward}
            >
                {
                    buttonsLoading.forward
                    ? <Autorenew className="animate-spin" fontSize="large"/>
                    : <ChevronRight fontSize="large" /> 
                }
            </button>
        </div>
    )
}