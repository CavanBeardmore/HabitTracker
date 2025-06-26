import { useEffect, useState } from 'react';
import './App.css';
import { Login } from './pages/login';
import { Home } from './pages/home';
import { Resolve } from '@here-mobility/micro-di';
import { AuthenticationService, Credentials } from './classes/AuthenticationService';
import { GlobalEventObserver } from './classes/GlobalEventObserver';
import { CredentialsWithEmail, UserService } from './classes/UserService';
import { IServerEventHandler } from './classes/IServerEventHandler';
import { Route, Routes, useNavigate } from 'react-router-dom';
import { EventType } from './classes/ServerEventHandler';
import { Habits } from './pages/habits';
import { ToastContainer } from './components/toast/toast-container';
import { AddHabit } from './pages/add-habit';
import { useToast } from './hooks/useToast';
import { useHabitService } from './hooks/useHabitService';

const UNAUTHORISED_CODE = 401;

export enum Paths {
    LOGIN = "/login",
    HOME = "/",
    HABITS = "/habits",
    HABITS_ADD = "/habits/add",
    LOGS = "/logs",
    CHARTS = "/charts",
    ACCOUNT = "/account"
}

export const App = () => {
    const authService = Resolve<AuthenticationService>(AuthenticationService);
    const userService = Resolve<UserService>(UserService);
    const globalEvents = Resolve<GlobalEventObserver>(GlobalEventObserver);
    const sseEventHandler = Resolve<IServerEventHandler>("IServerEventHandler");

    //@ts-ignore
    window.auth = authService;

    const navigate = useNavigate();
    const {showToast} = useToast();

    const [isAuthed, setIsAuthed] = useState<boolean>(false);

    const onLoginSubmit = async (creds: Credentials): Promise<void> => {
        const [success, errorMessage] = await authService.Login(creds);

        if (success) {
            setIsAuthed(true);
            navigate("/");
            return;
        }

        globalEvents.raise("login-failed", errorMessage);
    }

    const onRegisterSubmit = async (creds: CredentialsWithEmail): Promise<void> => {
        const [success, errorMessage] = await userService.CreateUser(creds);

        if (success) {
            globalEvents.raise("register-user-success");
        } else {
            globalEvents.raise("register-user-failure", errorMessage);
        }
    }

    const createServerConnection = async (): Promise<void> => {
        await sseEventHandler.CreateConnection();
    }

    const closeServerConnection = (): void => {
        sseEventHandler.CloseConnection();
    }

    const handleError = (statusCode: number, message: string) => {
        showToast(message);
        if (statusCode === UNAUTHORISED_CODE) {
            closeServerConnection();
            authService.RemoveAuthToken();
            handleUnauthed();
        }
    }

    const handleUnauthed = () => {
        setIsAuthed(false);
        navigate("/login");
    }

    useEffect(() => {
        const isAuthed = authService.IsUserAuthed();

        if (isAuthed === false) {
            console.log("NOT AUTHED")
            handleUnauthed();
            return;
        }

        createServerConnection();
        setIsAuthed(true);
        globalEvents.add(
            EventType.ERROR, 
            `${EventType.ERROR}_ID`, 
            ({statusCode, message}: {statusCode: number, message: string}) => handleError(statusCode, message)
        );

        return () => {
            globalEvents.remove(
                EventType.ERROR, 
                `${EventType.ERROR}_ID`,
            );
            closeServerConnection();
        }
    }, []);

    return (
        <div className='w-full font-sans'>
            {isAuthed && (
                <Routes>
                    <Route path={Paths.HOME} element={<Home />} />
                    <Route path={Paths.HABITS} element={<Habits />} />
                    <Route path={Paths.HABITS_ADD} element={<AddHabit />} />
                </Routes>
            )}
            {isAuthed === false && (
                <Routes>
                    <Route path={Paths.LOGIN} element={<Login onLoginSubmit={onLoginSubmit} onRegisterSubmit={onRegisterSubmit}/>} />
                </Routes>
            )}
            <ToastContainer />
        </div>
    );
}