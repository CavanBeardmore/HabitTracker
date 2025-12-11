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
import { ToastContainer } from './components/toast/toast-container';;
import { useToast } from './hooks/useToast';
import { Account } from './pages/account';
import { AddHabit } from './pages/add-habit';
import { SelectedHabit } from './pages/habit';

const UNAUTHORISED_CODE = 401;

export enum Paths {
    LOGIN = "/login",
    HOME = "/",
    HABITS = "/habits",
    HABIT = "/habit",
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

    const handleError = (error: {statusCode: number, errorMessage: string}) => {
        showToast(error.errorMessage, []);
        if (error.statusCode === UNAUTHORISED_CODE) {
            handleUnauthed();
        }
    }

    const handleUnauthed = () => {
        authService.RemoveAuthToken();
        setIsAuthed(false);
        closeServerConnection();
        navigate("/login");
    }

    useEffect(() => {
        const isAuthed = authService.IsUserAuthed();

        if (isAuthed === false) {
            console.log("NOT AUTHED")
            handleUnauthed();
            return;
        }

        setIsAuthed(true);
    }, []);

    useEffect(() => {
        if (!isAuthed) return;

        createServerConnection();

        return () => {
            if (!isAuthed) return;
            closeServerConnection();
        } 
    }, [isAuthed]);

    useEffect(() => {
        globalEvents.add(
            EventType.ERROR, 
            `APP_${EventType.ERROR}_ID`, 
            (error: {statusCode: number, errorMessage: string}) => handleError(error)
        );

        globalEvents.add(
            EventType.UNAUTHED, 
            `APP_${EventType.UNAUTHED}_ID`, 
            () => {
                console.log("HANDLING UNAUTHED");
                handleUnauthed();
            }
        );

        globalEvents.add(
            EventType.USER_DELETED,
            `APP_${EventType.USER_DELETED}_ID`, 
            () => {
                setTimeout(() => {
                    handleUnauthed();
                }, 2_000);
            }
        )

        return () => {
            globalEvents.remove(
                EventType.ERROR, 
                `${EventType.ERROR}_ID`,
            );
            globalEvents.remove(
                EventType.UNAUTHED, 
                `APP_${EventType.UNAUTHED}_ID`,
            );
            globalEvents.remove(
                EventType.USER_DELETED,
                `APP_${EventType.USER_DELETED}_ID`,
            )
            closeServerConnection();
        }

    }, [globalEvents])

    return (
        <div className='w-full font-sans'>
            {isAuthed && (
                <Routes>
                    <Route path={Paths.HOME} element={<Home />} />
                    <Route path={Paths.HABITS} element={<Habits />} />
                    <Route path={Paths.HABITS_ADD} element={<AddHabit />} />
                    <Route path={Paths.HABIT} element={<SelectedHabit />} />
                    <Route path={Paths.ACCOUNT} element={<Account onLogOut={handleUnauthed} />} />
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