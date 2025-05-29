import { ReactNode } from "react"
import { Header } from "./header"
import { Wrapper } from "./wrapper"

export const AuthedWrapper = ({children}: {children: ReactNode}) => {
    return (
        <Wrapper>
            <Header />
            {children}
        </Wrapper>
    )
}