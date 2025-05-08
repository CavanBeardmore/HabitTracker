import { ReactNode, useEffect, useState } from 'react';
import './App.css';
import { Login } from './pages/login';
import { Home } from './pages/home';
import { Resolve } from '@here-mobility/micro-di';
import { AuthenticationService, Credentials } from './classes/AuthenticationService';
import { GlobalEventObserver } from './classes/GlobalEventObserver';
import { CredentialsWithEmail, UserService } from './classes/UserService';
import { IServerEventHandler } from './classes/IServerEventHandler';
import { HabitService } from './classes/HabitService';

enum UnauthedPages {
    LOGIN
}

enum AuthedPages {
    HOME
}

export const App = () => {
    const authService = Resolve<AuthenticationService>(AuthenticationService);
    const userService = Resolve<UserService>(UserService);
    const globalEvents = Resolve<GlobalEventObserver>(GlobalEventObserver);
    const sseEventHandler = Resolve<IServerEventHandler>("IServerEventHandler");
    const habitService = Resolve<HabitService>(HabitService);

    //@ts-ignore
    window.auth = authService;

    const [currentAuthedPage, setAuthedCurrentPage] = useState<AuthedPages>(AuthedPages.HOME);
    const [currentUnauthedPage, setUnauthedCurrentPage] = useState<UnauthedPages>(UnauthedPages.LOGIN);
    const [isAuthed, setIsAuthed] = useState<boolean>(false);

    const onLoginSubmit = async (creds: Credentials): Promise<void> => {
        const [success, errorMessage] = await authService.Login(creds);

        if (success) {
            setIsAuthed(true);
            setAuthedCurrentPage(AuthedPages.HOME);
            return;
        }

        globalEvents.raise("login-failed", errorMessage);
    }

    const onRegisterSubmit = async (creds: CredentialsWithEmail): Promise<void> => {
        const [success, errorMessage] = await userService.CreateUser(creds);

        if (success) {
            globalEvents.raise("register-user-success");
        } else {
            console.log("FAILURE IN APP", errorMessage)
            globalEvents.raise("register-user-failure", errorMessage);
        }
    }

    const authedPagesLookup = new Map<AuthedPages, ReactNode>([
        [AuthedPages.HOME, <Home />]
    ]);
    
    const unauthedPagesLookup = new Map<UnauthedPages, ReactNode>([
        [UnauthedPages.LOGIN, <Login onLoginSubmit={onLoginSubmit} onRegisterSubmit={onRegisterSubmit}/>]
    ]);

    const createServerConnection = async (): Promise<void> => {
        await sseEventHandler.CreateConnection();
    }

    const closeServerConnection = (): void => {
        sseEventHandler.CloseConnection();
    }

    useEffect(() => {
        const isAuthed = authService.IsUserAuthed();

        if (isAuthed === false) {
            setUnauthedCurrentPage(UnauthedPages.LOGIN);
            setIsAuthed(false);
            return;
        }

        createServerConnection();
        setAuthedCurrentPage(AuthedPages.HOME);
        setIsAuthed(true);
    }, []);

    useEffect(() => {
        return () => {
            closeServerConnection();
        }
    }, [])

    return (
        <div className='h-full w-full'>
            {
                isAuthed
                ? authedPagesLookup.get(currentAuthedPage)
                : unauthedPagesLookup.get(currentUnauthedPage)
            }
        </div>
    );
}