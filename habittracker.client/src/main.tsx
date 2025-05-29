import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { App } from './App'
import { BrowserRouter } from 'react-router-dom'
import 'reflect-metadata'
import { AuthenticationService } from './classes/AuthenticationService'
import { RegisterSingleton, RegisterResolver } from '@here-mobility/micro-di';
import { GlobalEventObserver } from './classes/GlobalEventObserver'
import { UserService } from './classes/UserService'
import { ServerEventHandler } from './classes/ServerEventHandler'
import { HabitService } from './classes/HabitService'

const {VITE_BACKEND_URL} = import.meta.env;

RegisterSingleton(AuthenticationService, () => new AuthenticationService(VITE_BACKEND_URL));
RegisterResolver(UserService, () => new UserService(VITE_BACKEND_URL));
RegisterSingleton(GlobalEventObserver, () => new GlobalEventObserver());
RegisterSingleton("IServerEventHandler", () => new ServerEventHandler(VITE_BACKEND_URL));
RegisterResolver(HabitService, () => new HabitService(VITE_BACKEND_URL));

createRoot(document.getElementById('root')!).render(
  <BrowserRouter>
    <App />
  </BrowserRouter>
)
