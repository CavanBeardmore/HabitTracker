import { ReactNode, useEffect, useState } from 'react';
import './App.css';
import { Login } from './pages/login';
import { Home } from './pages/home';
import { Resolve } from '@here-mobility/micro-di';
import { AuthenticationService, Credentials } from './classes/AuthenticationService';
import { GlobalEventObserver } from './classes/GlobalEventObserver';
import { CredentialsWithEmail, UserService } from './classes/UserService';
import { IServerEventHandler } from './classes/IServerEventHandler';
import { Route, Routes, useNavigate } from 'react-router-dom';

export const App = () => {
    const authService = Resolve<AuthenticationService>(AuthenticationService);
    const userService = Resolve<UserService>(UserService);
    const globalEvents = Resolve<GlobalEventObserver>(GlobalEventObserver);
    const sseEventHandler = Resolve<IServerEventHandler>("IServerEventHandler");

    //@ts-ignore
    window.auth = authService;

    const navigate = useNavigate();

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

    useEffect(() => {
        const isAuthed = authService.IsUserAuthed();

        if (isAuthed === false) {
            setIsAuthed(false);
            navigate("/login");
            return;
        }

        createServerConnection();
        setIsAuthed(true);
    }, []);

    useEffect(() => {
        return () => {
            closeServerConnection();
        }
    }, [])

    return (
        <div className='h-full w-full select-none font-sans'>
            {isAuthed && (
                <Routes>
                    <Route path='/' element={<Home />} />
                </Routes>
            )}
            {isAuthed === false && (
                <Routes>
                    <Route path='/login' element={<Login onLoginSubmit={onLoginSubmit} onRegisterSubmit={onRegisterSubmit}/>} />
                </Routes>
            )}
        </div>
    );
}