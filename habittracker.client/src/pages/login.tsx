import { ChangeEvent, useEffect, useState } from "react";
import { Wrapper } from "../components/wrapper"
import { Input, InputType } from "../components/input";
import atomic from "../assets/atom.png"
import { GlobalEventObserver } from "../classes/GlobalEventObserver";
import { Resolve } from "@here-mobility/micro-di";
import { createRandomUUID } from "../utils/createRandomUUID";
import { Autorenew } from "@mui/icons-material";
import { Credentials } from "../classes/AuthenticationService";
import { CredentialsWithEmail } from "../classes/UserService";

interface LoginProps {
    onLoginSubmit: (creds: Credentials) => void;
    onRegisterSubmit: (creds: CredentialsWithEmail) => void;
}

export const Login = ({ onLoginSubmit, onRegisterSubmit }: LoginProps) => {

    const globalEvents = Resolve<GlobalEventObserver>(GlobalEventObserver);

    const [loading, setLoading] = useState<boolean>(false);
    const [loginEnabled, setLoginEnabled] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [registrationSuccess, setRegistrationSuccess] = useState<boolean>(false);
    const [loginCredentials, setLoginCredentials] = useState<Credentials>({
        username: "",
        password: ""
    })
    const [registerCredentials, setRegisterCredentials] = useState<CredentialsWithEmail>({
        username: "",
        password: "",
        email: ""
    })

    const toggleShowLogin = () => {
        setError(null);
        setRegistrationSuccess(false);
        setLoginEnabled((prev) => {
            return !prev;
        });
    }

    const onLoginCredsChange = (e: ChangeEvent<HTMLInputElement>) => {
        const {value, name} = e.target;

        setLoginCredentials((prev) => {
            return {
                ...prev,
                [name]: value
            }
        })
    }

    const onRegisterCredsChange = (e: ChangeEvent<HTMLInputElement>) => {
        const {value, name} = e.target;

        setRegisterCredentials((prev) => {
            return {
                ...prev,
                [name]: value
            }
        })
    }
    
    const onRegisterSubmitClicked = () => {

        if (
            !registerCredentials.username || 
            !registerCredentials.password ||
            !registerCredentials.email
        ) {
            setError("All fields are required");
            return;
        }
        setLoading(true)
        setError(null);
        setRegistrationSuccess(false);
        onRegisterSubmit(registerCredentials);
    }

    const onLoginSubmitClicked = () => {

        if (!loginCredentials.username || !loginCredentials.password) {
            setError("All fields are required");
            return;
        }
        setLoading(true)
        setError(null);
        onLoginSubmit(loginCredentials);
    }

    useEffect(() => {
        const id = createRandomUUID();
        globalEvents.add("login-failed", id, (message: string) => {
            setError(message);
            setLoading(false);
        });

        globalEvents.add("register-user-success", id, () => {
            setRegistrationSuccess(true);
            setLoading(false);
        });

        globalEvents.add("register-user-failure", id, (message: string) => {
            console.log("FAILURE IN LOGIN", message)
            setError(message);
            setLoading(false);
        });

        return () => {
            globalEvents.remove("login-failed", id);
            globalEvents.remove("register-user-success", id);
            globalEvents.remove("register-user-failure", id);
        }
    }, [])

    useEffect(() => {
        console.log(registerCredentials)
    }, [registerCredentials])

    return (
        <Wrapper>
            <div className="flex flex-col justify-center text-center items-center bg-gray-700 h-fit w-[45%] rounded-lg m-auto py-8 shadow-lg">
                <div className="flex flex-col w-[85%] space-y-4">
                    <img src={atomic} alt="atomic-icon" height={100} width={100} className="m-auto"/>
                    <p className="text-stone-300 font-bold text-3xl">ATOMIC TRACKER</p>

                    {
                        loginEnabled 
                            ? (
                                <>
                                    <Input 
                                        onChange={onLoginCredsChange}
                                        value={loginCredentials.username} 
                                        placeholder={"Username"}
                                        name={"username"}
                                        type={InputType.TEXT}
                                        required
                                    />
                                    <Input 
                                        onChange={onLoginCredsChange}
                                        value={loginCredentials.password} 
                                        placeholder={"Password"}
                                        name={"password"}
                                        type={InputType.PASSWORD}
                                        required
                                    />
                                    <button 
                                        className="bg-gray-500 p-2 rounded-lg font-bold shadow-lg text-stone-300 text-lg border-1 border-stone-300"
                                        disabled={loading}
                                        onClick={onLoginSubmitClicked}
                                    >
                                        {
                                            loading
                                            ? <Autorenew className="animate-spin"/>
                                            : "Login"
                                        }  
                                    </button>
                                    <div className="flex flex-col space-y-2 text-lg text-stone-300">
                                        <p>
                                            Don't have an account? 
                                        </p>
                                        <button disabled={loading} className="underline w-fit m-auto" onClick={toggleShowLogin}>
                                                Sign up
                                        </button>
                                    </div>
                                </>
                            )
                            : (
                                <>
                                    <Input 
                                        onChange={onRegisterCredsChange}
                                        value={registerCredentials.username} 
                                        placeholder={"Username"}
                                        name={"username"}
                                        type={InputType.TEXT}
                                        required
                                    />
                                    <Input 
                                        onChange={onRegisterCredsChange}
                                        value={registerCredentials.email} 
                                        placeholder={"Email"}
                                        name={"email"}
                                        type={InputType.TEXT}
                                        required
                                    />
                                    <Input 
                                        onChange={onRegisterCredsChange}
                                        value={registerCredentials.password} 
                                        placeholder={"Password"}
                                        name={"password"}
                                        type={InputType.PASSWORD}
                                        required
                                    />
                                    <button 
                                        className="bg-gray-500 p-2 rounded-lg font-bold shadow-lg text-stone-300 text-lg border-1 border-stone-300"
                                        disabled={loading}
                                        onClick={onRegisterSubmitClicked}
                                    >
                                        {
                                            loading
                                            ? <Autorenew className="animate-spin"/>
                                            : "Register"
                                        }     
                                    </button>
                                    <div className="flex flex-col space-y-2 text-stone-300 font-light text-lg">
                                        <p>
                                            Already have an account? 
                                        </p>
                                        <button disabled={loading} className="underline w-fit m-auto" onClick={toggleShowLogin}>
                                            Login
                                        </button>
                                    </div>
                
                                </>
                            )
                    }
                    {error && (
                        <p className="text-red-500">{error}</p>
                    )}
                    {registrationSuccess && (
                        <p className="text-green-500">Successfully created account!</p>
                    )}
                </div>
            </div>
        </Wrapper>
    )
}